using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibTeXLibrary;

namespace DigitalProduction.UnitTests;

public class BibEntryInitializationTests
{
	#region Fields

	private readonly string _initializationFilePath;

	#endregion

	#region Construction

	public BibEntryInitializationTests()
	{
		_initializationFilePath = FindRepositoryFile("User Files", "Bib Entry Initialization.tagord");
	}

	#endregion

	#region Tests

	[Fact]
	public void TestDeserialize()
	{
		BibEntryInitialization? initialization = BibEntryInitialization.Deserialize(_initializationFilePath);

		Assert.NotNull(initialization);
		Assert.NotNull(initialization!.Aliases);
		Assert.NotNull(initialization.Templates);
	}

	[Fact]
	public void TestDeserializeReadsExpectedAliasAndTemplate()
	{
		BibEntryInitialization? initialization = BibEntryInitialization.Deserialize(_initializationFilePath);

		Assert.NotNull(initialization);

		Assert.Contains("conference", initialization!.TypeNames);
		Assert.Contains("article", initialization.TemplateNames);

		Assert.Equal("basic", initialization.Aliases["conference"]);
		Assert.Equal("inproceedings", initialization.Aliases["incollection"]);

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

		Assert.Equal("basic", initialization!.Aliases["conference"]);
		Assert.Equal("inproceedings", initialization.Aliases["incollection"]);
		Assert.Equal("thesis", initialization.Aliases["mastersthesis"]);
		Assert.Equal("thesis", initialization.Aliases["phdthesis"]);
	}

	[Fact]
	public void TestGetDefaultTagsFromString()
	{
		BibEntryInitialization? initialization = BibEntryInitialization.Deserialize(_initializationFilePath);

		Assert.NotNull(initialization);

		List<string> tags = initialization!.GetDefaultTags("ConFeRence");

		Assert.Equal(["author", "title"], tags);
	}

	[Fact]
	public void TestBibEntryInitialization()
	{
		BibEntryInitialization? initialization = BibEntryInitialization.Deserialize(_initializationFilePath);

		BibEntry entry = BibEntry.NewBibEntryFromTemplate(initialization!, "inproceedings");

		List<string> tags = entry.TagNames;

		Assert.Equal("author", tags[0]);
		Assert.Equal("affiliation", tags[1]);
		Assert.Equal("title", tags[2]);
		Assert.Equal("booktitle", tags[3]);
	}

	[Fact]
	public void TestGetDefaultTagsForUnknownType()
	{
		BibEntryInitialization? initialization = BibEntryInitialization.Deserialize(_initializationFilePath);

		Assert.NotNull(initialization);

		List<string> tags = initialization!.GetDefaultTags("notARealType");

		Assert.Empty(tags);
	}

	#endregion

	#region Private Methods

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

	#endregion
} // End class.
