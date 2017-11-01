using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

using CryptoNote.Model;
using CryptoNote.Adapters;
using CryptoNote.Security;

namespace CryptoNote.Activities
{
    /// <summary>
    /// Note editor page activity
    /// </summary>
    [Activity(Label = "NoteActivity", Theme = "@style/AppTheme", WindowSoftInputMode = SoftInput.StateVisible|SoftInput.AdjustResize)]
    public class NoteActivity : AppCompatActivity
    {
        private EditText _noteText;
        private ImageButton _saveButton;
        private Spinner _categoriesSpinner;
        private Note _originalNote;

        /// <summary>
        /// Activity creation handler. 
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.ActivityTransitions);
            Window.RequestFeature(WindowFeatures.ContentTransitions);
            base.OnCreate(savedInstanceState);
            _originalNote = NoteStorage.Notes.Find(note => note.GetHashCode() == Intent.GetIntExtra("NoteHash", 0));
            SetContentView(Resource.Layout.NotePage);
        }

        /// <summary>
        /// Activity start handler
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            _categoriesSpinner = FindViewById<Spinner>(Resource.Id.cathegoriesSpinner);
            _categoriesSpinner.Adapter = new SpinnerAdapter(this, Resource.Layout.SpinnerItem, NoteStorage.GetCurrentCategories(this));
            InitializeUI();
        }

        /// <summary>
        /// UI views acquisition and initialization
        /// </summary>
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

        /// <summary>
        /// Note creation or update
        /// </summary>
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