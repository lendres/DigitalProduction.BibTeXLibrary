using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// The state of the parser.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
internal enum ParserState
{
	/// <summary>Begin.</summary>
	[Description("Begin")]
	Begin,

	/// <summary>In the header.</summary>
	[Description("In Header")]
	InHeader,

	/// <summary>At the start of a bibliography entry where the type is written.</summary>
	[Description("In Start")]
	InStart,

	/// <summary>Immediatly after the start of an entry and before the cite key (the opening brace).</summary>
	[Description("In Entry")]
	InEntry,

	/// <summary>In the cite key.</summary>
	[Description("In Cite Key")]
	InKey,

	/// <summary>After the cite key.</summary>
	[Description("Out of Cite Key")]
	OutKey,

	/// <summary>Immediatly after the start of a string constant and before the string definition (the opening brace).</summary>
	[Description("In String Definition")]
	InStringEntry,

	/// <summary>In the name of a field.</summary>
	[Description("In Field Name")]
	InFiledName,

	/// <summary>At the equal sign betwen a field name and value.</summary>
	[Description("In Field Equal")]
	InFieldEqual,

	/// <summary>In the value of a field.</summary>
	[Description("In Field Value")]
	InFieldValue,

	/// <summary>Finish reading the field value.</summary>
	[Description("Out of Field Value")]
	OutFieldValue,

	/// <summary>Read a comment.</summary>
	[Description("In comment")]
	InComment,

	/// <summary>At the end of an entry (the closing brace).</summary>
	[Description("Out of Entry")]
	OutEntry

} // End enum.