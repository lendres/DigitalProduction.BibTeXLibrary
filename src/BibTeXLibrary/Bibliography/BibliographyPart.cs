using DigitalProduction.ComponentModel;
using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Default constructor.
/// </remarks>
public abstract class BibliographyPart : NotifyPropertyModifiedChanged
{
	#region Fields

	/// <summary>Specifies if the fields are case sensitive.</summary>
	protected readonly bool		_caseSensitiveFields		= caseSensitiveFields;

	#endregion

	#region Construction

	protected BibliographyPart(bool caseSensitiveFields)
	{
		_caseSensitiveFields = caseSensitiveFields;
	}

	protected BibliographyPart(BibliographyPart bibliographyPart)
	{
		_caseSensitiveFields	= bibliographyPart._caseSensitiveFields;
		Type					= bibliographyPart.Type;
		Comment					= bibliographyPart.Comment;
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Public interface to mark the bibliography part as saved.  This is used to reset the Modified property after saving.
	/// </summary>
	public abstract void MarkSaved();

	#endregion

	#region Properties

	/// <summary>
	/// Bibliography entry type, e.g. "book", "thesis", "string".  This is the name that follows the "@".
	/// </summary>
	public abstract string Type { get; set; }

	// <summary>Comment associated with the bibliography part. It is the comment in the file immediately before the entry.</summary>
	public string Comment { get; set; } = string.Empty;

	#endregion

	#region Events

	protected void OnFieldModifiedChanged(object sender, bool modified)
	{
		Modified = true;
	}

	protected void OnFieldPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
	{
		OnPropertyChanged(sender, eventArgs);
	}

	#endregion

	#region Public Field Value Methods

	/// <summary>
	/// Set a FieldValue.
	/// </summary>
	/// <param name="fieldName">Name of the field to set.</param>
	public abstract void SetField(string fieldName, string fieldValue, FieldValueType fieldValueType);

	#endregion

	#region Public String Writing Methods

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	public override string ToString()
	{
		return ToString(new WriteSettings() { WhiteSpace = WhiteSpace.Space, TabSize = 2, AlignFieldValues = false });
	}

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	/// <param name="writeSettings">The settings for writing the bibliography file.</param>
	public abstract string ToString(WriteSettings writeSettings);

	#endregion

} // End class.