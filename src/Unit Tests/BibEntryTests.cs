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
    public void TestSettingTagType()
    {
        const string title	= "Mapreduce";
		BibEntry entry		= new();

		entry.SetTagValue("Title", title, TagValueType.StringConstant);
		Assert.Equal(title, entry.Title);
		Assert.Equal(title, entry.GetTagValue("title").ToString());

		entry.SetTagValue("Title", title, TagValueType.String);
        Assert.Equal(title, entry.Title);
        Assert.Equal("{"+title+"}", entry.GetTagValue("title").ToString());
    }

	#endregion

	#region Find and Search Tests

	/// <summary>
	/// Find key of a tag value by searching through the tag values.
	/// </summary>
	[Fact]
    public void TestFindTagValue()
	{
		// Test with quotes.
		string tagValue		= "SPE Drilling Conference and Exhibition";
		string bibString	= "@book{ref:key, booktitle = \"" + tagValue + "\", author = {Author}, year = {2023}}";
		string key			= ParseAndGetKey(tagValue, bibString);
		Assert.Equal("booktitle", key);

		// Test with braces.
		bibString = "@book{ref:key, booktitle = {" + tagValue + "}, author = {Author}, year = {2023}}";
		key			= ParseAndGetKey(tagValue, bibString);
		Assert.Equal("booktitle", key);
	}

	/// <summary>
	/// Verifies that the DoTagsContainString method correctly identifies when a specified string is present in the values
	/// of given BibTeX entry tags.
	/// </summary>
	/// <remarks>
	/// This test ensures that searching for a substring within multiple tag values returns the expected
	/// result. It demonstrates usage with a typical BibTeX entry and a list of common tag names.
	/// </remarks>
	[Fact]
    public void TestSearchInTagValues()
    {
		string tagValue		= "Acme Journal of Science";
		string bibString	= "@book{ref:key, booktitle = \"" + tagValue + "\", author = {John Smith}, year = {2023}, abstract = {Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}}";

		List<string> tagNames	= ["booktitle", "author", "year", "abstract"];
		BibEntry entry			= ParseBibEntry(bibString);

		// Case insensitive search should find "acme" in the booktitle tag.
		Assert.True(entry.DoTagsContainString(tagNames, "acme"));

		// Case sensitive search should not find "acme" in the booktitle tag.
		Assert.False(entry.DoTagsContainString(tagNames, "acme", true));
	}

	#endregion

	#region Helper Methods

	private static string ParseAndGetKey(string tagValue, string bibString)
	{
		BibEntry entry = ParseBibEntry(bibString);
		return entry.FindTagValue(tagValue);
	}

	private static BibEntry ParseBibEntry(string bibString)
	{
		BibParser parser = new(new StringReader(bibString));
		return parser.Parse().Entries[0];
	}

	#endregion

} // End class.