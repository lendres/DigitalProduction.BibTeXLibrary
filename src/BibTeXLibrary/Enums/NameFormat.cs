using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// Format of name.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
public enum NameFormat
{
	/// <summary>First name.</summary>
	[Description("First Name")]
	First,

	/// <summary>Full name.</summary>
	[Description("Full Name")]
	Full,

	/// <summary>Last name.</summary>
	[Description("Last Name")]
	Last

} // End enum.