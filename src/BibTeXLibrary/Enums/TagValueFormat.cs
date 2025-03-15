using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// Format of name.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
public enum TagValueFormat
{
	/// <summary>Bracketed.</summary>
	[Description("Add brackets around the value.")]
	Bracket,

	/// <summary>Quoted.</summary>
	[Description("Add quotes the value.")]
	Quote,

	/// <summary>None.</summary>
	[Description("Do not apply formating to the value.")]
	None

} // End enum.