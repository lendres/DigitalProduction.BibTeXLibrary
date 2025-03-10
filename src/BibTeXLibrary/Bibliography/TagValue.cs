﻿namespace BibTeXLibrary;

/// <summary>
/// The tag value for a BibTeX library.
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
	/// <param name="isString">Specifies if the tag value string.  If false, the value is a "Name" (named BibTeX @string).</param>
	public TagValue(string content, bool isString)
	{
		Content		= content;
		IsString	= isString;
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
	public bool IsString { get; set; } = true;

	#endregion

	#region Methods

	/// <summary>
	/// Get a string representation.
	/// </summary>
	public override string ToString()
	{
		return IsString ? "{"+Content+"}" : Content;
	}

	#endregion

} // End class.