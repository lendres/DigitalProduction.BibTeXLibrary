namespace BibTeXLibrary;

/// <summary>
/// 
/// </summary>
public class CompareByCiteKey : IComparer<BibEntry>
{
	#region Fields

	#endregion

	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public CompareByCiteKey()
	{
	}

	#endregion

	#region Properties

	#endregion

	#region Methods

	/// <summary>
	/// Compare two BibEntrys.
	/// </summary>
	/// <param name="entry1">The first BibEntry.</param>
	/// <param name="entry2">The second BibEntry.</param>
	public int Compare(BibEntry? entry1, BibEntry? entry2)
	{
		if (entry1 == null && entry2 == null) return 0;
		if (entry1 == null) return -1;
		if (entry2 == null) return 1;
		return entry1.Key.CompareTo(entry2.Key);
	}

	#endregion

} // End class.