using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class UnrecognizableCharacterExceptionTest
{
    [Fact]
    public void TestConstructor()
    {
        var exception = new UnrecognizableCharacterException(1, 10, '?');
        Assert.Equal(1,  exception.LineNumber);
        Assert.Equal(10, exception.ColumnNumber);
        Assert.Equal("An unexpected character was found.\nCharacter: '?'.\nAt line 2, column 11.", exception.Message);
	}

}// End class.