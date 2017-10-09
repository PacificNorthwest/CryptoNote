using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CryptoTouch
{
    class Settings
    {
        public static int ColumnsCount { get; set; } = 2;
        public static string Language { get; set; } = "RU";

        public static List<string> LanguageOptions { get; set; } = new List<string>() { "RU", "EN" };
    }
}