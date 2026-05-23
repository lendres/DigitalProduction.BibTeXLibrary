using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class BibEntryTests
{
	#region Indexer and Property Tests

	[Fact]
    public void TestIndexer()
    {
        const string title	= "Mapreduce";
        BibEntry entry		= new() {["Title"] = title};

        Assert.Equal(title, entry["title"]);
        Assert.Equal(title, entry["Title"]);
        Assert.Equal(title, entry["TitlE"]);
    }

    [Fact]
    public void TestProperty()
    {
        const string title	= "Mapreduce";
        BibEntry entry		= new() {["Title"] = title};

        Assert.Equal(title, entry.Title);
    }

    [Fact]
    public void TestSetType()
    {
        BibEntry entry = new() {Type = "inbook"};
        Assert.Equal("inbook", entry.Type);

        entry.Type = "inBoOK";
        Assert.Equal("inBoOK", entry.Type);
    }

    [Fact]
    public void TestSettingFieldType()
    {
        const string title	= "Mapreduce";
		BibEntry entry		= new();

		// Test setting as a string constant. In this case, it should not matter what we pass in for the field value type, as the
		// property should always be set to a string constant.
		entry.SetField("Title", title, FieldValueType.StringConstant);
		Assert.Equal(title, entry.Title);
		Assert.Equal(title, entry.GetField("title").FieldValue.ToString(FieldValueFormat.Quotes));

		// Test setting as a standard string. In this case, the field value type should be set to what we pass in.
		entry.SetField("Title", title, FieldValueType.String);
        Assert.Equal(title, entry.Title);
        Assert.Equal("{"+title+"}", entry.GetField("title").FieldValue.ToString(FieldValueFormat.CurlyBraces));
    }

	#endregion

	#region Copy Constructor Tests

	[Fact]
	public void TestCopyConstructorCopiesBibEntryValues()
	{
		BibEntry originalEntry = new()
		{
			Type	= "article",
			Key		= "Dean2008",
			Comment	= "% Comment before entry\n"
		};

		originalEntry.SetField("title", "Mapreduce", FieldValueType.String);
		originalEntry.SetField("journal", "CACM", FieldValueType.StringConstant);

		BibEntry copiedEntry = new(originalEntry);

		Assert.Equal(originalEntry.Type, copiedEntry.Type);
		Assert.Equal(originalEntry.Key, copiedEntry.Key);
		Assert.Equal(originalEntry.Comment, copiedEntry.Comment);
		Assert.Equal(originalEntry.Title, copiedEntry.Title);
		Assert.Equal(originalEntry.Journal, copiedEntry.Journal);
		Assert.Equal(originalEntry.GetField("journal").FieldValue, copiedEntry.GetField("journal").FieldValue);
	}

	[Fact]
	public void TestCopyConstructorCreatesIndependentBibEntryFields()
	{
		BibEntry originalEntry = new()
		{
			Type	= "article",
			Key		= "Dean2008"
		};

		originalEntry.SetField("title", "Mapreduce", FieldValueType.String);

		BibEntry copiedEntry = new(originalEntry);

		Assert.NotSame(originalEntry.GetField("title"), copiedEntry.GetField("title"));
		Assert.NotSame(originalEntry.GetField("title").FieldValue, copiedEntry.GetField("title").FieldValue);

		originalEntry.Title = "Updated Title";

		Assert.Equal("Updated Title", originalEntry.Title);
		Assert.Equal("Mapreduce", copiedEntry.Title);
	}

	#endregion

	#region Find and Search Tests

	/// <summary>
	/// Find key of a field value by searching through the field values.
	/// </summary>
	[Fact]
    public void TestFindFieldValue()
	{
		// Test with quotes.
		string fieldValue	= "SPE Drilling Conference and Exhibition";
		string bibString	= "@book{ref:key, booktitle = \"" + fieldValue + "\", author = {Author}, year = {2023}}";
		string key			= ParseAndGetKey(fieldValue, bibString);
		Assert.Equal("booktitle", key);

		// Test with braces.
		bibString	= "@book{ref:key, booktitle = {" + fieldValue + "}, author = {Author}, year = {2023}}";
		key			= ParseAndGetKey(fieldValue, bibString);
		Assert.Equal("booktitle", key);
	}

	/// <summary>
	/// Verifies that the DoFieldsContainString method correctly identifies when a specified string is present in the values
	/// of given BibTeX entry fields.
	/// </summary>
	/// <remarks>
	/// This test ensures that searching for a substring within multiple field values returns the expected
	/// result. It demonstrates usage with a typical BibTeX entry and a list of common field names.
	/// </remarks>
	[Fact]
    public void TestSearchInFieldValues()
    {
		string fieldValue		= "Acme Journal of Science";
		string bibString		= "@book{ref:key, booktitle = \"" + fieldValue + "\", author = {John Smith}, year = {2023}, abstract = {Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}}";

		List<string> fieldNames	= ["booktitle", "author", "year", "abstract"];
		BibEntry entry			= ParseBibEntry(bibString);

		// Case insensitive search should find "acme" in the booktitle field.
		Assert.True(entry.DoFieldsContainString(fieldNames, "acme"));

		// Case sensitive search should not find "acme" in the booktitle field.
		Assert.False(entry.DoFieldsContainString(fieldNames, "acme", true));
	}

	#endregion

	#region String Writing Tests

	[Theory]
	[InlineData(EntryBracketType.CurlyBraces, "@article{Dean2008,")]
	[InlineData(EntryBracketType.CurlyBraces, "}")]
	[InlineData(EntryBracketType.Parentheses, "@article(Dean2008,")]
	[InlineData(EntryBracketType.Parentheses, ")")]
	public void TestToStringWithWriteSettingsBracketType(EntryBracketType bracketType, string expected)
	{
		BibEntry entry = new() { Type = "article", Key = "Dean2008" };
		entry["title"] = "Mapreduce";

		WriteSettings writeSettings = new()
		{
			BibEntryFieldValueFormat	= FieldValueFormat.Quotes,
			BibEntryBracketType		= bracketType
		};

		string result = entry.ToString(writeSettings);

		Assert.Contains(expected, result);
	}

	[Theory]
	[InlineData(FieldValueFormat.CurlyBraces, "title = {Mapreduce}")]
	[InlineData(FieldValueFormat.Quotes, "title = \"Mapreduce\"")]
	[InlineData(FieldValueFormat.None, "title = Mapreduce")]
	public void TestToStringWithWriteSettingsFormatsFieldValues(FieldValueFormat fieldValueFormat, string expectedFieldLine)
	{
		BibEntry entry = new() { Type = "article", Key = "Dean2008" };
		entry["title"] = "Mapreduce";

		WriteSettings writeSettings = new()
		{
			AlignFieldValues			= false,
			WhiteSpace					= WhiteSpace.Space,
			BibEntryFieldValueFormat	= fieldValueFormat,
			BibEntryBracketType			= EntryBracketType.CurlyBraces
		};

		string result = entry.ToString(writeSettings);

		Assert.Contains("@article{Dean2008,", result);
		Assert.Contains(expectedFieldLine, result);
		Assert.Contains("}", result);
	}

	[Fact]
	public void TestToStringWithWriteSettingsFormatsMultipleFieldValues()
	{
		BibEntry entry = new()
		{
			Type	= "article",
			Key		= "Dean2008"
		};

		entry["title"]	= "Mapreduce";
		entry["author"]	= "Jeffrey Dean and Sanjay Ghemawat";
		entry["year"]	= "2008";

		WriteSettings writeSettings = new()
		{
			AlignFieldValues			= false,
			WhiteSpace					= WhiteSpace.Space,
			BibEntryFieldValueFormat	= FieldValueFormat.CurlyBraces
		};

		string result = entry.ToString(writeSettings);

		Assert.Contains("title = {Mapreduce}", result);
		Assert.Contains("author = {Jeffrey Dean and Sanjay Ghemawat}", result);
		Assert.Contains("year = {2008}", result);
	}

	#endregion

	#region Helper Methods

	private static string ParseAndGetKey(string fieldValue, string bibString)
	{
		BibEntry entry = ParseBibEntry(bibString);
		return entry.FindNameByValue(fieldValue);
	}

	private static BibEntry ParseBibEntry(string bibString)
	{
		BibliographyParser parser = new(new StringReader(bibString));
		return parser.Parse().BibliographyEntries[0];
	}

	#endregion

} // End class.