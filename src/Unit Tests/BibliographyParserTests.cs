using BibTeXLibrary;
using System.Collections.ObjectModel;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace DigitalProduction.UnitTests;

public class BibliographyParserTests
{
	#region Basic BibEntry Parsing

	[Fact]
    public void TestParserRegularBibEntry()
    {
		BibliographyParser parser = new(new StringReader("@Article{keyword, title = {\"0\"{123}456{789}}, year = 2012, address=\"PingLeYuan\"}"));
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
		BibliographyParser parser = new(new StringReader("@article{keyword, title = \"hello \\\"world\\\"\", address=\"Ping\" # \"Le\" # \"Yuan\",}"));
		BibEntry entry = parser.Parse().Entries[0];

		Assert.Equal("article"            , entry.Type);
		Assert.Equal("hello \\\"world\\\"", entry.Title);
		Assert.Equal("PingLeYuan"         , entry.Address);

		parser.Dispose();
    }

    [Fact]
    public void TestParserWithoutKey()
    {
		BibliographyParser parser = new(new StringReader("@book{, title = {}}"));
		BibEntry entry = parser.Parse().Entries[0];

		Assert.Equal("book", entry.Type);
		Assert.Equal(""    , entry.Title);

		parser.Dispose();
    }

    [Fact]
    public void TestParserWithoutKeyAndFields()
    {
		BibliographyParser parser = new(new StringReader("@book{}"));
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
		BibliographyParser		parser	= new(new StringReader(content));
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
		BibliographyParser		parser	= new(new StringReader(content));
		StringEntry	entry	= parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("value", entry.Value);

		parser.Dispose();
    }

	[Fact]
    public void TestParserStringConstantWithInternalBrackets()
    {
		BibliographyParser		parser	= new(new StringReader("@string(key = {The {VALUE}})"));
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
		using BibliographyParser parser = new(new StringReader("@book{,"));
		Assert.Throws<UnexpectedTokenException>(() => parser.Parse());
	}

    [Fact]
    public void TestParserWithIncompletedField()
    {
		using BibliographyParser parser = new(new StringReader("@book{,title=,}"));
		Assert.Throws<UnexpectedTokenException>(() => parser.Parse());
	}

    [Fact]
    public void TestParserWithBrokenField()
    {
		using BibliographyParser parser = new(new StringReader("@book{,titl"));
		Assert.Throws<UnexpectedTokenException>(() => parser.Parse());
	}

    [Fact]
    public void TestParserWithBrokenNumber()
    {
		using BibliographyParser parser = new(new StringReader("@book{,title = 2014"));
		Assert.Throws<UnexpectedTokenException>(() => parser.Parse());
	}

    [Fact]
    public void TestParserWithUnexpectedCharacter()
    {
		using BibliographyParser parser = new(new StringReader("@book{,ti?le = {Hadoop}}"));
		Assert.Throws<UnrecognizableCharacterException>(() => parser.Parse());
	}

	#endregion

	#region Reading from a File

    [Fact]
    public void TestParserWithBibFile()
    {
		BibliographyParser parser = new(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default));
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
		ObservableCollection<BibEntry> entries = BibliographyParser.Parse(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default)).Entries;

		Assert.Equal(4,														entries.Count);
		Assert.Equal("nobody",												entries[0].Publisher);
		Assert.Equal("Apache hadoop yarn: Yet another resource negotiator",	entries[1].Title);
		Assert.Equal("KalavriShang-797",									entries[2].Key);
    }

    [Fact]
    public void TestParserResult()
    {
		BibliographyParser parser	= new(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default));
		BibEntry entry				= parser.Parse().Entries[0];
		string entryString			= entry.ToString().TrimEnd('\n').TrimEnd('\r');
		string expected				= "@Article{mrx05,\r\n  author = {Mr. X},\r\n  title = {Something Great},\r\n  publisher = {nobody},\r\n  year = {2005}\r\n}";

		Assert.Equal(expected, entryString);
		parser.Dispose();
    }

	#endregion

	#region Syntax Variations

	[Fact]
	public void TestParserBibStringWithBrackets()
	{
		BibliographyParser parser = new(new StringReader("@string{NAME = {Title of Conference}}"));
		StringEntry entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithParentheses()
	{
		BibliographyParser parser = new(new StringReader("@string(NAME = {Title of Conference})"));
		StringEntry entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithBracketsAndQuotes()
	{
		BibliographyParser parser = new(new StringReader("@string{NAME = \"Title of Conference\"}"));
		StringEntry entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithParenthesisAndQuotes()
	{
		BibliographyParser parser = new(new StringReader("@string(NAME = \"Title of Conference\")"));
		StringEntry entry = parser.Parse().StringConstants[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	#endregion

	#region Header Parsing

	[Fact]
	public void HeaderPreservesHeaderComments()
	{
		string bibString =
			"% First header line\n" +
			"% Second header line\n" +
			"@book{ref:keyA1, author = {John Smith}, year = {2023}}";

		BibliographyDOM bibliographyDom = ParseBibEntry(bibString);

		List<string> lines = GetHeaderAsSeparateLines(bibliographyDom.Header);

		Assert.Equal("% First header line", lines[0]);
		Assert.Equal("% Second header line", lines[1]);
		Assert.Single(bibliographyDom.Entries);
	}

	[Fact]
	public void BlankLineBetweenHeaderComments()
	{
		string bibString =
			"% First header line\n" +
			"% Second header line\n" +
			"\n" +
			"% Comment line\n" +
			"@book{ref:keyA1, author = {John Smith}, year = {2023}}";

		BibliographyDOM bibliographyDom = ParseBibEntry(bibString);

		List<string> lines = GetHeaderAsSeparateLines(bibliographyDom.Header);

		Assert.Equal(2, lines.Count);
		Assert.Equal("% First header line", lines[0]);
		Assert.Equal("% Second header line", lines[1]);
		Assert.Single(bibliographyDom.Entries);
	}

	private List<string> GetHeaderAsSeparateLines(string header)
	{
		string[] trimChars = new[] { "\r\n", "\r", "\n" };
		header = DigitalProduction.Strings.Format.TrimEnd(header, trimChars);
		return header.Split(trimChars, StringSplitOptions.None).ToList();
	}

	#endregion

	#region Helper Methods

	private static BibliographyDOM ParseBibEntry(string bibString)
	{
		BibliographyParser parser = new(new StringReader(bibString));
		return parser.Parse();
	}

	#endregion

} // End class.