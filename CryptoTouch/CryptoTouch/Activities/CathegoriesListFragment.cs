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
using Android.Text;
using Android.Text.Style;
using Android.Views.InputMethods;
using Android.Support.V4.Content;
using Android.Animation;
using Android.Views.Animations;

namespace CryptoTouch.Activities
{
    class CategoriesListFragment : Android.Support.V4.App.Fragment
    {
        private Activity _rootActivity;
        private LinearLayout _list;
        private View _frame;
        private Button _revealDialogButton;
        private Button _deleteCategoriesButton;
        private EditText _title;
        private List<View> _selectedEntrys = new List<View>();
        private string _selectedCategory = "All";

        public CategoriesListFragment(Activity activity) { _rootActivity = activity; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.CategoriesList, container, false);
            PopulateList(_list = view.FindViewById<LinearLayout>(Resource.Id.cathegoriesList));
            InitializeUI(view);
            return view;
        }

        private void InitializeUI(View root)
        {
            _frame = root.FindViewById<RelativeLayout>(Resource.Id.newCategoryFrame);
            View dialog = root.FindViewById<LinearLayout>(Resource.Id.newCategoryDialog);
            dialog.Click += (object sender, EventArgs e) => { };
            _title = root.FindViewById<EditText>(Resource.Id.newCategoryName);
            _frame.Click += (object sender, EventArgs e) => HideDialog();
            root.FindViewById<Button>(Resource.Id.buttonAddCategory).Click += (object sender, EventArgs e)
                                                                            => {if (_title.Text != string.Empty)
                                                                                {
                                                                                    Addcategory(_title.Text);
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

        private void Addcategory(string category)
        {
            NoteStorage.Categories.Add(category);
            XmlManager.SaveCategories(NoteStorage.Categories);
            PopulateList(_list);
        }

        private void RemoveCategories()
        {
            foreach (string category in _selectedEntrys
                                        .Select(view => view.FindViewById<TextView>(Resource.Id.categoryName).Text))
            {
                NoteStorage.Categories.Remove(category);
                foreach (Note note in NoteStorage.Notes.Where(note => note.Category == category))
                    note.Category = "No category";
                SecurityProvider.SaveNotesAsync();
                XmlManager.SaveCategories(NoteStorage.Categories);
                
            }
            _selectedEntrys.Clear();
            HideDeleteButton();
            PopulateList(_list);
        }

        private void PopulateList(LinearLayout layout)
        {
            layout.RemoveAllViews();

            View entry = View.Inflate(_rootActivity, Resource.Layout.CategoriesListItem, null);
            entry.FindViewById<TextView>(Resource.Id.categoryName).Text = "All";
            entry.FindViewById<TextView>(Resource.Id.categoryName).SetTextColor(BuildColor("All"));
            entry.FindViewById<TextView>(Resource.Id.notesCount).Text = $"({NoteStorage.Notes.Count.ToString()})";
            entry.Click += (object sender, EventArgs e) =>
                            {
                                if (_selectedEntrys.Count == 0)
                                {
                                    NotesListFragment.ChangeDataSet(NoteStorage.Notes);
                                    MainPageActivity.Navigation.SetCurrentItem(0, true);
                                    _selectedCategory = "All";
                                    Refresh(layout);
                                }
                            };
            entry.LongClick += (object sender, View.LongClickEventArgs e) => { };
            layout.AddView(entry);

            foreach (string category in NoteStorage.Categories)
            {
                entry = View.Inflate(_rootActivity, Resource.Layout.CategoriesListItem, null);
                entry.FindViewById<TextView>(Resource.Id.categoryName).Text = category;
                entry.FindViewById<TextView>(Resource.Id.categoryName).SetTextColor(BuildColor(category));
                entry.FindViewById<TextView>(Resource.Id.notesCount).Text = $"({NoteStorage.Notes.Where(n => n.Category == category).Count().ToString()})";
                entry.Click += (object sender, EventArgs e) =>
                                {
                                    if (_selectedEntrys.Count == 0)
                                    {
                                        NotesListFragment.ChangeDataSet(NoteStorage.Notes.Where(n => n.Category == category).ToList());
                                        MainPageActivity.Navigation.SetCurrentItem(0, true);
                                        _selectedCategory = category;
                                        Refresh(layout);
                                    }
                                };
                if (category != "No category")
                {
                    entry.Click += (object sender, EventArgs e) => { if (_selectedEntrys.Count > 0) LongClickSelectEntry(sender as View); };
                    entry.LongClick += (object sender, View.LongClickEventArgs e) => LongClickSelectEntry(sender as View);
                }
                else
                    entry.LongClick += (object sender, View.LongClickEventArgs e) => { };
                layout.AddView(entry);
            }
        }
        
        private void LongClickSelectEntry(View entry)
        {
            if (!_selectedEntrys.Contains(entry))
            {
                _selectedEntrys.Add(entry);
                entry.FindViewById<View>(Resource.Id.underline).SetBackgroundColor(Android.Graphics.Color.Red);
                entry.FindViewById<TextView>(Resource.Id.categoryName).SetTextColor(Android.Graphics.Color.Red);
                entry.FindViewById<TextView>(Resource.Id.notesCount).SetTextColor(Android.Graphics.Color.Red);
                if (_selectedEntrys.Count == 1)
                    ShowDeleteButton();
            }
            else
            {
                _selectedEntrys.Remove(entry);
                entry.FindViewById<View>(Resource.Id.underline).SetBackgroundColor(new Android.Graphics.Color(
                                                                ContextCompat.GetColor(_rootActivity, Resource.Color.MainAppColor)));
                entry.FindViewById<TextView>(Resource.Id.categoryName).SetTextColor(
                                                                        BuildColor(entry.FindViewById<TextView>(Resource.Id.categoryName).Text));
                entry.FindViewById<TextView>(Resource.Id.notesCount).SetTextColor(Android.Graphics.Color.Argb(255, 128, 128, 128));
                if (_selectedEntrys.Count == 0)
                    HideDeleteButton();
            }
        }

        private void ShowDeleteButton()
        {
            Animation animNewNoteButton = new RotateAnimation(0, 45, Dimension.RelativeToSelf, .5f, Dimension.RelativeToSelf, .5f) { Duration = 500 };
            animNewNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => _deleteCategoriesButton.BringToFront();
            _revealDialogButton.StartAnimation(animNewNoteButton);
        }

        private void HideDeleteButton()
        {
            Animation animDeleteNoteButton = new RotateAnimation(0, -45, Dimension.RelativeToSelf, .5F, Dimension.RelativeToSelf, .5F) { Duration = 500 };
            animDeleteNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => _revealDialogButton.BringToFront();
            _deleteCategoriesButton.StartAnimation(animDeleteNoteButton);
        }

        private void Refresh(LinearLayout layout)
        {
            layout.GetChildAt(0).FindViewById<TextView>(Resource.Id.categoryName).SetTextColor(BuildColor("All"));
            for (int i = 1; i < layout.ChildCount; i++)
                layout.GetChildAt(i).FindViewById<TextView>(Resource.Id.categoryName).SetTextColor(BuildColor(NoteStorage.Categories[i-1]));
        }

        private Android.Graphics.Color BuildColor(string category)
            => (_selectedCategory == category) ? new Android.Graphics.Color(ContextCompat.GetColor(_rootActivity, Resource.Color.MainAppColor)) 
                                                 : Android.Graphics.Color.Black;

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