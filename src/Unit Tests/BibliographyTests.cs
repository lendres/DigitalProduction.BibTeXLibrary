using BibTeXLibrary;
using System.Collections.ObjectModel;
using System.Text;

namespace DigitalProduction.UnitTests;

public class BibliographyTests
{
	# region Fields

	private readonly Bibliography _bibliography = new();

	private readonly string _bibString = 
		"@book{ref:keyA1, booktitle = {Acme Journal of Science}, author = {John Smith}, year = {2023}, abstract = {Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}} " +
		"@book{ref:keyB2, booktitle = {Journal of {SCIENCE} Fiction}, author = {Jane Smith}, year = {2023}, abstract = {John lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}}" +
		"@article{10.2118/87837-PA, author = {Menand, S. and Sellami, H. and Simon, C.}, year = {2004}, title = {PDC Bit Classification According to Steerability}, abstract = {lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}}";

	#endregion

	#region Construction

	public BibliographyTests()
	{
		_bibliography = ParseBibEntry(_bibString);
	}

	#endregion

	#region Cite Key Generation

	[Fact]
	public void TestHavingAndInName()
	{
		// Test that creating a citation key for "Menand" returns "menand2004a" and not ""men2004a"".
		BibEntry result = _bibliography.Entries[2];
		_bibliography.GenerateUniqueCiteKey(result);
		Assert.Equal("ref:menand2004a", result.Key);
	}

	#endregion

	#region Reading from a File

    [Fact]
    public void TestReadingBibFile()
    {
		Bibliography bibliography = new();
		bibliography.Read("TestData/BibParserTest1_In.bib");

		ObservableCollection<BibEntry> entries = bibliography.Entries;
		Assert.Equal(4,														entries.Count);
		Assert.Equal("nobody",												entries[0].Publisher);
		Assert.Equal("Apache hadoop yarn: Yet another resource negotiator",	entries[1].Title);
		Assert.Equal("KalavriShang-797",									entries[2].Key);

		ObservableCollection<StringConstant> stringConstants = bibliography.StringConstants;
		Assert.Equal(2,														stringConstants.Count);
		Assert.Equal("TRANSMATHSOFT",										stringConstants[0].Name);
		Assert.Equal("ACM T Math Software",									stringConstants[0].Value);
		Assert.Equal("CODEPROJ",											stringConstants[1].Name);
		Assert.Equal("Code Project",										stringConstants[1].Value);
    }

	[Fact]
	public void TestWritingBibFile()
	{
		Bibliography bibliography = new();
		bibliography.Read("TestData/BibParserTest1_In.bib");
		bibliography.Write("TestData/BibParserTest1_Out.bib");

		bibliography = new();
		bibliography.Read("TestData/BibParserTest1_Out.bib");
		Assert.Equal(4, bibliography.Entries.Count);
		Assert.Equal(2, bibliography.StringConstants.Count);
	}

	#endregion

	#region Helper Methods

	private Bibliography ParseBibEntry(string bibString)
	{
		BibParser parser = new(new StringReader(bibString));
		return (Bibliography)parser.Parse(_bibliography);
	}

	#endregion

} // End class.