using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace CryptoNote.Security
{
    /// <summary>
    /// Json serialization manager
    /// </summary>
    class JsonManager
    {
        private static readonly string path_categories = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "categories");
        private static readonly string path_settings = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "settings");

        /// <summary>
        /// Check for existing settings file
        /// </summary>
        /// <returns>True/False = Exists or not</returns>
        public static bool SettingsFileExists() => File.Exists(path_settings);

        /// <summary>
        /// Load categories file or create a new one with default categories
        /// </summary>
        /// <returns>Categories dictionary for various languages</returns>
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

        /// <summary>
        /// Serialize categories file into json
        /// </summary>
        /// <param name="categories">Categories dictionary</param>
        public static void SaveCategories(Dictionary<string, List<string>> categories)
        {
            using (FileStream file = new FileStream(path_categories, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (StreamWriter writer = new StreamWriter(file))
                writer.WriteLine(JsonConvert.SerializeObject(categories));
        }

        /// <summary>
        /// Serialize settings anonymous type into json
        /// </summary>
        /// <param name="columnCount">Column count settings</param>
        /// <param name="language">Language settings</param>
        public static void SaveSettings(int columnCount, string language)
        {
            using (FileStream file = new FileStream(path_settings, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (StreamWriter writer = new StreamWriter(file))
                writer.WriteLine(JsonConvert.SerializeObject(new { ColumnCount = columnCount, Language = language }));
        }

        /// <summary>
        /// Deserialize settings anonymous type from json
        /// </summary>
        /// <returns>Settings tuple</returns>
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