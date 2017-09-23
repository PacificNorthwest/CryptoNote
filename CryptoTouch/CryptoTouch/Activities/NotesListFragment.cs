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
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Transitions;
using Android.Graphics.Drawables;
using Android.Views.Animations;
using Android.Animation;
using Android.Graphics;
using System.Timers;
using System.Threading;

namespace CryptoTouch.Activities
{
    public class NotesListFragment : Android.Support.V4.App.Fragment
    {
        private static Activity _rootActivity;
        private static RecyclerView _notesGrid;
        private static Android.Support.V4.App.Fragment _instance;
        private List<View> _selectedItems = new List<View>();
        private Button _newNoteButton;
        private Button _deleteNoteButton;
        private RelativeLayout _sceneRoot;

        public bool ContainsSelectedItems => (_selectedItems.Count != 0) ? true : false;

        public NotesListFragment(Activity activity) { _rootActivity = activity; _instance = this; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.NotesList, container, false);

            InitializeUI(view);
            PopulateGrid();

            return view;
        }

        private void InitializeUI(View view)
        {
            _newNoteButton = view.FindViewById<Button>(Resource.Id.newNoteButton);
            _deleteNoteButton = view.FindViewById<Button>(Resource.Id.deleteNoteButton);
            _sceneRoot = view.FindViewById<RelativeLayout>(Resource.Id.layout);
            _notesGrid = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _deleteNoteButton.Click += (object sender, EventArgs e) => DeleteNotes();
            _newNoteButton.Click += (object sender, EventArgs e) =>
            {
                Intent intent = new Intent(_rootActivity, typeof(NoteActivity));
                StartActivity(intent);
            };
        }

        private void PopulateGrid()
        {
            _notesGrid.HasFixedSize = true;
            _notesGrid.SetLayoutManager(new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical));
            _notesGrid.AddItemDecoration(new RecyclerViewItemSpacing(30));
            _notesGrid.SetAdapter(new NotesListAdapter(_rootActivity, this, NoteStorage.Notes));
        }

        public static void ChangeDataSet(List<Note> notes)
        {
            _notesGrid.SwapAdapter(new NotesListAdapter(_rootActivity, _instance, notes), false);
        }

        public void SelectItem(View view)
        {
            if (!_selectedItems.Contains(view))
            {
                _selectedItems.Add(view);
                view.SetBackgroundResource(Resource.Drawable.CardSelectionBG);
                if (_deleteNoteButton.Visibility == ViewStates.Invisible)
                    ShowDeleteButton();
            }
            else
            {
                _selectedItems.Remove(view);
                view.SetBackgroundResource(Resource.Color.cardview_light_background);
                if (_selectedItems.Count == 0)
                    HideDeleteButton();
            }
        }

        private void ShowDeleteButton()
        {
            _deleteNoteButton.Visibility = ViewStates.Visible;
            _deleteNoteButton.Background.Alpha = 0;
            _deleteNoteButton.Rotation = -45F;

            Animation animNewNoteButton = new RotateAnimation(0, 45, Dimension.RelativeToSelf, .5F, Dimension.RelativeToSelf, .5F) { Duration = 500 };
            animNewNoteButton.AnimationStart += (object sender, Animation.AnimationStartEventArgs e) =>
            {
                ValueAnimator valueAnim = ValueAnimator.OfFloat(0F, 1F);
                valueAnim.Update += (object anim_sender, ValueAnimator.AnimatorUpdateEventArgs arg) =>
                {
                    float mul = (float)valueAnim.AnimatedValue;
                    _newNoteButton.Background.Alpha = 255 - Convert.ToInt32(255 * Convert.ToDouble(mul));
                };
                valueAnim.SetDuration(500);
                valueAnim.RepeatCount = 1;
                valueAnim.Start();
            };
            animNewNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => _newNoteButton.Visibility = ViewStates.Gone; 
            animNewNoteButton.FillAfter = true;

            Animation animDeleteNoteButton = new RotateAnimation(0, 45, Dimension.RelativeToSelf, .5F, Dimension.RelativeToSelf, .5F) { Duration = 500 };
            animDeleteNoteButton.AnimationStart += (object sender, Animation.AnimationStartEventArgs e) =>
            {
                ValueAnimator valueAnim = ValueAnimator.OfFloat(0F, 1F);
                valueAnim.Update += (object anim_sender, ValueAnimator.AnimatorUpdateEventArgs arg) =>
                {
                    float mul = (float)valueAnim.AnimatedValue;
                    _deleteNoteButton.Background.Alpha = Convert.ToInt32(255 * Convert.ToDouble(mul));
                };
                valueAnim.SetDuration(500);
                valueAnim.RepeatCount = 1;
                valueAnim.Start();
            };
            animDeleteNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e)
                                              =>
                                                {
                                                    View cover = View.Inflate(_rootActivity, Resource.Layout.ButtonCover, null);
                                                    cover.LayoutParameters = _deleteNoteButton.LayoutParameters;
                                                    _sceneRoot.AddView(cover);
                                                    cover.BringToFront();
                                                    _deleteNoteButton.BringToFront();
                                                };
            animDeleteNoteButton.FillAfter = true;

             _newNoteButton.StartAnimation(animNewNoteButton);
            _deleteNoteButton.StartAnimation(animDeleteNoteButton);
           
        }



        private void HideDeleteButton()
        {
            _deleteNoteButton.Visibility = ViewStates.Invisible;
            TransitionManager.BeginDelayedTransition(_sceneRoot);
            RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(180, 180);
            layoutParams.AddRule(LayoutRules.AlignParentRight);
            layoutParams.AddRule(LayoutRules.AlignParentBottom);
            layoutParams.BottomMargin = 30;
            layoutParams.RightMargin = 30;
            _deleteNoteButton.LayoutParameters = layoutParams;
        }

        private void DeleteNotes()
        {
            foreach (View view in _selectedItems)
                NoteStorage.Notes.Remove(NoteStorage.Notes.Find(note => note.GetHashCode() == (int)view.Tag));
            _selectedItems.Clear();
            SecurityProvider.SaveNotesAsync();
            //HideDeleteButton();
            PopulateGrid();

        }

    }
}