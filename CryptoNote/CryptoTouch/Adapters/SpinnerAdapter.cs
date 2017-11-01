using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace CryptoNote.Adapters
{
    /// <summary>
    /// Spinner adapter aimed on creating different views for dropdown and normal state
    /// </summary>
    class SpinnerAdapter : ArrayAdapter
    {
        private Context _context;
        private List<string> _items;
        private LayoutInflater _inflater;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resId"></param>
        /// <param name="items"></param>
        public SpinnerAdapter(Context context, int resId, List<string> items) : base(context, resId, items)
        {
            _inflater = (context as Activity).GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            _context = context;
            _items = items;
        }

        /// <summary>
        /// Get normal state view
        /// </summary>
        /// <param name="position">Item position</param>
        /// <param name="convertView"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
                convertView = _inflater.Inflate(Resource.Layout.SpinnerItem, null);
            convertView.FindViewById<TextView>(Resource.Id.spinnerEntryText).SetTextColor(Android.Graphics.Color.White);
            convertView.FindViewById<TextView>(Resource.Id.spinnerEntryText).Text = _items[position];
            return convertView;
        }

        /// <summary>
        /// Get dropdown view
        /// </summary>
        /// <param name="position">Item position</param>
        /// <param name="convertView"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            View entry = _inflater.Inflate(Resource.Layout.SpinnerItem, parent, false);
            entry.FindViewById<TextView>(Resource.Id.spinnerEntryText).SetTextColor(Android.Graphics.Color.Black);
            entry.FindViewById<TextView>(Resource.Id.spinnerEntryText).Gravity = GravityFlags.Center;
            entry.FindViewById<TextView>(Resource.Id.spinnerEntryText).Text = _items[position];
            return entry;
        }
    }
}