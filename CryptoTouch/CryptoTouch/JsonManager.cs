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
using Newtonsoft.Json;
using System.IO;

namespace CryptoTouch
{
    class JsonManager
    {
        private static string path_categories = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "categories");
        private static string path_settings = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "settings");

        public static bool SettingsFileExists() => File.Exists(path_settings);

        public static Dictionary<string, List<string>> LoadCategories()
        {
            Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>();
            if (File.Exists(path_categories))
            {
                using (FileStream file = new FileStream(path_categories, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader reader = new StreamReader(file))
                    categories = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(reader.ReadToEnd());
            }
            else
            {
                categories.Add("EN", new List<string> { "No category", "Work", "Family", "Friends", "Passwords", "Events" });
                categories.Add("RU", new List<string> { "Без категории", "Работа", "Семья", "Друзья", "Пароли", "События" });
                SaveCategories(categories);
            }

            return categories;
        }

        public static void SaveCategories(Dictionary<string, List<string>> categories)
        {
            using (FileStream file = new FileStream(path_categories, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (StreamWriter writer = new StreamWriter(file))
                writer.WriteLine(JsonConvert.SerializeObject(categories));
        }

        public static void SaveSettings(int columnCount, string language)
        {
            using (FileStream file = new FileStream(path_settings, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (StreamWriter writer = new StreamWriter(file))
                writer.WriteLine(JsonConvert.SerializeObject(new { ColumnCount = columnCount, Language = language }));
        }

        public static (int columnCount, string language) LoadSettings()
        {
            var pattern = new { ColumnCount = 0, Language = string.Empty };
            using (FileStream file = new FileStream(path_settings, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader reader = new StreamReader(file))
                pattern = JsonConvert.DeserializeAnonymousType(reader.ReadLine(), pattern);

            return (pattern.ColumnCount, pattern.Language);
        }
    }
}