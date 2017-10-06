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
        private static string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "categories.xml");

        public static List<string> LoadCategories()
        {
            if (File.Exists(path))
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<string>));
                using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    return xml.Deserialize(file) as List<string>;
            }
            else
            {
                List<string> cathegories = new List<string> { "No category", "Work", "Family", "Friends", "Passwords", "Events" };
                SaveCategories(cathegories);
                return cathegories;
            }
        }

        public static void SaveCategories(List<string> cathegories)
        {
            XmlSerializer xml = new XmlSerializer(cathegories.GetType());
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
                xml.Serialize(file, cathegories);
        }
    }
}