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
using Android.Transitions;
using static Android.Views.ViewTreeObserver;
using Android.Views.InputMethods;

namespace CryptoTouch.Activities
{
    [Activity(Label = "NoteActivity", WindowSoftInputMode = SoftInput.StateVisible|SoftInput.AdjustResize)]
    public class NoteActivity : Activity
    {
        private NoteEditText _noteText;
        private Button _saveButton;
        private Note _originalNote;

        public int SaveButtonMargin
        {
            get
            { return (_saveButton.LayoutParameters as ViewGroup.MarginLayoutParams).BottomMargin; }
            set
            { (_saveButton.LayoutParameters as ViewGroup.MarginLayoutParams).BottomMargin = value; }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.ActivityTransitions);
            Window.RequestFeature(WindowFeatures.ContentTransitions);
            base.OnCreate(savedInstanceState);
            _originalNote = NoteStorage.Notes.Find(note => note.GetHashCode() == Intent.GetIntExtra("NoteHash", 0));
            SetContentView(Resource.Layout.NotePage);

            InitializeUI();
        }

        private void InitializeUI()
        {
            _noteText = FindViewById<NoteEditText>(Resource.Id.noteText);
            _saveButton = FindViewById<Button>(Resource.Id.saveNoteButton);
            _saveButton.Click += (object sender, EventArgs e) => UpdateNotes();
            _noteText.RootActivity = this;
            SaveButtonMargin = 800;
            _noteText.Click += (object sender, EventArgs e) => SaveButtonMargin = 800;

            if (_originalNote != null)
                _noteText.Text = _originalNote.Text;
        }

        private void UpdateNotes()
        {
            if (_originalNote == null)
                NoteStorage.Notes.Add(new Note(_noteText.Text));
            else
                NoteStorage.Notes.Find(note => note == _originalNote).Text = _noteText.Text;
            SecurityProvider.SaveNotes();

            Intent intent = new Intent(this, typeof(MainPageActivity));
            StartActivity(intent);
        }
    }
}