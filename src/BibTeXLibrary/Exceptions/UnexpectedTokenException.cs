using System.Text;

namespace BibTeXLibrary;

[Serializable]
public sealed class UnexpectedTokenException : ParseErrorException
{
    #region Public Properties

    /// <summary>
    /// Error message.
    /// </summary>
    public override string Message { get; }

    #endregion

    #region Constructor

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="lineNumber">Line number where the token was found.</param>
	/// <param name="columnNumber">Column number where the token was found.</param>
	/// <param name="unexpectedToken">The token found.</param>
	/// <param name="expectedTokens">The list of tokens that were expected/allowed.</param>
    public UnexpectedTokenException(int lineNumber, int columnNumber, TokenType unexpectedToken, params TokenType[] expectedTokens) :
		base(lineNumber, columnNumber)
    {
		StringBuilder errorMsg = new StringBuilder($"An unexpected token was found.\nToken: '{unexpectedToken}'.\nAt line {lineNumber}, column {columnNumber}.");

		// Add a list of acceptable tokens.
		errorMsg.Append("\nExpected: ");
		foreach (TokenType item in expectedTokens)
		{
			errorMsg.Append($"{item}, ");
		}
		// Remove last comma and space.
		errorMsg.Remove(errorMsg.Length - 2, 2);

		Message = errorMsg.ToString();
    }

    #endregion

} // End class.