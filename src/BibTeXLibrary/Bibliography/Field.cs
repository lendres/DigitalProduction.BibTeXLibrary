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
		get => TagValue.Content;

		set
		{
			if (TagValue.Content != value)
			{
				TagValue.Content = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(TagValue));
			}
		}
	}

	/// <summary>
	/// Value of the string constant.
	/// </summary>
	public FieldValue TagValue
	{
		get => GetValueOrDefault<FieldValue>(new FieldValue(string.Empty, FieldValueType.String));
		protected set => SetValue(value);
	}

	#endregion
}
