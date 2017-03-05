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

namespace CryptoTouch.Activities
{
    [Activity(Label = "MainPageActivity")]
    public class MainPageActivity : Activity
    {
        RelativeLayout layout;
        Button newNoteButton;
        LinearLayout notesContainer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainPage);
            
            layout = FindViewById<RelativeLayout>(Resource.Id.layout);
            newNoteButton = FindViewById<Button>(Resource.Id.newNoteButton);
            notesContainer = FindViewById<LinearLayout>(Resource.Id.notesContainer);
            newNoteButton.Click += NewNoteButton_Click;

            try
            {
                NoteStorage.Notes = XmlManager.Load(NoteStorage.Notes.GetType()) as List<string>;
                InitializeUI();
            }
            catch { }

        }

        private void InitializeUI()
        {
            foreach(string note in NoteStorage.Notes)
            {
                Button button = new Button(this);
                button.Text = note;
                button.SetHeight(50);
                notesContainer.AddView(button);
            }
        }

        private void NewNoteButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(NoteActivity));
            StartActivity(intent);
        }
    }
}