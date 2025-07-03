using DigitalProduction.ComponentModel;
using DigitalProduction.Strings;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// Bibliography Document Object Model.
/// </summary>
public class BibliographyDOM : NotifyPropertyModifiedChanged
{
	#region Fields

	private readonly List<string>									_header				= [];
	private readonly ObservableCollection<BibEntry>					_bibEntries			= [];
	private readonly ObservableCollection<StringConstantPart>		_strings			= [];

	#endregion

	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public BibliographyDOM()
	{
		_bibEntries.CollectionChanged += OnBibEntriesCollectionChanged;
		_strings.CollectionChanged += OnStringsCollectionChanged;
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
	public ObservableCollection<StringConstantPart> StringConstants { get => _strings; }

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
	public void AddBibPart(BibliographyPart part)
	{
		part.PropertyChanged += OnPartPropertyChanged;
		part.ModifiedChanged += OnPartModifiedChanged;
		if (part.Type.Equals("string", StringComparison.CurrentCultureIgnoreCase))
		{
			_strings.Add((StringConstantPart)part);
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
			SortBy.FirstAuthorLastName => copy.OrderBy(entry => entry.GetFirstAuthorsName(NameFormat.Last, StringCase.LowerCase)),
			SortBy.Key => copy.OrderBy(entry => entry.Key),
			_ => throw new ArgumentException("The specified method of sorting is not valid."),
		};

		_bibEntries.Clear();
		foreach (BibEntry entry in sorted)
		{
			_bibEntries.Add(entry);
		}
	}

	public int FindInsertIndex(BibEntry entry, SortBy sortBy)
	{
		return sortBy switch
		{
			SortBy.FirstAuthorLastName => BinarySearch(_bibEntries, entry, new CompareByFirstAuthorLastName(), false),
			SortBy.Key => BinarySearch(_bibEntries, entry, new CompareByCiteKey(), false),
			_ => throw new ArgumentException("The specified method of sorting is not valid."),
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
	public static int BinarySearch<T>(ObservableCollection<T> list, T item, IComparer<T> comparer, bool itemMustExist = true)
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

	#region Search Methods

	/// <summary>
	/// Searches the values of the specified tags for the search string.
	/// </summary>
	/// <param name="tags">Bibliography tags to search.</param>
	/// <param name="value"></param>
	/// <param name="caseSensitive"></param>
	/// <returns></returns>
	public List<BibEntry> SearchBibEntries(IEnumerable<string> tags, bool searchkey, string value, bool caseSensitive = false)
	{
		List<BibEntry> matches = [];
		foreach (BibEntry entry in _bibEntries)
		{
			if (searchkey && entry.Key.Contains(value, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase))
			{
				matches.Add(entry);
				continue;
			}
			entry.Key.Contains(value);	
			if (entry.DoesTagsContainString(tags, value, caseSensitive))
			{
				matches.Add(entry);
			}
		}
		return matches;
	}

	#endregion

} // End class.