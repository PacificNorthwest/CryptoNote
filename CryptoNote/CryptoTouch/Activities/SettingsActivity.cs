using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Support.V7.App;

using CryptoNote.Security;
using CryptoNote.Model;
using Android.Graphics;

namespace CryptoNote.Activities
{
    /// <summary>
    /// Settings page activity
    /// </summary>
    [Activity(Label = "SettingsActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SettingsActivity : AppCompatActivity
    {
        private string _selectedLang = Settings.Language;
        private int _selectedColumnsCount = Settings.ColumnsCount;

        private EditText _oldPassword;
        private EditText _newPassword;
        private EditText _confirmPassword;
        private Button _twoColumnsOptionButton;
        private Button _oneColumnOptionButton;
        private Button _langEnButton;
        private Button _langRuButton;

        /// <summary>
        /// Activity creation handler
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SettingsPage);
            InitializeUI();
        }

        /// <summary>
        /// UI views acquisition and initialization
        /// </summary>
        private void InitializeUI()
        {
            _twoColumnsOptionButton = FindViewById<Button>(Resource.Id.TwoColumnsOptionButton);
            _oneColumnOptionButton = FindViewById<Button>(Resource.Id.OneColumnOptionButton);
            _langEnButton = FindViewById<Button>(Resource.Id.LanguageEnButton);
            _langRuButton = FindViewById<Button>(Resource.Id.LanguageRuButton);
            _oldPassword = FindViewById<EditText>(Resource.Id.settingsOldPasswordEditText);
            _newPassword = FindViewById<EditText>(Resource.Id.settingsNewPasswordEditText);
            _confirmPassword = FindViewById<EditText>(Resource.Id.settingsConfirmPasswordEditText);

            FindViewById<TextView>(Resource.Id.settingsPageTitle).Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");
            FindViewById<TextView>(Resource.Id.settingsPageColumnCountSectionTitle).Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");
            FindViewById<TextView>(Resource.Id.settingsPageLanguageSectionTitle).Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");
            FindViewById<TextView>(Resource.Id.settingsPagePasswordSectionTitle).Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");
            FindViewById<TextView>(Resource.Id.credits_textview).Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");
            FindViewById<Button>(Resource.Id.settingsSave).Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");
            _oldPassword.Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");
            _newPassword.Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");
            _confirmPassword.Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");

            FindViewById<Button>(Resource.Id.settingsSave).Click += (object sender, EventArgs e) => { SaveSettings(); Return(); };

            _twoColumnsOptionButton.Click += (object sender, EventArgs e) 
                                          => { _selectedColumnsCount = 2;
                                               _twoColumnsOptionButton.Alpha = 1.0f;
                                               _oneColumnOptionButton.Alpha = 0.4f; };
            _oneColumnOptionButton.Click += (object sender, EventArgs e) 
                                         => { _selectedColumnsCount = 1;
                                              _oneColumnOptionButton.Alpha = 1.0f;
                                              _twoColumnsOptionButton.Alpha = 0.4f; };
            _langEnButton.Click += (object sender, EventArgs e) 
                                => { _selectedLang = "EN";
                                     _langEnButton.Alpha = 1.0f;
                                     _langRuButton.Alpha = 0.4f; };
            _langRuButton.Click += (object sender, EventArgs e) 
                                => { _selectedLang = "RU";
                                     _langRuButton.Alpha = 1.0f;
                                     _langEnButton.Alpha = 0.4f; };

            switch (Settings.ColumnsCount)
            {
                case 1: _twoColumnsOptionButton.Alpha = 0.4f; break;
                case 2: _oneColumnOptionButton.Alpha = 0.4f; break;
                default: break;
            }

            switch (Settings.Language.ToUpper())
            {
                case "RU": _langEnButton.Alpha = 0.4f; break;
                case "EN": _langRuButton.Alpha = 0.4f; break;
                default: break;
            }
        }

        /// <summary>
        /// Settings saving
        /// </summary>
        private void SaveSettings()
        {
            Settings.ColumnsCount = _selectedColumnsCount;
            Settings.Language = _selectedLang;
            Settings.Save();
            this.Resources.Configuration.SetLocale(new Java.Util.Locale(Settings.Language));
            this.Resources.UpdateConfiguration(this.Resources.Configuration, this.Resources.DisplayMetrics);

            if (_oldPassword.Text != string.Empty && _newPassword.Text != string.Empty && _confirmPassword.Text != string.Empty)
            {
                try
                {
                    SecurityProvider.EnrollNewPassword(_oldPassword.Text, _newPassword.Text, _confirmPassword.Text);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            }
        }

        /// <summary>
        /// Back button handler
        /// </summary>
        public override void OnBackPressed()
        {
            Return();
        }

        /// <summary>
        /// Returning to a main activity
        /// </summary>
        private void Return()
        {
            StartActivity(new Intent(this, typeof(MainPageActivity)));
        }
    }
}