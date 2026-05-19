using DigitalProduction.ComponentModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BibTeXLibrary;

/// <summary>
/// Settings to use when writing a bib file.
/// </summary>
public class WriteSettings : NotifyPropertyModifiedChanged
{
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
		WhiteSpace					= writeSettings.WhiteSpace;
		TabSize						= writeSettings.TabSize;
		AlignFieldValues			= writeSettings.AlignFieldValues;
		StringEntryBracketType		= writeSettings.StringEntryBracketType;
		BibEntryBracketType			= writeSettings.BibEntryBracketType;
		StringEntryAlignAtColumn	= writeSettings.StringEntryAlignAtColumn;
		BibEntryAlignAtColumn		= writeSettings.BibEntryAlignAtColumn;
		StringEntryAlignAtTabStop	= writeSettings.StringEntryAlignAtTabStop;
		BibEntryAlignAtTabStop		= writeSettings.BibEntryAlignAtTabStop;
		StringEntryFieldValueFormat	= writeSettings.StringEntryFieldValueFormat;
		BibEntryFieldValueFormat	= writeSettings.BibEntryFieldValueFormat;
		RemoveLastComma				= writeSettings.RemoveLastComma;
		NewLine						= writeSettings.NewLine;
		Tab							= writeSettings.Tab;
		Modified					= false;
	}

	#endregion

	#region Properties

	/// <summary>
	/// Type of white space character to use.
	/// </summary>
	[XmlAttribute("whitespace")]
	public WhiteSpace WhiteSpace
	{
		get => GetValueOrDefault(WhiteSpace.Tab);
		set => SetValue(value);
	}

	/// <summary>
	/// The number of spaces per tab to use.
	/// </summary>
	[XmlAttribute("spacespertab")]
	public int TabSize
	{
		get => GetValueOrDefault(4);
		set => SetValue(value);
	}

	/// <summary>
	/// Specifies if the field values should be aligned at the equal sign.
	/// </summary>
	[XmlAttribute("alignatequals")]
	public bool AlignFieldValues
	{
		get => GetValueOrDefault(true);
		set => SetValue(value);
	}

	/// <summary>
	/// Specifies the column number to align field values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("stringentrybrackettype")]
	public EntryBracketType StringEntryBracketType
	{
		get => GetValueOrDefault(EntryBracketType.Parentheses);
		set => SetValue(value);
	}

	/// <summary>
	/// Specifies the column number to align field values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("bibentrybrackettype")]
	public EntryBracketType BibEntryBracketType
	{
		get => GetValueOrDefault(EntryBracketType.CurlyBraces);
		set => SetValue(value);
	}

	/// <summary>
	/// Specifies the column number to align field values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("stringentryalignatcolumn")]
	public int StringEntryAlignAtColumn
	{
		get => GetValueOrDefault(28);
		set => SetValue(value);
	}

	/// <summary>
	/// Specifies the column number to align field values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("bibentryalignatcolumn")]
	public int BibEntryAlignAtColumn
	{
		get => GetValueOrDefault(24);
		set => SetValue(value);
	}

	/// <summary>
	/// Specifies the tab stop number to align field values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("stringentryalignattabstop")]
	public int StringEntryAlignAtTabStop
	{
		get => GetValueOrDefault(6);
		set => SetValue(value);
	}

	/// <summary>
	/// Specifies the tab stop number to align field values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("bibentryalignattabstop")]
	public int BibEntryAlignAtTabStop
	{
		get => GetValueOrDefault(5);
		set => SetValue(value);
	}

	/// <summary>
	/// Specifies the format for writing the field values of StringConstant when the field value is a string.
	/// </summary>
	[XmlAttribute("stringentryfieldvalueformat")]
	public FieldValueFormat StringEntryFieldValueFormat
	{
		get => GetValueOrDefault(FieldValueFormat.Quotes);
		set => SetValue(value);
	}

	/// <summary>
	/// Specifies the format for writing the field values of BibEntries when the field value is a string.
	/// </summary>
	[XmlAttribute("bibentryfieldvalueformat")]
	public FieldValueFormat BibEntryFieldValueFormat
	{
		get => GetValueOrDefault(FieldValueFormat.CurlyBraces);
		set => SetValue(value);
	}

	/// <summary>
	/// Remove the comma after the last field in a BibEntry.
	/// </summary>
	[XmlAttribute("removelastcomma")]
	public bool RemoveLastComma
	{
		get => GetValueOrDefault(true);
		set => SetValue(value);
	}

	/// <summary>
	/// New line character or sequence.
	/// </summary>
	[XmlIgnore()]
	public string NewLine
	{
		get => GetValueOrDefault(Environment.NewLine);
		set => SetValue(value);
	}

	/// <summary>
	/// Tab character or sequence.
	/// </summary>
	[XmlIgnore()]
	public char Tab
	{
		get => GetValueOrDefault('\t');
		set => SetValue(value);
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

} // End class.