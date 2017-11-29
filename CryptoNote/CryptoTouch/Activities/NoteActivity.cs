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
using Android.Graphics;
using Android.Views.InputMethods;

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
        private Button _categoriesListButton;
        private LinearLayout _categoriesList;
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
            
            InitializeUI();
        }

        /// <summary>
        /// UI views acquisition and initialization
        /// </summary>
        private void InitializeUI()
        {
            _noteText = FindViewById<EditText>(Resource.Id.noteText);
            _saveButton = FindViewById<ImageButton>(Resource.Id.saveNoteButton);
            _categoriesListButton = FindViewById<Button>(Resource.Id.categoriesListButton);
            _categoriesList = FindViewById<LinearLayout>(Resource.Id.categoriesDialogEntriesList);
            _categoriesListButton.Click += (object sender, EventArgs e) => RevealCategoriesDialog();
            FindViewById<RelativeLayout>(Resource.Id.categoriesDialogBG).Click += (object sender, EventArgs e) => (sender as RelativeLayout).Visibility = ViewStates.Invisible;
            FindViewById<LinearLayout>(Resource.Id.categoriesDialogContainer).Click += (object sender, EventArgs e) => { };
            FindViewById<TextView>(Resource.Id.categoriesDialogTitle).Typeface = Typeface.CreateFromAsset(this.Assets, "fonts/MINIONPRO-REGULAR.OTF");
            _categoriesListButton.Typeface = Typeface.CreateFromAsset(this.Assets, "fonts/MINIONPRO-REGULAR.OTF");
            _saveButton.Click += (object sender, EventArgs e) => UpdateNotes();

            if (_originalNote != null)
            {
                _noteText.Text = _originalNote.Text;
                _categoriesListButton.Text = NoteStorage.GetCurrentCategories(this)[_originalNote.CategoryId];
                _categoriesListButton.Tag = _originalNote.CategoryId;
            }
            else
            {
                _categoriesListButton.Text = NoteStorage.GetCurrentCategories(this)[0];
                _categoriesListButton.Tag = 0;
            }
        }

        private void RevealCategoriesDialog()
        {
            var inflater = GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            _categoriesList.RemoveAllViews();
            View view = this.CurrentFocus;
            if (view != null)
                (this.GetSystemService(InputMethodService) as InputMethodManager)
                     .HideSoftInputFromWindow(view.WindowToken, 0);
            foreach (var category in NoteStorage.GetCurrentCategories(this))
            {
                View entry = inflater.Inflate(Resource.Layout.CategoriesDialogItem, null);
                entry.FindViewById<Button>(Resource.Id.categoriesDialogEntryText).Text = category;
                entry.FindViewById<Button>(Resource.Id.categoriesDialogEntryText).Typeface = Typeface.CreateFromAsset(this.Assets, "fonts/MINIONPRO-REGULAR.OTF");
                entry.Click += (object sender, EventArgs e) 
                            => { _categoriesListButton.Text = category;
                                 _categoriesListButton.Tag = NoteStorage.GetCurrentCategories(this).IndexOf(category);
                                 FindViewById<RelativeLayout>(Resource.Id.categoriesDialogBG).Visibility = ViewStates.Invisible; };
                _categoriesList.AddView(entry);
            }
            FindViewById<RelativeLayout>(Resource.Id.categoriesDialogBG).Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// Note creation or update
        /// </summary>
        private void UpdateNotes()
        {
            if (_noteText.Text != string.Empty)
            {
                if (_originalNote == null)
                    NoteStorage.Notes.Add(new Note(_noteText.Text) { CategoryId = (int)_categoriesListButton.Tag });
                else
                {
                    NoteStorage.Notes.Find(note => note == _originalNote).Text = _noteText.Text;
                    NoteStorage.Notes.Find(note => note == _originalNote).CategoryId = (int)_categoriesListButton.Tag;
                }
                SecurityProvider.SaveNotesAsync();

                ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this);
                Intent intent = new Intent(this, typeof(MainPageActivity));
                StartActivity(intent, options.ToBundle());
            }
        }
    }
}