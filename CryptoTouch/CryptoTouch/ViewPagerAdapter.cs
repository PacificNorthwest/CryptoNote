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
using Android.Support.V4.App;

namespace CryptoTouch
{
    class ViewPagerAdapter : FragmentPagerAdapter
    {
        private Activity _rootActivity;
        private Activities.NotesListFragment _notesFragment;
        private Activities.CategoriesListFragment _cathegoriesFragment;
        
        public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm, Activity activity) : base(fm)
        {
            _rootActivity = activity;
            _notesFragment = new Activities.NotesListFragment(_rootActivity);
            _cathegoriesFragment = new Activities.CategoriesListFragment(_rootActivity);
        }

        public override int Count => 2;

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0: return _notesFragment;
                case 1: return _cathegoriesFragment;
                default: break;
            }

            return null;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            switch (position)
            {
                case 0: return new Java.Lang.String("Notes");
                case 1: return new Java.Lang.String("Cathegories");
                default: break;
            }
            return null;
        }

        public void HandleOnBackPressed(Action baseHandler)
        {
            if (Activities.MainPageActivity.Navigation.CurrentItem == 1)
                _cathegoriesFragment.HandleOnBackPressed();
            else
                baseHandler.Invoke();
        }
    }
}