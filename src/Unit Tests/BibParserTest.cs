using BibTeXLibrary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace DigitalProduction.UnitTests;

public class BibParserTest
{
    [Fact]
    public void TestParserRegularBibEntry()
    {
		BibParser parser = new(new StringReader("@Article{keyword, title = {\"0\"{123}456{789}}, year = 2012, address=\"PingLeYuan\"}"));
		BibEntry entry = parser.Parse().Entries[0];

		Assert.Equal("Article"           , entry.Type);
		Assert.Equal("\"0\"{123}456{789}", entry.Title);
		Assert.Equal("2012"              , entry.Year);
		Assert.Equal("PingLeYuan"        , entry.Address);

		parser.Dispose();
    }

    [Fact]
    public void TestParserString()
    {
		BibParser parser = new(new StringReader("@article{keyword, title = \"hello \\\"world\\\"\", address=\"Ping\" # \"Le\" # \"Yuan\",}"));
		BibEntry entry = parser.Parse().Entries[0];

		Assert.Equal("article"            , entry.Type);
		Assert.Equal("hello \\\"world\\\"", entry.Title);
		Assert.Equal("PingLeYuan"         , entry.Address);

		parser.Dispose();
    }

    [Fact]
    public void TestParserWithoutKey()
    {
		BibParser parser = new(new StringReader("@book{, title = {}}"));
		BibEntry entry = parser.Parse().Entries[0];

		Assert.Equal("book", entry.Type);
		Assert.Equal(""    , entry.Title);

		parser.Dispose();
    }

    [Fact]
    public void TestParserWithoutKeyAndTags()
    {
		BibParser parser = new(new StringReader("@book{}"));
		BibEntry entry = parser.Parse().Entries[0];

		Assert.Equal("book", entry.Type);

		parser.Dispose();
    }

    [Fact]
    public void TestParserWithBorkenBibEntry()
    {
		using BibParser parser = new(new StringReader("@book{,"));
		Assert.Throws<UnexpectedTokenException>(() => parser.Parse());
	}

    [Fact]
    public void TestParserWithIncompletedTag()
    {
		using BibParser parser = new(new StringReader("@book{,title=,}"));
		Assert.Throws<UnexpectedTokenException>(() => parser.Parse());
	}

    [Fact]
    public void TestParserWithBrokenTag()
    {
		using BibParser parser = new(new StringReader("@book{,titl"));
		Assert.Throws<UnexpectedTokenException>(() => parser.Parse());
	}

    [Fact]
    public void TestParserWithBrokenNumber()
    {
		using BibParser parser = new(new StringReader("@book{,title = 2014"));
		Assert.Throws<UnexpectedTokenException>(() => parser.Parse());
	}

    [Fact]
    public void TestParserWithUnexpectedCharacter()
    {
		using BibParser parser = new(new StringReader("@book{,ti?le = {Hadoop}}"));
		Assert.Throws<UnrecognizableCharacterException>(() => parser.Parse());
	}

    [Fact]
    public void TestParserWithBibFile()
    {
		BibParser parser = new(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default));
		ObservableCollection<BibEntry> entries = parser.Parse().Entries;

		Assert.Equal(4,														entries.Count);
		Assert.Equal("nobody",												entries[0].Publisher);
		Assert.Equal("Apache hadoop yarn: Yet another resource negotiator",	entries[1].Title);
		Assert.Equal("KalavriShang-797",										entries[2].Key);
		parser.Dispose();
    }

    [Fact]
    public void TestStaticParseWithBibFile()
    {
		ObservableCollection<BibEntry> entries = BibParser.Parse(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default)).Entries;

		Assert.Equal(4,														entries.Count);
		Assert.Equal("nobody",												entries[0].Publisher);
		Assert.Equal("Apache hadoop yarn: Yet another resource negotiator",	entries[1].Title);
		Assert.Equal("KalavriShang-797",										entries[2].Key);
    }

    [Fact]
    public void TestParserResult()
    {
		BibParser parser = new(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default));
		BibEntry entry = parser.Parse().Entries[0];
		string entryString = entry.ToString().TrimEnd('\n').TrimEnd('\r');

		StreamReader sr = new("TestData/BibParserTest1_Out1.bib", Encoding.Default);
		string expected = sr.ReadToEnd();

		Assert.Equal(expected, entryString);
		parser.Dispose();
    }

	[Fact]
	public void TestParserBibStringWithBrackets()
	{
		BibParser parser = new(new StringReader("@string{NAME = {Title of Conference}}"));
		StringConstantPart entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringConstantPart.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithParentheses()
	{
		BibParser parser = new(new StringReader("@string(NAME = {Title of Conference})"));
		StringConstantPart entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringConstantPart.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithBracketsAndQuotes()
	{
		BibParser parser = new(new StringReader("@string{NAME = \"Title of Conference\"}"));
		StringConstantPart entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringConstantPart.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithParenthesisAndQuotes()
	{
		BibParser parser = new(new StringReader("@string(NAME = \"Title of Conference\")"));
		StringConstantPart entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringConstantPart.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

} // End class.