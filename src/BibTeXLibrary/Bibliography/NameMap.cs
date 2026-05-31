using DigitalProduction.ComponentModel;
using System.Xml.Serialization;

namespace BibTeXLibrary;

public partial class NameMap : NotifyPropertyModifiedChanged
{
	#region Construction

	public NameMap()
	{
	}

	public NameMap(string fromName, string toName)
	{
		From		= fromName;
		To			= toName;
		Modified	= false;
	}

	public NameMap(NameMap other)
	{
		From		= other.From;
		To			= other.To;
		Modified	= false;
	}

	#endregion

	#region Properties

	[XmlAttribute("from")]
	public string From
	{
		get => GetValueOrDefault(string.Empty);
		set => SetValue(value);
	}

	[XmlAttribute("to")]
	public string To
	{
		get => GetValueOrDefault(string.Empty);
		set => SetValue(value);
	}

	#endregion
}