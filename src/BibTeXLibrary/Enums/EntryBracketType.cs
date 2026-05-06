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
	[Description("Add brackets around the value.")]
	CurlyBraces,

	/// <summary>Quoted.</summary>
	[Description("Use parentheses for the open and closing characters.")]
	Parentheses

} // End enum.