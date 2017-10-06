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
    class SpinnerAdapter : ArrayAdapter
    {
        private Context _context;
        private List<string> _items;
        private LayoutInflater _inflater;

        public SpinnerAdapter(Context context, int resId, List<string> items) : base(context, resId, items)
        {
            _inflater = (context as Activity).GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            _context = context;
            _items = items;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
                convertView = _inflater.Inflate(Resource.Layout.SpinnerItem, null);
            convertView.FindViewById<TextView>(Resource.Id.spinnerEntryText).SetTextColor(Android.Graphics.Color.White);
            convertView.FindViewById<TextView>(Resource.Id.spinnerEntryText).Text = _items[position];
            return convertView;
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            View entry = _inflater.Inflate(Resource.Layout.SpinnerItem, parent, false);
            entry.FindViewById<TextView>(Resource.Id.spinnerEntryText).SetTextColor(Android.Graphics.Color.Black);
            entry.FindViewById<TextView>(Resource.Id.spinnerEntryText).Text = _items[position];
            return entry;
        }
    }
}