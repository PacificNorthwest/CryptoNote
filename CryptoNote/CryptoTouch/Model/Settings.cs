using Android.App;
using CryptoNote.Security;

namespace CryptoNote.Model
{
    class Settings
    {
        public static int ColumnsCount { get; set; } = 2;
        public static string Language { get; set; }

        public static void Save()
        {
            JsonManager.SaveSettings(ColumnsCount, Language);
        }

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