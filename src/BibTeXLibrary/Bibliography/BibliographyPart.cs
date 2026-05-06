using DigitalProduction.ComponentModel;
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

	#region Public Methods

	/// <summary>
	/// Public interface to mark the bibliography part as saved.  This is used to reset the Modified property after saving.
	/// </summary>
	public void MarkSaved()
	{
		Modified = false;
	}

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
	public abstract void SetTagValue(string tagName, string tagValue, FieldValueType tagValueType);

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

	#region String Writing

	/// <summary>
	/// Get the space between the tag "key" and the tag "value".
	/// 
	/// Examples:
	/// % Use a space between the key and value (no alignment).
	/// title = {Title of Work}
	/// author = {John Q. Author}
	/// year = {2000}
	/// 
	/// % Align the key and value.
	/// title    = {Title of Work}
	/// author   = {John Q. Author}
	/// year     = {2000}
	/// 
	/// % Use a space between the key and value (no alignment).
	/// </summary>
	/// <param name="tagKey">The tag key as a string.</param>
	protected string GetInterTagSpacing(string tagKey, WriteSettings writeSettings, int alignAtTabStop, int alignAtColumn)
	{
		if (writeSettings.AlignTagValues)
		{
			// To align the values is much more complicated.  First decide if spaces or tabs are going to be inserted.
			int requiredCharacters = 0;
			char whiteSpacechar = ' ';
			switch (writeSettings.WhiteSpace)
			{
				case WhiteSpace.Tab:
					{
						// Subtract the initial line indent and the length of the key from the desired number of tabs.
						requiredCharacters = alignAtTabStop - 1 - (int)System.Math.Ceiling((double)(tagKey.Length / writeSettings.TabSize));
						whiteSpacechar = writeSettings.Tab;
						break;
					}
				case WhiteSpace.Space:
					{
						// Subtract the initial line indent and the length of the key from the desired aligning column.
						requiredCharacters = alignAtColumn - 1 - tagKey.Length - writeSettings.TabSize;
						whiteSpacechar = ' ';
						break;
					}
				default:
					{
						throw new InvalidEnumArgumentException("Invalid \"WhiteSpace\" value.");
					}
			}

			if (requiredCharacters < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(tagKey), "The key is too long for the space allocated for aligning tag values.");
			}
			return new string(whiteSpacechar, requiredCharacters);
		}
		else
		{
			// If we are not aligning values, just return a space.
			return " ";
		}
	}

	#endregion

} // End class.