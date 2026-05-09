using BibTeXLibrary;
using System.ComponentModel;
using System.Xml.Linq;

namespace DigitalProduction.UnitTests;

public class FieldTests
{
	[Fact]
	public void TestDefaultConstructor()
	{
		Field field = new();

		Assert.Equal(string.Empty, field.Name);
		Assert.Equal(string.Empty, field.Value);
		Assert.Equal(new FieldValue(string.Empty, FieldValueType.String), field.FieldValue);
	}

	[Fact]
	public void TestCopyConstructor()
	{
		Field originalField = new();
		//{
			//Name = "title",
			//FieldValue = new FieldValue("My Title Content", FieldValueType.String)
		//};
originalField.Name = "title";
originalField.FieldValue = new FieldValue("My Title Content", FieldValueType.String);

		Field copiedField = new(originalField);

		Assert.Equal(originalField.Name, copiedField.Name);
		Assert.Equal(originalField.FieldValue, copiedField.FieldValue);
		Assert.NotSame(originalField.FieldValue, copiedField.FieldValue);
	}

	[Fact]
	public void TestValueUpdatesFieldValueContent()
	{
		Field field = new();
		FieldValue fValue = field.FieldValue;

		field.Value = "Updated Value";

		Assert.Equal("Updated Value", field.Value);
		Assert.Equal("Updated Value", field.FieldValue.Content);
		Assert.True(field.Modified);
	}

	[Fact]
	public void TestMarkSavedClearsModified()
	{
		Field field = new();

		field.Value = "Updated Value";
		field.MarkSaved();

		Assert.False(field.Modified);
	}

	[Fact]
	public void TestToStringThrowsNotSupportedException()
	{
		Field field = new();

		NotSupportedException exception = Assert.Throws<NotSupportedException>(() => field.ToString());

		Assert.Equal("This method is disabled.", exception.Message);
	}

	[Fact]
	public void TestToStringWithoutAlignment()
	{
		Field field = new()
		{
			Name = "title",
			FieldValue = new FieldValue("My Title", FieldValueType.String)
		};

		WriteSettings writeSettings = new()
		{
			AlignFieldValues = false
		};

		string actual = field.ToString(writeSettings, FieldValueFormat.CurlyBraces, 5, 24);

		Assert.Equal("title = {My Title}", actual);
	}

	[Fact]
	public void TestToStringWithSpaceAlignment()
	{
		Field field = new()
		{
			Name = "title",
			FieldValue = new FieldValue("My Title", FieldValueType.String)
		};

		WriteSettings writeSettings = new()
		{
			AlignFieldValues = true,
			WhiteSpace = WhiteSpace.Space,
			TabSize = 4
		};

		string actual = field.ToString(writeSettings, FieldValueFormat.CurlyBraces, 5, 24);

		Assert.Equal("title              = {My Title}", actual);
	}

	[Fact]
	public void TestToStringWithTabAlignment()
	{
		Field field = new()
		{
			Name = "title",
			FieldValue = new FieldValue("My Title", FieldValueType.String)
		};

		WriteSettings writeSettings = new()
		{
			AlignFieldValues = true,
			WhiteSpace = WhiteSpace.Tab,
			TabSize = 4
		};

		string actual = field.ToString(writeSettings, FieldValueFormat.CurlyBraces, 5, 24);

		Assert.Equal("title\t\t\t= {My Title}", actual);
	}

	[Fact]
	public void TestGetSpacingThrowsArgumentOutOfRangeExceptionForLongFieldName()
	{
		TestField field = new();

		WriteSettings writeSettings = new()
		{
			AlignFieldValues = true,
			WhiteSpace = WhiteSpace.Space,
			TabSize = 4
		};

		ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
			() => field.TestGetSpacing("averyverylongfieldname", writeSettings, 5, 10));

		Assert.Equal("fieldName", exception.ParamName);
		Assert.Contains("The name is too long for the space allocated for aligning the field values.", exception.Message);
		Assert.Contains("Name: averyverylongfieldname", exception.Message);
		Assert.Contains("Overage length:", exception.Message);
		Assert.Contains("spaces", exception.Message);
	}

	[Fact]
	public void TestGetSpacingThrowsInvalidEnumArgumentExceptionForInvalidWhiteSpace()
	{
		TestField field = new();

		WriteSettings writeSettings = new()
		{
			AlignFieldValues = true,
			WhiteSpace = (WhiteSpace)999
		};

		InvalidEnumArgumentException exception = Assert.Throws<InvalidEnumArgumentException>(
			() => field.TestGetSpacing("title", writeSettings, 5, 24));

		Assert.Equal("Invalid \"WhiteSpace\" value.", exception.Message);
	}

	private class TestField : Field
	{
		public string TestGetSpacing(string fieldName, WriteSettings writeSettings, int alignAtTabStop, int alignAtColumn)
		{
			return GetSpacing(fieldName, writeSettings, alignAtTabStop, alignAtColumn);
		}
	}
}