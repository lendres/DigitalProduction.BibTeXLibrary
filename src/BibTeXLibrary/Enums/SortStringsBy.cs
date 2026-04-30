using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// Method for sorting StringConstants.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
public enum SortStringsBy
{
	/// <summary>Sort by the Name.</summary>
	[Description("Name")]
	Name,

	/// <summary>Sort by the Value.</summary>
	[Description("Value")]
	Value

} // End enum.