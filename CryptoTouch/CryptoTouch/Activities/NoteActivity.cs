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

namespace CryptoTouch.Activities
{
    [Activity(Label = "NoteActivity", WindowSoftInputMode = SoftInput.StateVisible)]
    public class NoteActivity : Activity
    {
        private EditText _editText;
        private Button _saveButton;
        private Note _originalNote;

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
            _editText = FindViewById<EditText>(Resource.Id.noteText);
            _saveButton = FindViewById<Button>(Resource.Id.saveNoteButton);
            _saveButton.Click += (object sender, EventArgs e) => UpdateNotes();
            if (_originalNote != null)
                _editText.Text = _originalNote.Text;
        }

        private void UpdateNotes()
        {
            if (_originalNote == null)
                NoteStorage.Notes.Add(new Note(_editText.Text));
            else
                NoteStorage.Notes.Find(note => note == _originalNote).Text = _editText.Text;
            SecurityProvider.SaveNotes();

            Intent intent = new Intent(this, typeof(MainPageActivity));
            StartActivity(intent);
        }
    }
}