using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class TagValueTests
{
	[Theory]
	[InlineData(TagValueFormat.Bracket, "{Mapreduce}")]
	[InlineData(TagValueFormat.Quote, "\"Mapreduce\"")]
	[InlineData(TagValueFormat.None, "Mapreduce")]
	public void TestToStringWithFormat(TagValueFormat format, string expected)
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

		Assert.Equal("{Updated}", tagValue.ToString(TagValueFormat.Bracket));
		Assert.Equal("\"Updated\"", tagValue.ToString(TagValueFormat.Quote));
		Assert.Equal("Updated", tagValue.ToString(TagValueFormat.None));
	}

	[Fact]
	public void TestDefaultConstructorInitializesEmptyContent()
	{
		TagValue tagValue = new();

		Assert.Equal("{}", tagValue.ToString(TagValueFormat.Bracket));
		Assert.Equal("\"\"", tagValue.ToString(TagValueFormat.Quote));
		Assert.Equal(string.Empty, tagValue.ToString(TagValueFormat.None));
	}

	[Fact]
	public void TestToStringWithInvalidFormatThrowsException()
	{
		TagValue tagValue = new("Mapreduce");

		Exception exception = Assert.Throws<Exception>(() => tagValue.ToString((TagValueFormat)999));

		Assert.Equal("Invalid tag format.", exception.Message);
	}
}