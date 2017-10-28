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
using Java.Lang;
using Android.Graphics;

namespace CryptoTouch
{
    class NotesListAdapter : RecyclerView.Adapter
    {
        private List<Note> _items;
        private List<ViewHolder> _holdersPull = new List<ViewHolder>();
        private Android.Support.V4.App.Fragment _fragment;
        private Activity _activity;

        public NotesListAdapter(Activity activity, Android.Support.V4.App.Fragment context, List<Note> items)
        {
            _items = items;
            _fragment = context;
            _activity = activity;
            UpdateDataSet(items);
        }

        public void UpdateDataSet(List<Note> items)
        {
            _holdersPull.Clear();
            _items = items;
            foreach (Note note in _items)
            {
                ViewHolder holder = new ViewHolder(_fragment, _activity, LayoutInflater.From(_activity).Inflate(Resource.Layout.NoteItemLayout, null));
                holder.Note = note;
                holder.NoteText.Text = note.Text;
                holder.Date.Text = note.Date.ToShortDateString();
                _holdersPull.Add(holder);
            }
        }

        public class ViewHolder : RecyclerView.ViewHolder
        {
            private Android.Support.V4.App.Fragment _fragment;
            public View Item { get; set; }
            public Note Note { get; set; }
            public TextView NoteText { get; set; }
            public TextView Date { get; set; }

            public ViewHolder(Android.Support.V4.App.Fragment fragment, Activity activity, View itemView) : base(itemView)
            {
                _fragment = fragment;
                Item = itemView;
                NoteText = Item.FindViewById<TextView>(Resource.Id.cardNoteTextPreview);
                Date = Item.FindViewById<TextView>(Resource.Id.cardNoteDate);

                Item.Click += (object sender, EventArgs e) =>
                                    {
                                        if ((fragment as NotesListFragment).ContainsSelectedItems)
                                        {
                                            Item.Tag = Note.GetHashCode();
                                            (fragment as NotesListFragment).SelectItem(Item);
                                        }
                                        else
                                        {
                                            Intent intent = new Intent(activity, typeof(NoteActivity));
                                            intent.PutExtra("NoteHash", Note.GetHashCode());
                                            activity.StartActivity(intent);
                                        }
                                    };

                Item.LongClick += (object sender, View.LongClickEventArgs e) => { Item.Tag = Note.GetHashCode(); (fragment as NotesListFragment).SelectItem(Item); };
            }
        }

        public override int ItemCount => _items.Count;
        public override int GetItemViewType(int position) => position;
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int position) => _holdersPull[position];
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) { SetAnimation(holder.ItemView); }

        private void SetAnimation(View view)
        {
            AlphaAnimation fade = new AlphaAnimation(0.0f, 1.0f) { Duration = 150 };
            ScaleAnimation scale = new ScaleAnimation(0.0f, 1.0f, 0.0f, 1.0f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f) { Duration = 150 };
            view.StartAnimation(fade);
            view.StartAnimation(scale);
        }
    }
}