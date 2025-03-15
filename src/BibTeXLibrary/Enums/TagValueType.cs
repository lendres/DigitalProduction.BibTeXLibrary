using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// Format of name.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
public enum TagValueType
{
	/// <summary>Bracketed.</summary>
	[Description("A regular string.")]
	String,

	/// <summary>String constant.</summary>
	[Description("A constant/variable that represents a string.")]
	StringConstant

} // End enum.