using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class StringConstantSearchingTests
{
	# region Fields

	private readonly BibliographyDOM _bibliographyDom;

	private readonly string _stringConstants = 
		"@book{ref:keyA1, booktitle = {Acme Journal of Science}, author = {John Smith}, year = {2023}, abstract = {Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}} " +
		"@book{ref:keyB2, booktitle = {Journal of {SCIENCE} Fiction}, author = {Jane Smith}, year = {2023}, abstract = {John lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.}}";

	#endregion

	#region Construction

	public StringConstantSearchingTests()
	{
		_bibliographyDom = ParseBibEntry(_stringConstants);
	}

	#endregion


	private static BibliographyDOM ParseBibEntry(string stringConstants)
	{
		BibParser parser = new(new StringReader(stringConstants));
		return parser.Parse();
	}

} // End class.