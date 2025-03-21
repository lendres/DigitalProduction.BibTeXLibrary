﻿using DigitalProduction.ComponentModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BibTeXLibrary;

/// <summary>
/// Settings to use when writing a bib file.
/// </summary>
public class WriteSettings : NotifyModifiedChanged
{
	#region Fields

	private WhiteSpace			_whiteSpace			= WhiteSpace.Tab;
	private int					_tabSize			= 4;
	private bool				_alignTagValues		= true;
	private int					_alignAtColumn		= 24;
	private int					_alignAtTabStop		= 5;
	private bool				_removeLastComma	= true;
	private string				_newLine			= Environment.NewLine;
	private char				_tab				= '\t';

	#endregion

	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public WriteSettings()
	{
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	public WriteSettings(WriteSettings writeSettings)
	{
		_whiteSpace			= writeSettings._whiteSpace;
		_tabSize			= writeSettings._tabSize;
		_alignTagValues		= writeSettings._alignTagValues;
		_alignAtColumn		= writeSettings._alignAtColumn;
		_alignAtTabStop		= writeSettings._alignAtTabStop;
		_removeLastComma	= writeSettings._removeLastComma;
		_newLine			= writeSettings._newLine;
		_tab				= writeSettings._tab;
	}

	#endregion

	#region Properties

	/// <summary>
	/// Type of white space character to use.
	/// </summary>
	[XmlAttribute("whitespace")]
	public WhiteSpace WhiteSpace
	{
		get => _whiteSpace;

		set
		{
			if (_whiteSpace != value)
			{
				_whiteSpace = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// The number of spaces per tab to use.
	/// </summary>
	[XmlAttribute("spacespertab")]
	public int TabSize
	{
		get => _tabSize;

		set
		{
			if (_tabSize != value)
			{
				_tabSize = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Specifies if the tag values should be aligned at the equal sign.
	/// </summary>
	[XmlAttribute("alignatequals")]
	public bool AlignTagValues
	{
		get => _alignTagValues;

		set
		{
			if (_alignTagValues != value)
			{
				_alignTagValues = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Specifies the column number to align tag values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("alignatcolumn")]
	public int AlignAtColumn
	{
		get => _alignAtColumn;

		set
		{
			if (_alignAtColumn != value)
			{
				_alignAtColumn = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Specifies the tab stop number to align tag values at when using spaces as the white space.
	/// </summary>
	[XmlAttribute("alignattabstop")]
	public int AlignAtTabStop
	{
		get => _alignAtTabStop;

		set
		{
			if (_alignAtTabStop != value)
			{
				_alignAtTabStop = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Remove the comma after the last tag in a BibEntry.
	/// </summary>
	[XmlAttribute("removelastcomma")]
	public bool RemoveLastComma
	{
		get => _removeLastComma;

		set
		{
			if (_removeLastComma != value)
			{
				_removeLastComma = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// New line character or sequence.
	/// </summary>
	[XmlIgnore()]
	public string NewLine
	{
		get => _newLine;

		set
		{
			if (_newLine != value)
			{
				_newLine = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Tab character or sequence.
	/// </summary>
	[XmlIgnore()]
	public char Tab
	{
		get => _tab;

		set
		{
			if (_tab != value)
			{
				_tab = value;
				Modified = true;
			}
		}
	}

	/// <summary>
	/// Get the same amount of white space a tab would take up as a string of spaces.
	/// </summary>
	private string TabAsSpaces { get => new(' ', TabSize); }
	
	/// <summary>
	/// Get a tab or the same amount of spaces as a tab would use.
	/// </summary>
	[XmlIgnore()]
	public string Indent
	{
		get
		{
			return WhiteSpace switch
			{
				WhiteSpace.Tab => new string(Tab, 1),
				WhiteSpace.Space => TabAsSpaces,
				_ => throw new InvalidEnumArgumentException("Invalid \"WhiteSpace\" value."),
			};
		}
	}

	#endregion

	#region Methods

	/// <summary>
	/// The WriteSettings do not save/serialize themselves.  Therefore, we provide a method for to indicate the object was saved.
	/// </summary>
	public void MarkSaved()
	{
		Modified = false;
	}

	/// <summary>
	/// Get the space between the tag "key" and the tag "value".
	/// 
	/// Examples:
	/// % Use a space between the key and value (no alignment).
	/// title = {Title of Work}
	/// author = {John Q. Author}
	/// year = {2000}
	/// 
	/// % Align the key and value.
	/// title    = {Title of Work}
	/// author   = {John Q. Author}
	/// year     = {2000}
	/// 
	/// % Use a space between the key and value (no alignment).
	/// </summary>
	/// <param name="tagKey">The tag key as a string.</param>
	public string GetInterTagSpacing(string tagKey)
	{
		if (AlignTagValues)
		{
			// To align the values is much more complicated.  First decide if spaces or tabs are going to be inserted.
			switch (WhiteSpace)
			{
				case WhiteSpace.Tab:
				{
					// Subtract the initial line indent and the length of the key from the desired number of tabs.
					int requiredTabs = AlignAtTabStop - 1 - (int)System.Math.Ceiling((double)(tagKey.Length / TabSize));
					if (requiredTabs < 0)
					{
						throw new ArgumentOutOfRangeException(nameof(tagKey), "The key is too long for the space allocated for aligning tag values.");
					}
					//int tabs = (int)System.Math.Ceiling((double)requiredTabs / TabSize);
					return new string(Tab, requiredTabs);
				}
				case WhiteSpace.Space:
				{
					// Subtract the initial line indent and the length of the key from the desired aligning column.
					int requiredSpaces = AlignAtColumn - 1 - tagKey.Length - TabSize;
					if (requiredSpaces < 0)
					{
						throw new ArgumentOutOfRangeException(nameof(tagKey), "The key is too long for the space allocated for aligning tag values.");
					}
					return new string(' ', requiredSpaces);
				}
				default:
				{
					throw new InvalidEnumArgumentException("Invalid \"WhiteSpace\" value.");
				}
			}
		}
		else
		{
			// If we are not aligning values, just return a space.
			return " ";
		}

	}

	#endregion

} // End class.