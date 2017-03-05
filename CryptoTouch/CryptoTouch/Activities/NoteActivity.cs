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
    [Activity(Label = "NoteActivity", WindowSoftInputMode = SoftInput.StateVisible)]
    public class NoteActivity : Activity
    {
        EditText editText;
        Button saveButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.NotePage);

            editText = FindViewById<EditText>(Resource.Id.editText);
            saveButton = FindViewById<Button>(Resource.Id.saveNoteButton);
            saveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            NoteStorage.Notes.Add(editText.Text);
            XmlManager.Save(NoteStorage.Notes);
            Intent intent = new Intent(this, typeof(MainPageActivity));
            StartActivity(intent);
        }
    }
}