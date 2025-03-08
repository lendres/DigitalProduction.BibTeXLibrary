using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// Add summary here.
/// 
/// The "Description" attribute can be accessed using Reflection to get a string representing the enumeration type.
/// </summary>
internal enum BibBuilderState
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
	[Description("Set Tag Name")]
	SetTagName,

	/// <summary>Set the .</summary>
	[Description("Set Tag Value")]
	SetTagValue,

	/// <summary>Set the .</summary>
	[Description("Set Tag")]
	SetTag,

	/// <summary>Build.</summary>
	[Description("Build")]
	Build,

	/// <summary>Skip.</summary>
	[Description("Skip")]
	Skip

} // End enum.