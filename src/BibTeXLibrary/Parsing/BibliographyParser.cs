using System.Diagnostics;
using System.Text;

namespace BibTeXLibrary;

internal record struct Next(ParserState State, BuildAction Action);
internal class TokenToNextMap : Dictionary<TokenType, Next> { }
internal class StateMap : Dictionary<ParserState, TokenToNextMap> { }

/// <summxary>
/// BibTeX file parser.
/// </summary>
public sealed class BibliographyParser : IDisposable
{
	#region Static Fields

	private static readonly char[] _beginCommentCharacters = ['%'];

	#endregion

	#region State Map

	/// <summary>
	/// State tranfer map.
	/// curState --Token--> (nextState, BibBuilderAction)
	/// </summary>
	private static readonly StateMap StateMap = new()
	{
		{ParserState.Begin,			new TokenToNextMap {
			{ TokenType.BlankLine,			new Next(ParserState.InStart,		BuildAction.Skip) },
			{ TokenType.Comment,			new Next(ParserState.InHeader,		BuildAction.AddHeaderLine) },
			{ TokenType.Start,				new Next(ParserState.InStart,		BuildAction.Skip) }
		} },

		{ParserState.InHeader,		new TokenToNextMap {
			{ TokenType.BlankLine,			new Next(ParserState.OutHeader,		BuildAction.Skip) },
			{ TokenType.Comment,			new Next(ParserState.InHeader,		BuildAction.AddHeaderLine) },
			{ TokenType.Start,				new Next(ParserState.InStart,		BuildAction.Skip) }
		} },

		{ParserState.OutHeader,		new TokenToNextMap {
			{ TokenType.BlankLine,			new Next(ParserState.OutHeader,		BuildAction.Skip) },
			{ TokenType.Comment,			new Next(ParserState.OutHeader,		BuildAction.Skip) },
			{ TokenType.Start,				new Next(ParserState.InStart,		BuildAction.Skip) }
		} },

		{ParserState.InStart,		new TokenToNextMap {
			{ TokenType.Name,				new Next(ParserState.InEntry,		BuildAction.SetType) },
			{ TokenType.StringType,			new Next(ParserState.InStringEntry,	BuildAction.SetType) }
		} },

		{ParserState.InEntry,		new TokenToNextMap {
			{ TokenType.LeftBrace,			new Next(ParserState.InKey,			BuildAction.Skip) }
		} },

		{ParserState.InStringEntry,	new TokenToNextMap {
			{ TokenType.LeftBrace,			new Next(ParserState.InFiledName,	BuildAction.Skip) },
			{ TokenType.LeftParenthesis,	new Next(ParserState.InFiledName,	BuildAction.Skip) }
		} },

		{ParserState.InKey,			new TokenToNextMap {
			{ TokenType.BlankLine,			new Next(ParserState.InKey,		BuildAction.Skip) },
			{ TokenType.RightBrace,			new Next(ParserState.OutEntry,		BuildAction.AddBibliographyPart) },
			{ TokenType.Name,				new Next(ParserState.OutKey,		BuildAction.SetKey) },
			{ TokenType.String,				new Next(ParserState.OutKey,		BuildAction.SetKey) },
			{ TokenType.Comma,				new Next(ParserState.InFiledName,	BuildAction.Skip) }
		} },

		{ParserState.OutKey,		new TokenToNextMap {
			{ TokenType.BlankLine,			new Next(ParserState.OutKey,		BuildAction.Skip) },
			{ TokenType.Comma,				new Next(ParserState.InFiledName,	BuildAction.Skip) }
		} },

		{ParserState.InFiledName,		new TokenToNextMap {
			{ TokenType.BlankLine,			new Next(ParserState.InFiledName,		BuildAction.Skip) },
			{ TokenType.Name,				new Next(ParserState.InFieldEqual,	BuildAction.SetFieldName) },
			{ TokenType.RightBrace,			new Next(ParserState.OutEntry,		BuildAction.AddBibliographyPart) }
		} },

		{ParserState.InFieldEqual,	new TokenToNextMap {
			{ TokenType.BlankLine,			new Next(ParserState.InFieldEqual,		BuildAction.Skip) },
			{ TokenType.Equal,				new Next(ParserState.InFieldValue,	BuildAction.Skip) }
		} },

		{ParserState.InFieldValue,	new TokenToNextMap {
			{ TokenType.BlankLine,			new Next(ParserState.InFieldValue,		BuildAction.Skip) },
			{ TokenType.String,				new Next(ParserState.OutFieldValue,	BuildAction.SetFieldValue) },
			{ TokenType.Name,				new Next(ParserState.OutFieldValue,	BuildAction.SetFieldValue) }
		} },

		{ParserState.OutFieldValue,	new TokenToNextMap {
			{ TokenType.BlankLine,			new Next(ParserState.OutFieldValue,		BuildAction.Skip) },
			{ TokenType.Concatenation,		new Next(ParserState.InFieldValue,	BuildAction.Skip) },
			{ TokenType.Comma,				new Next(ParserState.InFiledName,	BuildAction.SetField) },
			{ TokenType.RightBrace,			new Next(ParserState.OutEntry,		BuildAction.AddBibliographyPart) },
			{ TokenType.RightParenthesis,	new Next(ParserState.OutEntry,		BuildAction.AddBibliographyPart) },
			{ TokenType.Comment,			new Next(ParserState.OutFieldValue,	BuildAction.Skip) },
		} },

		{ParserState.OutEntry,		new TokenToNextMap {
			{ TokenType.BlankLine,			new Next(ParserState.OutEntry,		BuildAction.Skip) },
			{ TokenType.Start,				new Next(ParserState.InStart,		BuildAction.Skip) },
			{ TokenType.Comment,			new Next(ParserState.InComment,		BuildAction.Skip) }
		} },

		{ParserState.InComment,		new TokenToNextMap {
			{ TokenType.BlankLine,			new Next(ParserState.InComment,		BuildAction.Skip) },
			{ TokenType.Start,				new Next(ParserState.InStart,		BuildAction.Skip) },
			{ TokenType.Comment,			new Next(ParserState.InComment,		BuildAction.Skip) }
		} },
	};

	#endregion

	#region Private Fields

	/// <summary>
	/// Input text stream.
	/// </summary>
	private readonly TextReader     _inputText;

	/// <summary>
	/// Line No. counter.
	/// </summary>
	private int                     _lineCount                  = 1;

	/// <summary>
	/// Column counter.
	/// </summary>
	private int                     _columnCount;

	/// <summary>
	/// Initializer for BibEntrys.  Used  to allow a defined order of fields.
	/// </summary>
	private BibEntryInitialization  _bibEntryInitialization     = new();

	#endregion

	#region Public Fields

	/// <summary>
	/// Initializer for BibEntrys.  Used  to allow a defined order of fields.
	/// </summary>
	public BibEntryInitialization BibEntryInitializer { get => _bibEntryInitialization; set => _bibEntryInitialization = value; }

	#endregion

	#region Constructors

	/// <summary>
	/// Constructor that reads a file using a StreamReader with default encoding.
	/// </summary>
	/// <param name="path">Full path and file name to the file to reader.</param>
	public BibliographyParser(string path) :
		this(new StreamReader(path, Encoding.UTF8))
	{
	}

	/// <summary>
	/// Constructor that reads a file using a StreamReader with default encoding.
	/// </summary>
	/// <param name="path">Full path and file name to the file to reader.</param>'
	/// <param name="bibEntryInitializationFile">Path of the BibEntry initialization information.</param>
	public BibliographyParser(string path, string bibEntryInitializationFile) :
		this(new StreamReader(path, Encoding.UTF8), bibEntryInitializationFile)
	{
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="textReader">TextReader.</param>
	public BibliographyParser(TextReader textReader)
	{
		_inputText = textReader;
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="textReader">TextReader.</param>
	/// <param name="bibEntryInitializationFile">Path to a BibEntryInitialization file.</param>
	public BibliographyParser(TextReader textReader, string bibEntryInitializationFile) :
		this(textReader, BibEntryInitialization.Deserialize(bibEntryInitializationFile))
	{
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="textReader">TextReader.</param>
	/// <param name="bibEntryInitialization">BibEntryInitialization.</param>
	public BibliographyParser(TextReader textReader, BibEntryInitialization? bibEntryInitialization)
	{
		if (bibEntryInitialization == null)
		{
			throw new ArgumentNullException(nameof(bibEntryInitialization), "The BibEntryInitializaiton is null.");
		}
		_inputText              = textReader;
		_bibEntryInitialization = bibEntryInitialization;
	}

	#endregion

	#region Public Static Methods

	/// <summary>
	/// Parse a file at the specified path..
	/// </summary>
	/// <param name="path">Full path and file name to the file to reader.</param>
	public static BibliographyDOM Parse(string path)
	{
		using BibliographyParser parser = new(path);
		try
		{
			return parser.Parse();
		}
		catch (UnexpectedTokenException exception)
		{
			throw new Exception($"An error occured reading the file:\n" + path + "\n\n" + exception.Message);
		}
	}

	/// <summary>
	/// Parse by given input text reader.
	/// </summary>
	/// <param name="inputText">TextReader containing the input text to be parsed.</param>
	public static BibliographyDOM Parse(TextReader inputText)
	{
		using BibliographyParser parser = new(inputText);
		return parser.Parse();
	}

	/// <summary>
	/// Parse by given input text reader.
	/// </summary>
	/// <param name="inputText">TextReader containing the input text to be parsed.</param>
	/// <param name="bibEntryInitializationFile">Path of the BibEntry initialization information.</param>
	public static BibliographyDOM Parse(TextReader inputText, string bibEntryInitializationFile)
	{
		return Parse(inputText, BibEntryInitialization.Deserialize(bibEntryInitializationFile));
	}

	/// <summary>
	/// Parse by given input text reader.
	/// </summary>
	/// <param name="inputText">TextReader containing the input text to be parsed.</param>
	/// <param name="bibEntryInitialization">BibEntryInitialization.</param>
	public static BibliographyDOM Parse(TextReader inputText, BibEntryInitialization? bibEntryInitialization)
	{
		using BibliographyParser parser = new(inputText, bibEntryInitialization);
		return parser.Parse();
	}

	#endregion

	#region Parse Methods

	/// <summary>
	/// Get all results from the Parser.
	/// </summary>
	public BibliographyDOM Parse()
	{
		BibliographyDOM bibliographyDOM = new();
		Parse(bibliographyDOM);
		return bibliographyDOM;
	}

	/// <summary>
	/// Get all results from the Parser.
	/// </summary>
	public BibliographyDOM Parse(BibliographyDOM bibliographyDOM)
	{
		try
		{
			ParserState			currentState			= ParserState.Begin;
			BibliographyPart?	bibliographyPart				= null;
			string				fieldName			= "";
			FieldValueType		fieldValueType		= FieldValueType.String;
			StringBuilder		fieldValueBuilder	= new();

			// Fetch token from Tokenizer and build BibEntry.
			foreach (Token token in Tokenize())
			{
				// Get the transfer state. Throw an exception if the token is not expected in the current state.
				if (!StateMap[currentState].TryGetValue(token.Type, out Next next))
				{
					IEnumerable<TokenType> expected = from pair in StateMap[currentState] select pair.Key;
					throw new UnexpectedTokenException(_lineCount, _columnCount, token.Type, [.. expected]);
				}
				// Build BibEntry.
				switch (next.Action)
				{
					case BuildAction.AddHeaderLine:
					{
						bibliographyDOM.AddHeaderLine(token.Value);
						break;
					}

					case BuildAction.SetType:
					{
						if (token.Value.ToLower() == StringEntry.TypeString.ToLower())
						{
							bibliographyPart = new StringEntry();
						}
						else
						{
							// Must add the value before doing the initialization.
							BibEntry bibEntry = new() { Type = token.Value };
							bibEntry.Initialize(_bibEntryInitialization.GetDefaultFields(bibEntry));
							bibliographyPart = bibEntry;
						}
						break;
					}

					case BuildAction.SetKey:
					{
						Debug.Assert(bibliographyPart != null, "Bibliography part is null.");
						BibEntry? bibEntry = bibliographyPart as BibEntry;
						Debug.Assert(bibEntry != null, "Invalid operation, the state should only be SetKey for a BibEntry.");
						bibEntry.Key = token.Value;
						break;
					}

					case BuildAction.SetFieldName:
					{
						fieldName = token.Value;
						break;
					}

					case BuildAction.SetFieldValue:
					{
						if (token.Type != TokenType.Concatenation)
						{
							fieldValueType = token.Type == TokenType.String ? FieldValueType.String : FieldValueType.StringConstant;
						}
						fieldValueBuilder.Append(token.Value);
						break;
					}

					case BuildAction.SetField:
					{
						Debug.Assert(bibliographyPart != null, "Bibliography part is null.");
						SetField(bibliographyPart, ref fieldName, fieldValueType, fieldValueBuilder);
						break;
					}

					case BuildAction.AddBibliographyPart:
					{
						Debug.Assert(bibliographyPart != null, "Bibliography part is null.");
						if (fieldName != string.Empty)
						{
							SetField(bibliographyPart, ref fieldName, fieldValueType, fieldValueBuilder);
						}
						bibliographyDOM.Add(bibliographyPart);
						break;
					}
				}
				currentState = next.State;
			}

			// Check the current state.  Valid exit options are:
			//    ParserState.OutEntry : We have completed an entire entry.
			//    ParserState.Begin    : There are no entries and no header information.
			//    ParserState.InHeader : We read header information, but did not find any entries in the file.
			if (currentState != ParserState.OutEntry & currentState != ParserState.Begin & currentState != ParserState.InHeader)
			{
				IEnumerable<BibTeXLibrary.TokenType> expected = from pair in StateMap[currentState] select pair.Key;
				throw new UnexpectedTokenException(_lineCount, _columnCount, TokenType.EOF, [.. expected]);
			}
		}
		finally
		{
			Dispose();
		}

		return bibliographyDOM;
	}

	/// <summary>
	/// Sets the field and resets all the variables used to build the field.
	/// </summary>
	/// <param name="bibliographyPart">BibliographyPart.</param>
	/// <param name="fieldName">The name of the field.</param>
	/// <param name="fieldValueType">A boolean to indicate if the value of the field is a name (string constant) or an ordinary string.</param>
	/// <param name="fieldValueBuilder">String builder used to build the field value.</param>
	private static void SetField(BibliographyPart bibliographyPart, ref string fieldName, FieldValueType fieldValueType, StringBuilder fieldValueBuilder)
	{
		Debug.Assert(bibliographyPart != null, "bib != null");
		bibliographyPart.SetField(fieldName, fieldValueBuilder.ToString(), fieldValueType);
		fieldValueBuilder.Clear();
		fieldName = string.Empty;
	}

	/// <summary>
	/// Tokenizer for BibTeX entry.
	/// </summary>
	/// <returns></returns>
	private IEnumerable<Token> Tokenize()
	{
		int     code;
		char    c;
		int     braceCount          = 0;
		int     parenthesisCount    = 0;
		bool	isBlankLine			= true;

		while ((code = Peek()) != -1)
		{
			c = (char)code;

			if (c == '@')
			{
				isBlankLine = false;
				Read();
				yield return new Token(TokenType.Start);
			}
			else if (IsStringCharacter(c))
			{
				isBlankLine			= false;
				StringBuilder value	= new();

				while (true)
				{
					c = (char)Read();
					value.Append(c);

					if ((code = Peek()) == -1)
					{
						break;
					}
					c = (char)code;

					if (!IsStringCharacter(c))
					{
						break;
					}
				}

				string valueString = value.ToString();
				TokenType tokenType = valueString.ToLower().Trim() == StringEntry.TypeString ? TokenType.StringType : TokenType.Name;

				yield return new Token(tokenType, valueString);
			}
			else if (c == '"')
			{
				isBlankLine				= false;
				StringBuilder value     = new();
				int internalBraceCount  = 0;

				Read();

				while ((code = Peek()) != -1)
				{
					if (c == '{')
					{
						internalBraceCount++;
					}
					else if (c == '}')
					{
						internalBraceCount--;
					}

					// We don't want to stop while we have open braces.  This is for cases like: title = "{This is a Title}"
					if (internalBraceCount == 0)
					{
						// Stop when we find the quotation mark.  Don't stop for \".
						if (c != '\\' && code == '"')
						{
							// Read the closing quote and exit.
							Read();
							break;
						}
					}

					c = (char)Read();
					value.Append(c);
				}
				yield return new Token(TokenType.String, value.ToString());
			}
			else if (c == '{')
			{
				isBlankLine = false;

				// Braces have to be handled differently depending on if the are the opening bracket for a group or
				// internal backets used to internally group for keeping capital letters, et cetera.
				// To parse BibTex strings, we have to allow for a parentheses used as the grouping characters, so we need
				// to also prevent returning the left bracket in those cases.
				if (braceCount == 0 && parenthesisCount == 0)
				{
					braceCount++;
					Read();
					yield return new Token(TokenType.LeftBrace);
				}
				else
				{
					StringBuilder value = new();
					// Read the brace (was only peeked).
					Read();
					int internalBraceCount = 1;
					while (internalBraceCount > 0 && Peek() != -1)
					{
						c = (char)Read();
						if (c == '{')
						{
							internalBraceCount++;
						}
						else if (c == '}')
						{
							internalBraceCount--;
						}

						if (internalBraceCount > 0)
						{
							value.Append(c);
						}
					}
					yield return new Token(TokenType.String, value.ToString());
				}
			}
			else if (c == '}')
			{
				isBlankLine = false;
				Read();
				braceCount--;
				yield return new Token(TokenType.RightBrace);
			}
			else if (c == '(')
			{
				isBlankLine = false;
				Read();
				parenthesisCount++;
				yield return new Token(TokenType.LeftParenthesis);
			}
			else if (c == ')')
			{
				isBlankLine = false;
				Read();
				parenthesisCount--;
				yield return new Token(TokenType.RightParenthesis);
			}
			else if (c == ',')
			{
				isBlankLine = false;
				Read();
				yield return new Token(TokenType.Comma);
			}
			else if (c == '#')
			{
				isBlankLine = false;
				Read();
				yield return new Token(TokenType.Concatenation);
			}
			else if (c == '=')
			{
				isBlankLine = false;
				Read();
				yield return new Token(TokenType.Equal);
			}
			else if (c == '\n')
			{
				Read();
				_columnCount = 0;
				_lineCount++;
				if (isBlankLine)
				{
					yield return new Token(TokenType.BlankLine);
				}
				isBlankLine = true;
			}
			else if (_beginCommentCharacters.Any(item => item == c))
			{
				isBlankLine = false;
				_columnCount = 0;
				_lineCount++;
				isBlankLine = true;
				yield return new Token(TokenType.Comment, _inputText.ReadLine()!);
			}
			else if (!char.IsWhiteSpace(c))
			{
				throw new UnrecognizableCharacterException(_lineCount, _columnCount, c);
			}
			else
			{
				// Read white space.
				Read();
			}
		}
	}

	private static bool IsStringCharacter(char c)
	{
		if (char.IsLetterOrDigit(c) ||
			c == '-' ||
			c == '.' ||
			c == '_' ||
			c == '—' ||
			c == ':' ||
			c == '/' ||
			c == '\\')
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Peek next char but not move forward.
	/// </summary>
	/// <returns></returns>
	private int Peek()
	{
		return _inputText.Peek();
	}

	/// <summary>
	/// Read next char and move forward.
	/// </summary>
	/// <returns></returns>
	private int Read()
	{
		_columnCount++;
		if (_inputText.Peek() != -1)
		{
			return _inputText.Read();
		}
		else
		{
			return -1;
		}
	}

	#endregion

	#region Implement Interface "IDisposable"

	/// <summary>
	/// Dispose stream resource.
	/// </summary>
	public void Dispose()
	{
		_inputText?.Dispose();
	}

	#endregion

} // End class.