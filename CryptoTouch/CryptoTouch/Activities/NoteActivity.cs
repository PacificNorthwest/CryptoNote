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
        private ImageButton _saveButton;
        private Spinner _categoriesSpinner;
        private Note _originalNote;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.ActivityTransitions);
            Window.RequestFeature(WindowFeatures.ContentTransitions);
            base.OnCreate(savedInstanceState);
            _originalNote = NoteStorage.Notes.Find(note => note.GetHashCode() == Intent.GetIntExtra("NoteHash", 0));
            SetContentView(Resource.Layout.NotePage);
        }

        protected override void OnStart()
        {
            base.OnStart();
            _categoriesSpinner = FindViewById<Spinner>(Resource.Id.cathegoriesSpinner);
            _categoriesSpinner.Adapter = new SpinnerAdapter(this, Resource.Layout.SpinnerItem, NoteStorage.GetCurrentCategories(this));
            InitializeUI();
        }

        private void InitializeUI()
        {
            _noteText = FindViewById<EditText>(Resource.Id.noteText);
            _saveButton = FindViewById<ImageButton>(Resource.Id.saveNoteButton);
            _saveButton.Click += (object sender, EventArgs e) => UpdateNotes();
            
            if (_originalNote != null)
            {
                _noteText.Text = _originalNote.Text;
                _categoriesSpinner.SetSelection(_originalNote.CategoryId);
            }
        }

        private void UpdateNotes()
        {
            if (_originalNote == null)
                NoteStorage.Notes.Add(new Note(_noteText.Text) { CategoryId = (int)_categoriesSpinner.SelectedItemId });
            else
            {
                NoteStorage.Notes.Find(note => note == _originalNote).Text = _noteText.Text;
                NoteStorage.Notes.Find(note => note == _originalNote).CategoryId = (int)_categoriesSpinner.SelectedItemId;
            }
            SecurityProvider.SaveNotesAsync();

            ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this);
            Intent intent = new Intent(this, typeof(MainPageActivity));
            StartActivity(intent, options.ToBundle());
        }
    }
}