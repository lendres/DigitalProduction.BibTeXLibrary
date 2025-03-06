using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class UnexpectedTokenExceptionTest
{
    [Fact]
    public void TestConstructor()
    {
        var exception = new UnexpectedTokenException(1, 10, TokenType.EOF, TokenType.Comma, TokenType.RightBrace);
        Assert.Equal(1,  exception.LineNumber);
        Assert.Equal(10, exception.ColumnNumber);
        Assert.Equal("An unexpected token was found.\nToken: 'EOF'.\nAt line 1, column 10.\nExpected: Comma, RightBrace", exception.Message);
    }

} // End class.