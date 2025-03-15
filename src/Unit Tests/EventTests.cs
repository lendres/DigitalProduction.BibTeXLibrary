using BibTeXLibrary;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace DigitalProduction.UnitTests;

public class EventTests
{
	string _message	= "";
	string _file	= "testoutput.bib";

	/// <summary>
	/// Test that "Modified" is set to the correct value and that the event is triggered properly.
	/// </summary>
    [Fact]
    public void TestBibEntryAdded()
    {
		Bibliography bibliography = new();
		bibliography.ModifiedChanged += OnModifiedChanged;

		bibliography.Read("TestData/BibParserTest1_In.bib");
		Assert.False(bibliography.Modified);
		Assert.Equal("False", _message);

		BibEntry entry = new() {["Title"] = "Mapreduce"};
		bibliography.AddBibPart(entry);
		Assert.True(bibliography.Modified);
		Assert.Equal("True", _message);

		// Writing should mark the bibliography as unmodified.
		bibliography.Write(_file);
		CleanUp();
		Assert.False(bibliography.Modified);
		Assert.Equal("False", _message);

		// Test removing an entry.  This should cause it to be marked as modified again.
		bibliography.Entries.Remove(entry);
		Assert.True(bibliography.Modified);
		Assert.Equal("True", _message);
    }

	private void OnModifiedChanged(object sender, bool modified)
	{
		_message = modified.ToString();
	}

	private void CleanUp()
	{
		System.IO.File.Delete(_file);
	}

} // End class.