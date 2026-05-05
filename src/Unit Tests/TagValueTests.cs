using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class TagValueTests
{
	[Theory]
	[InlineData(FieldValueFormat.Bracket, "{Mapreduce}")]
	[InlineData(FieldValueFormat.Quote, "\"Mapreduce\"")]
	[InlineData(FieldValueFormat.None, "Mapreduce")]
	public void TestToStringWithFormat(FieldValueFormat format, string expected)
	{
		TagValue tagValue = new("Mapreduce");

		string actual = tagValue.ToString(format);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TestToStringWithFormatUsesCurrentContent()
	{
		TagValue tagValue = new("Original");

		tagValue.Content = "Updated";

		Assert.Equal("{Updated}", tagValue.ToString(FieldValueFormat.Bracket));
		Assert.Equal("\"Updated\"", tagValue.ToString(FieldValueFormat.Quote));
		Assert.Equal("Updated", tagValue.ToString(FieldValueFormat.None));
	}

	[Fact]
	public void TestDefaultConstructorInitializesEmptyContent()
	{
		TagValue tagValue = new();

		Assert.Equal("{}", tagValue.ToString(FieldValueFormat.Bracket));
		Assert.Equal("\"\"", tagValue.ToString(FieldValueFormat.Quote));
		Assert.Equal(string.Empty, tagValue.ToString(FieldValueFormat.None));
	}

	[Fact]
	public void TestToStringWithInvalidFormatThrowsException()
	{
		TagValue tagValue = new("Mapreduce");

		Exception exception = Assert.Throws<Exception>(() => tagValue.ToString((FieldValueFormat)999));

		Assert.Equal("Invalid tag format.", exception.Message);
	}
}