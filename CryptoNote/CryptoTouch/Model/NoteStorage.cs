using System;
using System.Collections.Generic;

using Android.App;
using CryptoNote.Security;

namespace CryptoNote.Model
{
    /// <summary>
    /// Static notes storage 
    /// </summary>
    class NoteStorage
    {
        public static List<Note> Notes { get; set; } = new List<Note>();
        private static Dictionary<string, List<string>> _categories = JsonManager.LoadCategories();

        /// <summary>
        /// Get categories list depending on current language
        /// </summary>
        /// <param name="activity">Root activity</param>
        /// <returns></returns>
        public static List<String> GetCurrentCategories(Activity activity)
                   => _categories.GetValueOrDefault(activity.Resources.GetString(Resource.String.Locale));

        /// <summary>
        /// Delete category from all parallel lists
        /// </summary>
        /// <param name="activity">Root activity</param>
        /// <param name="category">Category to delete</param>
        public static void RemoveCategory(Activity activity, string category)
        {
            int id = GetCurrentCategories(activity).IndexOf(category);
            foreach (var list in _categories)
                list.Value.RemoveAt(id);
            SaveCategories();
        }

        /// <summary>
        /// Add new category to all parallel lists
        /// </summary>
        /// <param name="category"></param>
        public static void AddCategory(string category)
        {
            foreach (var list in _categories)
                list.Value.Add(category);
            SaveCategories();
        }

        /// <summary>
        /// Save categories to file
        /// </summary>
        public static void SaveCategories() { JsonManager.SaveCategories(_categories); }
    }
}