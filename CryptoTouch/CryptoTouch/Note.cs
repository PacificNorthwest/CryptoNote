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

namespace CryptoTouch
{
    [Serializable]
    class Note
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public Note(string text)
        {
            Text = text;
            Date = DateTime.Now;
        }
    }
}