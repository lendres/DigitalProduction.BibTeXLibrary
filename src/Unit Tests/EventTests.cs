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
		Assert.Equal("False", GetAndResetMessage());

		BibEntry entry = new() {["Title"] = "Mapreduce"};
		bibliography.Add(entry);
		Assert.True(bibliography.Modified);
		Assert.Equal("True", GetAndResetMessage());

		// Writing should mark the bibliography as unmodified.
		CleanUp(bibliography);

		// Test removing an entry.  This should cause it to be marked as modified again.
		bibliography.Entries.Remove(entry);
		Assert.True(bibliography.Modified);
		Assert.Equal("True", GetAndResetMessage());
    }

	/// <summary>
	/// Test that "Modified" is set to the correct value and that the event is triggered properly.
	/// </summary>
    [Fact]
    public void TestBibEntryModified()
    {
		Bibliography bibliography = new();
		bibliography.ModifiedChanged += OnModifiedChanged;

		bibliography.Read("TestData/BibParserTest1_In.bib");
		Assert.False(bibliography.Modified);
		Assert.Equal("False", GetAndResetMessage());

		BibEntry entry = bibliography.Entries[0];
		Assert.False(bibliography.Modified);

		entry.Title = "Changed Title";
		Assert.Equal("True", GetAndResetMessage());
		CleanUp(bibliography);

		// Test changing the entry using bracket notation. This should cause it to be marked as modified again.
		entry["Title"] = "Changed Again";
		Assert.Equal("True", GetAndResetMessage());
		CleanUp(bibliography);

		// Test adding an entry using bracket notation. This should cause it to be marked as modified again.
		entry["NewTag"] = "New tag added using bracket notation.";
		Assert.Equal("True", GetAndResetMessage());
		CleanUp(bibliography);

		// Test adding the same value. This should NOT cause it to be marked as modified again.
		entry["NewTag"] = "New tag added using bracket notation.";
		Assert.False(bibliography.Modified);
		Assert.Equal("", GetAndResetMessage());
    }

	private void OnModifiedChanged(object sender, bool modified)
	{
		_message = modified.ToString();
	}

	private void CleanUp(Bibliography bibliography)
	{
		bibliography.Write(_file);
		System.IO.File.Delete(_file);
		Assert.False(bibliography.Modified);
		Assert.Equal("False", GetAndResetMessage());
	}

	private string GetAndResetMessage()
	{
		string message	= _message;
		_message		= string.Empty;
		return message;
	}

} // End class.