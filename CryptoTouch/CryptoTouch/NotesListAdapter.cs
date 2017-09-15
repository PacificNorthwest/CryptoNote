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
using Android.Views.Animations;

namespace CryptoTouch
{
    class NotesListAdapter : RecyclerView.Adapter
    {
        private List<Note> _items;
        private Android.Support.V4.App.Fragment _fragment;
        private Activity _activity;

        public NotesListAdapter(Activity activity, Android.Support.V4.App.Fragment context, List<Note> items) { _items = items; _fragment = context; _activity = activity; }

        public class ViewHolder : RecyclerView.ViewHolder
        {
            public Note Note { get; set; }
            public TextView NoteText { get; set; }
            public TextView Date { get; set; }

            public ViewHolder(Android.Support.V4.App.Fragment fragment, Activity activity, View itemView) : base(itemView)
            {
                NoteText = itemView.FindViewById<TextView>(Resource.Id.cardNoteTextPreview);
                Date = itemView.FindViewById<TextView>(Resource.Id.cardNoteDate);

                itemView.Click += (object sender, EventArgs e) =>
                                    {
                                        if ((fragment as NotesListFragment).ContainsSelectedItems)
                                        {
                                            itemView.Tag = Note.GetHashCode();
                                            (fragment as NotesListFragment).SelectItem(itemView);
                                        }
                                        else
                                        {
                                            Intent intent = new Intent(activity, typeof(NoteActivity));
                                            intent.PutExtra("NoteHash", Note.GetHashCode());
                                            activity.StartActivity(intent);
                                        }
                                    };

                itemView.LongClick += (object sender, View.LongClickEventArgs e) => { itemView.Tag = Note.GetHashCode(); (fragment as NotesListFragment).SelectItem(itemView); };
            }
        }

        public override int ItemCount => _items.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
            new ViewHolder(_fragment, _activity, LayoutInflater.From(parent.Context).Inflate(Resource.Layout.NoteItemLayout, null));

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as ViewHolder).Note = _items[position];
            (holder as ViewHolder).NoteText.Text = _items[position].Text;
            (holder as ViewHolder).Date.Text = _items[position].Date.ToString();
            SetAnimation(holder.ItemView);
        }

        private void SetAnimation(View view)
        {
            AlphaAnimation fade = new AlphaAnimation(0.0f, 1.0f) { Duration = 300 };
            ScaleAnimation scale = new ScaleAnimation(0.0f, 1.0f, 0.0f, 1.0f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f) { Duration = 300 };
            view.StartAnimation(fade);
            view.StartAnimation(scale);
        }
    }
}