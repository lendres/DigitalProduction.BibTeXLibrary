namespace BibTeXLibrary;

/// <summary>
/// 
/// </summary>
public class CompareByStringName : IComparer<StringConstant>
{
	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public CompareByStringName()
	{
	}

	#endregion

	#region Methods

	/// <summary>
	/// Compare two BibEntrys.
	/// </summary>
	/// <param name="entry1">The first StringConstant.</param>
	/// <param name="entry2">The second StringConstant.</param>
	public int Compare(StringConstant? entry1, StringConstant? entry2)
	{
		if (entry1 == null && entry2 == null) return 0;
		if (entry1 == null) return -1;
		if (entry2 == null) return 1;
		return entry1.Name.CompareTo(entry2.Name);
	}

	#endregion

} // End class.