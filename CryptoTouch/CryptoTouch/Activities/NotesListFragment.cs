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
using Android.Support.V7.Widget;
using Android.Transitions;
using Android.Graphics.Drawables;
using Android.Views.Animations;
using Android.Animation;
using Android.Graphics;
using System.Timers;
using System.Threading;

namespace CryptoTouch.Activities
{
    public class NotesListFragment : Android.Support.V4.App.Fragment
    {
        public List<View> SelectedItems { get; set; } = new List<View>();
        private static Activity _rootActivity;
        private static RecyclerView _notesGrid;
        private static RelativeLayout _nullStateTile;
        private static Android.Support.V4.App.Fragment _instance;
        private Button _newNoteButton;
        private Button _deleteNoteButton;
        private RelativeLayout _sceneRoot;

        public bool ContainsSelectedItems => (SelectedItems.Count != 0) ? true : false;

        public NotesListFragment(Activity activity) { _rootActivity = activity; _instance = this; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.NotesList, container, false);

            InitializeUI(view);
            PopulateGrid();

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
            PopulateGrid();
        }

        private void InitializeUI(View view)
        {
            _newNoteButton = view.FindViewById<Button>(Resource.Id.newNoteButton);
            _deleteNoteButton = view.FindViewById<Button>(Resource.Id.deleteNoteButton);
            _sceneRoot = view.FindViewById<RelativeLayout>(Resource.Id.layout);
            _nullStateTile = view.FindViewById<RelativeLayout>(Resource.Id.nullStateTile);
            _notesGrid = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            _notesGrid.AddItemDecoration(new RecyclerViewItemSpacing(15));
            _newNoteButton.Click += (object sender, EventArgs e) => StartActivity(new Intent(_rootActivity, typeof(NoteActivity)));
            _deleteNoteButton.Click += (object sender, EventArgs e) => DeleteNotes();        
        }

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

        public static void ChangeDataSet(List<Note> notes)
        {
            _notesGrid.SwapAdapter(new NotesListAdapter(_rootActivity, _instance, notes), false);

            if (notes.Count == 0)
                _nullStateTile.Visibility = ViewStates.Visible;
            else
                _nullStateTile.Visibility = ViewStates.Gone;
        }

        public void SelectItem(View view)
        {
            if (!SelectedItems.Contains(view))
            {
                SelectedItems.Add(view);
                (view.FindViewById<FrameLayout>(Resource.Id.selectionMask).LayoutParameters as RelativeLayout.LayoutParams).Height =
                                                                    view.FindViewById<LinearLayout>(Resource.Id.noteItem).MeasuredHeight;
                view.FindViewById<FrameLayout>(Resource.Id.selectionMask).SetBackgroundResource(Resource.Drawable.CardSelectionBG);
                if (SelectedItems.Count == 1)
                    ShowDeleteButton();
            }
            else
            {
                SelectedItems.Remove(view);
                view.FindViewById<FrameLayout>(Resource.Id.selectionMask).SetBackgroundColor(Color.Transparent);
                if (SelectedItems.Count == 0)
                    HideDeleteButton();
            }
        }

        private void ShowDeleteButton()
        {
            Animation animNewNoteButton = new RotateAnimation(0, 45, Dimension.RelativeToSelf, .5f, Dimension.RelativeToSelf, .5f) { Duration = 500 };
            animNewNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => _deleteNoteButton.BringToFront();
            _newNoteButton.StartAnimation(animNewNoteButton);
        }
        
        private void HideDeleteButton()
        {
            Animation animDeleteNoteButton = new RotateAnimation(0, -45, Dimension.RelativeToSelf, .5F, Dimension.RelativeToSelf, .5F) { Duration = 500 };
            animDeleteNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => _newNoteButton.BringToFront();
            _deleteNoteButton.StartAnimation(animDeleteNoteButton);
        }

        private void DeleteNotes()
        {
            foreach (View view in SelectedItems)
                NoteStorage.Notes.Remove(NoteStorage.Notes.Find(note => note.GetHashCode() == (int)view.Tag));
            int initialCount = SelectedItems.Count;
            for (int i = 0; i < initialCount; i++)
                SelectItem(SelectedItems[0]);
            SecurityProvider.SaveNotesAsync();
            PopulateGrid();
            (MainPageActivity.Navigation.Adapter as ViewPagerAdapter).UpdateCategoriesFragment();
        }

        public void HandleOnBackPressed(Action baseHandler)
        {
            if (SelectedItems.Count != 0)
            {
                int initialCount = SelectedItems.Count;
                for (int i = 0; i < initialCount; i++)
                    SelectItem(SelectedItems[0]);
            }
            else
                baseHandler.Invoke();
        }

    }
}