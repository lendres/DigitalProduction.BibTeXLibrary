# BibTeXLibrary
A utility library for BibTeX files written in C#.

## Usage at a glance
- string -> BibEntry
```csharp
var parser = new BibParser(
                new StringReader(
                  "@article{keyword, title = {\"0\"{123}456{789}}, year = 2012, address=\"PingLeYuan\"}"));
var entry = parser.GetAllResult()[0];
```

- BibEntry
```csharp
// Get Property
entry.Type;       // string: Article
entry.Title;      // string: 0{123}456{789}
entry["Title"];   // string: 0{123}456{789}
```

- BibEntry -> string
```csharp
entry.ToString();
// @Article{keyword,
//   title = {0{123}456{789}},
//   year = {2012},
//   address = {PingLeYuan},
// }
```

- Local File -> BibEntries
```csharp
var parser = new BibParser(new StreamReader("text.bib", Encoding.Default));
var entries = parser.GetAllResult();

foreach(var entry in entries) { ... }
```
## Ancestry
**[BibTeXLibrary](https://github.com/BERef/BibTeXLibrary)**\
Original library.

**[BibTeX-Library](https://github.com/lendres/BibTeX-Library)**\
A library that heavily modified the original to correct issues and add additions.  The objective of these modifications is to prepare the library for use in an application for maintaining a BibTeX file (see [BibTeX Manager](https://github.com/lendres/BibTeX-Manager))  As a result, it is no longer compatible with the original library.

**[This Library](https://github.com/lendres/DigitalProduction.BibTeXLibrary)**\
A version of the modified library that updates to newer .Net platforms and is used for a Maui-based BibTeX file manager (see [BibTeX Maui Manager](https://github.com/lendres/BibTex-Manager-Maui)).
