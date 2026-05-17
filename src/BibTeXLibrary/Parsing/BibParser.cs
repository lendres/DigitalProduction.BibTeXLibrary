using System.Diagnostics;
using System.Text;

namespace BibTeXLibrary;

internal record struct Next(ParserState State, BuilderAction Action);
internal class TokenToNextMap : Dictionary<TokenType, Next> { }
internal class StateMap : Dictionary<ParserState, TokenToNextMap> { }

/// <summxary>
/// BibTeX file parser.
/// </summary>
public sealed class BibParser : IDisposable
{
	#region Static Fields

	private static readonly char[] _beginCommentCharacters = ['%'];

	#endregion

	#region Constant Fields

	/// <summary>
	/// State tranfer map.
	/// curState --Token--> (nextState, BibBuilderAction)
	/// </summary>
	private static readonly StateMap StateMap = new()
	{
		{ParserState.Begin,			new TokenToNextMap {
			{ TokenType.Comment,			new Next(ParserState.InHeader,		BuilderAction.SetHeader) },
			{ TokenType.Start,				new Next(ParserState.InStart,		BuilderAction.Skip) }
		} },

		{ParserState.InHeader,		new TokenToNextMap {
			{ TokenType.Comment,			new Next(ParserState.InHeader,		BuilderAction.SetHeader) },
			{ TokenType.Start,				new Next(ParserState.InStart,		BuilderAction.Skip) }
		} },

		{ParserState.InStart,		new TokenToNextMap {
			{ TokenType.Name,				new Next(ParserState.InEntry,		BuilderAction.SetType) },
			{ TokenType.StringType,			new Next(ParserState.InStringEntry,	BuilderAction.SetType) }
		} },

		{ParserState.InEntry,		new TokenToNextMap {
			{ TokenType.LeftBrace,			new Next(ParserState.InKey,			BuilderAction.Skip) }
		} },

		{ParserState.InStringEntry,	new TokenToNextMap {
			{ TokenType.LeftBrace,			new Next(ParserState.InTagName,		BuilderAction.Skip) },
			{ TokenType.LeftParenthesis,	new Next(ParserState.InTagName,		BuilderAction.Skip) }
		} },

		{ParserState.InKey,			new TokenToNextMap {
			{ TokenType.RightBrace,			new Next(ParserState.OutEntry,		BuilderAction.Build) },
			{ TokenType.Name,				new Next(ParserState.OutKey,		BuilderAction.SetKey) },
			{ TokenType.String,				new Next(ParserState.OutKey,		BuilderAction.SetKey) },
			{ TokenType.Comma,				new Next(ParserState.InTagName,		BuilderAction.Skip) }
		} },

		{ParserState.OutKey,		new TokenToNextMap {
			{ TokenType.Comma,				new Next(ParserState.InTagName,		BuilderAction.Skip) }
		} },

		{ParserState.InTagName,		new TokenToNextMap {
			{ TokenType.Name,				new Next(ParserState.InTagEqual,	BuilderAction.SetTagName) },
			{ TokenType.RightBrace,			new Next(ParserState.OutEntry,		BuilderAction.Build) }
		} },

		{ParserState.InTagEqual,	new TokenToNextMap {
			{ TokenType.Equal,				new Next(ParserState.InTagValue,	BuilderAction.Skip) }
		} },

		{ParserState.InTagValue,	new TokenToNextMap {
			{ TokenType.String,				new Next(ParserState.OutTagValue,	BuilderAction.SetTagValue) },
			{ TokenType.Name,				new Next(ParserState.OutTagValue,	BuilderAction.SetTagValue) }
		} },

		{ParserState.OutTagValue,	new TokenToNextMap {
			{ TokenType.Concatenation,		new Next(ParserState.InTagValue,	BuilderAction.Skip) },
			{ TokenType.Comma,				new Next(ParserState.InTagName,		BuilderAction.SetTag) },
			{ TokenType.RightBrace,			new Next(ParserState.OutEntry,		BuilderAction.Build) },
			{ TokenType.RightParenthesis,	new Next(ParserState.OutEntry,		BuilderAction.Build) },
			{ TokenType.Comment,			new Next(ParserState.OutTagValue,	BuilderAction.Skip) },
		} },

		{ParserState.OutEntry,		new TokenToNextMap {
			{ TokenType.Start,				new Next(ParserState.InStart,		BuilderAction.Skip) },
			{ TokenType.Comment,			new Next(ParserState.InComment,		BuilderAction.Skip) }
		} },

		{ParserState.InComment,		new TokenToNextMap {
			{ TokenType.Start,				new Next(ParserState.InStart,		BuilderAction.Skip) },
			{ TokenType.Comment,			new Next(ParserState.InComment,		BuilderAction.Skip) }
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
	/// Initializer for BibEntrys.  Used  to allow a defined order of tags.
	/// </summary>
	private BibEntryInitialization  _bibEntryInitialization     = new();

	#endregion

	#region Public Fields

	/// <summary>
	/// Initializer for BibEntrys.  Used  to allow a defined order of tags.
	/// </summary>
	public BibEntryInitialization BibEntryInitializer { get => _bibEntryInitialization; set => _bibEntryInitialization = value; }

	#endregion

	#region Constructors

	/// <summary>
	/// Constructor that reads a file using a StreamReader with default encoding.
	/// </summary>
	/// <param name="path">Full path and file name to the file to reader.</param>
	public BibParser(string path) :
		this(new StreamReader(path, Encoding.UTF8))
	{
	}

	/// <summary>
	/// Constructor that reads a file using a StreamReader with default encoding.
	/// </summary>
	/// <param name="path">Full path and file name to the file to reader.</param>'
	/// <param name="bibEntryInitializationFile">Path of the BibEntry initialization information.</param>
	public BibParser(string path, string bibEntryInitializationFile) :
		this(new StreamReader(path, Encoding.UTF8), bibEntryInitializationFile)
	{
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="textReader">TextReader.</param>
	public BibParser(TextReader textReader)
	{
		_inputText = textReader;
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="textReader">TextReader.</param>
	/// <param name="bibEntryInitializationFile">Path to a BibEntryInitialization file.</param>
	public BibParser(TextReader textReader, string bibEntryInitializationFile) :
		this(textReader, BibEntryInitialization.Deserialize(bibEntryInitializationFile))
	{
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="textReader">TextReader.</param>
	/// <param name="bibEntryInitialization">BibEntryInitialization.</param>
	public BibParser(TextReader textReader, BibEntryInitialization? bibEntryInitialization)
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
		using BibParser parser = new(path);
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
		using BibParser parser = new(inputText);
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
		using BibParser parser = new(inputText, bibEntryInitialization);
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
			ParserState				curState			= ParserState.Begin;
			ParserState				nextState			= ParserState.Begin;

			BibliographyPart?		bibPart				= null;
			string					tagName				= "";
			FieldValueType			tagValueType		= FieldValueType.String;
			StringBuilder			tagValueBuilder		= new();

			// Fetch token from Tokenizer and build BibEntry.
			foreach (Token token in Tokenize())
			{
				// Transfer state.
				if (StateMap[curState].TryGetValue(token.Type, out Next next))
				{
					nextState = next.State;
				}
				else
				{
					IEnumerable<TokenType> expected = from pair in StateMap[curState] select pair.Key;
					throw new UnexpectedTokenException(_lineCount, _columnCount, token.Type, [.. expected]);
				}
				// Build BibEntry.
				switch (StateMap[curState][token.Type].Action)
				{
					case BuilderAction.SetHeader:
					{
						bibliographyDOM.AddHeaderLine(token.Value);
						break;
					}

					case BuilderAction.SetType:
					{
						if (token.Value.ToLower() == StringEntry.TypeString.ToLower())
						{
							bibPart = new StringEntry();
						}
						else
						{
							// Must add the value before doing the initialization.
							BibEntry bibEntry = new() { Type = token.Value };
							bibEntry.Initialize(_bibEntryInitialization.GetDefaultTags(bibEntry));
							bibPart = bibEntry;
						}
						break;
					}

					case BuilderAction.SetKey:
					{
						Debug.Assert(bibPart != null, "Bibliography part is null.");
						BibEntry? bibEntry = bibPart as BibEntry;
						Debug.Assert(bibEntry != null, "Invalid operation, the state should only be SetKey for a BibEntry.");
						bibEntry.Key = token.Value;
						break;
					}

					case BuilderAction.SetTagName:
					{
						tagName = token.Value;
						break;
					}

					case BuilderAction.SetTagValue:
					{
						if (token.Type != TokenType.Concatenation)
						{
							tagValueType = token.Type == TokenType.String ? FieldValueType.String : FieldValueType.StringConstant;
						}
						tagValueBuilder.Append(token.Value);
						break;
					}

					case BuilderAction.SetTag:
					{
						Debug.Assert(bibPart != null, "bib != null");
						SetTag(bibPart, ref tagName, tagValueType, tagValueBuilder);
						break;
					}

					case BuilderAction.Build:
					{
						Debug.Assert(bibPart != null, "bib != null");
						if (tagName != string.Empty)
						{
							SetTag(bibPart, ref tagName, tagValueType, tagValueBuilder);
						}
						bibliographyDOM.Add(bibPart);
						break;
					}
				}
				curState = nextState;
			}

			// Check the current state.  Valid exit options are:
			//    ParserState.OutEntry : We have completed an entire entry.
			//    ParserState.Begin    : There are no entries and no header information.
			//    ParserState.InHeader : We read header information, but did not find any entries in the file.
			if (curState != ParserState.OutEntry & curState != ParserState.Begin & curState != ParserState.InHeader)
			{
				IEnumerable<BibTeXLibrary.TokenType> expected = from pair in StateMap[curState] select pair.Key;
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
	/// Sets the tag and resets all the variables used to build the tag.
	/// </summary>
	/// <param name="bibPart">BibliographyPart.</param>
	/// <param name="tagName">The name of the tag.</param>
	/// <param name="tagValueIsString">A boolean to indicate if the value of the tag is a name (string constant) or an ordinary string.</param>
	/// <param name="tagValueBuilder">String builder used to build the tag value.</param>
	private static void SetTag(BibliographyPart bibPart, ref string tagName, FieldValueType tagValueType, StringBuilder tagValueBuilder)
	{
		Debug.Assert(bibPart != null, "bib != null");
		bibPart.SetField(tagName, tagValueBuilder.ToString(), tagValueType);
		tagValueBuilder.Clear();
		tagName = string.Empty;
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

		while ((code = Peek()) != -1)
		{
			c = (char)code;

			if (c == '@')
			{
				Read();
				yield return new Token(TokenType.Start);
			}
			else if (IsStringCharacter(c))
			{
				StringBuilder value = new();

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
				Read();
				braceCount--;
				yield return new Token(TokenType.RightBrace);
			}
			else if (c == '(')
			{
				Read();
				parenthesisCount++;
				yield return new Token(TokenType.LeftParenthesis);
			}
			else if (c == ')')
			{
				Read();
				parenthesisCount--;
				yield return new Token(TokenType.RightParenthesis);
			}
			else if (c == ',')
			{
				Read();
				yield return new Token(TokenType.Comma);
			}
			else if (c == '#')
			{
				Read();
				yield return new Token(TokenType.Concatenation);
			}
			else if (c == '=')
			{
				Read();
				yield return new Token(TokenType.Equal);
			}
			else if (c == '\n')
			{
				Read();
				_columnCount = 0;
				_lineCount++;
			}
			else if (_beginCommentCharacters.Any(item => item == c))
			{
				_columnCount = 0;
				_lineCount++;
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

	#region Impement Interface "IDisposable"

	/// <summary>
	/// Dispose stream resource.
	/// </summary>
	public void Dispose()
	{
		_inputText?.Dispose();
	}

	#endregion

} // End class.