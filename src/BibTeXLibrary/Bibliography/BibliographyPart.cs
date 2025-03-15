using DigitalProduction.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Default constructor.
/// </remarks>
public abstract class BibliographyPart(bool caseSensitivetags) : NotifyPropertyModifiedChanged
{
	#region Fields

	/// <summary>Specifies if the tags are case sensitive.</summary>
	protected readonly bool		_caseSensitivetags		= caseSensitivetags;

	#endregion

	#region Properties

	/// <summary>
	/// Bibliography entry type, e.g. "book", "thesis", "string".  This is the name that follows the "@".
	/// </summary>
	public abstract string Type { get; set; }

	#endregion

	#region Public Tag Value Methods

	/// <summary>
	/// Set a TagValue.
	/// </summary>
	/// <param name="tagName">Name of the tag to get.</param>
	public abstract void SetTagValue(string tagName, string tagValue, TagValueType tagValueType);

	#endregion

	#region Public String Writing Methods

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	public override string ToString()
	{
		return ToString(new WriteSettings() { WhiteSpace = WhiteSpace.Space, TabSize = 2, AlignTagValues = false });
	}

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	/// <param name="writeSettings">The settings for writing the bibliography file.</param>
	public abstract string ToString(WriteSettings writeSettings);

	#endregion

} // End class.