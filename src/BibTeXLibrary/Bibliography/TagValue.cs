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
	/// Content constructor.
	/// </summary>
	/// <param name="content">The tag content.</param>
	public TagValue(string content)
	{
		Content = content;
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	/// <param name="tagValue">The tag content.</param>
	public TagValue(TagValue tagValue)
	{
		Content			= tagValue.Content;
		TagValueType	= tagValue.TagValueType;
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	/// <param name="content">The tag content.</param>
	/// <param name="format">Specifies the format to write in.</param>
	public TagValue(string content, FieldValueType tagValueType)
	{
		Content			= content;
		TagValueType	= tagValueType;
	}

	#endregion

	#region Properties

	/// <summary>
	/// The content of the tag value.
	/// </summary>
	public string Content { get; set; } = string.Empty;

	/// <summary>
	/// The type of the tag value, which determines how it is written when ToString is called with a format.  The default
	/// is TagValueType.String, which means that the content will be written with enclosing brackets/quoates around it. If
	/// the type is TagValueType.StringConstant, then the content will be written without brackets or quotes when ToString
	/// regardless of the format specified.
	/// </summary>
	public FieldValueType TagValueType { get; set; } = FieldValueType.String;

	#endregion

	#region Methods

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string? ToString()
	{
		// Prevent accidently calling a version where the format is not specified.
		// All objects have a public "ToString()" function that it inherits. We need to turn this of so it does
		// not accidently get called.
		throw new NotSupportedException("This method is disabled.");
	}

	/// <summary>
	/// Get a string representation.
	/// </summary>
	public string ToString(FieldValueFormat format)
	{
		// For string constants, we ignore the format and just return the content.
		if (TagValueType == FieldValueType.StringConstant)
		{
			return Content;
		}

		return format switch
		{
			FieldValueFormat.Bracket	=> "{"+Content+"}",
			FieldValueFormat.Quote	=> "\""+Content+"\"",
			FieldValueFormat.None		=> Content,
			_						=> throw new Exception("Invalid tag format."),
		};
	}

	#endregion

} // End class.