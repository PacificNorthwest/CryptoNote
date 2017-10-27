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
        public static string Language { get; set; }

        public static void Save()
        {
            JsonManager.SaveSettings(ColumnsCount, Language);
        }

        public static void Load(Activity environment)
        {
            if (JsonManager.SettingsFileExists())
                (ColumnsCount, Language) = JsonManager.LoadSettings();
            else
            {
                Language = environment.Resources.Configuration.Locale.Language;
                Save();
            }
        }
    }
}