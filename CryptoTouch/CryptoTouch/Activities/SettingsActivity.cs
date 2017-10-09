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
using Android.Support.V7.App;
using Android.Graphics;
using Android.Content.Res;

namespace CryptoTouch.Activities
{
    [Activity(Label = "SettingsActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SettingsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SettingsPage);
            InitializeUI();
        }

        private void InitializeUI()
        {
            Typeface font = Typeface.CreateFromAsset(Assets, "fonts/BROADW.ttf");
            FindViewById<TextView>(Resource.Id.settingsPageTitle).Typeface = font;
            Spinner columnSpinner = FindViewById<Spinner>(Resource.Id.columnsSpinner);
            columnSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.SpinnerItem, new List<string>() { "1", "2" });
            columnSpinner.SetSelection(Settings.ColumnsCount - 1);
            columnSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) 
                                       => Settings.ColumnsCount = Convert.ToInt32((e.View as TextView).Text);

            Spinner languageSpinner = FindViewById<Spinner>(Resource.Id.languageSpinner);
            languageSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.SpinnerItem, Settings.LanguageOptions);
            languageSpinner.SetSelection(Settings.LanguageOptions.IndexOf(Settings.Language));
            languageSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e)
                                       =>
                                        {
                                            Settings.Language = (e.View as TextView).Text;
                                            this.Resources.Configuration.SetLocale(new Java.Util.Locale(Settings.Language));
                                            this.Resources.UpdateConfiguration(this.Resources.Configuration, this.Resources.DisplayMetrics);
                                            OnConfigurationChanged(Resources.Configuration);
                                        };
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            FindViewById<TextView>(Resource.Id.settingsPageTitle).Text = Resources.GetString(Resource.String.Settings);
            FindViewById<TextView>(Resource.Id.settingsPageColumnsTextView).Text = Resources.GetString(Resource.String.ColumnsNumber);
            FindViewById<TextView>(Resource.Id.settingsPageLanguageTextView).Text = Resources.GetString(Resource.String.Language);
            base.OnConfigurationChanged(newConfig);
        }

        public override void OnBackPressed()
        {
            StartActivity(new Intent(this, typeof(MainPageActivity)));
        }
    }
}