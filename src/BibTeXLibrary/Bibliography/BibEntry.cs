using DigitalProduction.Strings;
using Google.Apis.CustomSearchAPI.v1.Data;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BibTeXLibrary;

/// <summary>
/// A bibliography entry.
/// </summary>
public class BibEntry : BibliographyPart
{
	#region Fields

	private static readonly string[]						_nameSuffixes		= ["jr", "jr.", "sr", "sr.", "ii", "iii", "iv", "v", @"p\`{e}re", "fils"];

	/// <summary>Store all fields.</summary>
	protected readonly OrderedDictionary<string, Field>		_fields				= [];

	#endregion

	#region Construction

	/// <summary>
	/// Default contructor.
	/// </summary>
	public BibEntry() :
		base(false)
	{
	}

	#endregion

	#region Public Static Methods

	/// <summary>
	/// Create a new BibEntry template.  The template is an initialized, but blank BibEntry.
	/// </summary>
	/// <param name="bibEntryInitialization">BibEntryInitialization.</param>
	/// <param name="type">The "type" of the bibliography entry.  The type must have an initialization template.</param>
	public static BibEntry NewBibEntryFromTemplate(BibEntryInitialization bibEntryInitialization, string type)
	{
		BibEntry bibEntry = new() { Type = type };
		bibEntry.Initialize(bibEntryInitialization.GetDefaultFields(type));

		return bibEntry;
	}

	#endregion

	#region Public Properties

	/// <summary>
	/// Bibliography entry type, e.g. "book", "thesis", "string".  This is the name that follows the "@".
	/// </summary>
	public override string Type { get => GetValueOrDefault(string.Empty); set => SetValue(value); }

	/// <summary>
	/// Entry's key.
	/// </summary>
	public string Key { get => GetValueOrDefault(string.Empty); set => SetValue(value); }

	/// <summary>
	/// Get the names of the fields.
	/// </summary>
	public List<string> FieldNames { get => _fields.Keys.ToList(); }

	#region Common Fields

	/// <summary>
	/// The address entry or an empty string if the address was not specified.
	/// </summary>
	public string Address
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The annote entry or an empty string if the annote was not specified.
	/// </summary>
	public string Annote
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The author entry or an empty string if the author was not specified.
	/// </summary>
	public string Author
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The booktitle entry or an empty string if the booktitle was not specified.
	/// </summary>
	public string BookTitle
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The chapter entry or an empty string if the chapter was not specified.
	/// </summary>
	public string Chapter
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The crossref entry or an empty string if the crossref was not specified.
	/// </summary>
	public string CrossRef
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The edition entry or an empty string if the edition was not specified.
	/// </summary>
	public string Edition
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The editor entry or an empty string if the editor was not specified.
	/// </summary>
	public string Editor
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The howpublished entry or an empty string if the howpublished was not specified.
	/// </summary>
	public string HowPublished
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The institution entry or an empty string if the institution was not specified.
	/// </summary>
	public string Institution
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The journal entry or an empty string if the journal was not specified.
	/// </summary>
	public string Journal
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The note entry or an empty string if the note was not specified.
	/// </summary>
	public string Note
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The number entry or an empty string if the number was not specified.
	/// </summary>
	public string Number
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The organization entry or an empty string if the organization was not specified.
	/// </summary>
	public string Organization
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The pages entry or an empty string if the pages was not specified.
	/// </summary>
	public string Pages
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The publisher entry or an empty string if the publisher was not specified.
	/// </summary>
	public string Publisher
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The school entry or an empty string if the school was not specified.
	/// </summary>
	public string School
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The series entry or an empty string if the series was not specified.
	/// </summary>
	public string Series
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The title entry or an empty string if the title was not specified.
	/// </summary>
	public string Title
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The volume entry or an empty string if the volume was not specified.
	/// </summary>
	public string Volume
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The year entry or an empty string if the year was not specified.
	/// </summary>
	public string Year
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The month entry or an empty string if the month was not specified.
	/// </summary>
	public string Month
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	/// <summary>
	/// The abstract entry or an empty string if the abstract was not specified.
	/// </summary>
	public string Abstract
	{
		get => this[GetFormattedName()];
		set => SetProperty(GetFormattedName(), value);
	}

	#endregion

	#endregion

	#region Events

	//private void OnollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	//{
	//	Modified = true;
	//	OnPropertyChanged(nameof(FieldNames));
	//}

	#endregion

	#region Public Methods

	/// <summary>
	/// Public interface to mark the bibliography part as saved.  This is used to reset the Modified property after saving.
	/// </summary>
	public override void MarkSaved()
	{
		IDictionaryEnumerator fieldEnumerator = _fields.GetEnumerator();
		while (fieldEnumerator.MoveNext())
		{
			Field field = (Field)fieldEnumerator.Value!;
			field.MarkSaved();
		}
		Modified = false;
	}

	/// <summary>
	/// Searches for the first key in the collection whose associated value matches the specified value.	
	/// </summary>
	/// <remarks>
	/// If multiple keys have the same value, only the first matching key is returned. The comparison uses
	/// the string representation of each field value.
	/// </remarks>
	/// <param name="value">The value to search for among the field values.</param>
	/// <param name="caseSensitive">true to perform a case-sensitive comparison; otherwise, false. The default is false.</param>
	/// <returns>The key associated with the first matching value, or an empty string if no match is found.</returns>
	public string FindNameByValue(string value, bool caseSensitive = false)
	{
		string result		= "";
		string matchValue	= caseSensitive ? value : value.ToLower();

		IDictionaryEnumerator fieldEnumerator = _fields.GetEnumerator();
		while (fieldEnumerator.MoveNext())
		{
			Field field				= (Field)fieldEnumerator.Value!;
			string fieldValueString	= field.Value;

			if (!caseSensitive)
			{
				fieldValueString = fieldValueString.ToLower();
			}

			if (fieldValueString == matchValue)
			{
				result = field.Name;
				break;
			}
		}

		return result;
	}

	/// <summary>
	/// Check if the BibEntry contains a field with the given name.
	/// </summary>
	/// <param name="fieldName">Name to check for.</param>
	/// <returns>True if the field name exists, false otherwise.</returns>
	public bool ContainsFieldName(string fieldName)
	{
		if (!_caseSensitiveFields)
		{
			fieldName = fieldName.ToLower();
		}
		return _fields.ContainsKey(fieldName);
	}

	/// <summary>
	/// Get value by given field name (index) or create new field by index and value.
	/// </summary>
	/// <param name="fieldName">Field name.</param>
	public string this[string fieldName]
	{
		get
		{
			if (!_caseSensitiveFields)
			{
				fieldName = fieldName.ToLower();
			}
			return _fields.TryGetValue(fieldName, out Field? field) ? field.Value : "";
		}

		set
		{
			if (!_caseSensitiveFields)
			{
				fieldName = fieldName.ToLower();
			}

			if (_fields.TryGetValue(fieldName, out Field? field))
			{
				if (field.Value != value)
				{
					field.Value = value;
					Modified = true;
				}
			}
			else
			{
				AddNewField(new Field() { Name = fieldName, FieldValue = new FieldValue(value) });
			}
		}
	}

	/// <summary>
	/// Get a FieldValue.
	/// </summary>
	/// <param name="fieldName">Name of the field to get.</param>
	public Field GetField(string fieldName)
	{
		if (!_caseSensitiveFields)
		{
			fieldName = fieldName.ToLower();
		}
		if (_fields.TryGetValue(fieldName, out Field? field))
		{
			return field;
		}
		throw new Exception("Invalid field name: "+fieldName);
	}

	/// <summary>
	/// Set a FieldValue.
	/// </summary>
	/// <param name="fieldName">Name of the field to get.</param>
	public override void SetField(string fieldName, string fieldValue, FieldValueType fieldValueType)
	{
		if (!_caseSensitiveFields)
		{
			fieldName = fieldName.ToLower();
		}

		FieldValue newFieldValueObject = new(fieldValue, fieldValueType);

		bool exists = _fields.ContainsKey(fieldName);
		if (exists)
		{
			Field existing = _fields[fieldName]!;
			if (existing.FieldValue != newFieldValueObject)
			{
				_fields[fieldName].FieldValue = newFieldValueObject;
				Modified = true;
			}
		}
		else
		{
			AddNewField(new Field() { Name = fieldName, FieldValue = newFieldValueObject });
		}
	}

	/// <summary>
	/// Determine if any of the specified fields contain the search string.
	/// </summary>
	/// <param name="fields">The fields to search within.</param>
	/// <param name="searchString">The string to search for.</param>
	/// <param name="caseSensitive">Whether the search should be case-sensitive.</param>
	/// <returns>True if any field contains the search string, false otherwise.</returns>
	public bool DoFieldsContainString(IEnumerable<string> fields, string searchString, bool caseSensitive = false)
	{
		StringComparison stringComparison = caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
		foreach (string fieldName in fields)
		{
			if (ContainsFieldName(fieldName))
			{
				// Some fields may contain brackets "{}" to indicate that the value should not be changed by BibTeX.  We do not want the search to include the brackets.
				string fieldValue = this[fieldName].Replace("{", string.Empty).Replace("}", string.Empty);
				if (fieldValue.Contains(searchString, stringComparison))
				{
					return true;
				}
			}
		}
		return false;
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Uses the calling member name to create a lowercase name to use as an index.
	/// </summary>
	/// <param name="propertyName">Name of the property/calling method.</param>
	private string GetFormattedName([CallerMemberName] string? propertyName = null)
	{
		System.Diagnostics.Debug.Assert(propertyName != null);
		if (!_caseSensitiveFields)
		{
			propertyName = propertyName.ToLower();
		}
		return propertyName;
	}

	/// <summary>
	/// Reusable property setter. Used for common fields such as Author, Title, Year, etc. This will set the value
	/// and raise the PropertyChanged event if the value is different than the current value.
	/// </summary>
	/// <param name="formattedPropertyName">Formatted property name used for storing and retrieving the value.</param>
	/// <param name="value">Value to set.</param>
	/// <param name="propertyName">The name of the property that changed.  Used for event notifications.</param>
	private void SetProperty(string formattedPropertyName, string value, [CallerMemberName] string propertyName = null!)
	{
		if (this[formattedPropertyName] != value)
		{
			this[formattedPropertyName] = value;
			OnPropertyChanged(propertyName);
		}
	}

	/// <summary>
	/// Add a new Field. This will add the Field to the collection and subscribe to the events. It will also raise
	/// the PropertyChanged event for the FieldNames property, as well as set Modified to true.
	/// </summary>
	/// <param name="field">Field to add.</param>
	private void AddNewField(Field field)
	{
		field.ModifiedChanged += OnFieldModifiedChanged;
		field.PropertyChanged += OnFieldPropertyChanged;
		_fields[field.Name] = field;
		OnPropertyChanged(nameof(FieldNames));
		Modified = true;
	}

	#endregion

	#region Public String Writing Methods

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	/// <param name="writeSettings">The settings for writing the bibliography file.</param>
	public override string ToString(WriteSettings writeSettings)
	{
		StringBuilder bibliographyPart = new();

		// Add the comment, if there is one.
		if (!string.IsNullOrEmpty(Comment))
		{
			bibliographyPart.Append(Comment);
		}

		// Build the entry opening and key.
		bibliographyPart.Append("@");
		bibliographyPart.Append(Type);
		char bracketCharacter = writeSettings.BibEntryBracketType == EntryBracketType.CurlyBraces ? '{' : '(';
		bibliographyPart.Append(bracketCharacter);
		bibliographyPart.Append(Key);
		bibliographyPart.Append(',');
		bibliographyPart.Append(writeSettings.NewLine);

		// Write all the Fields.
		OrderedDictionary<string, Field>.Enumerator fieldEnumerator = _fields.GetEnumerator();
		while (fieldEnumerator.MoveNext())
		{
			// Write the initial line indent, Field, line ending comma, and a new line.
			bibliographyPart.Append(writeSettings.Indent);
			bibliographyPart.Append(fieldEnumerator.Current.Value.ToString(writeSettings, writeSettings.BibEntryFieldValueFormat, writeSettings.BibEntryAlignAtTabStop, writeSettings.BibEntryAlignAtColumn));
			bibliographyPart.Append(',');
			bibliographyPart.Append(writeSettings.NewLine);
		}

		// Option to remove comma after last field.
		if (writeSettings.RemoveLastComma)
		{
			// Remove comma after the last field.  To do that, we need to remove the new line character and the
			// comma and then replace it with a new line character.
			bibliographyPart.Remove(bibliographyPart.Length - 1 - writeSettings.NewLine.Length, 1 + writeSettings.NewLine.Length);
			bibliographyPart.Append(writeSettings.NewLine);
		}

		// Closing bracket and end of entry.
		bracketCharacter = writeSettings.BibEntryBracketType == EntryBracketType.CurlyBraces ? '}' : ')';
		bibliographyPart.Append(bracketCharacter);
		bibliographyPart.Append(writeSettings.NewLine);

		return bibliographyPart.ToString();
	}

	#endregion

	#region Public Methods for Names

	/// <summary>
	/// Initialize with a set of (ordered) fields.
	/// </summary>
	public void Initialize(List<string> names)
	{
		foreach (string name in names)
		{
			this[name] = "";
		}
	}

	/// <summary>
	/// Change the Key of a field.
	/// </summary>
	/// <param name="fieldName">Field Key to change.</param>
	/// <param name="newFieldName">New field name.</param>
	/// <exception cref="ArgumentException">Thrown if the new field name already exists.</exception>
	public void RenameField(string fieldName, string newFieldName)
	{
		List<string> fieldNames = FieldNames;

		// It should have already been checked that the key is contained before getting here.
		System.Diagnostics.Trace.Assert(fieldNames.Contains(fieldName));

		Field existingField	= GetField(fieldName);
		existingField.Name	= newFieldName;

		_fields.Remove(fieldName);
		_fields[newFieldName] = existingField;
	}

	#endregion

	#region Public Methods for Values

	/// <summary>
	/// Gets the first author's name.
	/// </summary>
	/// <param name="format">What part of the name should be returned.</param>
	/// <param name="toCase">Text case to return the name in.</param>
	/// <exception cref="NotSupportedException">The name format specified was not valid.</exception>
	public string GetFirstAuthorsName(NameFormat format, StringCase toCase)
	{
		// Get the authors.  The first step is to remove any internal braces ({}).  Then split on the " and " string (need spaces).
		// If there are no authors, return a blank string.
		string authorField = Author.TrimStart('{').TrimEnd('}');
		string[] authors = authorField.Split([" and "], StringSplitOptions.RemoveEmptyEntries);
		if (authors.Length == 0)
		{
			return "";
		}

		string firstAuthorName  = "";
		string result           = "";

		// Split the first author on a comma.  Author names can be in the formats of:
		// William Shakespeare
		// Shakespeare, William
		// If it is in the second format, we will reverse it so we have the name always specified in the same manner.
		// If there is no comma, we should only get 1 result.
		string[] firstAuthorArray   = authors[0].Split([','], StringSplitOptions.RemoveEmptyEntries);
		if (firstAuthorArray.Length == 1)
		{
			// William Shakespeare, nothing required.
			firstAuthorName = firstAuthorArray[0];
		}
		else
		{
			// Shakespeare, William, reverse the order.
			firstAuthorName = firstAuthorArray[1] + " " + firstAuthorArray[0];
		}

		switch (format)
		{
			case NameFormat.Full:
				result = firstAuthorName;
				break;

			case NameFormat.First:
				result = (firstAuthorName.Split([' '], StringSplitOptions.RemoveEmptyEntries))[0];
				break;

			case NameFormat.Last:
				// Split the full name into separate words/name.
				firstAuthorArray = firstAuthorName.Split([' '], StringSplitOptions.RemoveEmptyEntries);

				// We don't want to return "Sr.", "Jr.", et cetera, so work backwards and ignore any of those.
				// The first word we find that is not in our rejected list, we will treat as the last name.
				for (int i = firstAuthorArray.Length-1; i >= 0; i--)
				{
					if (!_nameSuffixes.Any(item => item == firstAuthorArray[i]))
					{
						result = firstAuthorArray[i];
						break;
					}
				}
				break;

			default:
				throw new NotSupportedException("The name format specified is not valid.");
		}

		return Format.ChangeCase(result, toCase);
	}

	#endregion

} // End class.