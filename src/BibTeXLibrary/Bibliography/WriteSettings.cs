using DigitalProduction.ComponentModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BibTeXLibrary;

/// <summary>
/// Settings to use when writing a bib file.
/// </summary>
public class WriteSettings : NotifyModifiedChanged
{
	#region Fields

	private WhiteSpace			_whiteSpace						= WhiteSpace.Tab;
	private int					_tabSize						= 4;
	private bool				_alignTagValues					= true;
	private int					_stringEntryAlignAtColumn		= 28;
	private int					_bibEntryAlignAtColumn			= 24;
	private int					_stringEntryAlignAtTabStop		= 6;
	private int					_bibEntryAlignAtTabStop			= 5;
	private FieldValueFormat		_stringConstantTagValueFormat	= FieldValueFormat.Quote;
	private FieldValueFormat		_bibEntryTagValueFormat			= FieldValueFormat.Bracket;
	private bool				_removeLastComma				= true;
	private string				_newLine						= Environment.NewLine;
	private char				_tab							= '\t';

	#endregion

	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public WriteSettings()
	{
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	public WriteSettings(WriteSettings writeSettings)
	{
		_whiteSpace			= writeSettings._whiteSpace;
		_tabSize			= writeSettings._tabSize;
		_alignTagValues		= writeSettings._alignTagValues;
		_bibEntryAlignAtColumn		= writeSettings._bibEntryAlignAtColumn;
		_bibEntryAlignAtTabStop		= writeSettings._bibEntryAlignAtTabStop;
		_removeLastComma	= writeSettings._removeLastComma;
		_newLine			= writeSettings._newLine;
		_tab				= writeSettings._tab;
	}

	#endregion

	#region Properties

	/// <summary>
	/// Type of white space character to use.
	/// </summary>
	[XmlAttribute("whitespace")]
	public WhiteSpace WhiteSpace
	{
		get => _whiteSpace;

		set
		{
			if (_whiteSpace != value)
			{
				_whiteSpace = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// The number of spaces per tab to use.
	/// </summary>
	[XmlAttribute("spacespertab")]
	public int TabSize
	{
		get => _tabSize;

		set
		{
			if (_tabSize != value)
			{
				_tabSize = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Specifies if the tag values should be aligned at the equal sign.
	/// </summary>
	[XmlAttribute("alignatequals")]
	public bool AlignTagValues
	{
		get => _alignTagValues;

		set
		{
			if (_alignTagValues != value)
			{
				_alignTagValues = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Specifies the column number to align tag values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("stringentryalignatcolumn")]
	public int StringEntryAlignAtColumn
	{
		get => _stringEntryAlignAtColumn;

		set
		{
			if (_stringEntryAlignAtColumn != value)
			{
				_stringEntryAlignAtColumn = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Specifies the column number to align tag values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("bibentryalignatcolumn")]
	public int BibEntryAlignAtColumn
	{
		get => _bibEntryAlignAtColumn;

		set
		{
			if (_bibEntryAlignAtColumn != value)
			{
				_bibEntryAlignAtColumn = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Specifies the tab stop number to align tag values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("stringentryalignattabstop")]
	public int StringEntryAlignAtTabStop
	{
		get => _stringEntryAlignAtTabStop;

		set
		{
			if (_stringEntryAlignAtTabStop != value)
			{
				_stringEntryAlignAtTabStop = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Specifies the tab stop number to align tag values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("bibentryalignattabstop")]
	public int BibEntryAlignAtTabStop
	{
		get => _bibEntryAlignAtTabStop;

		set
		{
			if (_bibEntryAlignAtTabStop != value)
			{
				_bibEntryAlignAtTabStop = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Specifies the format for writing the tag values of StringConstant when the tag value is a string.
	/// </summary>
	[XmlAttribute("stringentrytagvalueformat")]
	public FieldValueFormat StringConstantTagValueFormat
	{
		get => _stringConstantTagValueFormat;

		set
		{
			if (_stringConstantTagValueFormat != value)
			{
				_stringConstantTagValueFormat = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Specifies the format for writing the tag values of BibEntries when the tag value is a string.
	/// </summary>
	[XmlAttribute("bibentrytagvalueformat")]
	public FieldValueFormat BibEntryTagValueFormat
	{
		get => _bibEntryTagValueFormat;

		set
		{
			if (_bibEntryTagValueFormat != value)
			{
				_bibEntryTagValueFormat = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Remove the comma after the last tag in a BibEntry.
	/// </summary>
	[XmlAttribute("removelastcomma")]
	public bool RemoveLastComma
	{
		get => _removeLastComma;

		set
		{
			if (_removeLastComma != value)
			{
				_removeLastComma = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// New line character or sequence.
	/// </summary>
	[XmlIgnore()]
	public string NewLine
	{
		get => _newLine;

		set
		{
			if (_newLine != value)
			{
				_newLine = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Tab character or sequence.
	/// </summary>
	[XmlIgnore()]
	public char Tab
	{
		get => _tab;

		set
		{
			if (_tab != value)
			{
				_tab = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Get the same amount of white space a tab would take up as a string of spaces.
	/// </summary>
	private string TabAsSpaces { get => new(' ', TabSize); }
	
	/// <summary>
	/// Get a tab or the same amount of spaces as a tab would use.
	/// </summary>
	[XmlIgnore()]
	public string Indent
	{
		get
		{
			return WhiteSpace switch
			{
				WhiteSpace.Tab		=> new string(Tab, 1),
				WhiteSpace.Space	=> TabAsSpaces,
				_					=> throw new InvalidEnumArgumentException("Invalid \"WhiteSpace\" value."),
			};
		}
	}

	#endregion

	#region Methods

	/// <summary>
	/// The WriteSettings do not save/serialize themselves.  Therefore, we provide a method for to indicate the object was saved.
	/// </summary>
	public void MarkSaved()
	{
		Modified = false;
	}

	#endregion

} // End class.