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

namespace CryptoTouch.Activities
{
    [Activity(Label = "MainPageActivity")]
    public class MainPageActivity : Activity
    {
        private object _noteToDelete;
        private Button _newNoteButton;
        private Button _deleteNoteButton;
        private RelativeLayout _sceneRoot;
        private LinearLayout _leftColumn;
        private LinearLayout _rightColumn;

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

            _newNoteButton = FindViewById<Button>(Resource.Id.newNoteButton);
            _deleteNoteButton = FindViewById<Button>(Resource.Id.deleteNoteButton);
            _sceneRoot = FindViewById<RelativeLayout>(Resource.Id.layout);
            _leftColumn = FindViewById<LinearLayout>(Resource.Id.leftColumn);
            _rightColumn = FindViewById<LinearLayout>(Resource.Id.rightColumn);
            //_newNoteButton.BringToFront();
            _newNoteButton.Click += NewNoteButton_Click;
            _deleteNoteButton.Click += DeleteNoteButton_Click;
            
            //NoteStorage.Notes = XmlManager.Load(NoteStorage.Notes.GetType()) as List<string>;
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
            //_leftColumn.RemoveAllViews();
            //_rightColumn.RemoveAllViews();
            //bool isLeftColumn = true;
            //foreach(string note in NoteStorage.Notes)
            //{
            //    Button button = new Button(this);
            //    button.Text = note;
            //    LinearLayout.LayoutParams linearLayoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
            //                                                                                 LinearLayout.LayoutParams.MatchParent);
            //    linearLayoutParams.Weight = new Random().Next(2, 4);
            //    button.LayoutParameters = linearLayoutParams;
            //    button.SetBackgroundResource(Resource.Drawable.rcPannel);
            //    button.SetTextColor(Color.Black);
            //    button.Click += EditNote;
            //    button.LongClick += NoteLongClick;
            //    if (isLeftColumn)
            //        _leftColumn.AddView(button);
            //    else
            //        _rightColumn.AddView(button);
            //    isLeftColumn = !isLeftColumn;
                    
            //}
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

        private void HideDelteButton()
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
            XmlManager.Save(NoteStorage.Notes);
            HideDelteButton();
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

        private void NewNoteButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(NoteActivity));
            StartActivity(intent);
        }
    }
}