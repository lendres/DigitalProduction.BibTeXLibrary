﻿using DigitalProduction.Strings;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace BibTeXLibrary;

/// <summary>
/// A bibliography entry.
/// </summary>
public class BibEntry : BibliographyPart
{
	#region Fields

	private static readonly string[]                            _nameSuffixes           = ["jr", "jr.", "sr", "sr.", "ii", "iii", "iv", "v", @"p\`{e}re", "fils"];

	/// <summary>Store all tags.</summary>
	protected readonly OrderedDictionary<string, TagValue>      _tags                   = [];

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
		bibEntry.Initialize(bibEntryInitialization.GetDefaultTags(type));

		return bibEntry;
	}

	#endregion

	#region Public Properties

	/// <summary>
	/// Bibliography entry type, e.g. "book", "thesis", "string".  This is the name that follows the "@".
	/// </summary>
	public override string Type { get => GetValueOrDefault<string>(string.Empty); set => SetValue(value); }

	/// <summary>
	/// Entry's key.
	/// </summary>
	public string Key { get => GetValueOrDefault<string>(string.Empty); set => SetValue(value); }

	/// <summary>
	/// Get the names of the tags.
	/// </summary>
	public List<string> TagNames { get => (from string item in _tags.Keys select item).ToList(); }

	/// <summary>
	/// The address entry or an empty string if the address was not specified.
	/// </summary>
	public string Address
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The annote entry or an empty string if the annote was not specified.
	/// </summary>
	public string Annote
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The author entry or an empty string if the author was not specified.
	/// </summary>
	public string Author
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The booktitle entry or an empty string if the booktitle was not specified.
	/// </summary>
	public string BookTitle
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The chapter entry or an empty string if the chapter was not specified.
	/// </summary>
	public string Chapter
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The crossref entry or an empty string if the crossref was not specified.
	/// </summary>
	public string CrossRef
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The edition entry or an empty string if the edition was not specified.
	/// </summary>
	public string Edition
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The editor entry or an empty string if the editor was not specified.
	/// </summary>
	public string Editor
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The howpublished entry or an empty string if the howpublished was not specified.
	/// </summary>
	public string HowPublished
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The institution entry or an empty string if the institution was not specified.
	/// </summary>
	public string Institution
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The journal entry or an empty string if the journal was not specified.
	/// </summary>
	public string Journal
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The note entry or an empty string if the note was not specified.
	/// </summary>
	public string Note
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The number entry or an empty string if the number was not specified.
	/// </summary>
	public string Number
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The organization entry or an empty string if the organization was not specified.
	/// </summary>
	public string Organization
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The pages entry or an empty string if the pages was not specified.
	/// </summary>
	public string Pages
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The publisher entry or an empty string if the publisher was not specified.
	/// </summary>
	public string Publisher
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The school entry or an empty string if the school was not specified.
	/// </summary>
	public string School
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The series entry or an empty string if the series was not specified.
	/// </summary>
	public string Series
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The title entry or an empty string if the title was not specified.
	/// </summary>
	public string Title
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The volume entry or an empty string if the volume was not specified.
	/// </summary>
	public string Volume
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The year entry or an empty string if the year was not specified.
	/// </summary>
	public string Year
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The month entry or an empty string if the month was not specified.
	/// </summary>
	public string Month
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The abstract entry or an empty string if the abstract was not specified.
	/// </summary>
	public string Abstract
	{
		get => this[GetFormattedName()];

		set
		{
			this[GetFormattedName()] = value;
			OnPropertyChanged();
		}
	}

	#endregion

	#region Public Methods

	public string FindTagValue(string value, bool caseSensitive = false)
	{
		string result = "";
		string matchValue = caseSensitive ? value : value.ToLower();

		IDictionaryEnumerator tagEnumerator = _tags.GetEnumerator();
		while (tagEnumerator.MoveNext())
		{
			TagValue tagValue		= (TagValue)tagEnumerator.Value!;
			string tagValueString	= tagValue.ToString(TagValueFormat.None)!;

			if (!caseSensitive)
			{
				tagValueString = tagValueString.ToLower();
			}

			if (tagValueString == matchValue)
			{
				result = tagEnumerator.Key.ToString()!;
				break;
			}
		}

		return result;
	}

	/// <summary>
	/// Get value by given tag name (index) or create new tag by index and value.
	/// </summary>
	/// <param name="tagName">Tag name.</param>
	public string this[string tagName]
	{
		get
		{
			if (!_caseSensitivetags)
			{
				tagName = tagName.ToLower();
			}
			return _tags.ContainsKey(tagName) ? _tags[tagName].Content : "";
		}

		set
		{
			if (!_caseSensitivetags)
			{
				tagName = tagName.ToLower();
			}

			if (_tags.TryGetValue(tagName, out TagValue? tagValue))
			{
				tagValue.Content = value;
				Modified = true;
			}
			else
			{
				_tags[tagName] = new TagValue(value);
				OnPropertyChanged(nameof(TagNames));
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Get a TagValue.
	/// </summary>
	/// <param name="tagName">Name of the tag to get.</param>
	public TagValue GetTagValue(string tagName)
	{
		if (!_caseSensitivetags)
		{
			tagName = tagName.ToLower();
		}
		if (_tags.TryGetValue(tagName, out TagValue? tagValue))
		{
			return tagValue;
		}
		throw new Exception("Invalid tag name: "+tagName);
	}

	/// <summary>
	/// Set a TagValue.
	/// </summary>
	/// <param name="tagName">Name of the tag to get.</param>
	public override void SetTagValue(string tagName, string tagValue, TagValueType tagValueType)
	{
		if (!_caseSensitivetags)
		{
			tagName = tagName.ToLower();
		}

		TagValue tagValueObject = new(tagValue);
		switch (tagValueType)
		{
			case TagValueType.String:
				// This is a regular string, so put brackets around it.
				tagValueObject.Format = TagValueFormat.Bracket;
				break;

			case TagValueType.StringConstant:
				// This is a string constant so we do not want brackets around it.
				tagValueObject.Format = TagValueFormat.None;
				break;
		}

		bool exists = _tags.ContainsKey(tagName);
		_tags[tagName] = tagValueObject;
		Modified = true;
		if (!exists)
		{
			OnPropertyChanged(nameof(TagNames));
		}
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
		if (!_caseSensitivetags)
		{
			propertyName = propertyName.ToLower();
		}
		return propertyName;
	}

	#endregion

	#region Public String Writing Methods

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	/// <param name="writeSettings">The settings for writing the bibliography file.</param>
	public override string ToString(WriteSettings writeSettings)
	{
		// Build the entry opening and key.
		StringBuilder bibliographyPart = new("@");
		bibliographyPart.Append(Type);
		bibliographyPart.Append('{');
		bibliographyPart.Append(Key);
		bibliographyPart.Append(',');
		bibliographyPart.Append(writeSettings.NewLine);

		// Write all the tags.
		IDictionaryEnumerator tagEnumerator = _tags.GetEnumerator();
		while (tagEnumerator.MoveNext())
		{
			// Initial line indent and tag key.
			bibliographyPart.Append(writeSettings.Indent);
			//bib.Append(tag.Key);
			bibliographyPart.Append(tagEnumerator.Key.ToString());

			// Add the space between the key and equal sign.
			bibliographyPart.Append(writeSettings.GetInterTagSpacing(tagEnumerator.Key.ToString()!));

			// Add the tag value.
			bibliographyPart.Append("= ");
			bibliographyPart.Append(tagEnumerator.Value!.ToString());
			bibliographyPart.Append(',');

			// End the line.
			bibliographyPart.Append(writeSettings.NewLine);
		}
		// Option to remove comma after last tag.
		if (writeSettings.RemoveLastComma)
		{
			// Remove comma after the last tag.  To do that, we need to remove the new line character and the
			// comma and then replace it with a new line character.
			bibliographyPart.Remove(bibliographyPart.Length - 1 - writeSettings.NewLine.Length, 1 + writeSettings.NewLine.Length);
			bibliographyPart.Append(writeSettings.NewLine);
		}

		// Closing bracket and end of entry.
		bibliographyPart.Append('}');
		bibliographyPart.Append(writeSettings.NewLine);

		return bibliographyPart.ToString();
	}

	#endregion

	#region Public Tag Keys

	/// <summary>
	/// Initialize with a set of (ordered) tags.
	/// </summary>
	public void Initialize(List<string> tags)
	{
		foreach (string tag in tags)
		{
			this[tag] = "";
		}
	}

	/// <summary>
	/// Change the Key of a tag.
	/// </summary>
	/// <param name="tagKey">Tag Key to change.</param>
	/// <param name="newTagKey">New tag Key.</param>
	/// <exception cref="ArgumentException">Thrown if the new tag Key already exists.</exception>
	public void RenameTagKey(string tagKey, string newTagKey)
	{
		List<string> tagNames = TagNames;

		// It should have already been checked that the key is contained before getting here.
		System.Diagnostics.Debug.Assert(tagNames.Contains(tagKey));

		TagValue value = GetTagValue(tagKey);

		_tags.Remove(tagKey);

		if (tagNames.Contains(newTagKey))
		{
			// The new tag key can exist, but it must be empty.  Don't overwrite existing content.
			if (GetTagValue(newTagKey).Content != "")
			{
				throw new ArgumentException("The tag key \"" + newTagKey + "\" already exists.");
			}

			_tags[newTagKey] = value;
		}
		else
		{
			_tags.Add(newTagKey, value);
		}
	}

	#endregion

	#region Public Tag Values

	/// <summary>
	/// Gets the first author's name.
	/// </summary>
	/// <param name="format">What part of the name should be returned.</param>
	/// <param name="toCase">Text case to return the name in.</param>
	/// <exception cref="NotSupportedException">The name format specified was not valid.</exception>
	public string GetFirstAuthorsName(NameFormat format, StringCase toCase)
	{
		// Get the authors.  The first step is to remove any internal braces ({}).  Then split on the "and" string.
		// If there are no authors, return a blank string.
		string authorTag = Author.TrimStart('{').TrimEnd('}');
		string[] authors = authorTag.Split(["and"], StringSplitOptions.RemoveEmptyEntries);
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