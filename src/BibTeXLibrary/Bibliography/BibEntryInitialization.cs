using DigitalProduction.Xml.Serialization;
using System.Xml.Serialization;

namespace BibTeXLibrary;

/// <summary>
/// 
/// </summary>
[XmlRoot("bibentryinitialization")]
public class BibEntryInitialization
{
	#region Fields

	private string											_path						= string.Empty;
	private SerializableDictionary<string, NameMap>			_typeToTemplateMappings		= [];
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
	[XmlElement("typetotemplatemappings")]
	public SerializableDictionary<string, NameMap> TypeToTemplateMappings { get => _typeToTemplateMappings; set => _typeToTemplateMappings = value; }

	/// <summary>
	/// The templates used to initialize a BibEntry.
	/// </summary>
	[XmlElement("templates")]
	public SerializableDictionary<string, List<string>> Templates { get => _templates; set => _templates = value; }

	[XmlIgnore()]
	public List<string> TypeNames { get => [.. from string item in _typeToTemplateMappings.Keys select item]; }

	[XmlIgnore()]
	public List<NameMap> NameMaps { get => [.. from NameMap item in _typeToTemplateMappings.Values select item]; }

	[XmlIgnore()]
	public List<string> TemplateNames { get => [.. from string item in _templates.Keys select item]; }

	public string this[string type]
	{
		get
		{
			type = type.ToLower();
			bool foundTemplate = _typeToTemplateMappings.TryGetValue(type, out NameMap? nameMap);
			return foundTemplate? nameMap!.To : string.Empty;
		}
	}

	#endregion

	#region Methods

	/// <summary>
	/// Interface for user interfaces to use to set the type to template mappings.
	/// </summary>
	/// <param name="nameMaps">The NameMaps</param>
	public IEnumerable<NameMap> CopyNameMaps()
	{
		List<NameMap> newNameMaps = new();
		foreach (KeyValuePair<string, NameMap> keyValuyePair in _typeToTemplateMappings)
		{
			newNameMaps.Add(new NameMap(keyValuyePair.Value));
		}
		return newNameMaps;
	}

	/// <summary>
	/// Interface for user interfaces to use to set the type to template mappings.
	/// </summary>
	/// <param name="nameMaps">The NameMaps</param>
	public void SetNameMaps(IEnumerable<NameMap> nameMaps)
	{
		_typeToTemplateMappings.Clear();
		foreach (NameMap nameMap in nameMaps)
		{
			_typeToTemplateMappings[nameMap.From] = new NameMap(nameMap);
		}
	}

	/// <summary>
	/// Interface for user interfaces to use to set the type to templates.
	/// </summary>
	/// <param name="nameMaps"></param>
	public void SetTemplates(SerializableDictionary<string, List<string>> templates)
	{
		_templates.Clear();
		foreach (KeyValuePair<string, List<string>> template in templates)
		{
			_templates[template.Key] = new List<string>(template.Value);
		}
	}

	/// <summary>
	/// Gets the default set of (ordered) fields for a type of bibliography entry.
	/// </summary>
	/// <param name="bibEntry">BibTex entry type.</param>
	public List<string> GetDefaultFields(BibEntry bibEntry)
	{
		return GetDefaultFields(bibEntry.Type);
	}

	/// <summary>
	/// Gets the default set of (ordered) fields for a type of bibliography entry.
	/// </summary>
	/// <param name="type">BibTex entry type.</param>
	public List<string> GetDefaultFields(string type)
	{
		string toType = this[type];
		return string.IsNullOrEmpty(toType) ? [] : _templates[toType];
	}

	#endregion

	#region XML

	/// <summary>
	/// Write this object to a file to the default path (where it was deserialized from).
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when the projects path is not valid.</exception>
	public void Serialize()
	{
		Serialize(_path);
	}

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
		BibEntryInitialization? initializer = Serialization.DeserializeObject<BibEntryInitialization>(path);
		if (initializer != null)
		{
			initializer._path = path;
		}
		return initializer;
	}

	#endregion

} // End class.