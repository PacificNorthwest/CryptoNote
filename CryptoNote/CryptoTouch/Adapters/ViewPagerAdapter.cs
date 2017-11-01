using System;
using Android.App;
using Android.Support.V4.App;

namespace CryptoNote.Adapters
{
    /// <summary>
    /// ViewPager adapter
    /// </summary>
    class ViewPagerAdapter : FragmentPagerAdapter
    {
        private Activity _rootActivity;
        private Activities.NotesListFragment _notesFragment;
        private Activities.CategoriesListFragment _categoriesFragment;
        
        /// <summary>
        /// ViewPager constructor
        /// </summary>
        /// <param name="fm"></param>
        /// <param name="activity"></param>
        public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm, Activity activity) : base(fm)
        {
            _rootActivity = activity;
            _notesFragment = new Activities.NotesListFragment(_rootActivity);
            _categoriesFragment = new Activities.CategoriesListFragment(_rootActivity);
        }

        /// <summary>
        /// Pages amount
        /// </summary>
        public override int Count => 2;

        /// <summary>
        /// Acquiring precreated page
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Acquiring page name
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Back button handler
        /// </summary>
        /// <param name="baseHandler">Base back button handler</param>
        public void HandleOnBackPressed(Action baseHandler)
        {
            if (Activities.MainPageActivity.Navigation.CurrentItem == 1)
                _categoriesFragment.HandleOnBackPressed();
            else if (Activities.MainPageActivity.Navigation.CurrentItem == 0)
                _notesFragment.HandleOnBackPressed(baseHandler);
            else
                baseHandler.Invoke();
        }

        /// <summary>
        /// Updating categories page in case something changed
        /// </summary>
        public void UpdateCategoriesFragment()
        {
            _categoriesFragment.Update();
        }
    }
}