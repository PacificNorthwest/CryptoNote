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
using Android.Views.InputMethods;
using Android.Support.V7;
using Android.Support.V7.App;

namespace CryptoTouch.Activities
{
    [Activity(Label = "NoteActivity", Theme = "@style/AppTheme", WindowSoftInputMode = SoftInput.StateVisible|SoftInput.AdjustResize)]
    public class NoteActivity : AppCompatActivity
    {
        private EditText _noteText;
        private Button _saveButton;
        private Spinner _cathegoriesSpinner;
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
            _noteText = FindViewById<EditText>(Resource.Id.noteText);
            _saveButton = FindViewById<Button>(Resource.Id.saveNoteButton);
            _cathegoriesSpinner = FindViewById<Spinner>(Resource.Id.cathegoriesSpinner);
            _saveButton.Click += (object sender, EventArgs e) => UpdateNotes();
            _cathegoriesSpinner.Adapter = new SpinnerAdapter(this, Resource.Layout.SpinnerItem, NoteStorage.GetCurrentCategories());
            if (_originalNote != null)
            {
                _noteText.Text = _originalNote.Text;
                _cathegoriesSpinner.SetSelection(_originalNote.CategoryId);
            }
        }

        private void UpdateNotes()
        {
            if (_originalNote == null)
                NoteStorage.Notes.Add(new Note(_noteText.Text) { CategoryId = (int)_cathegoriesSpinner.SelectedItemId });
            else
            {
                NoteStorage.Notes.Find(note => note == _originalNote).Text = _noteText.Text;
                NoteStorage.Notes.Find(note => note == _originalNote).CategoryId = (int)_cathegoriesSpinner.SelectedItemId;
            }
            SecurityProvider.SaveNotesAsync();

            ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this);
            Intent intent = new Intent(this, typeof(MainPageActivity));
            StartActivity(intent, options.ToBundle());
        }
    }
}