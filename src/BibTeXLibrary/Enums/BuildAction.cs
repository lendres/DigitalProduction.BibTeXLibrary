using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// Add summary here.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
internal enum BuildAction
{
	/// <summary>Add a header line.</summary>
	[Description("Add Header Line")]
	AddHeaderLine,

	/// <summary>Set the .</summary>
	[Description("Set Type")]
	SetType,

	/// <summary>Set the .</summary>
	[Description("Set Key")]
	SetKey,

	/// <summary>Set the .</summary>
	[Description("Set Field Name")]
	SetFieldName,

	/// <summary>Set the .</summary>
	[Description("Set Field Value")]
	SetFieldValue,

	/// <summary>Set the .</summary>
	[Description("Set Field")]
	SetField,

	/// <summary>Build.</summary>
	[Description("Build")]
	AddBibliographyPart,

	/// <summary>Skip.</summary>
	[Description("Skip")]
	Skip

} // End enum.