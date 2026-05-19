using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class FieldValueTests
{
	[Theory]
	[InlineData(FieldValueFormat.CurlyBraces, "{Mapreduce}")]
	[InlineData(FieldValueFormat.Quotes, "\"Mapreduce\"")]
	[InlineData(FieldValueFormat.None, "Mapreduce")]
	public void TestToStringWithFormat(FieldValueFormat format, string expected)
	{
		FieldValue fieldValue = new("Mapreduce");

		string actual = fieldValue.ToString(format);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TestToStringWithFormatUsesCurrentContent()
	{
		FieldValue fieldValue = new("Original");

		fieldValue.Content = "Updated";

		Assert.Equal("{Updated}", fieldValue.ToString(FieldValueFormat.CurlyBraces));
		Assert.Equal("\"Updated\"", fieldValue.ToString(FieldValueFormat.Quotes));
		Assert.Equal("Updated", fieldValue.ToString(FieldValueFormat.None));
	}

	[Fact]
	public void TestDefaultConstructorInitializesEmptyContent()
	{
		FieldValue fieldValue = new();

		Assert.Equal("{}", fieldValue.ToString(FieldValueFormat.CurlyBraces));
		Assert.Equal("\"\"", fieldValue.ToString(FieldValueFormat.Quotes));
		Assert.Equal(string.Empty, fieldValue.ToString(FieldValueFormat.None));
	}

	[Fact]
	public void TestToStringWithInvalidFormatThrowsException()
	{
		FieldValue fieldValue = new("Mapreduce");

		Exception exception = Assert.Throws<Exception>(() => fieldValue.ToString((FieldValueFormat)999));

		Assert.Equal("Invalid field format.", exception.Message);
	}
}