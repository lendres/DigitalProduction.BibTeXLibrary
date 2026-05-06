using DigitalProduction.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibTeXLibrary;

public class Field : NotifyPropertyModifiedChanged
{
	#region Construction

	#endregion

	#region Properties

	/// <summary>
	/// Name of the string constant.
	/// </summary>
	public string Name
	{
		get => GetValueOrDefault<string>(string.Empty);
		set => SetValue(value);
	}

	/// <summary>
	/// Value of the string constant.
	/// </summary>
	public string Value
	{
		get => FieldValue.Content;

		set
		{
			if (FieldValue.Content != value)
			{
				FieldValue.Content	= value;
				Modified			= true;
				OnPropertyChanged();
			}
		}
	}

	/// <summary>
	/// Value of the string constant.
	/// </summary>
	public FieldValue FieldValue
	{
		get => GetValueOrDefault<FieldValue>(new FieldValue(string.Empty, FieldValueType.String));
		protected set => SetValue(value);
	}

	#endregion

	#region Public String Writing Methods

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	public override string ToString()
	{
		return ToString(new WriteSettings() { WhiteSpace = WhiteSpace.Space, TabSize = 2, AlignTagValues = false });
	}

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	/// <param name="writeSettings">The settings for writing the bibliography file.</param>
	public string ToString(WriteSettings writeSettings)
	{
		// Build the entry opening and key.
		StringBuilder bibliographyPart = new("@");
		bibliographyPart.Append(Type);
		bibliographyPart.Append('(');

		// Write the name of the string constant.
		bibliographyPart.Append(Name);

		// Add the space between the key and equal sign.
		bibliographyPart.Append(GetInterTagSpacing(Name, writeSettings, writeSettings.StringEntryAlignAtTabStop, writeSettings.StringEntryAlignAtColumn));

		// Add the string constant value.
		bibliographyPart.Append("= ");
		bibliographyPart.Append(FieldValue.ToString(writeSettings.StringConstantTagValueFormat));
		bibliographyPart.Append(")");

		bibliographyPart.Append(writeSettings.NewLine);

		return bibliographyPart.ToString();
	}

	#endregion
}
