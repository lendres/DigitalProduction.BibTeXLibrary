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
    public void TestToString()
    {
        //TODO:
    }

} // End class.