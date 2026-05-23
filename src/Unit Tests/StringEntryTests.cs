using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class StringEntryTests
{
	# region Fields

	private readonly BibliographyDOM _bibliographyDom;

	private readonly string _stringConstants = 
		"@string{keyA1 = {Acme Journal of Science}}" +
		"@string(keyB2 = {Journal of {SCIENCE} Fiction})" +
		"@string(keyC3 = \"Commun {NUMER} Methods Eng\")";

	#endregion

	#region Construction

	public StringEntryTests()
	{
		_bibliographyDom = ParseBibEntry(_stringConstants);
	}

	#endregion

	#region Copy Constructor Tests

	[Fact]
	public void TestCopyConstructorCopiesStringEntryValues()
	{
		StringEntry originalEntry = new()
		{
			Name	= "jcp",
			Value	= "Journal of Chemical Physics",
			Comment	= "% Comment before string\n"
		};

		StringEntry copiedEntry = new(originalEntry);

		Assert.Equal(originalEntry.Type, copiedEntry.Type);
		Assert.Equal(originalEntry.Name, copiedEntry.Name);
		Assert.Equal(originalEntry.Value, copiedEntry.Value);
		Assert.Equal(originalEntry.Comment, copiedEntry.Comment);
		Assert.Equal(originalEntry.FieldValue, copiedEntry.FieldValue);
	}

	[Fact]
	public void TestCopyConstructorCreatesIndependentStringEntryFieldValue()
	{
		StringEntry originalEntry = new()
		{
			Name	= "jcp",
			Value	= "Journal of Chemical Physics"
		};

		StringEntry copiedEntry = new(originalEntry);

		Assert.NotSame(originalEntry.FieldValue, copiedEntry.FieldValue);

		originalEntry.Value = "Updated Journal";

		Assert.Equal("Updated Journal", originalEntry.Value);
		Assert.Equal("Journal of Chemical Physics", copiedEntry.Value);
	}

	#endregion

	#region Searching Tests

	[Fact]
    public void TestSearchInValues()
    {
		List<StringEntry> result = _bibliographyDom.SearchStringConstants(false, "Fiction");
		Assert.Single(result);
		
		result = _bibliographyDom.SearchStringConstants(false, "Journal");
		Assert.Equal(2, result.Count);

		result = _bibliographyDom.SearchStringConstants(false, "{NUMER}");
		Assert.Single(result);
	}

	[Fact]
    public void TestSearchInNames()
    {
		List<StringEntry> result = _bibliographyDom.SearchStringConstants(true, "A1");
		Assert.Single(result);
		
		result = _bibliographyDom.SearchStringConstants(true, "key");
		Assert.Equal(3, result.Count);
	}

	#endregion

	#region Helper Methods

	private static BibliographyDOM ParseBibEntry(string stringConstants)
	{
		BibliographyParser parser = new(new StringReader(stringConstants));
		return parser.Parse();
	}

	#endregion

} // End class.