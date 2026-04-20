using BibTeXLibrary;
using System.ComponentModel;
using System.Diagnostics;

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

	#region Basic Tests

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

	#endregion

	#region Reading from File Tests

	[Fact]
	public void ReadBibFilePerformance()
	{
		// Setup.
		string filePath = GetTestFilePath("BibParserTest1_In.bib");

		Bibliography bibliography = new();
		bibliography.ModifiedChanged += OnModifiedChanged;
		bibliography.PropertyChanged += OnPropertyChanged;

		// Read the file.
		Stopwatch stopwatch = Stopwatch.StartNew();
		bibliography.Read(filePath);
		stopwatch.Stop();
		long firstReadMs = stopwatch.ElapsedMilliseconds;
		int count1 = bibliography.NumberOfBibliographyEntries;

		// Act - Second read.
		stopwatch.Restart();
		bibliography.Read(filePath);
		stopwatch.Stop();
		long secondReadMs	= stopwatch.ElapsedMilliseconds;
		int count2			= bibliography.NumberOfBibliographyEntries;

		// Optional: ensure same number of entries, etc.
		Assert.Equal(count1, count2);

		// Output timing (diagnostic, not assertion).
		Console.WriteLine($"First read:  {firstReadMs} ms");
		Console.WriteLine($"Second read: {secondReadMs} ms");

		// Optional soft assertion (not strict).
		// This just checks nothing pathological happened.
		Assert.True(secondReadMs < firstReadMs * 2, "Second read is unexpectedly slower by a large margin.");
		Assert.True(firstReadMs < secondReadMs * 2, "First read is unexpectedly slower by a large margin.");
	}

	private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		System.Threading.Thread.Sleep(2);
	}

	private void OnModifiedChanged(object sender, bool modified)
	{
		System.Threading.Thread.Sleep(2);
	}

	private string GetTestFilePath(string fileName)
	{
		// Adjust based on your project structure.
		return Path.Combine(AppContext.BaseDirectory, "TestData", fileName);
	}

	#endregion

} // End class.