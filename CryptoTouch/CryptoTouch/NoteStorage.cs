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

        public static void SaveCategories()
        {
            JsonManager.SaveCategories(_categories);
        }
    }
}