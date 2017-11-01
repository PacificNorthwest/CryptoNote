using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Views.Animations;

using CryptoNote.Activities;
using CryptoNote.Model;

namespace CryptoNote.Adapters
{
    /// <summary>
    /// RecyclerView adapter
    /// </summary>
    class NotesListAdapter : RecyclerView.Adapter
    {
        private List<Note> _items;
        private List<ViewHolder> _holdersPull = new List<ViewHolder>();
        private Android.Support.V4.App.Fragment _fragment;
        private Activity _activity;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="activity">Root activity</param>
        /// <param name="context">NotesList fragment</param>
        /// <param name="items">Notes list</param>
        public NotesListAdapter(Activity activity, Android.Support.V4.App.Fragment context, List<Note> items)
        {
            _items = items;
            _fragment = context;
            _activity = activity;
            UpdateDataSet(items);
        }

        /// <summary>
        /// Precreating viewholders pull for later usage
        /// </summary>
        /// <param name="items"></param>
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

        /// <summary>
        /// ViewHolder nested class
        /// </summary>
        public class ViewHolder : RecyclerView.ViewHolder
        {
            private Android.Support.V4.App.Fragment _fragment;
            public View Item { get; set; }
            public Note Note { get; set; }
            public TextView NoteText { get; set; }
            public TextView Date { get; set; }

            /// <summary>
            /// Creating a viewholder and initializing it with basic references and handlers
            /// </summary>
            /// <param name="fragment">NotesList fragment</param>
            /// <param name="activity">Root activity</param>
            /// <param name="itemView">Actual view</param>
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

        /// <summary>
        /// Getting items count
        /// </summary>
        public override int ItemCount => _items.Count;

        /// <summary>
        /// Returning actual viewholder position in precreated pull instead of a view id
        /// </summary>
        /// <param name="position">ViewHolder position</param>
        /// <returns>ViewHolder position</returns>
        public override int GetItemViewType(int position) => position;

        /// <summary>
        /// Acquiring ViewHolder instance from precreated pull instead of creating a new one
        /// </summary>
        /// <param name="parent">Parent ViewGroup</param>
        /// <param name="position">Position in pull</param>
        /// <returns>Precreated ViewHolder</returns>
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int position) => _holdersPull[position];

        /// <summary>
        /// Blank method in order to block ViewHolder binding
        /// </summary>
        /// <param name="holder">ViewHolder</param>
        /// <param name="position">Item position in notes list</param>
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) { }
    }
}