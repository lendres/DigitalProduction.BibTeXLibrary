using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// The type of character to fill white space with.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
public enum WhiteSpace
{
	/// <summary>Use spaces for white space.</summary>
	[Description("Space")]
	Space,

	/// <summary>Use tabs for white space.</summary>
	[Description("Tab")]
	Tab

} // End enum.