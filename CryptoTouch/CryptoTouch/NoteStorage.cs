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
    class NoteStorage
    {
        public static List<Note> Notes { get; set; } = new List<Note>();
        private static List<String> _categories_en;
        private static List<String> _categories_ru;

        static NoteStorage() { (_categories_en, _categories_ru) = XmlManager.LoadCategories(); }

        public static List<String> GetCurrentCategories()
        {
            //for now
            return _categories_ru;
        }

        public static void RemoveCategory(string category)
        {
            int id = GetCurrentCategories().IndexOf(category);
            _categories_en.RemoveAt(id);
            _categories_ru.RemoveAt(id);
            SaveCategories();
        }

        public static void AddCategory(string category)
        {
            _categories_en.Add(category);
            _categories_ru.Add(category);
            SaveCategories();
        }

        public static void SaveCategories()
        {
            XmlManager.SaveCategories(new string[] { "_en.xml", "_ru.xml" }, _categories_en, _categories_ru);
        }
    }
}