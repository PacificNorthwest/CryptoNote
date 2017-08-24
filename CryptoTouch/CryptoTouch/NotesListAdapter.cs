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
using CryptoTouch.Activities;

namespace CryptoTouch
{
    class NotesListAdapter : RecyclerView.Adapter
    {
        private List<Note> _items;
        private Activity _context;

        public NotesListAdapter(Activity context, List<Note> items) { _items = items; _context = context; }

        public class ViewHolder : RecyclerView.ViewHolder
        {
            public Note Note { get; set; }
            public TextView NoteText { get; set; }
            public TextView Date { get; set; }

            public ViewHolder(Activity context, View itemView) : base(itemView)
            {
                NoteText = itemView.FindViewById<TextView>(Resource.Id.cardNoteTextPreview);
                Date = itemView.FindViewById<TextView>(Resource.Id.cardNoteDate);

                itemView.Click += (object sender, EventArgs e) =>
                                    {
                                        (sender as View).TransitionName = "Note";
                                        ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(context, sender as View, "Note");
                                        Intent intent = new Intent(context, typeof(NoteActivity));
                                        intent.PutExtra("NoteHash", Note.GetHashCode());
                                        context.StartActivity(intent, options.ToBundle());
                                    };

                itemView.LongClick += (object sender, View.LongClickEventArgs e) => { itemView.Tag = Note.GetHashCode(); (context as MainPageActivity).SelectItem(itemView); };
            }
        }

        public override int ItemCount => _items.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
            new ViewHolder(_context, LayoutInflater.From(parent.Context).Inflate(Resource.Layout.NoteItemLayout, null));

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as ViewHolder).Note = _items[position];
            (holder as ViewHolder).NoteText.Text = _items[position].Text;
            (holder as ViewHolder).Date.Text = _items[position].Date.ToString();
        }
    }
}