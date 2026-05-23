using BibTeXLibrary;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

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

	#region Basic Tests

	[Fact]
	public void TestDefaultConstructor()
	{
		StringEntry stringEntry = new();

		Assert.Equal("string", stringEntry.Type);
		Assert.Equal(string.Empty, stringEntry.Name);
		Assert.Equal(string.Empty, stringEntry.Value);
		Assert.Equal(FieldValueType.String, stringEntry.FieldValue.FieldValueType);
		Assert.False(stringEntry.Modified);
	}

	[Fact]
	public void TestSetTypeThrowsException()
	{
		StringEntry stringEntry = new();

		Exception exception = Assert.Throws<Exception>(() => stringEntry.Type = "article");

		Assert.Equal("You cannot set the type of a StringConstantPart", exception.Message);
	}

	[Fact]
	public void TestSetFieldSetsNameAndValue()
	{
		StringEntry stringEntry = new();

		stringEntry.SetField("JournalName", "Journal of Chemical Physics", FieldValueType.String);

		Assert.Equal("JournalName", stringEntry.Name);
		Assert.Equal("Journal of Chemical Physics", stringEntry.Value);
		Assert.Equal(FieldValueType.String, stringEntry.FieldValue.FieldValueType);
	}

	[Fact]
	public void TestNameChangeSetsModified()
	{
		StringEntry stringEntry = new();

		stringEntry.MarkSaved();
		stringEntry.Name = "jcp";

		Assert.True(stringEntry.Modified);
	}

	[Fact]
	public void TestValueChangeSetsModified()
	{
		StringEntry stringEntry = new();

		stringEntry.MarkSaved();
		stringEntry.Value = "Journal of Chemical Physics";

		Assert.True(stringEntry.Modified);
	}

	[Fact]
	public void TestMarkSavedClearsModified()
	{
		StringEntry stringEntry = new();
		//{
		//			Name	= "jcp",
		//		Value	= "Journal of Chemical Physics"
		//};

		stringEntry.Name = "jcp";
		stringEntry.Value = "Journal of Chemical Physics";


		Assert.True(stringEntry.Modified);

		stringEntry.MarkSaved();

		Assert.False(stringEntry.Modified);
	}

	[Theory]
	[InlineData(EntryBracketType.CurlyBraces, "@string{jcp = \"Journal of Chemical Physics\"}")]
	[InlineData(EntryBracketType.Parentheses, "@string(jcp = \"Journal of Chemical Physics\")")]
	public void TestToStringWithBracketType(EntryBracketType bracketType, string expected)
	{
		StringEntry stringEntry = new()
		{
			Name = "jcp",
			Value = "Journal of Chemical Physics"
		};

		WriteSettings writeSettings = new()
		{
			AlignFieldValues = false,
			StringEntryBracketType = bracketType,
			StringEntryFieldValueFormat = FieldValueFormat.Quotes,
			NewLine = "\n"
		};

		string result = stringEntry.ToString(writeSettings);

		Assert.Equal(expected + "\n", result);
	}

	[Theory]
	[InlineData(FieldValueFormat.CurlyBraces, "@string(jcp = {Journal of Chemical Physics})\n")]
	[InlineData(FieldValueFormat.Quotes, "@string(jcp = \"Journal of Chemical Physics\")\n")]
	[InlineData(FieldValueFormat.None, "@string(jcp = Journal of Chemical Physics)\n")]
	public void TestToStringWithFieldValueFormat(FieldValueFormat fieldValueFormat, string expected)
	{
		StringEntry stringEntry = new()
		{
			Name = "jcp",
			Value = "Journal of Chemical Physics"
		};

		WriteSettings writeSettings = new()
		{
			AlignFieldValues = false,
			StringEntryBracketType = EntryBracketType.Parentheses,
			StringEntryFieldValueFormat = fieldValueFormat,
			NewLine = "\n"
		};

		string result = stringEntry.ToString(writeSettings);

		Assert.Equal(expected, result);
	}

	[Fact]
	public void TestToStringIncludesComment()
	{
		StringEntry stringEntry = new()
		{
			Comment = "% Comment before string\n",
			Name = "jcp",
			Value = "Journal of Chemical Physics"
		};

		WriteSettings writeSettings = new()
		{
			AlignFieldValues = false,
			StringEntryBracketType = EntryBracketType.Parentheses,
			StringEntryFieldValueFormat = FieldValueFormat.Quotes,
			NewLine = "\n"
		};

		string result = stringEntry.ToString(writeSettings);
		Assert.Equal("% Comment before string\n@string(jcp = \"Journal of Chemical Physics\")\n", result);
	}

	[Fact]
	public void TestCopyConstructorCopiesComment()
	{
		StringEntry originalEntry = new()
		{
			Comment = "% Comment before string\n",
			Name = "jcp",
			Value = "Journal of Chemical Physics"
		};

		StringEntry copiedEntry = new(originalEntry);

		Assert.Equal(originalEntry.Comment, copiedEntry.Comment);
	}

	[Fact]
	public void TestCopyConstructorCreatesIndependentFieldValue()
	{
		StringEntry originalEntry = new()
		{
			Name = "jcp",
			Value = "Journal of Chemical Physics"
		};

		StringEntry copiedEntry = new(originalEntry);

		Assert.NotSame(originalEntry.FieldValue, copiedEntry.FieldValue);

		originalEntry.Value = "Updated Journal";

		Assert.Equal("Updated Journal", originalEntry.Value);
		Assert.Equal("Journal of Chemical Physics", copiedEntry.Value);
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
		List<StringEntry> result = _bibliographyDom.SearchStringEntries(false, "Fiction");
		Assert.Single(result);
		
		result = _bibliographyDom.SearchStringEntries(false, "Journal");
		Assert.Equal(2, result.Count);

		result = _bibliographyDom.SearchStringEntries(false, "{NUMER}");
		Assert.Single(result);
	}

	[Fact]
    public void TestSearchInNames()
    {
		List<StringEntry> result = _bibliographyDom.SearchStringEntries(true, "A1");
		Assert.Single(result);
		
		result = _bibliographyDom.SearchStringEntries(true, "key");
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