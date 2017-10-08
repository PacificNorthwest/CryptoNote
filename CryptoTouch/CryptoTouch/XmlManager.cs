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
using System.IO;
using System.Xml.Serialization;

namespace CryptoTouch
{
    class XmlManager
    {
        private static string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "categories");

        public static (List<string>, List<string>) LoadCategories()
        {
            List<string> categories_en;
            List<string> categories_ru;

            if (File.Exists(path + "_en.xml") && File.Exists(path + "_ru.xml"))
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<string>));
                using (FileStream file = new FileStream(path + "_en.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
                    categories_en = xml.Deserialize(file) as List<string>;
                using (FileStream file = new FileStream(path + "_ru.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
                    categories_ru = xml.Deserialize(file) as List<string>;
            }
            else
            {
                categories_en = new List<string> { "No category", "Work", "Family", "Friends", "Passwords", "Events" };
                categories_ru = new List<string> { "Без категории", "Работа", "Семья", "Друзья", "Пароли", "События" };
                SaveCategories(new string[] { "_en.xml", "_ru.xml" }, categories_en, categories_ru);
            }

            return (categories_en, categories_ru);
        }

        public static void SaveCategories(string[] postfixes, params List<string>[] lists)
        {
            for (int i = 0; i < lists.Length; i++)
            {
                XmlSerializer xml = new XmlSerializer(lists[i].GetType());
                using (FileStream file = new FileStream(path+postfixes[i], FileMode.Create, FileAccess.Write, FileShare.Write))
                    xml.Serialize(file, lists[i]);
            }
        }
    }
}