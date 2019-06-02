# Introduction
Lexical.Localization is a localization class library for .NET.

**Deployment features**
* Dependency Injection
* File providers
* Control over how assets are organized

**Interoperability**
* Microsoft.Extensions.Localization.Abstractions
* Microsoft.Extensions.DependencyInjection.Abstractions
* ResourceManager

**File types**
 * Language Strings
 * Localization of binary assets for gfx and audio

**File formats**
 * .ini
 * .json
 * .xml
 * .resx
 * .resources

**Other features**
* Best practice recommendations
* Dynamic and non-dynamic use
* Inlined language strings
* Automatic inlined string scanning
* Build tool, T4 script, command line tool
* Formatting strings
* Cache
* Singletons
* File format conversion tools

**Very Short Example**
```C#
ILine key = LineRoot.Global
    .Logger(Console.Out, LineStatusSeverity.Ok)
    .Key("hello")
    .Format("Hello, {0}.")
    .Inline("Culture:fi", "Hei, {0}")
    .Inline("Culture:de", "Hallo, {0}");

Console.WriteLine(key.Value("mr. anonymous"));
```

**Links**
* [Website](http://lexical.fi/Localization/index.html)
* [Github](https://github.com/tagcode/Lexical.Localization)
* [Nuget](https://www.nuget.org/packages/Lexical.Localization/)
