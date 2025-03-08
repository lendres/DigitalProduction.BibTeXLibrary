using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// Add summary here.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
public enum SortBy
{
	/// <summary>Last name of the first author.</summary>
	[Description("First Author Last Name")]
	FirstAuthorLastName,

	/// <summary>Key.</summary>
	[Description("Key")]
	Key

} // End enum.