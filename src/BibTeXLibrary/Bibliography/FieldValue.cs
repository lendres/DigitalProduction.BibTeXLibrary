using System.ComponentModel;

namespace BibTeXLibrary;

/// <summary>
/// The field value for a BibTeX library.  This is an object to allow more complex behavior.  Specifically,
/// it allows different types of writing (ToString) for the value.
/// </summary>
public class FieldValue : IEquatable<FieldValue>, IComparable<FieldValue>
{
	#region Construction

	/// <summary>
	/// Default constructor.
	/// </summary>
	public FieldValue()
	{
	}

	/// <summary>
	/// Content constructor.
	/// </summary>
	/// <param name="content">The field content.</param>
	public FieldValue(string content)
	{
		Content = content;
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	/// <param name="fieldValue">The FieldValue to copy.</param>
	public FieldValue(FieldValue fieldValue)
	{
		Content			= fieldValue.Content;
		FieldValueType	= fieldValue.FieldValueType;
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	/// <param name="content">The field content.</param>
	/// <param name="format">Specifies the format to write in.</param>
	public FieldValue(string content, FieldValueType fieldValueType)
	{
		Content			= content;
		FieldValueType	= fieldValueType;
	}

	#endregion

	#region Properties

	/// <summary>
	/// The content of the field value.
	/// </summary>
	public string Content { get; set; } = string.Empty;

	/// <summary>
	/// The type of the field value, which determines how it is written when ToString is called with a format.  The default
	/// is FieldValueType.String, which means that the content will be written with enclosing brackets/quoates around it. If
	/// the type is FieldValueType.StringConstant, then the content will be written without brackets or quotes when ToString
	/// regardless of the format specified.
	/// </summary>
	public FieldValueType FieldValueType { get; set; } = FieldValueType.String;

	#endregion

	#region Methods

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string? ToString()
	{
		// Prevent accidently calling a version where the format is not specified. All objects have a public "ToString()"
		// function that it inherits. We need to turn this of so it does not accidently get called.
		throw new NotSupportedException("This method is disabled.");
	}

	/// <summary>
	/// Get a string representation.
	/// </summary>
	public string ToString(FieldValueFormat format)
	{
		// For string constants, we ignore the format and just return the content.
		if (FieldValueType == FieldValueType.StringConstant)
		{
			return Content;
		}

		return format switch
		{
			FieldValueFormat.CurlyBraces	=> "{"+Content+"}",
			FieldValueFormat.Quotes		=> "\""+Content+"\"",
			FieldValueFormat.None		=> Content,
			_							=> throw new Exception("Invalid field format."),
		};
	}

	#endregion

	#region Comparison

	/// <summary>
	/// Compare this FieldValue to another FieldValue.
	/// </summary>
	public int CompareTo(FieldValue? other)
	{
		if (other is null)
		{
			return 1;
		}

		int contentComparison = string.Compare(Content, other.Content, StringComparison.Ordinal);

		if (contentComparison != 0)
		{
			return contentComparison;
		}

		return FieldValueType.CompareTo(other.FieldValueType);
	}

	/// <summary>
	/// Determine if this FieldValue is equal to another FieldValue.
	/// </summary>
	public bool Equals(FieldValue? other)
	{
		if (other is null)
		{
			return false;
		}

		return Content == other.Content && FieldValueType == other.FieldValueType;
	}

	/// <summary>
	/// Determine if this FieldValue is equal to another object.
	/// </summary>
	public override bool Equals(object? obj)
	{
		return Equals(obj as FieldValue);
	}

	/// <summary>
	/// Get the hash code.
	/// </summary>
	public override int GetHashCode()
	{
		return HashCode.Combine(Content, FieldValueType);
	}

	public static bool operator ==(FieldValue? left, FieldValue? right)
	{
		return EqualityComparer<FieldValue>.Default.Equals(left, right);
	}

	public static bool operator !=(FieldValue? left, FieldValue? right)
	{
		return !(left == right);
	}

	public static bool operator <(FieldValue? left, FieldValue? right)
	{
		if (left is null)
		{
			return right is not null;
		}

		return left.CompareTo(right) < 0;
	}

	public static bool operator >(FieldValue? left, FieldValue? right)
	{
		if (left is null)
		{
			return false;
		}

		return left.CompareTo(right) > 0;
	}

	public static bool operator <=(FieldValue? left, FieldValue? right)
	{
		return !(left > right);
	}

	public static bool operator >=(FieldValue? left, FieldValue? right)
	{
		return !(left < right);
	}

	#endregion

} // End class.