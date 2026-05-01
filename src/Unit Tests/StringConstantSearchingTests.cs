using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class StringConstantSearchingTests
{
	# region Fields

	private readonly BibliographyDOM _bibliographyDom;

	private readonly string _stringConstants = 
		"@string{keyA1 = {Acme Journal of Science}}" +
		"@string(keyB2 = {Journal of {SCIENCE} Fiction})" +
		"@string(keyC3 = \"Commun {NUMER} Methods Eng\")";

	#endregion

	#region Construction

	public StringConstantSearchingTests()
	{
		_bibliographyDom = ParseBibEntry(_stringConstants);
	}

	#endregion

	#region Tests

	[Fact]
    public void TestSearchInValues()
    {
		List<StringConstant> result = _bibliographyDom.SearchStringConstants(false, "Fiction");
		Assert.Single(result);
		
		result = _bibliographyDom.SearchStringConstants(false, "Journal");
		Assert.Equal(2, result.Count);

		result = _bibliographyDom.SearchStringConstants(false, "{NUMER}");
		Assert.Single(result);
	}

	[Fact]
    public void TestSearchInNames()
    {
		List<StringConstant> result = _bibliographyDom.SearchStringConstants(true, "A1");
		Assert.Single(result);
		
		result = _bibliographyDom.SearchStringConstants(true, "key");
		Assert.Equal(3, result.Count);
	}

	#endregion

	#region Helper Methods

	private static BibliographyDOM ParseBibEntry(string stringConstants)
	{
		BibParser parser = new(new StringReader(stringConstants));
		return parser.Parse();
	}

	#endregion

} // End class.