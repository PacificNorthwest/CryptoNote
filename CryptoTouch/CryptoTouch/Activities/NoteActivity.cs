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
        private EditText _editText;
        private Button _saveButton;
        private string _originalNote;
        private bool _isNewNote;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.NotePage);

            _editText = FindViewById<EditText>(Resource.Id.editText);
            _saveButton = FindViewById<Button>(Resource.Id.saveNoteButton);
            _saveButton.Click += SaveButton_Click;

            Intent intent = Intent;
            if (intent.Extras != null)
            {
                _originalNote = intent.GetStringExtra("NoteToEdit");
                _editText.Text = _originalNote;
                _isNewNote = false;
            }
            else
                _isNewNote = true;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (_isNewNote)
                NoteStorage.Notes.Add(_editText.Text);
            else
                NoteStorage.Notes[NoteStorage.Notes.IndexOf(_originalNote)] = _editText.Text;

            XmlManager.Save(NoteStorage.Notes);
            Intent intent = new Intent(this, typeof(MainPageActivity));
            StartActivity(intent);
        }
    }
}