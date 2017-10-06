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
    class NoteStorage
    {
        public static List<Note> Notes { get; set; } = new List<Note>();
        public static List<String> Categories { get; set; } = XmlManager.LoadCategories();
    }
}