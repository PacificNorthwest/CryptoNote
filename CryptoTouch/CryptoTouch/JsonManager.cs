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
        private static string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "categories");

        public static Dictionary<string, List<string>> LoadCategories()
        {
            Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>();
            if (File.Exists(path))
            {
                using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
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
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (StreamWriter writer = new StreamWriter(file))
                writer.WriteLine(JsonConvert.SerializeObject(categories));
        }
    }
}