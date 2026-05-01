using System.Collections;
using System.Text;

namespace BibTeXLibrary;

/// <summary>
/// A bibliography string constant.
/// </summary>
public class StringConstant : BibliographyPart
{
	#region Fields

	public static readonly string TypeString = "string";

	#endregion

	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public StringConstant() :
		base(true)
	{
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	public StringConstant(StringConstant stringConstant) :
		base(true)
	{
		Name		= stringConstant.Name;
		TagValue	= new TagValue(stringConstant.TagValue.Content, stringConstant.TagValue.Format);
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
		set
		{
			SetValue(value);
			OnPropertyChanged();
			OnPropertyChanged(nameof(Name));
		}
	}

	/// <summary>
	/// Value of the string constant.
	/// </summary>
	public string Value
	{
		get => TagValue.Content;

		set
		{
			TagValue.Content = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(TagValue));
		}
	}

	/// <summary>
	/// Value of the string constant.
	/// </summary>
	public TagValue TagValue
	{
		get				=> GetValueOrDefault<TagValue>(new TagValue(string.Empty, TagValueFormat.Quote));
		protected set	=> SetValue(value);
	}

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

		Name		= tagName;
		TagValue	= new TagValue(tagValue, TagValueFormat.Quote);
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
		bibliographyPart.Append(writeSettings.GetInterTagSpacing(Name));

		// Add the string constant value.
		bibliographyPart.Append("= ");
		bibliographyPart.Append(TagValue.ToString());
		bibliographyPart.Append(")");

		bibliographyPart.Append(writeSettings.NewLine);

		return bibliographyPart.ToString();
	}

	#endregion

} // End class.