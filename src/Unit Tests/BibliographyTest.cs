using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class BibliographyTest
{
	# region Fields

	private readonly Bibliography _bibliography = new();

	private readonly string _bibString = 
		"@book{ref:keyA1, booktitle = {Acme Journal of Science}, author = {John Smith}, year = {2023}, abstract = {Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}} " +
		"@book{ref:keyB2, booktitle = {Journal of {SCIENCE} Fiction}, author = {Jane Smith}, year = {2023}, abstract = {John lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}}" +
		"@article{10.2118/87837-PA, author = {Menand, S. and Sellami, H. and Simon, C.}, year = {2004}, title = {PDC Bit Classification According to Steerability}, abstract = {lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}}";

	#endregion

	#region Construction

	public BibliographyTest()
	{
		_bibliography = ParseBibEntry(_bibString);
	}

	#endregion

	[Fact]
	public void TestHavingAndInName()
	{
		// Test that creating a citation key for "Menand" returns "menand2004a" and not ""men2004a"".
		BibEntry result = _bibliography.Entries[2];
		_bibliography.GenerateUniqueCiteKey(result);
		Assert.Equal("ref:menand2004a", result.Key);
	}

	private Bibliography ParseBibEntry(string bibString)
	{
		BibParser parser = new(new StringReader(bibString));
		return (Bibliography)parser.Parse(_bibliography);
	}

} // End class.