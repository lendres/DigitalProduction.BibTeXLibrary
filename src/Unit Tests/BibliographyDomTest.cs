using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class BibliographyDomTest
{
	# region Fields

	private readonly BibliographyDOM _bibliographyDom;

	private readonly string _bibString = 
		"@book{ref:keyA1, booktitle = {Acme Journal of Science}, author = {John Smith}, year = {2023}, abstract = {Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}}" +
		"@book{ref:keyB2, booktitle = {Journal of Science Fiction}, author = {Jane Smith}, year = {2023}, abstract = {John lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}}";

	#endregion

	#region Construction

	public BibliographyDomTest()
	{
		_bibliographyDom = ParseBibEntry(_bibString);
	}

	#endregion

	[Fact]
    public void TestSearchInTagValues()
    {
		List<string> tagNames = ["booktitle", "author", "year", "abstract"];

		List<BibEntry> result = _bibliographyDom.SearchBibEntries(tagNames, false, "acme");
		Assert.Single(result);
		
		result = _bibliographyDom.SearchBibEntries(tagNames, false, "Journal");
		Assert.Equal(2, result.Count);
	}

	[Fact]
    public void TestSearchInSpecificValues()
    {
		List<string> tagNames = ["author"];
		List<BibEntry> result = _bibliographyDom.SearchBibEntries(tagNames, false, "John");
		Assert.Single(result);
	
		tagNames = ["author", "abstract"];
		result = _bibliographyDom.SearchBibEntries(tagNames, false, "John");
		Assert.Equal(2, result.Count);
	}

	[Fact]
    public void TestSearchWithKeys()
    {
		List<string> tagNames = ["author"];
		List<BibEntry> result = _bibliographyDom.SearchBibEntries(tagNames, true, "A1");
		Assert.Single(result);
	}

	private static BibliographyDOM ParseBibEntry(string bibString)
	{
		BibParser parser = new(new StringReader(bibString));
		return parser.Parse();
	}

} // End class.