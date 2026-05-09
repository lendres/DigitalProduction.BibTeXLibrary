namespace BibTeXLibrary;

/// <summary>
/// Exception thrown when bibliography output cannot be written correctly.
/// </summary>
public class BibliographyWriteException : Exception
{
	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public BibliographyWriteException()
	{
	}

	/// <summary>
	/// Message constructor.
	/// </summary>
	/// <param name="message">The exception message.</param>
	public BibliographyWriteException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Message and inner exception constructor.
	/// </summary>
	/// <param name="message">The exception message.</param>
	/// <param name="innerException">The inner exception.</param>
	public BibliographyWriteException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	#endregion
}