using BibTeXLibrary;
using System.Collections.ObjectModel;
using System.Text;

namespace DigitalProduction.UnitTests;

public class BibParserTests
{
	#region Basic BibEntry Parsing

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
    public void TestParserStringConcatenation()
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
    public void TestParserWithoutKeyAndFields()
    {
		BibParser parser = new(new StringReader("@book{}"));
		BibEntry entry = parser.Parse().Entries[0];

		Assert.Equal("book", entry.Type);

		parser.Dispose();
    }

	#endregion

	#region Basic String Constant Parsing

	[Theory]
	[InlineData("@string(key = \"value\")")]
	[InlineData("@String(key = \"value\")")]
	[InlineData("@STRING(key = \"value\")")]
	public void TestParserBasicStringConstant(string content)
    {
		BibParser		parser	= new(new StringReader(content));
		StringEntry	entry	= parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("value", entry.Value);

		parser.Dispose();
    }

	[Theory]
	[InlineData("@string(key = {value})")]
	[InlineData("@string{key = {value}}")]
	[InlineData("@string{key = \"value\"}")]
	public void TestParserStringConstantSyntax(string content)
    {
		BibParser		parser	= new(new StringReader(content));
		StringEntry	entry	= parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("value", entry.Value);

		parser.Dispose();
    }

	[Fact]
    public void TestParserStringConstantWithInternalBrackets()
    {
		BibParser		parser	= new(new StringReader("@string(key = {The {VALUE}})"));
		StringEntry	entry	= parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("The {VALUE}", entry.Value);

		parser.Dispose();

		parser	= new(new StringReader("@string{key = {The {VALUE}}}"));
		entry	= parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("The {VALUE}", entry.Value);

		parser.Dispose();

		parser	= new(new StringReader("@string{key = \"The {VALUE}\"}"));
		entry	= parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("The {VALUE}", entry.Value);

		parser.Dispose();
    }

	#endregion

	#region Exceptions

	[Fact]
    public void TestParserWithBorkenBibEntry()
    {
		using BibParser parser = new(new StringReader("@book{,"));
		Assert.Throws<UnexpectedTokenException>(() => parser.Parse());
	}

    [Fact]
    public void TestParserWithIncompletedField()
    {
		using BibParser parser = new(new StringReader("@book{,title=,}"));
		Assert.Throws<UnexpectedTokenException>(() => parser.Parse());
	}

    [Fact]
    public void TestParserWithBrokenField()
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

	#endregion

	#region Reading from a File

    [Fact]
    public void TestParserWithBibFile()
    {
		BibParser parser = new(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default));
		ObservableCollection<BibEntry> entries = parser.Parse().Entries;

		Assert.Equal(4,														entries.Count);
		Assert.Equal("nobody",												entries[0].Publisher);
		Assert.Equal("Apache hadoop yarn: Yet another resource negotiator",	entries[1].Title);
		Assert.Equal("KalavriShang-797",									entries[2].Key);
		parser.Dispose();
    }

    [Fact]
    public void TestStaticParseWithBibFile()
    {
		ObservableCollection<BibEntry> entries = BibParser.Parse(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default)).Entries;

		Assert.Equal(4,														entries.Count);
		Assert.Equal("nobody",												entries[0].Publisher);
		Assert.Equal("Apache hadoop yarn: Yet another resource negotiator",	entries[1].Title);
		Assert.Equal("KalavriShang-797",									entries[2].Key);
    }

    [Fact]
    public void TestParserResult()
    {
		BibParser parser	= new(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default));
		BibEntry entry		= parser.Parse().Entries[0];
		string entryString	= entry.ToString().TrimEnd('\n').TrimEnd('\r');
		string expected		= "@Article{mrx05,\r\n  author = {Mr. X},\r\n  title = {Something Great},\r\n  publisher = {nobody},\r\n  year = {2005}\r\n}";

		Assert.Equal(expected, entryString);
		parser.Dispose();
    }

	#endregion

	#region Syntax Variations

	[Fact]
	public void TestParserBibStringWithBrackets()
	{
		BibParser parser = new(new StringReader("@string{NAME = {Title of Conference}}"));
		StringEntry entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithParentheses()
	{
		BibParser parser = new(new StringReader("@string(NAME = {Title of Conference})"));
		StringEntry entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithBracketsAndQuotes()
	{
		BibParser parser = new(new StringReader("@string{NAME = \"Title of Conference\"}"));
		StringEntry entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithParenthesisAndQuotes()
	{
		BibParser parser = new(new StringReader("@string(NAME = \"Title of Conference\")"));
		StringEntry entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	#endregion

	#region Header Parsing

	[Fact]
	public void ReadPreservesHeaderComments()
	{
		string bibString =
			"% First header line\n" +
			"% Second header line\n" +
			"@book{ref:keyA1, author = {John Smith}, year = {2023}}";

		BibliographyDOM bibliographyDom = ParseBibEntry(bibString);

		Assert.Equal(2, bibliographyDom.Header.Count);
		Assert.Equal("% First header line", bibliographyDom.Header[0]);
		Assert.Equal("% Second header line", bibliographyDom.Header[1]);
		Assert.Single(bibliographyDom.Entries);
	}

	[Fact]
	public void ReadPreservesBlankLineBetweenHeaderComments()
	{
		string bibString =
			"% First header line\n" +
			"% Second header line\n" +
			"\n" +
			"% Comment line\n" +
			"@book{ref:keyA1, author = {John Smith}, year = {2023}}";

		BibliographyDOM bibliographyDom = ParseBibEntry(bibString);

		Assert.Equal(3, bibliographyDom.Header.Count);
		Assert.Equal("% First header line", bibliographyDom.Header[0]);
		Assert.Equal("% Second header line", bibliographyDom.Header[1]);
		Assert.Single(bibliographyDom.Entries);
	}

	#endregion

	#region Helper Methods

	private static BibliographyDOM ParseBibEntry(string bibString)
	{
		BibParser parser = new(new StringReader(bibString));
		return parser.Parse();
	}

	#endregion

} // End class.