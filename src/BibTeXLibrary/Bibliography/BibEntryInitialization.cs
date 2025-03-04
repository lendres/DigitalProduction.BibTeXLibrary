﻿using DigitalProduction.Xml.Serialization;
using System.Xml.Serialization;

namespace BibTeXLibrary;

/// <summary>
/// 
/// </summary>
[XmlRoot("bibentryinitialization")]
public class BibEntryInitialization
{
	#region Fields

	private SerializableDictionary<string, string>			_typeToTemplateMap			= [];
	private SerializableDictionary<string, List<string>>	_templates					= [];

	#endregion

	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public BibEntryInitialization()
	{
	}

	#endregion

	#region Properties

	/// <summary>
	/// Type bibliography type to template map.
	/// </summary>
	[XmlElement("typetotemplatemap")]
	public SerializableDictionary<string, string> TypeToTemplateMap { get => _typeToTemplateMap; set => _typeToTemplateMap = value; }

	/// <summary>
	/// The templates used to initialize a BibEntry.
	/// </summary>
	[XmlElement("templates")]
	public SerializableDictionary<string, List<string>> Templates { get => _templates; set => _templates = value; }

	public List<string> TypeNames { get => [.. from string item in _typeToTemplateMap.Keys select item]; }

	public List<string> TemplateNames { get => [.. from string item in _templates.Keys select item]; }

	#endregion

	#region Methods

	/// <summary>
	/// Gets the default set of (ordered) tags for a type of bibliography entry.
	/// </summary>
	/// <param name="bibEntry">BibTex entry type.</param>
	public List<string> GetDefaultTags(BibEntry bibEntry)
	{
		return GetDefaultTags(bibEntry.Type);
	}

	/// <summary>
	/// Gets the default set of (ordered) tags for a type of bibliography entry.
	/// </summary>
	/// <param name="type">BibTex entry type.</param>
	public List<string> GetDefaultTags(string type)
	{
		type = type.ToLower();

		if (_typeToTemplateMap.TryGetValue(type, out string? template))
		{
			return _templates[template];
		}
		else
		{
			return [];
		}
	}

	#endregion

	#region XML

	/// <summary>
	/// Write this object to a file to the provided path.
	/// </summary>
	/// <param name="path">Path (full path and filename) to write to.</param>
	/// <exception cref="InvalidOperationException">Thrown when the projects path is not valid.</exception>
	public void Serialize(string path)
	{
		if (!DigitalProduction.IO.Path.PathIsWritable(path))
		{
			throw new InvalidOperationException("The file cannot be saved.  A valid path must be specified.");
		}
		SerializationSettings settings = new(this, path);
		settings.XmlSettings.NewLineOnAttributes = false;
		Serialization.SerializeObject(settings);
	}

	/// <summary>
	/// Create an instance from a file.
	/// </summary>
	/// <param name="path">The file to read from.</param>
	public static BibEntryInitialization? Deserialize(string path)
	{
		return Serialization.DeserializeObject<BibEntryInitialization>(path);
	}

	#endregion

} // End class.