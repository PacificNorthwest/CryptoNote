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
            
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            FindViewById<TextView>(Resource.Id.settingsPageTitle).Text = Resources.GetString(Resource.String.Settings);
            //FindViewById<TextView>(Resource.Id.settingsPageColumnsTextView).Text = Resources.GetString(Resource.String.ColumnsNumber);
            //FindViewById<TextView>(Resource.Id.settingsPageLanguageTextView).Text = Resources.GetString(Resource.String.Language);
            base.OnConfigurationChanged(newConfig);
        }

        public override void OnBackPressed()
        {
            StartActivity(new Intent(this, typeof(MainPageActivity)));
        }
    }
}