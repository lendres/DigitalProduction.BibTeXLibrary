using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class BibliographyEventTests
{
	#region Fields

	string _message	= "";
	string _file	= "testoutput.bib";

	#endregion

	#region Tests

	/// <summary>
	/// Test that "Modified" is set to the correct value and that the event is triggered properly.
	/// </summary>
	[Fact]
    public void TestBibEntryAdded()
    {
		Bibliography bibliography = CreateBibliography();

		BibEntry entry = new() {["Title"] = "Mapreduce"};
		bibliography.Add(entry);
		Assert.True(bibliography.Modified);
		Assert.Equal("True", GetAndResetMessage());

		// Writing should mark the bibliography as unmodified.
		SaveBibliography(bibliography);

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
		Bibliography bibliography = CreateBibliography();

		BibEntry entry = bibliography.Entries[0];
		Assert.False(bibliography.Modified);

		entry.Title = "Changed Title";
		Assert.Equal("True", GetAndResetMessage());
		SaveBibliography(bibliography);

		// Test changing the entry using bracket notation. This should cause it to be marked as modified again.
		entry["Title"] = "Changed Again";
		Assert.Equal("True", GetAndResetMessage());
		SaveBibliography(bibliography);

		// Test adding an entry using bracket notation. This should cause it to be marked as modified again.
		entry["NewField"] = "New field added using bracket notation.";
		Assert.Equal("True", GetAndResetMessage());
		SaveBibliography(bibliography);

		// Test adding the same value. This should NOT cause it to be marked as modified again.
		entry["NewField"] = "New field added using bracket notation.";
		Assert.False(bibliography.Modified);
		Assert.Equal("", GetAndResetMessage());
    }


	/// <summary>
	/// Test that "Modified" is set to the correct value and that the event is triggered properly.
	/// </summary>
	[Fact]
	public void TestHeaderModified()
	{
		Bibliography bibliography = CreateBibliography();

		string header = bibliography.Header;
		Assert.False(bibliography.Modified);

		bibliography.Header = header + "\n% New header comment added.";
		Assert.Equal("True", GetAndResetMessage());
		Assert.True(bibliography.Modified);

		SaveBibliography(bibliography);
	}

	#endregion

	#region Helper Methods

	private void OnModifiedChanged(object sender, bool modified)
	{
		_message = modified.ToString();
	}

	private void SaveBibliography(Bibliography bibliography)
	{
		bibliography.Write(_file);
		System.IO.File.Delete(_file);

		// Test the after saving the bibliography is not marked as modified.
		Assert.False(bibliography.Modified);
		Assert.Equal("False", GetAndResetMessage());
	}

	private Bibliography CreateBibliography()
	{
		Bibliography bibliography = new();
		bibliography.ModifiedChanged += OnModifiedChanged;
		bibliography.Read("TestData/BibParserTest1_In.bib");

		// Test that after a read, the bibliography is not marked as modified.
		Assert.False(bibliography.Modified);
		Assert.Equal("False", GetAndResetMessage());

		return bibliography;
	}

	private string GetAndResetMessage()
	{
		string message	= _message;
		_message		= string.Empty;
		return message;
	}

	#endregion

} // End class.