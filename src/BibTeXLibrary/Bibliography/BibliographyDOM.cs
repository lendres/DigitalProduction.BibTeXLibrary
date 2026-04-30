using DigitalProduction.ComponentModel;
using DigitalProduction.Projects;
using DigitalProduction.Strings;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace BibTeXLibrary;

/// <summary>
/// Bibliography Document Object Model.
/// </summary>
public class BibliographyDOM : NotifyPropertyModifiedChanged
{
	#region Fields

	private readonly List<string>								_header				= [];
	private readonly ObservableCollection<BibEntry>				_bibEntries			= [];
	private readonly ObservableCollection<StringConstant>		_strings			= [];

	#endregion

	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public BibliographyDOM()
	{
		_bibEntries.CollectionChanged	+= OnBibEntriesCollectionChanged;
		_strings.CollectionChanged		+= OnStringsCollectionChanged;
	}

	#endregion

	#region Properties

	/// <summary>
	/// The file header.
	/// </summary>
	public List<string> Header { get { return _header; } }

	/// <summary>
	/// Get the bibliography entries.
	/// </summary>
	public ObservableCollection<BibEntry> Entries { get => _bibEntries; }

	/// <summary>
	/// The number of bibliography entries.
	/// </summary>
	public int NumberOfBibliographyEntries { get => _bibEntries.Count; }

	/// <summary>
	/// String constants.
	/// </summary>
	public ObservableCollection<StringConstant> StringConstants { get => _strings; }

	/// <summary>
	/// The number of string constants.
	/// </summary>
	public int NumberOfStringConstants { get => _strings.Count; }

	#endregion

	#region Events

	private void OnBibEntriesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		Modified = true;
		OnPropertyChanged(nameof(Entries));
		OnPropertyChanged(nameof(NumberOfBibliographyEntries));
	}

	private void OnStringsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		Modified = true;
		OnPropertyChanged(nameof(StringConstants));
		OnPropertyChanged(nameof(NumberOfStringConstants));
	}

	private void OnPartModifiedChanged(object sender, bool modified)
	{
		Modified = true;
	}

	private void OnPartPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
	{
		OnPropertyChanged(sender, eventArgs);
	}

	#endregion

	#region General Methods

	/// <summary>
	/// Clear all the data.
	/// </summary>
	public void Clear()
	{
		_header.Clear();
		_bibEntries.Clear();
		_strings.Clear();
	}

	#endregion

	#region Add Methods

	/// <summary>
	/// Add a bibliography entry or a string.
	/// </summary>
	/// <param name="part">BibliographyPart.</param>
	public void Add(BibliographyPart part)
	{
		part.PropertyChanged += OnPartPropertyChanged;
		part.ModifiedChanged += OnPartModifiedChanged;
		if (part.Type.Equals("string", StringComparison.CurrentCultureIgnoreCase))
		{
			_strings.Add((StringConstant)part);
		}
		else
		{
			_bibEntries.Add((BibEntry)part);
		}
	}

	/// <summary>
	/// Add a line to the header.
	/// </summary>
	/// <param name="line">Line to add.</param>
	public void AddHeaderLine(string line)
	{
		_header.Add(line);
	}

	/// <summary>
	/// Insert a bibliography entry 
	/// </summary>
	/// <param name="part">BibliographyPart.</param>
	public void Insert(BibEntry bibEntry, SortBy sortBy)
	{
		int position = FindInsertIndex(bibEntry, sortBy);
		Insert(bibEntry, position);
	}

	/// <summary>
	/// Add a bibliography entry or a string.
	/// </summary>
	/// <param name="part">BibliographyPart.</param>
	public void Insert(BibEntry bibEntry, int position)
	{
		bibEntry.PropertyChanged += OnPartPropertyChanged;
		bibEntry.ModifiedChanged += OnPartModifiedChanged;

		_bibEntries.Insert(position, bibEntry);
	}

	/// <summary>
	/// Add a bibliography entry or a string.
	/// </summary>
	/// <param name="part">BibliographyPart.</param>
	public void Insert(StringConstant stringConstant, SortStringsBy sortBy)
	{
		int position = FindInsertIndex(stringConstant, sortBy);
		Insert(stringConstant, position);
	}

	/// <summary>
	/// Add a bibliography entry or a string.
	/// </summary>
	/// <param name="part">BibliographyPart.</param>
	public void Insert(StringConstant stringConstant, int position)
	{
		stringConstant.PropertyChanged += OnPartPropertyChanged;
		stringConstant.ModifiedChanged += OnPartModifiedChanged;

		_strings.Insert(position, stringConstant);
	}

	#endregion

	#region Get Methods

	/// <summary>
	/// Returns an enumerable collection of string names contained in the current instance.
	/// </summary>
	/// <returns>
	/// An enumerable collection of strings representing the names of all stored string constants. The collection will be
	/// empty if no string constants are present.
	/// </returns>
	public IEnumerable<string> GetStringNames()
	{
		List<string> names = [];
		foreach (StringConstant str in _strings)
		{
			names.Add(str.Name);
		}
		return names;
	}

	/// <summary>
	/// Returns an enumerable collection of string values contained in the current instance.	
	/// </summary>
	/// <returns>
	/// An enumerable collection of strings representing the values of all stored string constants. The collection will be
	/// empty if no string constants are present.
	/// </returns>
	public IEnumerable<string> GetStringValues()
	{
		List<string> values = [];
		foreach (StringConstant str in _strings)
		{
			values.Add(str.Value);
		}
		return values;
	}

	#endregion

	#region Organization Methods

	/// <summary>
	/// Sort the bibliography entries.
	/// </summary>
	/// <param name="sortBy">Method to sort the bibliography entries by.</param>
	/// <exception cref="ArgumentException">The SortBy method is not valid.</exception>
	public void SortBibEntries(SortBy sortBy)
	{
		// The copy constructor doesn't work, it points to the _bibEntry list and when that list is cleared, both are cleared (and the enumerators).
		BindingList<BibEntry> copy = [];
		foreach (BibEntry entry in _bibEntries)
		{
			copy.Add(entry);
		}

		IOrderedEnumerable<BibEntry> sorted = sortBy switch
		{
			SortBy.FirstAuthorLastName	=> copy.OrderBy(entry => entry.GetFirstAuthorsName(NameFormat.Last, StringCase.LowerCase)),
			SortBy.Key					=> copy.OrderBy(entry => entry.Key),
			_							=> throw new ArgumentException("The specified method of sorting is not valid.")
		};

		_bibEntries.Clear();
		foreach (BibEntry entry in sorted)
		{
			_bibEntries.Add(entry);
		}
	}

	/// <summary>
	/// Sort the string entries.
	/// </summary>
	/// <param name="sortBy">Method to sort the entries by.</param>
	/// <exception cref="ArgumentException">The SortBy method is not valid.</exception>
	public void SortStringConstants(SortStringsBy sortBy)
	{
		// The copy constructor doesn't work, it points to the _bibEntry list and when that list is cleared, both are cleared (and the enumerators).
		BindingList<StringConstant> copy = [];
		foreach (StringConstant entry in _strings)
		{
			copy.Add(entry);
		}

		IOrderedEnumerable<StringConstant> sorted = sortBy switch
		{
			SortStringsBy.Name	=> copy.OrderBy(entry => entry.Name),
			SortStringsBy.Value	=> copy.OrderBy(entry => entry.Value),
			_					=> throw new ArgumentException("The specified method of sorting is not valid.")
		};

		_strings.Clear();
		foreach (StringConstant entry in sorted)
		{
			_strings.Add(entry);
		}
	}

	#endregion

	#region Search Methods

	/// <summary>
	/// Searches the values of the specified tags for the search string.
	/// </summary>
	/// <param name="tags">Bibliography tags to search.</param>
	/// <param name="searchKey">Whether to search the cite key.</param>
	/// <param name="searchString">The string to search for.</param>
	/// <param name="caseSensitive">Whether the search should be case-sensitive.</param>
	/// <returns>A list of matching bibliography entries.</returns>
	public List<BibEntry> SearchBibEntries(IEnumerable<string> tags, bool searchKey, string searchString, bool caseSensitive = false)
	{
		List<BibEntry> matches		= [];
		StringComparison comparison	= caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

		foreach (BibEntry entry in _bibEntries)
		{
			if (searchKey && entry.Key.Contains(searchString, comparison))
			{
				matches.Add(entry);
				continue;
			}

			if (entry.DoTagsContainString(tags, searchString, caseSensitive))
			{
				matches.Add(entry);
			}
		}
		return matches;
	}

	/// <summary>
	/// Searches the values of the specified tags for the search string.
	/// </summary>
	/// <param name="searchName">Whether to search the name of the string constant.</param>
	/// <param name="searchString">The string to search for.</param>
	/// <param name="caseSensitive">Whether the search should be case-sensitive.</param>
	/// <returns>A list of matching string constants.</returns>
	public List<StringConstant> SearchStringConstants(bool searchName, string searchString, bool caseSensitive = false)
	{
		List<StringConstant> matches	= [];
		StringComparison comparison		= caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

		foreach (StringConstant entry in _strings)
		{
			if (searchName && entry.Name.Contains(searchString, comparison))
			{
				matches.Add(entry);
				continue;
			}

			if (entry.Value.Contains(searchString, comparison))
			{
				matches.Add(entry);
			}
		}

		return matches;
	}

	/// <summary>
	/// Finds the index at which the specified entry should be inserted based on the sorting method.
	/// </summary>
	/// <param name="entry">The entry to find the insert index for.</param>
	/// <param name="sortBy">The sorting method to use.</param>
	/// <returns>The index at which the entry should be inserted.</returns>
	/// <exception cref="ArgumentException"></exception>
	public int FindInsertIndex(BibEntry entry, SortBy sortBy)
	{
		return sortBy switch
		{
			SortBy.FirstAuthorLastName	=> BinarySearch(_bibEntries, entry, new CompareByFirstAuthorLastName(), false),
			SortBy.Key					=> BinarySearch(_bibEntries, entry, new CompareByCiteKey(), false),
			_							=> throw new ArgumentException("The specified method of sorting is not valid."),
		};
	}

	/// <summary>
	/// Finds the index at which the specified entry should be inserted based on the sorting method.
	/// </summary>
	/// <param name="entry">The entry to find the insert index for.</param>
	/// <param name="sortBy">The sorting method to use.</param>
	/// <returns>The index at which the entry should be inserted.</returns>
	/// <exception cref="ArgumentException"></exception>
	public int FindInsertIndex(StringConstant entry, SortStringsBy sortBy)
	{
		return sortBy switch
		{
			SortStringsBy.Name	=> BinarySearch(_strings, entry, new CompareByStringName(), false),
			SortStringsBy.Value	=> BinarySearch(_strings, entry, new CompareByStringValue(), false),
			_					=> throw new ArgumentException("The specified method of sorting is not valid."),
		};
	}

	/// <summary>
	/// A binarySearch.
	/// </summary>
	/// <typeparam name="T">Template type.</typeparam>
	/// <param name="list">The list of items to search.</param>
	/// <param name="item">The item to search for.</param>
	/// <param name="comparer">Comparison function.</param>
	/// <param name="itemMustExist">If the item must exist, an error is thrown if the item is not found.  Otherwise, the position where the item would be found is returned.</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private static int BinarySearch<T>(ObservableCollection<T> list, T item, IComparer<T> comparer, bool itemMustExist = true)
	{
		int min = 0;
		int max = list.Count - 1;

		while (min <= max)
		{
			int mid     = (min + max) / 2;
			int result  = comparer.Compare(item, list[mid]);

			if (result == 0)
			{
				return ++mid;
			}
			else if (result < 0)
			{
				max = mid - 1;
			}
			else
			{
				min = mid + 1;
			}
		}

		if (itemMustExist)
		{
			throw new Exception("The item");
		}

		return min;
	}

	#endregion

} // End class.