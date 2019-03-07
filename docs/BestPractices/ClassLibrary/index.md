﻿# Class Library

This article describes recommended practice for writing a localized class library that doesn't use dependency injection.

The developer of class library may want to provide its own builtin localizations. 
The recommended practice is to create a class **LibraryAssets** into the class library.
It should use **[AssetSources]** attribute to a signal that this class provides the localizations.

Internal localization files are typically added embedded resources.
[!code-csharp[Snippet](LibraryAssets.cs)]
<details>
  <summary>The example localization file *LibraryLocalization1-de.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../LibraryLocalization1-de.xml)]
</details>
<br/>

There should be another class called **LibraryLocalization** that is used as the *IAssetRoot* for the classes that use localization.
This root is linked to the global static root and shares its assets.
[!code-csharp[Snippet](LibraryLocalization.cs)]
<br/> 

All the other code in the class library can now use the library's localization root.
[!code-csharp[Snippet](MyClass.cs)]
<br/>

When another class library or application uses the class library the localization works out-of-the-box.
# [Snippet](#tab/snippet-2)
[!code-csharp[Snippet](LibraryConsumer1.cs#Snippet)]
# [Full Code](#tab/full-2)
[!code-csharp[Snippet](LibraryConsumer1.cs)]
***

<br/>
The application can supply additional localizations by placing *IAssetSource*s to the global static **LocalizationRoot.Builder**.
# [Snippet](#tab/snippet-3)
[!code-csharp[Snippet](LibraryConsumer2.cs#Snippet)]
# [Full Code](#tab/full-3)
[!code-csharp[Snippet](LibraryConsumer2.cs)]
***

<details>
  <summary>The example localization file *LibraryLocalization1-fi.xml*.  (<u>click here</u>)</summary>
[!code-xml[Snippet](../../LibraryLocalization1-fi.xml)]
</details>
<br/>