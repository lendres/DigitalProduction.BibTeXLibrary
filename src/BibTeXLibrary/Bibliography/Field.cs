using DigitalProduction.ComponentModel;
using System.ComponentModel;
using System.Text;

namespace BibTeXLibrary;

public class Field : NotifyPropertyModifiedChanged
{
	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public Field()
	{
		FieldValue = new FieldValue();
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	/// <param name="field">The field content.</param>
	public Field(Field field)
	{
		Name		= field.Name;
		FieldValue	= new FieldValue(field.FieldValue);
	}

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
		set => SetValue(value);
	}

	#endregion

	#region Methods

	/// <summary>
	/// Public interface to mark the bibliography part as saved.  This is used to reset the Modified property after saving.
	/// </summary>
	public void MarkSaved()
	{
		Modified = false;
	}

	#endregion

	#region Public String Writing Methods

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	public override string ToString()
	{
		// Prevent accidently calling a version where the format is not specified. All objects have a public "ToString()"
		// function that it inherits. We need to turn this of so it does not accidently get called.
		throw new NotSupportedException("This method is disabled.");
	}

	/// <summary>
	/// Convert the BibTeX entry to a string.
	/// </summary>
	/// <param name="writeSettings">The settings for writing the bibliography file.</param>
	public string ToString(WriteSettings writeSettings, FieldValueFormat fieldValueFormat, int alignAtTabStop, int alignAtColumn)
	{
		// Write the name of the field.
		StringBuilder fliedString = new(Name);
		
		// Add the space between the name and equal sign.
		fliedString.Append(GetSpacing(Name, writeSettings, alignAtTabStop, alignAtColumn));

		// Add the equal sign and value..
		fliedString.Append("= ");
		fliedString.Append(FieldValue.ToString(fieldValueFormat));

		return fliedString.ToString();
	}

	/// <summary>
	/// Get the space between the field "name" and the field "value".
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
	/// <param name="fieldName">The field key as a string.</param>
	protected string GetSpacing(string fieldName, WriteSettings writeSettings, int alignAtTabStop, int alignAtColumn)
	{
		// If we are not aligning values, just return a space.
		if (!writeSettings.AlignFieldValues)
		{
			return " ";
		}

		// To align the values is much more complicated.  First decide if spaces or tabs are going to be inserted.
		int requiredCharacters	= 0;
		char whiteSpacechar		= ' ';
		int maxLength			= 0;

		switch (writeSettings.WhiteSpace)
		{
			case WhiteSpace.Tab:
			{
				// Subtract the initial line indent and the length of the key from the desired number of tabs.
				requiredCharacters	= alignAtTabStop - 1 - (int)System.Math.Ceiling((double)(fieldName.Length / writeSettings.TabSize));
				whiteSpacechar		= writeSettings.Tab;
				maxLength			= (alignAtTabStop - 1) * writeSettings.TabSize - 1;
				break;
			}
			case WhiteSpace.Space:
			{
				// Subtract the initial line indent and the length of the key from the desired aligning column.
				requiredCharacters	= alignAtColumn - 1 - fieldName.Length - writeSettings.TabSize;
				whiteSpacechar		= ' ';
				maxLength			= alignAtColumn - 1 - writeSettings.TabSize;
				break;
			}
			default:
			{
				throw new InvalidEnumArgumentException("Invalid \"WhiteSpace\" value.");
			}
		}

		if (requiredCharacters < 0)
		{
			throw new BibliographyWriteException(
				$"The field name is too long for the space allocated for aligning field values.\n" +
				$"Field name: {fieldName}\n" +
				$"Field name length: {fieldName.Length}\n" +
				$"Maximum length: {maxLength}");
		}
		return new string(whiteSpacechar, requiredCharacters);
	}

	#endregion
}