namespace BibTeXLibrary;

/// <summary>
/// The tag value for a BibTeX library.  This is an object to allow more complex behavior.  Specifically,
/// it allows different types of writing (ToString) for the value.
/// </summary>
public class TagValue
{
	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public TagValue()
	{
	}

	/// <summary>
	/// Default constructor.
	/// </summary>
	/// <param name="content">The tag content.</param>
	public TagValue(string content)
	{
		Content = content;
	}

	/// <summary>
	/// Default constructor.
	/// </summary>
	/// <param name="content">The tag content.</param>
	/// <param name="format">Specifies the format to write in.</param>
	public TagValue(string content, TagValueFormat format)
	{
		Content	= content;
		Format	= format;
	}

	#endregion

	#region Properties

	/// <summary>
	/// The content of the tag value.
	/// </summary>
	public string Content { get; set; } = string.Empty;

	/// <summary>
	/// Specifies is the value is a common entry (text) or a BibTeX string.
	/// </summary>
	public TagValueFormat Format { get; set; } = TagValueFormat.Bracket;

	#endregion

	#region Methods

	/// <summary>
	/// Get a string representation.
	/// </summary>
	public override string ToString()
	{
		return ToString(Format);
	}

	/// <summary>
	/// Get a string representation.
	/// </summary>
	public string ToString(TagValueFormat format)
	{
		return format switch
		{
			TagValueFormat.Bracket => "{"+Content+"}",
			TagValueFormat.Quote => "\""+Content+"\"",
			TagValueFormat.None => Content,
			_ => throw new Exception("Invalid tag format."),
		};
	}

	#endregion

} // End class.