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

namespace CryptoTouch.Activities
{
    [Activity(Label = "MainPageActivity")]
    public class MainPageActivity : Activity
    {
        private List<View> _selectedItems = new List<View>();
        private Button _newNoteButton;
        private Button _deleteNoteButton;
        private RelativeLayout _sceneRoot;
        private RecyclerView _notesGrid;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.ActivityTransitions);
            Window.RequestFeature(WindowFeatures.ContentTransitions);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainPage);
            
            InitializeUI();
            Animate();
            PopulateGrid();
        }

        private void InitializeUI()
        {
            _newNoteButton = FindViewById<Button>(Resource.Id.newNoteButton);
            _deleteNoteButton = FindViewById<Button>(Resource.Id.deleteNoteButton);
            _sceneRoot = FindViewById<RelativeLayout>(Resource.Id.layout);
            _notesGrid = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _deleteNoteButton.Click += (object sender, EventArgs e) => DeleteNotes();
            _newNoteButton.Click += (object sender, EventArgs e) => {
                Intent intent = new Intent(this, typeof(NoteActivity));
                StartActivity(intent);
            };            
        }

        private void PopulateGrid()
        {
            _notesGrid.HasFixedSize = true;
            _notesGrid.SetLayoutManager(new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical));
            _notesGrid.AddItemDecoration(new RecyclerViewItemSpacing(20));
            _notesGrid.SetAdapter(new NotesListAdapter(this, NoteStorage.Notes));
        }

        private void Animate()
        {
            TransitionSet set = new TransitionSet();
            set.AddTransition(new Fade());
            set.AddTransition(new Explode());
            set.ExcludeTarget(_notesGrid, true);
            set.ExcludeTarget(_sceneRoot, true);
            Window.EnterTransition = set;
        }

        public void SelectItem(View view)
        {
            if (!_selectedItems.Contains(view))
            {
                _selectedItems.Add(view);
                view.SetBackgroundResource(Resource.Drawable.CardSelectionBG);
                if (_deleteNoteButton.Visibility == ViewStates.Invisible)
                    ShowDeleteButton();
            }
            else
            {
                _selectedItems.Remove(view);
                view.SetBackgroundResource(Resource.Color.cardview_light_background);
                if (_selectedItems.Count == 0)
                    HideDeleteButton();
            }
        }

        private void ShowDeleteButton()
        {
            _deleteNoteButton.Visibility = ViewStates.Visible;
            TransitionManager.BeginDelayedTransition(_sceneRoot);
            RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(180, 180);
            layoutParams.AddRule(LayoutRules.LeftOf, Resource.Id.newNoteButton);
            layoutParams.AddRule(LayoutRules.AlignParentBottom);
            layoutParams.BottomMargin = 30;
            layoutParams.RightMargin = 30;
            _deleteNoteButton.LayoutParameters = layoutParams;
        }

        private void HideDeleteButton()
        {
            _deleteNoteButton.Visibility = ViewStates.Invisible;
            TransitionManager.BeginDelayedTransition(_sceneRoot);
            RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(180, 180);
            layoutParams.AddRule(LayoutRules.AlignParentRight);
            layoutParams.AddRule(LayoutRules.AlignParentBottom);
            layoutParams.BottomMargin = 30;
            layoutParams.RightMargin = 30;
            _deleteNoteButton.LayoutParameters = layoutParams;
        }

        private void DeleteNotes()
        {
            foreach (View view in _selectedItems)
                NoteStorage.Notes.Remove(NoteStorage.Notes.Find(note => note.GetHashCode() == (int)view.Tag));
            _selectedItems.Clear();
            SecurityProvider.SaveNotes();
            HideDeleteButton();
            PopulateGrid();
        }
    }
}