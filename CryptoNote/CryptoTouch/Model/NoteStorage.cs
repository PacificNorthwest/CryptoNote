using System;
using System.Collections.Generic;

using Android.App;
using CryptoNote.Security;

namespace CryptoNote.Model
{
    class NoteStorage
    {
        public static List<Note> Notes { get; set; } = new List<Note>();
        private static Dictionary<string, List<string>> _categories = JsonManager.LoadCategories();

        public static List<String> GetCurrentCategories(Activity activity)
                   => _categories.GetValueOrDefault(activity.Resources.GetString(Resource.String.Locale));

        public static void RemoveCategory(Activity activity, string category)
        {
            int id = GetCurrentCategories(activity).IndexOf(category);
            foreach (var list in _categories)
                list.Value.RemoveAt(id);
            SaveCategories();
        }

        public static void AddCategory(string category)
        {
            foreach (var list in _categories)
                list.Value.Add(category);
            SaveCategories();
        }

        public static void SaveCategories() { JsonManager.SaveCategories(_categories); }
    }
}