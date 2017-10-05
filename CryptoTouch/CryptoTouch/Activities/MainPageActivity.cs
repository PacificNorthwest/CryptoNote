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
using Android.Graphics;
using Android.Transitions;
using Android.Support.V7.Widget;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V7.App;

namespace CryptoTouch.Activities
{
    [Activity(Label = "MainPageActivity", Theme = "@style/AppTheme")]
    public class MainPageActivity : AppCompatActivity
    {
        public static ViewPager Navigation { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainPage);
            Navigation = FindViewById<ViewPager>(Resource.Id.viewpager);
            Navigation.Adapter = new ViewPagerAdapter(this.SupportFragmentManager, this);
            FindViewById<Android.Support.Design.Widget.TabLayout>(Resource.Id.tabs).SetupWithViewPager(Navigation);
        }

        public override void OnBackPressed()
        {
            (Navigation.Adapter as ViewPagerAdapter).HandleOnBackPressed(() => base.OnBackPressed());
        }
    }
}