using BibTeXLibrary;
using System.Xml.Serialization;

namespace DigitalProduction.UnitTests;

public class BibEntryInitializationTests
{
	#region Fields

	private readonly string _initializationFilePath;

	#endregion

	#region Construction

	public BibEntryInitializationTests()
	{
		_initializationFilePath = FindRepositoryFile("User Files", "Bibliography Entry Initialization.bibtmp");
	}

	#endregion

	#region Tests

	[Fact]
	public void TestDeserialize()
	{
		BibEntryInitialization? initialization = BibEntryInitialization.Deserialize(_initializationFilePath);

		Assert.NotNull(initialization);
		Assert.NotNull(initialization!.TypeToTemplateMappings);
		Assert.NotNull(initialization.Templates);
	}

	[Fact]
	public void TestDeserializeReadsExpectedAliasAndTemplate()
	{
		BibEntryInitialization? initialization = BibEntryInitialization.Deserialize(_initializationFilePath);

		Assert.NotNull(initialization);

		Assert.Contains("conference", initialization!.TypeNames);
		Assert.Contains("article", initialization.TemplateNames);

		Assert.Equal("basic", initialization["conference"]);
		Assert.Equal("inproceedings", initialization["incollection"]);

		Assert.Equal(["author", "title"], initialization.Templates["basic"]);
		Assert.Equal("author", initialization.Templates["article"][0]);
		Assert.Equal("title", initialization.Templates["article"][1]);
		Assert.Equal("journal", initialization.Templates["article"][2]);
	}

	[Fact]
	public void TestAliases()
	{
		BibEntryInitialization? initialization = BibEntryInitialization.Deserialize(_initializationFilePath);

		Assert.NotNull(initialization);

		Assert.Equal("basic", initialization!["conference"]);
		Assert.Equal("inproceedings", initialization["incollection"]);
		Assert.Equal("thesis", initialization["mastersthesis"]);
		Assert.Equal("thesis", initialization["phdthesis"]);
	}

	[Fact]
	public void TestGetDefaultFieldsFromString()
	{
		BibEntryInitialization? initialization = BibEntryInitialization.Deserialize(_initializationFilePath);

		Assert.NotNull(initialization);

		List<string> fields = initialization!.GetDefaultFields("ConFeRence");

		Assert.Equal(["author", "title"], fields);
	}

	[Fact]
	public void TestBibEntryInitialization()
	{
		BibEntryInitialization? initialization = BibEntryInitialization.Deserialize(_initializationFilePath);

		BibEntry entry = BibEntry.NewBibEntryFromTemplate(initialization!, "proceedings");

		List<string> fields = entry.FieldNames;

		Assert.Equal("author", fields[0]);
		Assert.Equal("affiliation", fields[1]);
		Assert.Equal("title", fields[2]);
		Assert.Equal("booktitle", fields[3]);
	}

	[Fact]
	public void TestGetDefaultFieldsForUnknownType()
	{
		BibEntryInitialization? initialization = BibEntryInitialization.Deserialize(_initializationFilePath);

		Assert.NotNull(initialization);

		List<string> fields = initialization!.GetDefaultFields("notARealType");

		Assert.Empty(fields);
	}

	[Fact]
	public void Deserialize()
	{
		string xml =
			"""
			<?xml version="1.0" encoding="us-ascii"?>
			<bibentryinitialization xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
			    <typetotemplatemappings>
			        <item key="booklet">
						<value from="booklet" to="basic"/>
			        </item>
			    </typetotemplatemappings>
			    <templates>
			        <item key="basic">
			            <value>author</value>
			            <value>title</value>
			        </item>
			    </templates>
			</bibentryinitialization>
			""";

		BibEntryInitialization initialization = DeserializeObjectFromString<BibEntryInitialization>(xml);

		Assert.Equal("basic", initialization["booklet"]);
	}

	[Fact]
	public void Serialize()
	{
		BibEntryInitialization initialization = CreateTestInstance();

		string xml = SerializeObjectToString(initialization);

		Assert.Contains("booklet", xml);
	}

	#endregion

	#region Helper Methods

	private static string FindRepositoryFile(params string[] pathParts)
	{
		DirectoryInfo? directory = new(AppContext.BaseDirectory);

		while (directory is not null)
		{
			string candidatePath = Path.Combine([directory.FullName, .. pathParts]);

			if (File.Exists(candidatePath))
			{
				return candidatePath;
			}

			directory = directory.Parent;
		}

		throw new FileNotFoundException(
			$"Could not locate repository file: {Path.Combine(pathParts)}");
	}

	private static BibEntryInitialization CreateTestInstance()
	{
		BibEntryInitialization initialization = new();

		initialization.TypeToTemplateMappings.Add("booklet", new NameMap("booklet", "basic"));
		initialization.Templates.Add("basic", new List<string> { "author", "title" });

		return initialization;
	}

	private static string SerializeObjectToString<T>(T value)
	{
		XmlSerializer serializer = new(typeof(T));

		using StringWriter stringWriter = new();
		serializer.Serialize(stringWriter, value);

		return stringWriter.ToString();
	}

	private static T DeserializeObjectFromString<T>(string xml)
	{
		XmlSerializer serializer = new(typeof(T));
		using StringReader stringReader = new(xml);
		return (T)serializer.Deserialize(stringReader)!;
	}

	#endregion

} // End class.