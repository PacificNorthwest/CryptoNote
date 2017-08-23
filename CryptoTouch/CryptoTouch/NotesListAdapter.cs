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
using Android.Support.V7.Widget;

namespace CryptoTouch
{
    class NotesListAdapter : RecyclerView.Adapter
    {
        private string[] _items;

        public NotesListAdapter(string[] items) { _items = items; }

        public class ViewHolder : RecyclerView.ViewHolder
        {
            public Button Button { get; set; }
            public ViewHolder(Button button) : base(button) { Button = button; }
        }

        public override int ItemCount => _items.Length;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var button = new Button(parent.Context);
            button.Text = string.Empty;
            return new ViewHolder(button);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as ViewHolder).Button.Text = _items[position];
        }
    }
}