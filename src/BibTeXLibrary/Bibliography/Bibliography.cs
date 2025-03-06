using DigitalProduction.Strings;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace BibTeXLibrary;

/// <summary>
/// Internal representation of a bib file.
/// </summary>
public class Bibliography
{
	#region Fields

	private readonly BibliographyDOM				_bibliographyDOM				= new();

	#endregion

	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public Bibliography()
	{	 
	}

	#endregion
	
	#region Properties

	/// <summary>
	/// BibTeX entries.
	/// </summary>
	[XmlIgnore()]
	public ObservableCollection<BibEntry> Entries { get => _bibliographyDOM.BibliographyEntries;}

	/// <summary>
	/// The bibliography document object model.
	/// </summary>
	public BibliographyDOM DocumentObjectModel { get => _bibliographyDOM; }

	#endregion

	#region Methods

	#region Reading and Writing

	/// <summary>
	/// Read the bibliography file.
	/// </summary>
	/// <param name="bibFilePath">Full path to the bibliography file.</param>
	public void Read(string bibFilePath)
	{
		_bibliographyDOM.Clear();
		try
		{
			BibParser parser = new(bibFilePath);
			parser.Parse(_bibliographyDOM);
		}
		catch (UnexpectedTokenException exception)
		{
			throw new Exception($"A parsing error occured reading the bibliography file:\n" + bibFilePath + "\n\n" + exception.Message);
		}
		catch (Exception exception)
		{
			throw new Exception($"An error occured reading the bibliography file:\n" + bibFilePath + "\n\n" + exception.Message);
		}
	}

	/// <summary>
	/// Read the bibliography file.
	/// </summary>
	/// <param name="bibFilePath">Full path to the bibliography file.</param>
	/// <param name="bibEntryInitializationFile">Full path to the bibliography entry initialization file.</param>
	public void Read(string bibFilePath, string bibEntryInitializationFile)
	{
		_bibliographyDOM.Clear();
		try
		{
			BibParser parser = new(bibFilePath, bibEntryInitializationFile);
			parser.Parse(_bibliographyDOM);
		}
		catch (UnexpectedTokenException exception)
		{
			throw new Exception($"An parsing error occured reading the bibliography file:\n" + bibFilePath + "\n\n" + exception.Message);
		}
		catch (Exception exception)
		{
			throw new Exception($"An error occured reading the bibliography file:\n" + bibFilePath + "\nUsing the initialization file:\n" + bibEntryInitializationFile + "\n\n" + exception.Message);
		}
	}

	/// <summary>
	/// Write the bibiography file.
	/// </summary>
	/// <param name="path">Full path to the bibiography file.</param>
	public void Write(string path)
	{
		Write(path, new WriteSettings());
	}

	/// <summary>
	/// Write the bibiography file.
	/// </summary>
	/// <param name="path">Full path to the bibiography file.</param>
	/// <param name="writeSettings">The settings for writing the bibliography file.</param>
	public void Write(string path, WriteSettings writeSettings)
	{
		//_bibEntryInitialization.Serialize(GetBibEntryInitializationPath(path));

		using StreamWriter streamWriter = new(path);

		// Make sure the BibEntries use the expected line feed and carriage return character(s).
		writeSettings.NewLine = streamWriter.NewLine;

		// Write the header.  The header is stored as separate lines so when we write it we can use
		// the expected line ending type (\r\n, \n) used by the writer.
		foreach (string line in _bibliographyDOM.Header)
		{
			streamWriter.WriteLine(line);
		}

		// Write each entry with a blank line preceeding it.
		foreach (BibEntry bibEntry in _bibliographyDOM.BibliographyEntries)
		{
			streamWriter.WriteLine();
			streamWriter.Write(bibEntry.ToString(writeSettings));
		}
	}

	/// <summary>
	/// Clean up.
	/// </summary>
	public void Close()
	{
	}

	#endregion

	#region Key Generation

	private static IAlphaNumericStringProvider GetSuffixGenerator()
	{
		// Provide a sequence of incremented strings.  For example, a,b,c or A,B,C.
		return new EnglishLowerCaseAlphabet();
	}

	/// <summary>
	/// Checks if the key follows the rules to be a valid auto key.
	/// </summary>
	/// <param name="entry">BibEntry to check.</param>
	public bool HasValidAutoCiteKey(BibEntry entry)
	{
		string keyBase = GenerateCiteKeyBase(entry);

		// If the key base is longer, it is definitely not valid and will cause an error when getting the sub string below.
		if (keyBase.Length > entry.Key.Length)
		{
			return false;
		}

		return keyBase == entry.Key[..keyBase.Length];
	}

	/// <summary>
	/// Generates a new, unique key for the entry and sets it.
	/// </summary>
	/// <param name="entry">BibEntry to generate a key for.</param>

	public void GenerateUniqueCiteKey(BibEntry entry)
	{
		string key = GenerateCiteKeyBase(entry);

		// Needs to be last.
		key += GenerateCiteKeySuffix(key.ToString());

		entry.Key = key.ToString();
	}

	/// <summary>
	/// Generate the base of a cite key (absent the suffix).
	/// </summary>
	/// <param name="entry">BibEntry.</param>
	private static string GenerateCiteKeyBase(BibEntry entry)
	{
		string prefix = "ref:";
		StringBuilder key = new(prefix);

		// This is setup to allow different key formats such as first and last name of author, et cetera.  For now last name and lower case.
		// The names may have special characters and those need to be removed.  We do this by only allowing certain letters.
		string name           = entry.GetFirstAuthorsName(NameFormat.Last, StringCase.LowerCase);
		StringBuilder keyName = new();
		foreach (char c in name.ToCharArray())
		{
			if (c >= 'a' & c <= 'z' | c >= '0' & c <= '9')
			{
				keyName.Append(c);
			}
		}

		key.Append(keyName);
		key.Append(entry.Year);
		return key.ToString();
	}

	/// <summary>
	/// Generation a cite key suffix.
	/// </summary>
	/// <param name="baseKey">The cite key base.</param>
	/// <exception cref="IndexOutOfRangeException">Thrown if the algorithm runs out of suffixes to try.</exception>
	private string GenerateCiteKeySuffix(string baseKey)
	{
		foreach (string suffix in GetSuffixGenerator().Get())
		{
			if (!IsKeyInUse(baseKey+suffix))
			{
				return suffix;
			}
		}

		// We only go through one loop of a suffix generator.  If we run out it is an exception, we don't currently
		// handle this case.  An example would be using lower case letters and all names from
		// ref:shakespeare1597a to ref:shakespeare1597z were used.  Highly unlikely.
		throw new IndexOutOfRangeException("Ran out of suffix characters.");
	}

	public bool IsKeyInUse(string key)
	{
		// BibTeX seems to be case sensitive keys.  I.e.
		// @book{ref:shakespeare,
		// is different from
		// @book{ref:Shakespeare,
		// However, this could be confusing or error prone, so (for now anyway) we will do a case insensitive comparison.
		key = key.ToLower();

		foreach (BibEntry entry in _bibliographyDOM.BibliographyEntries)
		{
			if (entry.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}

	#endregion

	#endregion

} // End class.