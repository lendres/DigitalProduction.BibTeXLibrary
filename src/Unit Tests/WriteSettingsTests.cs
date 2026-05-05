using BibTeXLibrary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DigitalProduction.UnitTests;

public class WriteSettingsTests
{

	[Fact]
	public void TestToStringWithWriteSettingsFormatsMultipleTagValues()
	{
		WriteSettings	settings	= new();
		string			serialized	= SerializeObjectToString(settings);
		WriteSettings?	result		= DeserializeObjectFromString<WriteSettings>(serialized);
		Assert.NotNull(result);
	}

	private class Utf8StringWriter : StringWriter
	{
		public override Encoding Encoding => System.Text.Encoding.UTF8;
	}

	public static string SerializeObjectToString<T>(T obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}

		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		XmlWriterSettings settings = new XmlWriterSettings
		{
			Encoding = Encoding.UTF8,
			Indent = true,
			OmitXmlDeclaration = false
		};
		using Utf8StringWriter utf8StringWriter = new Utf8StringWriter();
		using XmlWriter xmlWriter = XmlWriter.Create(utf8StringWriter, settings);
		xmlSerializer.Serialize(xmlWriter, obj);
		return utf8StringWriter.ToString();
	}

	public static T? DeserializeObjectFromString<T>(string xml)
	{
		if (string.IsNullOrWhiteSpace(xml))
		{
			throw new ArgumentException("Input XML is null or empty.", "xml");
		}

		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		using StringReader textReader = new StringReader(xml);
		return (T?)xmlSerializer.Deserialize(textReader);
	}
}