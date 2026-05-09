using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// Format of name.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
public enum EntryBracketType
{
	/// <summary>Bracketed.</summary>
	[Description("Curly Braces \"{...}\"")]
	CurlyBraces,

	/// <summary>Quoted.</summary>
	[Description("Parentheses \"(...)\"")]
	Parentheses

} // End enum.