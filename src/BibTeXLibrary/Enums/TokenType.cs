using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// Add summary here.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
public enum TokenType
{
	/// <summary>Comma.</summary>
	[Description("Comma")]
	Comma,

	/// <summary>Comment.</summary>
	[Description("Comment")]
	Comment,

	/// <summary>Concatenation.</summary>
	[Description("Concatenation")]
	Concatenation,

	/// <summary>Equal.</summary>
	[Description("Equal")]
	Equal,

	/// <summary>End of file.</summary>
	[Description("End of File")]
	EOF,

	/// <summary>Left brace.</summary>
	[Description("Left Brace")]
	LeftBrace,

	/// <summary>Left parenthesis.</summary>
	[Description("Left Parenthesis")]
	LeftParenthesis,

	/// <summary>The value of a cite key, tag name, or name of a string definition.</summary>
	[Description("Name")]
	Name,

	/// <summary>Quotation.</summary>
	[Description("Quotation")]
	Quotation,

	/// <summary>Right brace.</summary>
	[Description("Right Brace")]
	RightBrace,

	/// <summary>Right parenthesis.</summary>
	[Description("Right Parenthesis")]
	RightParenthesis,

	/// <summary>Start.</summary>
	[Description("Start")]
	Start,

	/// <summary>String.  The contents of a tag value.</summary>
	[Description("String")]
	String,

	/// <summary>String type.  A BibTeX string definition type, i.e., "@string".</summary>
	[Description("String Type")]
	StringType

} // End enum.