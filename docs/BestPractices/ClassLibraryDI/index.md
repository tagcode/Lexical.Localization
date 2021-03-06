﻿# Class Library with Dependency Injection
This article describes recommended practice for writing a localized class library that uses Depedency Injection.

## Localization Sources
The class library may want to provide builtin localizations. 
The recommended practice is to create a public class **AssetSources** which implements **IAssetSources** as a signal 
to indicate that this class provides localizations for this class library.

Internal localization files are typically embedded resources.
[!code-csharp[Snippet](AssetSources.cs)]
<details>
  <summary>The example localization file *TutorialLibrary2-de.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../TutorialLibrary2-de.xml)]
</details>

## Classes
Classes in the class library can use IStringLocalizer abstractions
[!code-csharp[Snippet](MyClass.cs)]

... or alternatively Lexical.Localization.Abstractions.
[!code-csharp[Snippet](MyClassB.cs)]

# Application
Application that deploys the localizer must include the internal localizations with 
**<i>IAssetBuilder</i>.AddAssetSources(*Assembly*)** which searches the **IAssetSources** from the library.
# [Snippet](#tab/snippet-1)
[!code-csharp[Snippet](Consumer1.cs#Snippet)]
# [Full Code](#tab/full-1)
[!code-csharp[Snippet](Consumer1.cs)]
***
<br/>

## Supplying Localizations
The application can supply additional localization sources with **<i>IAssetBuilder</i>.AddSource(*IAssetSource*)**
# [Snippet](#tab/snippet-2)
[!code-csharp[Snippet](Consumer2.cs#Snippet)]
# [Full Code](#tab/full-2)
[!code-csharp[Snippet](Consumer2.cs)]
***
<details>
  <summary>The example localization file *TutorialLibrary2-fi.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../TutorialLibrary2-fi.xml)]
</details>
<br/>

When class is initialized with *IServiceProvider*, additional localizations are added to *IServiceCollection* as *IAssetSource*s.
Extension method **AddLexicalLocalization(this <i>IServiceCollection</i>)** adds the default services for localization.
# [Snippet](#tab/snippet-3)
[!code-csharp[Snippet](Consumer3.cs#Snippet)]
# [Full Code](#tab/full-3)
[!code-csharp[Snippet](Consumer3.cs)]
***
