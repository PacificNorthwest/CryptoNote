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
        private object _noteToDelete;
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
            //
            Toast.MakeText(this, "Main menu loaded", ToastLength.Long).Show();
            SecurityProvider.LoadNotes();
            //
            
            InitializeUI();
            Animate();
        }

        private void Animate()
        {
            TransitionSet set = new TransitionSet();
            set.AddTransition(new Fade());
            set.AddTransition(new Explode());
            Window.ExitTransition = set;
        }

        private void InitializeUI()
        {
            _newNoteButton = FindViewById<Button>(Resource.Id.newNoteButton);
            _deleteNoteButton = FindViewById<Button>(Resource.Id.deleteNoteButton);
            _sceneRoot = FindViewById<RelativeLayout>(Resource.Id.layout);
            _notesGrid = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _deleteNoteButton.Click += (object sender, EventArgs e) => DeleteNote(_noteToDelete);
            _newNoteButton.Click += (object sender, EventArgs e) => {
                Intent intent = new Intent(this, typeof(NoteActivity));
                StartActivity(intent);
            };

            _notesGrid.HasFixedSize = true;
            _notesGrid.SetLayoutManager(new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical));
            _notesGrid.SetAdapter(new NotesListAdapter(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" }));
            
        }

        private void NoteLongClick(object sender, EventArgs e)
        {
            _noteToDelete = sender;
            ShowDelteButton();
        }

        private void ShowDelteButton()
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

        private void DeleteNoteButton_Click(object sender, EventArgs e)
        {
            DeleteNote(_noteToDelete);
        }

        private void DeleteNote(object sender)
        {
            //NoteStorage.Notes.Remove((sender as Button).Text);
            //XmlManager.Save(NoteStorage.Notes);
            HideDeleteButton();
            InitializeUI();
        }

        private void EditNote(object sender, EventArgs e)
        {
            (sender as Button).TransitionName = "Note";
            ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this, sender as View, "Note");
            Intent intent = new Intent(this, typeof(NoteActivity));
            intent.PutExtra("NoteToEdit", (sender as Button).Text);
            StartActivity(intent, options.ToBundle());
        }
    }
}