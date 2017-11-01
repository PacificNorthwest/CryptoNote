using Android.App;
using CryptoNote.Security;

namespace CryptoNote.Model
{
    /// <summary>
    /// Settings storage
    /// </summary>
    class Settings
    {
        public static int ColumnsCount { get; set; } = 2;
        public static string Language { get; set; }

        /// <summary>
        /// Save settings to file
        /// </summary>
        public static void Save()
        { JsonManager.SaveSettings(ColumnsCount, Language); }

        /// <summary>
        /// Load settings from file or create a new one
        /// </summary>
        /// <param name="environment"></param>
        public static void Load(Activity environment)
        {
            if (JsonManager.SettingsFileExists())
                (ColumnsCount, Language) = JsonManager.LoadSettings();
            else
            {
                Language = environment.Resources.GetString(Resource.String.Locale);
                Save();
            }
        }
    }
}