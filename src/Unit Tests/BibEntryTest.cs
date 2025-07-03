using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class BibEntryTest
{
    [Fact]
    public void TestIndexer()
    {
        const string title = "Mapreduce";
        BibEntry entry = new() {["Title"] = title};

        Assert.Equal(title, entry["title"]);
        Assert.Equal(title, entry["Title"]);
        Assert.Equal(title, entry["TitlE"]);
    }

    [Fact]
    public void TestProperty()
    {
        const string title = "Mapreduce";
        BibEntry entry = new() {["Title"] = title};

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
        const string title = "Mapreduce";
		BibEntry entry = new();
		entry.SetTagValue("Title", title, TagValueType.StringConstant);
		Assert.Equal(title, entry.Title);
		Assert.Equal(title, entry.GetTagValue("title").ToString());

		entry.SetTagValue("Title", title, TagValueType.String);
        Assert.Equal(title, entry.Title);
        Assert.Equal("{"+title+"}", entry.GetTagValue("title").ToString());
    }

    [Fact]
    public void TestFindTagValue()
	{
		string tagValue		= "SPE Drilling Conference and Exhibition";
		string bibString	= "@book{ref:key, booktitle = \"" + tagValue + "\", author = {Author}, year = {2023}}";
		string key			= ParseAndGetKey(tagValue, bibString);
		Assert.Equal("booktitle", key);

		bibString	= "@book{ref:key, booktitle = {" + tagValue + "}, author = {Author}, year = {2023}}";
		key			= ParseAndGetKey(tagValue, bibString);
		Assert.Equal("booktitle", key);
	}

    [Fact]
    public void TestSearchInTagValues()
    {
		string tagValue		= "Acme Journal of Science";
		string bibString	= "@book{ref:key, booktitle = \"" + tagValue + "\", author = {John Smith}, year = {2023}, abstract = {Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}}";

		List<string> tagNames = ["booktitle", "author", "year", "abstract"];
		BibEntry entry = ParseBibEntry(bibString);
		Assert.True(entry.DoesTagsContainString(tagNames, "acme"));

	}

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

} // End class.