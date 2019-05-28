﻿using Lexical.Localization;

namespace TutorialLibrary1
{
    public class MyClass
    {
        static ILine localizer = Localization.Root.Type<MyClass>();

        public string Do()
        {
            return localizer.Key("OK").Value("Operation Successful").ToString();
        }
    }
}
