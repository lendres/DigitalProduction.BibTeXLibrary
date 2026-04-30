using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class StringConstantSearchingTests
{
	# region Fields

	private readonly BibliographyDOM _bibliographyDom;

	private readonly string _stringConstants = 
		"@string{keyA1, = {Acme Journal of Science}}" +
		"@string(keyB2, {Journal of {SCIENCE} Fiction})" +
		"@string(keyC3, \"Commun {NUMER} Methods Eng\")";

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