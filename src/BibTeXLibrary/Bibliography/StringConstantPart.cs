﻿using System.Collections;
using System.Text;

namespace BibTeXLibrary;

/// <summary>
/// A bibliography string constant.
/// </summary>
public class StringConstantPart : BibliographyPart
{
	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public StringConstantPart() :
		base(true)
	{
	}

	#endregion

	#region Properties

	/// <summary>
	/// Name of the string constant.
	/// </summary>
	public string StringName
	{
		get
		{
			IDictionaryEnumerator tagEnumerator = _tags.GetEnumerator();
			tagEnumerator.MoveNext();
			return tagEnumerator.Key.ToString()!;
		}
	}

	/// <summary>
	/// Value of the string constant.
	/// </summary>
	public string StringValue
	{
		get
		{
			IDictionaryEnumerator tagEnumerator = _tags.GetEnumerator();
			tagEnumerator.MoveNext();
			return tagEnumerator.Value!.ToString()!;
		}
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
		bibliographyPart.Append('(');

		// Get the first (and only) entry.
		IDictionaryEnumerator tagEnumerator = _tags.GetEnumerator();
		tagEnumerator.MoveNext();

		// Write the name of the string constant.
		bibliographyPart.Append(tagEnumerator.Key.ToString());

		// Add the space between the key and equal sign.
		bibliographyPart.Append(writeSettings.GetInterTagSpacing(tagEnumerator.Key.ToString()!));

		// Add the string constant value.
		bibliographyPart.Append("= ");
		bibliographyPart.Append(tagEnumerator.Value!.ToString());
		bibliographyPart.Append(')');

		bibliographyPart.Append(writeSettings.NewLine);

		return bibliographyPart.ToString();
	}

	#endregion

} // End class.