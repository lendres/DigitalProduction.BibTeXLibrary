using System.Collections;
using System.Text;

namespace BibTeXLibrary;

/// <summary>
/// A bibliography string constant.
/// </summary>
public class StringEntry : BibliographyPart
{
	#region Fields

	public static readonly string	TypeString	= "string";
	private Field					_field		= new();

	#endregion

	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public StringEntry() :
		base(true)
	{
		HookUpEvents();
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	public StringEntry(StringEntry stringEntry) :
		base(stringEntry)
	{
		_field = new Field(stringEntry._field);
		HookUpEvents();
	}

	private void HookUpEvents()
	{
		_field.ModifiedChanged += OnFieldModifiedChanged;
		_field.PropertyChanged += OnFieldPropertyChanged;
	}

	#endregion

	#region Properties

	/// <summary>
	/// Bibliography entry type, e.g. "book", "thesis", "string".  This is the name that follows the "@".
	/// For a StringConstant, this is always "string".  This cannot be set, as it is determined by the class type.
	/// </summary>
	public override string Type
	{
		get => TypeString;
		set => throw new Exception("You cannot set the type of a StringConstantPart");
	}

	/// <summary>
	/// Name of the string constant.
	/// </summary>
	public string Name
	{
		get => _field.Name;
		set => _field.Name = value;
	}

	/// <summary>
	/// Value of the string constant.
	/// </summary>
	public string Value
	{
		get => _field.Value;
		set => _field.Value = value;
	}

	/// <summary>
	/// Value of the string constant.
	/// </summary>
	public FieldValue FieldValue
	{
		get				=> _field.FieldValue;
		protected set	=> _field.FieldValue = value;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Public interface to mark the bibliography part as saved.  This is used to reset the Modified property after saving.
	/// </summary>
	public override void MarkSaved()
	{
		_field.MarkSaved();
		Modified = false;
	}

	/// <summary>
	/// Set a FieldValue.
	/// </summary>
	/// <param name="fieldName">Name of the field to get.</param>
	public override void SetField(string fieldName, string fieldValue, FieldValueType fieldValueType)
	{
		if (!_caseSensitiveFields)
		{
			fieldName = fieldName.ToLower();
		}

		Name		= fieldName;
		FieldValue	= new FieldValue(fieldValue, FieldValueType.String);
	}

	#endregion

	#region Public String Writing Methods

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	/// <param name="writeSettings">The settings for writing the bibliography file.</param>
	public override string ToString(WriteSettings writeSettings)
	{
		StringBuilder bibliographyPart = new();

		// Add the comment, if there is one.
		if (!string.IsNullOrEmpty(Comment))
		{
			bibliographyPart.Append(Comment);
		}

		// Build the entry opening and key.
		bibliographyPart.Append("@");
		bibliographyPart.Append(Type);

		char bracketCharacter = writeSettings.StringEntryBracketType == EntryBracketType.CurlyBraces ? '{' : '(';
		bibliographyPart.Append(bracketCharacter);

		// Write the name of the string constant.
		bibliographyPart.Append(_field.ToString(writeSettings, writeSettings.StringEntryFieldValueFormat, writeSettings.StringEntryAlignAtTabStop, writeSettings.StringEntryAlignAtColumn));

		// Write the closing character.
		bracketCharacter = writeSettings.StringEntryBracketType == EntryBracketType.CurlyBraces ? '}' : ')';
		bibliographyPart.Append(bracketCharacter);

		bibliographyPart.Append(writeSettings.NewLine);

		return bibliographyPart.ToString();
	}

	#endregion

} // End class.