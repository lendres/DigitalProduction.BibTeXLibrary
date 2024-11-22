using BibTeXLibrary;
using System.ComponentModel;
using System.Text;

namespace UnitTest;

[TestClass]
public class BibParserTest
{
    [TestMethod]
    public void TestParserRegularBibEntry()
    {
		BibParser parser = new(new StringReader("@Article{keyword, title = {\"0\"{123}456{789}}, year = 2012, address=\"PingLeYuan\"}"));
		BibEntry entry = parser.Parse().BibliographyEntries[0];

		Assert.AreEqual("Article"           , entry.Type);
		Assert.AreEqual("\"0\"{123}456{789}", entry.Title);
		Assert.AreEqual("2012"              , entry.Year);
		Assert.AreEqual("PingLeYuan"        , entry.Address);

		parser.Dispose();
    }

    [TestMethod]
    public void TestParserString()
    {
		BibParser parser = new(new StringReader("@article{keyword, title = \"hello \\\"world\\\"\", address=\"Ping\" # \"Le\" # \"Yuan\",}"));
		BibEntry entry = parser.Parse().BibliographyEntries[0];

		Assert.AreEqual("article"            , entry.Type);
		Assert.AreEqual("hello \\\"world\\\"", entry.Title);
		Assert.AreEqual("PingLeYuan"         , entry.Address);

		parser.Dispose();
    }

    [TestMethod]
    public void TestParserWithoutKey()
    {
		BibParser parser = new(new StringReader("@book{, title = {}}"));
		BibEntry entry = parser.Parse().BibliographyEntries[0];

		Assert.AreEqual("book", entry.Type);
		Assert.AreEqual(""    , entry.Title);

		parser.Dispose();
    }

    [TestMethod]
    public void TestParserWithoutKeyAndTags()
    {
		BibParser parser = new(new StringReader("@book{}"));
		BibEntry entry = parser.Parse().BibliographyEntries[0];

		Assert.AreEqual("book", entry.Type);

		parser.Dispose();
    }

    [TestMethod]
    public void TestParserWithBorkenBibEntry()
    {
		using BibParser parser = new(new StringReader("@book{,"));
		Assert.ThrowsException<UnexpectedTokenException>(() => parser.Parse());
	}

    [TestMethod]
    public void TestParserWithIncompletedTag()
    {
		using BibParser parser = new(new StringReader("@book{,title=,}"));
		Assert.ThrowsException<UnexpectedTokenException>(() => parser.Parse());
	}

    [TestMethod]
    public void TestParserWithBrokenTag()
    {
		using BibParser parser = new(new StringReader("@book{,titl"));
		Assert.ThrowsException<UnexpectedTokenException>(() => parser.Parse());
	}

    [TestMethod]
    public void TestParserWithBrokenNumber()
    {
		using BibParser parser = new(new StringReader("@book{,title = 2014"));
		Assert.ThrowsException<UnexpectedTokenException>(() => parser.Parse());
	}

    [TestMethod]
    public void TestParserWithUnexpectedCharacter()
    {
		using BibParser parser = new(new StringReader("@book{,ti?le = {Hadoop}}"));
		Assert.ThrowsException<UnrecognizableCharacterException>(() => parser.Parse());
	}

    [TestMethod]
    public void TestParserWithBibFile()
    {
		BibParser parser = new(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default));
		BindingList<BibEntry> entries = parser.Parse().BibliographyEntries;

		Assert.AreEqual(4,														entries.Count);
		Assert.AreEqual("nobody",												entries[0].Publisher);
		Assert.AreEqual("Apache hadoop yarn: Yet another resource negotiator",	entries[1].Title);
		Assert.AreEqual("KalavriShang-797",										entries[2].Key);
		parser.Dispose();
    }

    [TestMethod]
    public void TestStaticParseWithBibFile()
    {
		BindingList<BibEntry> entries = BibParser.Parse(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default)).BibliographyEntries;

		Assert.AreEqual(4,														entries.Count);
		Assert.AreEqual("nobody",												entries[0].Publisher);
		Assert.AreEqual("Apache hadoop yarn: Yet another resource negotiator",	entries[1].Title);
		Assert.AreEqual("KalavriShang-797",										entries[2].Key);
    }

    [TestMethod]
    public void TestParserResult()
    {
		BibParser parser = new(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default));
		BibEntry entry = parser.Parse().BibliographyEntries[0];

		StreamReader sr = new("TestData/BibParserTest1_Out1.bib", Encoding.Default);
		string expected = sr.ReadToEnd().Replace("\r", "");

		Assert.AreEqual(expected, entry.ToString());

		parser.Dispose();
    }

	[TestMethod]
	public void TestParserBibStringWithBrackets()
	{
		BibParser parser = new(new StringReader("@string{NAME = {Title of Conference}}"));
		StringConstantPart entry = parser.Parse().StringConstants[0];

		Assert.AreEqual("string", entry.Type);
		Assert.AreEqual("Title of Conference", entry["NAME"]);

		parser.Dispose();
	}

	[TestMethod]
	public void TestParserBibStringWithParentheses()
	{
		BibParser parser = new(new StringReader("@string(NAME = {Title of Conference})"));
		StringConstantPart entry = parser.Parse().StringConstants[0];

		Assert.AreEqual("string", entry.Type);
		Assert.AreEqual("Title of Conference", entry["NAME"]);

		parser.Dispose();
	}

	[TestMethod]
	public void TestParserBibStringWithBracketsAndQuotes()
	{
		BibParser parser = new(new StringReader("@string{NAME = \"Title of Conference\"}"));
		StringConstantPart entry = parser.Parse().StringConstants[0];

		Assert.AreEqual("string", entry.Type);
		Assert.AreEqual("Title of Conference", entry["NAME"]);

		parser.Dispose();
	}

	[TestMethod]
	public void TestParserBibStringWithParenthesisAndQuotes()
	{
		BibParser parser = new(new StringReader("@string(NAME = \"Title of Conference\")"));
		StringConstantPart entry = parser.Parse().StringConstants[0];

		Assert.AreEqual("string", entry.Type);
		Assert.AreEqual("Title of Conference", entry["NAME"]);

		parser.Dispose();
	}

} // End class.