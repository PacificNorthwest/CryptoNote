using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;
using Android.Support.V4.Content;
using Android.Animation;
using Android.Views.Animations;

using CryptoNote.Model;
using CryptoNote.Security;
using Android.Graphics;

namespace CryptoNote.Activities
{
    /// <summary>
    /// ViewPager fragment that manages categories list. Yep, the name is pretty self-explanatory.
    /// </summary>
    class CategoriesListFragment : Android.Support.V4.App.Fragment
    {
        private Activity _rootActivity;
        private LinearLayout _list;
        private View _frame;
        private View _instance;
        private Button _revealDialogButton;
        private Button _deleteCategoriesButton;
        private EditText _title;
        private List<View> _selectedEntrys = new List<View>();
        private string _selectedCategory;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="activity">Root activity this fragment works with</param>
        public CategoriesListFragment(Activity activity) { _rootActivity = activity; _selectedCategory = _rootActivity.Resources.GetString(Resource.String.All); }

        /// <summary>
        /// Fragment creation event
        /// </summary>
        /// <param name="inflater">Layout inflater</param>
        /// <param name="container">Fragment container</param>
        /// <param name="savedInstanceState"></param>
        /// <returns>Categories list fragment</returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _instance = inflater.Inflate(Resource.Layout.CategoriesList, container, false);
            PopulateList(_list = _instance.FindViewById<LinearLayout>(Resource.Id.cathegoriesList));
            InitializeUI(_instance);
            return _instance;
        }

        /// <summary>
        /// Public method for list updating
        /// </summary>
        public void Update()
        {
            PopulateList(_list);
        }

        /// <summary>
        /// Acquiring all views and initializing them with event-handlers
        /// </summary>
        /// <param name="root">Root layout</param>
        private void InitializeUI(View root)
        {
            _frame = root.FindViewById<RelativeLayout>(Resource.Id.newCategoryFrame);
            View dialog = root.FindViewById<LinearLayout>(Resource.Id.newCategoryDialog);
            dialog.Click += (object sender, EventArgs e) => { };
            _title = root.FindViewById<EditText>(Resource.Id.newCategoryName);
            _title.Typeface = Typeface.CreateFromAsset(_rootActivity.Assets, "fonts/AlbertusNovaThin.otf"); ;
            root.FindViewById<Button>(Resource.Id.buttonAddCategory).Typeface = Typeface.CreateFromAsset(_rootActivity.Assets, "fonts/AlbertusNovaThin.otf");
            _frame.Click += (object sender, EventArgs e) => HideDialog();
            //Creating new category
            root.FindViewById<Button>(Resource.Id.buttonAddCategory).Click += (object sender, EventArgs e)
                                                                           => {
                                                                                if (_title.Text != string.Empty)
                                                                                {
                                                                                    AddCategory(_title.Text);
                                                                                    _frame.Visibility = ViewStates.Invisible;
                                                                                    _title.Text = string.Empty;
                                                                                    (_rootActivity.GetSystemService(Context.InputMethodService) as InputMethodManager)
                                                                                                                        .HideSoftInputFromWindow(_title.WindowToken, 0);
                                                                                    HideDialog();
                                                                                }
                                                                            };

            _revealDialogButton = root.FindViewById<Button>(Resource.Id.newCategoryButton);
            _deleteCategoriesButton = root.FindViewById<Button>(Resource.Id.deleteCategoryButton);
            _revealDialogButton.Click += (object sender, EventArgs e) => RevealDialog();
            _deleteCategoriesButton.Click += (object sender, EventArgs e) => RemoveCategories();
        }

        /// <summary>
        /// Add new category
        /// </summary>
        /// <param name="category">Category name</param>
        private void AddCategory(string category)
        {
            NoteStorage.AddCategory(category);
            PopulateList(_list);
        }

        /// <summary>
        /// Remove selected categories
        /// </summary>
        private void RemoveCategories()
        {
            foreach (string category in _selectedEntrys
                                        .Select(view => view.FindViewById<TextView>(Resource.Id.categoryName).Text))
            {
                foreach (Note note in NoteStorage.Notes.Where(note => note.CategoryId == NoteStorage.GetCurrentCategories(_rootActivity).IndexOf(category)))
                    note.CategoryId = 0;
                NoteStorage.RemoveCategory(_rootActivity, category);          
            }
            SecurityProvider.SaveNotesAsync();
            _selectedEntrys.Clear();
            HideDeleteButton();
            PopulateList(_list);
        }

        /// <summary>
        /// Populate list with categories
        /// </summary>
        /// <param name="layout">List layout</param>
        private void PopulateList(LinearLayout layout)
        {
            layout.RemoveAllViews();

            View entry = View.Inflate(_rootActivity, Resource.Layout.CategoriesListItem, null);
            entry.FindViewById<TextView>(Resource.Id.categoryName).Text = _rootActivity.GetString(Resource.String.All);
            entry.FindViewById<TextView>(Resource.Id.categoryName).SetTextColor(BuildColor(_rootActivity.GetString(Resource.String.All)));
            entry.FindViewById<TextView>(Resource.Id.categoryName).Typeface = Typeface.CreateFromAsset(_rootActivity.Assets, "fonts/AlbertusNovaThin.otf");
            entry.FindViewById<TextView>(Resource.Id.notesCount).Text = $"({NoteStorage.Notes.Count.ToString()})";
            
            entry.Click += (object sender, EventArgs e) =>
                            {
                                if (_selectedEntrys.Count == 0)
                                {
                                    NotesListFragment.ChangeDataSet(NoteStorage.Notes);
                                    MainPageActivity.Navigation.SetCurrentItem(0, true);
                                    _selectedCategory = _rootActivity.GetString(Resource.String.All);
                                    Refresh(layout);
                                }
                            };
            entry.LongClick += (object sender, View.LongClickEventArgs e) => { };
            layout.AddView(entry);

            foreach (string category in NoteStorage.GetCurrentCategories(_rootActivity))
            {
                int id = NoteStorage.GetCurrentCategories(_rootActivity).IndexOf(category);
                entry = View.Inflate(_rootActivity, Resource.Layout.CategoriesListItem, null);
                entry.FindViewById<TextView>(Resource.Id.categoryName).Text = category;
                entry.FindViewById<TextView>(Resource.Id.categoryName).SetTextColor(BuildColor(category));
                entry.FindViewById<TextView>(Resource.Id.categoryName).Typeface = Typeface.CreateFromAsset(_rootActivity.Assets, "fonts/AlbertusNovaThin.otf");
                entry.FindViewById<TextView>(Resource.Id.notesCount).Text = $"({NoteStorage.Notes.Where(n => n.CategoryId == id).Count().ToString()})";
                entry.Click += (object sender, EventArgs e) =>
                                {
                                    if (_selectedEntrys.Count == 0)
                                    {
                                        NotesListFragment.ChangeDataSet(NoteStorage.Notes.Where(n => n.CategoryId == id).ToList());
                                        MainPageActivity.Navigation.SetCurrentItem(0, true);
                                        _selectedCategory = category;
                                        Refresh(layout);
                                    }
                                };
                if (category != NoteStorage.GetCurrentCategories(_rootActivity)[0])
                {
                    entry.Click += (object sender, EventArgs e) => { if (_selectedEntrys.Count > 0) LongClickSelectEntry(sender as View); };
                    entry.LongClick += (object sender, View.LongClickEventArgs e) => LongClickSelectEntry(sender as View);
                }
                else
                    entry.LongClick += (object sender, View.LongClickEventArgs e) => { };
                layout.AddView(entry);
            }
        }
        
        /// <summary>
        /// Handling entry selection
        /// </summary>
        /// <param name="entry">Entry we are selecting/unselecting</param>
        private void LongClickSelectEntry(View entry)
        {
            if (!_selectedEntrys.Contains(entry))
            {
                _selectedEntrys.Add(entry);
                entry.FindViewById<RelativeLayout>(Resource.Id.categoriesListItem).SetBackgroundColor(Android.Graphics.Color.Argb(100, 163, 163, 163));
                if (_selectedEntrys.Count == 1)
                    ShowDeleteButton();
            }
            else
            {
                _selectedEntrys.Remove(entry);
                entry.FindViewById<RelativeLayout>(Resource.Id.categoriesListItem).SetBackgroundColor(Android.Graphics.Color.Transparent);
                if (_selectedEntrys.Count == 0)
                    HideDeleteButton();
            }
        }

        /// <summary>
        /// Delete button revelation animation
        /// </summary>
        private void ShowDeleteButton()
        {
            Animation animNewNoteButton = new RotateAnimation(0, 45, Dimension.RelativeToSelf, .5f, Dimension.RelativeToSelf, .5f) { Duration = 500 };
            animNewNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => _deleteCategoriesButton.BringToFront();
            _revealDialogButton.StartAnimation(animNewNoteButton);
        }

        /// <summary>
        /// Delete button hiding animation
        /// </summary>
        private void HideDeleteButton()
        {
            Animation animDeleteNoteButton = new RotateAnimation(0, -45, Dimension.RelativeToSelf, .5F, Dimension.RelativeToSelf, .5F) { Duration = 500 };
            animDeleteNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => _revealDialogButton.BringToFront();
            _deleteCategoriesButton.StartAnimation(animDeleteNoteButton);
        }

        /// <summary>
        /// Refreshing list method
        /// </summary>
        /// <param name="layout">List layout</param>
        private void Refresh(LinearLayout layout)
        {
            layout.GetChildAt(0).FindViewById<TextView>(Resource.Id.categoryName).SetTextColor(BuildColor(_rootActivity.GetString(Resource.String.All)));
            for (int i = 1; i < layout.ChildCount; i++)
                layout.GetChildAt(i).FindViewById<TextView>(Resource.Id.categoryName).SetTextColor(BuildColor(NoteStorage.GetCurrentCategories(_rootActivity)[i-1]));
        }

        /// <summary>
        /// Selecting color for each entry depending on selected category
        /// </summary>
        /// <param name="category">Category name</param>
        /// <returns>Category text color</returns>
        private Android.Graphics.Color BuildColor(string category)
            => (_selectedCategory == category) ? new Android.Graphics.Color(ContextCompat.GetColor(_rootActivity, Resource.Color.MainAppColor)) 
                                                   : Android.Graphics.Color.Black;

        /// <summary>
        /// Category creation dialog revelation animation
        /// </summary>
        private void RevealDialog()
        {
            if (_frame.Visibility == ViewStates.Invisible)
            {
                int centerX = _revealDialogButton.Left + (_revealDialogButton.Width / 2);
                int centerY = _revealDialogButton.Top + (_revealDialogButton.Height / 2);
                Animator reveal = ViewAnimationUtils.CreateCircularReveal(_frame, centerX, centerY, 0, _frame.Height + _frame.Width);
                reveal.SetDuration(500);
                _frame.Visibility = ViewStates.Visible;
                _frame.BringToFront();
                _title.RequestFocus();
                reveal.Start();
            }
        }

        /// <summary>
        /// Category creation dialog hiding animation
        /// </summary>
        private void HideDialog()
        {
            if (_frame.Visibility == ViewStates.Visible)
            {
                _title.Text = string.Empty;
                int centerX = _revealDialogButton.Left + (_revealDialogButton.Width / 2);
                int centerY = _revealDialogButton.Top + (_revealDialogButton.Height / 2);
                Animator reveal = ViewAnimationUtils.CreateCircularReveal(_frame, centerX, centerY, _frame.Height + _frame.Width, 0);
                reveal.SetDuration(500);
                reveal.AnimationEnd += (object sender, EventArgs e) => _frame.Visibility = ViewStates.Invisible;
                reveal.Start();
            }
        }

        /// <summary>
        /// Back button handling depending on various conditions
        /// </summary>
        public void HandleOnBackPressed()
        { 
            if (_frame.Visibility == ViewStates.Visible)
                HideDialog();
            else if (_selectedEntrys.Count != 0)
            {
                int initialCount = _selectedEntrys.Count;
                for (int i = 0; i < initialCount; i++)
                    LongClickSelectEntry(_selectedEntrys[0]);
            }
            else
                MainPageActivity.Navigation.SetCurrentItem(0, true);
        }
    }
}