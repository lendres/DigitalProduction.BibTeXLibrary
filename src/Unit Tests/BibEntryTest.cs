using BibTeXLibrary;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Buffers.Text;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Net;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Xml.Linq;
using System;

namespace DigitalProduction.UnitTests;

public class BibEntryTest
{
    [Fact]
    public void TestIndexer()
    {
        const string title = "Mapreduce";
        BibEntry entry = new() {["Title"] = title};

        Assert.Equal(title, entry["title"]);
        Assert.Equal(title, entry["Title"]);
        Assert.Equal(title, entry["TitlE"]);
    }

    [Fact]
    public void TestProperty()
    {
        const string title = "Mapreduce";
        BibEntry entry = new() {["Title"] = title};

        Assert.Equal(title, entry.Title);
    }

    [Fact]
    public void TestSetType()
    {
        BibEntry entry = new() {Type = "inbook"};
        Assert.Equal("inbook", entry.Type);

        entry.Type = "inBoOK";
        Assert.Equal("inBoOK", entry.Type);
    }


    [Fact]
    public void TestSettingTagType()
    {
        const string title = "Mapreduce";
		BibEntry entry = new();
		entry.SetTagValue("Title", title, TagValueType.StringConstant);
		Assert.Equal(title, entry.Title);
		Assert.Equal(title, entry.GetTagValue("title").ToString());

		entry.SetTagValue("Title", title, TagValueType.String);
        Assert.Equal(title, entry.Title);
        Assert.Equal("{"+title+"}", entry.GetTagValue("title").ToString());
    }

    [Fact]
    public void TestToString()
    {
		//TODO:
    }

    [Fact]
    public void TestFindTagValue()
	{
		string tagValue = "SPE Drilling Conference and Exhibition";

		string bibString	= "@book{ref:key, booktitle = \"" + tagValue + "\", author = {Author}, year = {2023}}";
		string key			= ParseAndGetKey(tagValue, bibString);
		Assert.Equal("booktitle", key);

		bibString	= "@book{ref:key, booktitle = {" + tagValue + "}, author = {Author}, year = {2023}}";
		key			= ParseAndGetKey(tagValue, bibString);
		Assert.Equal("booktitle", key);
	}

	private static string ParseAndGetKey(string tagValue, string bibString)
	{
		BibEntry entry = ParseBibEntry(bibString);
		return entry.FindTagValue(tagValue);
	}

	private static BibEntry ParseBibEntry(string bibString)
	{
		BibParser parser = new(new StringReader(bibString));
		return parser.Parse().Entries[0];
	}
} // End class.