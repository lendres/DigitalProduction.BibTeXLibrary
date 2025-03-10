using System.Collections;
using System.Text;

namespace BibTeXLibrary;

/// <summary>
/// A bibliography string constant.
/// </summary>
public class StringConstantPart : BibliographyPart
{
	#region Fields

	public static readonly string TypeString = "string";

	private string		_name		= string.Empty;
	private TagValue	_value		= new();

	#endregion

	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public StringConstantPart() :
		base(true)
	{
	}

	#endregion

	#region Properties

	/// <summary>
	/// Bibliography entry type, e.g. "book", "thesis", "string".  This is the name that follows the "@".
	/// </summary>
	public override string Type { get => TypeString; set => throw new Exception("You cannot set the type of a StringConstantPart"); }

	/// <summary>
	/// Name of the string constant.
	/// </summary>
	public string Name { get => _name; }

	/// <summary>
	/// Value of the string constant.
	/// </summary>
	public string Value { get => _value.Content; }

	#endregion

	#region Public Tag Value Methods

	/// <summary>
	/// Set a TagValue.
	/// </summary>
	/// <param name="tagName">Name of the tag to get.</param>
	public override void SetTagValue(string tagName, string tagValue, TagValueType tagValueType)
	{
		if (!_caseSensitivetags)
		{
			tagName = tagName.ToLower();
		}
		_name = tagName;
		_value = new TagValue(tagValue, TagValueFormat.Quote);
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
		bibliographyPart.Append(_name);

		// Add the space between the key and equal sign.
		bibliographyPart.Append(writeSettings.GetInterTagSpacing(Name));

		// Add the string constant value.
		bibliographyPart.Append("= ");
		bibliographyPart.Append(_value.ToString());
		bibliographyPart.Append(")");

		bibliographyPart.Append(writeSettings.NewLine);

		return bibliographyPart.ToString();
	}

	#endregion

} // End class.