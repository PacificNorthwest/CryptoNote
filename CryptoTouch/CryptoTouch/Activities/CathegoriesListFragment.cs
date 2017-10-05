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

namespace CryptoTouch.Activities
{
    class CathegoriesListFragment : Android.Support.V4.App.Fragment
    {
        private Activity _rootActivity;
        private LinearLayout _list;
        private View _frame;
        private Button _revealDialogButton;
        private EditText _title;
        private string _selectedCathegory = "All";

        public CathegoriesListFragment(Activity activity) { _rootActivity = activity; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.CathegoriesList, container, false);
            PopulateList(_list = view.FindViewById<LinearLayout>(Resource.Id.cathegoriesList));
            InitializeUI(view);
            return view;
        }

        private void InitializeUI(View root)
        {
            _frame = root.FindViewById<RelativeLayout>(Resource.Id.newCathegoryFrame);
            View dialog = root.FindViewById<LinearLayout>(Resource.Id.newCathegoryDialog);
            dialog.Click += (object sender, EventArgs e) => { };
            _title = root.FindViewById<EditText>(Resource.Id.newCathegoryName);
            _frame.Click += (object sender, EventArgs e) => HideDialog();
            root.FindViewById<Button>(Resource.Id.buttonAddCathegory).Click += (object sender, EventArgs e)
                                                                            => {if (_title.Text != string.Empty)
                                                                                {
                                                                                    AddCathegory(_title.Text);
                                                                                    _frame.Visibility = ViewStates.Invisible;
                                                                                    _title.Text = string.Empty;
                                                                                    (_rootActivity.GetSystemService(Context.InputMethodService) as InputMethodManager)
                                                                                                                        .HideSoftInputFromWindow(_title.WindowToken, 0);
                                                                                    HideDialog();
                                                                                }
                                                                            };

            _revealDialogButton = root.FindViewById<Button>(Resource.Id.newCathegoryButton);
            _revealDialogButton.Click += (object sender, EventArgs e) => RevealDialog();
        }

        private void AddCathegory(string cathegory)
        {
            NoteStorage.Cathegories.Add(cathegory);
            XmlManager.SaveCathegories(NoteStorage.Cathegories);
            PopulateList(_list);
        }

        private void PopulateList(LinearLayout layout)
        {
            layout.RemoveAllViews();

            View entry = View.Inflate(_rootActivity, Resource.Layout.CathegoriesListItem, null);
            entry.FindViewById<TextView>(Resource.Id.cathegoryName).Text = "All";
            entry.FindViewById<TextView>(Resource.Id.cathegoryName).SetTextColor(BuildColor("All"));
            entry.FindViewById<TextView>(Resource.Id.notesCount).Text = $"({NoteStorage.Notes.Count.ToString()})";
            entry.Click += (object sender, EventArgs e) =>
                            {
                                NotesListFragment.ChangeDataSet(NoteStorage.Notes);
                                MainPageActivity.Navigation.SetCurrentItem(0, true);
                                _selectedCathegory = "All";
                                Refresh(layout);
                            };
            layout.AddView(entry);

            foreach (string cathegory in NoteStorage.Cathegories)
            {
                entry = View.Inflate(_rootActivity, Resource.Layout.CathegoriesListItem, null);
                entry.FindViewById<TextView>(Resource.Id.cathegoryName).Text = cathegory;
                entry.FindViewById<TextView>(Resource.Id.cathegoryName).SetTextColor(BuildColor(cathegory));
                entry.FindViewById<TextView>(Resource.Id.notesCount).Text = $"({NoteStorage.Notes.Where(n => n.Cathegory == cathegory).Count().ToString()})";
                entry.Click += (object sender, EventArgs e) =>
                                {
                                    NotesListFragment.ChangeDataSet(NoteStorage.Notes.Where(n => n.Cathegory == cathegory).ToList());
                                    MainPageActivity.Navigation.SetCurrentItem(0, true);
                                    _selectedCathegory = cathegory;
                                    Refresh(layout);
                                };

                layout.AddView(entry);
            }
        }

        private void Refresh(LinearLayout layout)
        {
            layout.GetChildAt(0).FindViewById<TextView>(Resource.Id.cathegoryName).SetTextColor(BuildColor("All"));
            for (int i = 1; i < layout.ChildCount; i++)
                layout.GetChildAt(i).FindViewById<TextView>(Resource.Id.cathegoryName).SetTextColor(BuildColor(NoteStorage.Cathegories[i-1]));
        }

        private Android.Graphics.Color BuildColor(string cathegory)
            => (_selectedCathegory == cathegory) ? new Android.Graphics.Color(ContextCompat.GetColor(_rootActivity, Resource.Color.MainAppColor)) 
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
            else
                MainPageActivity.Navigation.SetCurrentItem(0, true);
        }
    }
}