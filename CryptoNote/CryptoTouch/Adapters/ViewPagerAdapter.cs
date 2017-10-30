using System;
using Android.App;
using Android.Support.V4.App;

namespace CryptoNote.Adapters
{
    class ViewPagerAdapter : FragmentPagerAdapter
    {
        private Activity _rootActivity;
        private Activities.NotesListFragment _notesFragment;
        private Activities.CategoriesListFragment _categoriesFragment;
        
        public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm, Activity activity) : base(fm)
        {
            _rootActivity = activity;
            _notesFragment = new Activities.NotesListFragment(_rootActivity);
            _categoriesFragment = new Activities.CategoriesListFragment(_rootActivity);
        }

        public override int Count => 2;

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0: return _notesFragment;
                case 1: return _categoriesFragment;
                default: break;
            }
            return null;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            switch (position)
            {
                case 0: return new Java.Lang.String(_rootActivity.Resources.GetString(Resource.String.Notes));
                case 1: return new Java.Lang.String(_rootActivity.Resources.GetString(Resource.String.Categories));
                default: break;
            }
            return null;
        }

        public void HandleOnBackPressed(Action baseHandler)
        {
            if (Activities.MainPageActivity.Navigation.CurrentItem == 1)
                _categoriesFragment.HandleOnBackPressed();
            else if (Activities.MainPageActivity.Navigation.CurrentItem == 0)
                _notesFragment.HandleOnBackPressed(baseHandler);
            else
                baseHandler.Invoke();
        }

        public void UpdateCategoriesFragment()
        {
            _categoriesFragment.Update();
        }
    }
}