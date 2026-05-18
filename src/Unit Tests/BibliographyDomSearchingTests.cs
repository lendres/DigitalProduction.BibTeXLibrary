using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class BibliographyDomSearchingTests
{
	# region Fields

	private readonly BibliographyDOM _bibliographyDom;

	private readonly string _bibString = 
		"@book{ref:keyA1, booktitle = {Acme Journal of Science}, author = {John Smith}, year = {2023}, abstract = {Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}} " +
		"@book{ref:keyB2, booktitle = {Journal of {SCIENCE} Fiction}, author = {Jane Smith}, year = {2023}, abstract = {John lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}}";

	#endregion

	#region Construction

	public BibliographyDomSearchingTests()
	{
		_bibliographyDom = ParseBibEntry(_bibString);
	}

	#endregion

	#region Tests

	[Fact]
    public void TestSearchInFieldValues()
    {
		List<string> fieldNames = ["booktitle", "author", "year", "abstract"];

		List<BibEntry> result = _bibliographyDom.SearchBibEntries(fieldNames, false, "acme");
		Assert.Single(result);
		
		result = _bibliographyDom.SearchBibEntries(fieldNames, false, "Journal");
		Assert.Equal(2, result.Count);
	}

	[Fact]
    public void TestSearchInSpecificValues()
    {
		List<string> fieldNames = ["author"];
		List<BibEntry> result = _bibliographyDom.SearchBibEntries(fieldNames, false, "John");
		Assert.Single(result);
	
		fieldNames = ["author", "abstract"];
		result = _bibliographyDom.SearchBibEntries(fieldNames, false, "John");
		Assert.Equal(2, result.Count);
	}

	[Fact]
    public void TestSearchWithKeys()
    {
		List<string> fieldNames = [];
		List<BibEntry> result = _bibliographyDom.SearchBibEntries(fieldNames, true, "A1");
		Assert.Single(result);
	}

	[Fact]
    public void TestSearchOfFieldWithBrackets()
    {
		// The book title is "Journal of {SCIENCE} Fiction" but we will search for "Journal of SCIENCE Fiction".
		List<string> fieldNames = ["booktitle"];
		List<BibEntry> result = _bibliographyDom.SearchBibEntries(fieldNames, false, "Journal of SCIENCE Fiction");
		Assert.Single(result);
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