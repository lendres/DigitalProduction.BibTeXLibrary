using BibTeXLibrary;
using System.Collections.ObjectModel;
using System.Text;

namespace DigitalProduction.UnitTests;

public class BibliographyParserTests
{
	#region Basic BibEntry Parsing

	[Fact]
    public void TestParserRegularBibEntry()
    {
		BibliographyParser parser = new(new StringReader("@Article{keyword, title = {\"0\"{123}456{789}}, year = 2012, address=\"PingLeYuan\"}"));
		BibEntry entry = parser.Parse().BibliographyEntries[0];

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
		BibEntry entry = parser.Parse().BibliographyEntries[0];

		Assert.Equal("article"            , entry.Type);
		Assert.Equal("hello \\\"world\\\"", entry.Title);
		Assert.Equal("PingLeYuan"         , entry.Address);

		parser.Dispose();
    }

    [Fact]
    public void TestParserWithoutKey()
    {
		BibliographyParser parser = new(new StringReader("@book{, title = {}}"));
		BibEntry entry = parser.Parse().BibliographyEntries[0];

		Assert.Equal("book", entry.Type);
		Assert.Equal(""    , entry.Title);

		parser.Dispose();
    }

    [Fact]
    public void TestParserWithoutKeyAndFields()
    {
		BibliographyParser parser = new(new StringReader("@book{}"));
		BibEntry entry = parser.Parse().BibliographyEntries[0];

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
		StringEntry	entry	= parser.Parse().StringEntries[0];

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
		StringEntry	entry	= parser.Parse().StringEntries[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("value", entry.Value);

		parser.Dispose();
    }

	[Fact]
    public void TestParserStringConstantWithInternalBrackets()
    {
		BibliographyParser		parser	= new(new StringReader("@string(key = {The {VALUE}})"));
		StringEntry	entry	= parser.Parse().StringEntries[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("The {VALUE}", entry.Value);

		parser.Dispose();

		parser	= new(new StringReader("@string{key = {The {VALUE}}}"));
		entry	= parser.Parse().StringEntries[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("The {VALUE}", entry.Value);

		parser.Dispose();

		parser	= new(new StringReader("@string{key = \"The {VALUE}\"}"));
		entry	= parser.Parse().StringEntries[0];

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
		ObservableCollection<BibEntry> entries = parser.Parse().BibliographyEntries;

		Assert.Equal(4,														entries.Count);
		Assert.Equal("nobody",												entries[0].Publisher);
		Assert.Equal("Apache hadoop yarn: Yet another resource negotiator",	entries[1].Title);
		Assert.Equal("KalavriShang-797",									entries[2].Key);
		parser.Dispose();
    }

    [Fact]
    public void TestStaticParseWithBibFile()
    {
		ObservableCollection<BibEntry> entries = BibliographyParser.Parse(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default)).BibliographyEntries;

		Assert.Equal(4,														entries.Count);
		Assert.Equal("nobody",												entries[0].Publisher);
		Assert.Equal("Apache hadoop yarn: Yet another resource negotiator",	entries[1].Title);
		Assert.Equal("KalavriShang-797",									entries[2].Key);
    }

    [Fact]
    public void TestParserResult()
    {
		BibliographyParser parser	= new(new StreamReader("TestData/BibParserTest1_In.bib", Encoding.Default));
		BibEntry entry				= parser.Parse().BibliographyEntries[0];
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
		StringEntry entry = parser.Parse().StringEntries[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithParentheses()
	{
		BibliographyParser parser = new(new StringReader("@string(NAME = {Title of Conference})"));
		StringEntry entry = parser.Parse().StringEntries[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithBracketsAndQuotes()
	{
		BibliographyParser parser = new(new StringReader("@string{NAME = \"Title of Conference\"}"));
		StringEntry entry = parser.Parse().StringEntries[0];

		Assert.Equal(StringEntry.TypeString, entry.Type);
		Assert.Equal("Title of Conference", entry.Value);

		parser.Dispose();
	}

	[Fact]
	public void TestParserBibStringWithParenthesisAndQuotes()
	{
		BibliographyParser parser = new(new StringReader("@string(NAME = \"Title of Conference\")"));
		StringEntry entry = parser.Parse().StringEntries[0];

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

		Assert.Equal("% First header line" + Environment.NewLine + "% Second header line" + Environment.NewLine, bibliographyDom.Header);
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

		string solution = "% First header line" + Environment.NewLine + "% Second header line" + Environment.NewLine;
		Assert.Equal(solution, bibliographyDom.Header);
		Assert.Single(bibliographyDom.BibliographyEntries);
		Assert.Equal("% Comment line" + Environment.NewLine, bibliographyDom.BibliographyEntries[0].Comment);
	}

	#endregion

	#region Comment Parsing

	[Fact]
	public void BibEntryCanHavePrecedingComment()
	{
		string bibString = "\n% Entry comment\n" + "@book{key1, title = {Title 1}}";

		BibliographyDOM bibliographyDom = ParseBibEntry(bibString);

		Assert.Single(bibliographyDom.BibliographyEntries);
		Assert.Equal("% Entry comment" + Environment.NewLine, bibliographyDom.BibliographyEntries[0].Comment);
		Assert.Equal("key1", bibliographyDom.BibliographyEntries[0].Key);
	}

	[Fact]
	public void StringEntryCanHavePrecedingComment()
	{
		string bibString = "\n% String comment\n" + "@string{journalName = {Journal Name}}";

		BibliographyDOM bibliographyDom = ParseBibEntry(bibString);

		Assert.Single(bibliographyDom.StringEntries);
		Assert.Equal("% String comment" + Environment.NewLine, bibliographyDom.StringEntries[0].Comment);
		Assert.Equal("journalName", bibliographyDom.StringEntries[0].Name);
	}

	[Fact]
	public void CommentAppliesOnlyToImmediatelyFollowingPart()
	{
		string bibString =
			"\n% Entry comment\n" +
			"@book{key1, title = {Title 1}}\n" +
			"@book{key2, title = {Title 2}}";

		BibliographyDOM bibliographyDom = ParseBibEntry(bibString);

		Assert.Equal(2, bibliographyDom.BibliographyEntries.Count);
		Assert.Equal("% Entry comment" + Environment.NewLine, bibliographyDom.BibliographyEntries[0].Comment);
		Assert.Equal(string.Empty, bibliographyDom.BibliographyEntries[1].Comment);
	}

	[Fact]
	public void MultiplePrecedingCommentsAreCombinedForFollowingPart()
	{
		string bibString = "\n% First comment\n" + "% Second comment\n" + "@book{key1, title = {Title 1}}";

		BibliographyDOM bibliographyDom = ParseBibEntry(bibString);

		Assert.Single(bibliographyDom.BibliographyEntries);
		Assert.Equal("% First comment" + Environment.NewLine + "% Second comment" + Environment.NewLine, bibliographyDom.BibliographyEntries[0].Comment);
	}

	[Fact]
	public void CommentSeparatedFromEntryByBlankLineIsNotAllowed()
	{
		string bibString = "\n% Entry comment\n" + "\n" + "@book{key1, title = {Title 1}}";
		BibliographyDOM bibliographyDom = ParseBibEntry(bibString);
		Assert.Equal("% Entry comment" + Environment.NewLine, bibliographyDom.BibliographyEntries[0].Comment);
	}

	[Fact]
	public void CommentAfterFieldValueIsNotAllowed()
	{
		string bibString = "@article{key1,\n" + " title = {Title 1} % Invalid field comment\n" + "}";
		BibliographyDOM bibliographyDom = ParseBibEntry(bibString);
		Assert.Equal("", bibliographyDom.BibliographyEntries[0].Comment);
	}

	#endregion

	#region Comment Writing

	#region BibliographyPart Comment Writing

	[Fact]
	public void BibEntryToStringWritesCommentBeforeEntry()
	{
		BibEntry entry = new()
		{
			Comment = "% Entry comment" + Environment.NewLine,
			Type = "book",
			Key = "ref:keyA1",
			Title = "Title 1"
		};

		string result = entry.ToString();

		string expected =
			"% Entry comment" + Environment.NewLine +
			"@book{ref:keyA1," + Environment.NewLine +
			"  title = {Title 1}" + Environment.NewLine +
			"}" + Environment.NewLine;

		Assert.Equal(expected, result);
	}

	[Fact]
	public void BibEntryToStringWritesMultipleCommentLinesBeforeEntry()
	{
		BibEntry entry = new()
		{
			Comment =
				"% First comment" + Environment.NewLine +
				"% Second comment" + Environment.NewLine,
			Type = "book",
			Key = "ref:keyA1",
			Title = "Title 1"
		};

		string result = entry.ToString();

		Assert.StartsWith(
			"% First comment" + Environment.NewLine +
			"% Second comment" + Environment.NewLine +
			"@book{ref:keyA1,",
			result);
	}

	[Fact]
	public void StringEntryToStringWritesCommentBeforeEntry()
	{
		StringEntry entry = new()
		{
			Comment = "% String comment" + Environment.NewLine,
			Name = "journalName",
			Value = "Journal Name"
		};

		string result = entry.ToString();

		string expected =
			"% String comment" + Environment.NewLine +
			"@string(journalName = \"Journal Name\")\r\n";

		Assert.Equal(expected, result);
	}

	[Fact]
	public void StringEntryToStringWritesMultipleCommentLinesBeforeEntry()
	{
		StringEntry entry = new()
		{
			Comment =
				"% First comment" + Environment.NewLine +
				"% Second comment" + Environment.NewLine,
			Name = "journalName",
			Value = "Journal Name"
		};

		string result = entry.ToString();

		Assert.Equal(
			"% First comment" + Environment.NewLine +
			"% Second comment" + Environment.NewLine +
			"@string(journalName = \"Journal Name\")\r\n",
			result);
	}

	[Fact]
	public void BibEntryWithoutCommentDoesNotWriteLeadingBlankLine()
	{
		BibEntry entry = new()
		{
			Type = "book",
			Key = "ref:keyA1",
			Title = "Title 1"
		};

		string result = entry.ToString();

		Assert.StartsWith("@book{ref:keyA1,", result);
	}

	[Fact]
	public void StringEntryWithoutCommentDoesNotWriteLeadingBlankLine()
	{
		StringEntry entry = new()
		{
			Name = "journalName",
			Value = "Journal Name"
		};

		string result = entry.ToString();

		Assert.StartsWith("@string(journalName", result);
	}

	#endregion

	#endregion

	#region Helper Methods

	private static BibliographyDOM ParseBibEntry(string bibString)
	{
		BibliographyParser parser = new(new StringReader(bibString));
		return parser.Parse();
	}

	#endregion

} // End class.