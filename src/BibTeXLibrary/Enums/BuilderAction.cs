using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// Add summary here.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
internal enum BuilderAction
{
	/// <summary>Set the header.</summary>
	[Description("Set Header")]
	SetHeader,

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
	Build,

	/// <summary>Skip.</summary>
	[Description("Skip")]
	Skip

} // End enum.