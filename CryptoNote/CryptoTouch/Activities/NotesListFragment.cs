using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Views.Animations;
using Android.Graphics;

using CryptoNote.Adapters;
using CryptoNote.Model;
using CryptoNote.Security;

namespace CryptoNote.Activities
{
    /// <summary>
    /// ViewPager fragment that manages notes list.
    /// </summary>
    public class NotesListFragment : Android.Support.V4.App.Fragment
    {
        private List<View> _selectedItems = new List<View>();
        private static Activity _rootActivity;
        private static RecyclerView _notesGrid;
        private static RelativeLayout _nullStateTile;
        private static Android.Support.V4.App.Fragment _instance;
        private Button _newNoteButton;
        private Button _deleteNoteButton;
        private RelativeLayout _sceneRoot;

        /// <summary>
        /// Selected items presence flag property
        /// </summary>
        public bool ContainsSelectedItems => (_selectedItems.Count != 0) ? true : false;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="activity">Root activity this fragment works with</param>
        public NotesListFragment(Activity activity) { _rootActivity = activity; _instance = this; }

        /// <summary>
        /// View creation event handler
        /// </summary>
        /// <param name="inflater">Layout inflater</param>
        /// <param name="container">Fragment container</param>
        /// <param name="savedInstanceState"></param>
        /// <returns>Fragment instance</returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.NotesList, container, false);
            InitializeUI(view);
            PopulateGrid(); //Not sure if it's needed here
            return view;
        }

        /// <summary>
        /// Resume event handler
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();
            PopulateGrid();
        }

        /// <summary>
        /// Acquiring all views and initializing them with event-handlers
        /// </summary>
        /// <param name="root">Root layout</param>
        private void InitializeUI(View root)
        {
            _newNoteButton = root.FindViewById<Button>(Resource.Id.newNoteButton);
            _deleteNoteButton = root.FindViewById<Button>(Resource.Id.deleteNoteButton);
            _sceneRoot = root.FindViewById<RelativeLayout>(Resource.Id.layout);
            _nullStateTile = root.FindViewById<RelativeLayout>(Resource.Id.nullStateTile);
            _notesGrid = root.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            _notesGrid.AddItemDecoration(new RecyclerViewItemSpacing(15));
            _newNoteButton.Click += (object sender, EventArgs e) => StartActivity(new Intent(_rootActivity, typeof(NoteActivity)));
            _deleteNoteButton.Click += (object sender, EventArgs e) => DeleteNotes();        
        }

        /// <summary>
        /// Populate staggered grid with notes
        /// </summary>
        private void PopulateGrid()
        { 
            _notesGrid.HasFixedSize = true;
            _notesGrid.SetLayoutManager(new StaggeredGridLayoutManager(Settings.ColumnsCount, StaggeredGridLayoutManager.Vertical));
            _notesGrid.SetAdapter(new NotesListAdapter(_rootActivity, this, NoteStorage.Notes));
            _notesGrid.GetRecycledViewPool().SetMaxRecycledViews(0, 0);
            if (NoteStorage.Notes.Count == 0)
                _nullStateTile.Visibility = ViewStates.Visible;
            else
                _nullStateTile.Visibility = ViewStates.Gone;
        }

        /// <summary>
        /// Change grid data-set
        /// </summary>
        /// <param name="notes"></param>
        public static void ChangeDataSet(List<Note> notes)
        {
            _notesGrid.SwapAdapter(new NotesListAdapter(_rootActivity, _instance, notes), false);

            if (notes.Count == 0)
                _nullStateTile.Visibility = ViewStates.Visible;
            else
                _nullStateTile.Visibility = ViewStates.Gone;
        }

        /// <summary>
        /// Item selection/unselection 
        /// </summary>
        /// <param name="view">Note item to select/unselect</param>
        public void SelectItem(View view)
        {
            if (!_selectedItems.Contains(view))
            {
                _selectedItems.Add(view);
                (view.FindViewById<FrameLayout>(Resource.Id.selectionMask).LayoutParameters as RelativeLayout.LayoutParams).Height =
                                                                    view.FindViewById<LinearLayout>(Resource.Id.noteItem).MeasuredHeight;
                view.FindViewById<FrameLayout>(Resource.Id.selectionMask).SetBackgroundResource(Resource.Drawable.CardSelectionBG);
                if (_selectedItems.Count == 1)
                    ShowDeleteButton();
            }
            else
            {
                _selectedItems.Remove(view);
                view.FindViewById<FrameLayout>(Resource.Id.selectionMask).SetBackgroundColor(Color.Transparent);
                if (_selectedItems.Count == 0)
                    HideDeleteButton();
            }
        }

        /// <summary>
        /// Delete button revelation animation
        /// </summary>
        private void ShowDeleteButton()
        {
            Animation animNewNoteButton = new RotateAnimation(0, 45, Dimension.RelativeToSelf, .5f, Dimension.RelativeToSelf, .5f) { Duration = 500 };
            animNewNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => _deleteNoteButton.BringToFront();
            _newNoteButton.StartAnimation(animNewNoteButton);
        }
        
        /// <summary>
        /// Delete button hiding animation
        /// </summary>
        private void HideDeleteButton()
        {
            Animation animDeleteNoteButton = new RotateAnimation(0, -45, Dimension.RelativeToSelf, .5F, Dimension.RelativeToSelf, .5F) { Duration = 500 };
            animDeleteNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => _newNoteButton.BringToFront();
            _deleteNoteButton.StartAnimation(animDeleteNoteButton);
        }

        /// <summary>
        /// Delete all selected note entrys
        /// </summary>
        private void DeleteNotes()
        {
            foreach (View view in _selectedItems)
                NoteStorage.Notes.Remove(NoteStorage.Notes.Find(note => note.GetHashCode() == (int)view.Tag));
            int initialCount = _selectedItems.Count;
            for (int i = 0; i < initialCount; i++)
                SelectItem(_selectedItems[0]);
            SecurityProvider.SaveNotesAsync();
            PopulateGrid();
            (MainPageActivity.Navigation.Adapter as ViewPagerAdapter).UpdateCategoriesFragment();
        }

        /// <summary>
        /// Back button handling depending on various conditions
        /// </summary>
        /// <param name="baseHandler">Base back button handler</param>
        public void HandleOnBackPressed(Action baseHandler)
        {
            if (_selectedItems.Count != 0)
            {
                int initialCount = _selectedItems.Count;
                for (int i = 0; i < initialCount; i++)
                    SelectItem(_selectedItems[0]);
            }
            else
                baseHandler.Invoke();
        }

    }
}