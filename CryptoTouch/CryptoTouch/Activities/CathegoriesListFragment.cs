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

namespace CryptoTouch.Activities
{
    class CathegoriesListFragment : Android.Support.V4.App.Fragment
    {
        private Activity _rootActivity;
        private LinearLayout _list;
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
            View frame = root.FindViewById<RelativeLayout>(Resource.Id.newCathegoryFrame);
            View dialog = root.FindViewById<LinearLayout>(Resource.Id.newCathegoryDialog);
            EditText title = root.FindViewById<EditText>(Resource.Id.newCathegoryName);
            frame.Click += (object sender, EventArgs e) => frame.Visibility = ViewStates.Invisible;
            root.FindViewById<Button>(Resource.Id.buttonAddCathegory).Click += (object sender, EventArgs e)
                                                                            => { AddCathegory(title.Text);
                                                                                 frame.Visibility = ViewStates.Invisible;
                                                                                 title.Text = string.Empty;
                                                                                (_rootActivity.GetSystemService(Context.InputMethodService) as InputMethodManager)
                                                                                                                    .HideSoftInputFromWindow(title.WindowToken, 0); };

            root.FindViewById<Button>(Resource.Id.newCathegoryButton).Click += (object sender, EventArgs e) => frame.Visibility = ViewStates.Visible;
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
            entry.FindViewById<TextView>(Resource.Id.notesCount).Text = NoteStorage.Notes.Count.ToString();
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
                entry.FindViewById<TextView>(Resource.Id.notesCount).Text = NoteStorage.Notes.Where(n => n.Cathegory == cathegory).Count().ToString();
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
            => (_selectedCathegory == cathegory) ? Android.Graphics.Color.Green : Android.Graphics.Color.Black;
    }
}