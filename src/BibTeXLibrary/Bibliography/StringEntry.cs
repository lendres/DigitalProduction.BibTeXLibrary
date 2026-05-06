using System.Collections;
using System.Text;

namespace BibTeXLibrary;

/// <summary>
/// A bibliography string constant.
/// </summary>
public class StringEntry : BibliographyPart
{
	#region Fields

	public static readonly string TypeString = "string";

	#endregion

	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public StringEntry() :
		base(true)
	{
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	public StringEntry(StringEntry stringConstant) :
		base(true)
	{
		Name		= stringConstant.Name;
		FieldValue	= new FieldValue(stringConstant.FieldValue);
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
		get => GetValueOrDefault<string>(string.Empty);
		set => SetValue(value);
	}

	/// <summary>
	/// Value of the string constant.
	/// </summary>
	public string Value
	{
		get => FieldValue.Content;

		set
		{
			if (FieldValue.Content != value)
			{
				FieldValue.Content = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(FieldValue));
			}
		}
	}

	/// <summary>
	/// Value of the string constant.
	/// </summary>
	public FieldValue FieldValue
	{
		get				=> GetValueOrDefault<FieldValue>(new FieldValue(string.Empty, FieldValueType.String));
		protected set	=> SetValue(value);
	}

	#endregion

	#region Public Tag Value Methods

	/// <summary>
	/// Set a TagValue.
	/// </summary>
	/// <param name="tagName">Name of the tag to get.</param>
	public override void SetTagValue(string tagName, string tagValue, FieldValueType tagValueType)
	{
		if (!_caseSensitivetags)
		{
			tagName = tagName.ToLower();
		}

		Name		= tagName;
		FieldValue	= new FieldValue(tagValue, FieldValueType.String);
	}

	#endregion

	#region Public String Writing Methods

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	/// <param name="writeSettings">The settings for writing the bibliography file.</param>
	public override string ToString(WriteSettings writeSettings)
	{
		// Build the entry opening and key.
		StringBuilder bibliographyPart = new("@");
		bibliographyPart.Append(Type);
		bibliographyPart.Append('(');

		// Write the name of the string constant.
		bibliographyPart.Append(Name);

		// Add the space between the key and equal sign.
		bibliographyPart.Append(GetInterTagSpacing(Name, writeSettings, writeSettings.StringEntryAlignAtTabStop, writeSettings.StringEntryAlignAtColumn));

		// Add the string constant value.
		bibliographyPart.Append("= ");
		bibliographyPart.Append(FieldValue.ToString(writeSettings.StringConstantTagValueFormat));
		bibliographyPart.Append(")");

		bibliographyPart.Append(writeSettings.NewLine);

		return bibliographyPart.ToString();
	}

	#endregion

} // End class.