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
        private View _newNoteButton;
        private View _deleteNoteButton;
        private View _cover;
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
            _cover = view.FindViewById<View>(Resource.Id.cover);
            _sceneRoot = view.FindViewById<RelativeLayout>(Resource.Id.layout);
            _notesGrid = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            _newNoteButton.Click += (object sender, EventArgs e) => StartActivity(new Intent(_rootActivity, typeof(NoteActivity)));
            _deleteNoteButton.Click += (object sender, EventArgs e) => DeleteNotes();        
        }

        private View CreateButton()
        {
            View button = new View(_rootActivity)
            { LayoutParameters = new RelativeLayout.LayoutParams(180, 180) };
            (button.LayoutParameters as RelativeLayout.LayoutParams).BottomMargin = 30;
            (button.LayoutParameters as RelativeLayout.LayoutParams).RightMargin = 30;
            (button.LayoutParameters as RelativeLayout.LayoutParams).AddRule(LayoutRules.AlignParentBottom);
            (button.LayoutParameters as RelativeLayout.LayoutParams).AddRule(LayoutRules.AlignParentRight);
            return button;
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
            _deleteNoteButton.Alpha = 0;
            _deleteNoteButton.Rotation = -45f;

            Animation animNewNoteButton = new RotateAnimation(0, 45, Dimension.RelativeToSelf, .5f, Dimension.RelativeToSelf, .5f) { Duration = 500, FillAfter = true };
            animNewNoteButton.AnimationStart += (object sender, Animation.AnimationStartEventArgs e) =>
            {
                ValueAnimator valueAnim = ValueAnimator.OfFloat(0F, 1F);
                valueAnim.Update += (object anim_sender, ValueAnimator.AnimatorUpdateEventArgs arg) =>
                {
                    float mul = (float)valueAnim.AnimatedValue;
                    _newNoteButton.Alpha = 255 - Convert.ToInt32(255 * Convert.ToDouble(mul));
                };
                valueAnim.SetDuration(500);
                valueAnim.RepeatCount = 1;
                valueAnim.Start();
            };
            animNewNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => _newNoteButton.Visibility = ViewStates.Invisible; 

            Animation animDeleteNoteButton = new RotateAnimation(0, 45, Dimension.RelativeToSelf, .5F, Dimension.RelativeToSelf, .5F) { Duration = 500, FillAfter = true };
            animDeleteNoteButton.AnimationStart += (object sender, Animation.AnimationStartEventArgs e) =>
            {
                ValueAnimator valueAnim = ValueAnimator.OfFloat(0F, 1F);
                valueAnim.Update += (object anim_sender, ValueAnimator.AnimatorUpdateEventArgs arg) =>
                {
                    float mul = (float)valueAnim.AnimatedValue;
                    _deleteNoteButton.Alpha = Convert.ToInt32(255 * Convert.ToDouble(mul));
                };
                valueAnim.SetDuration(500);
                valueAnim.RepeatCount = 1;
                valueAnim.Start();
            };
            animDeleteNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e)
                                              =>
                                                 { 
                                                   _cover.SetBackgroundResource(Resource.Drawable.error);
                                                   _cover.Alpha = 255;
                                                   _cover.BringToFront();
                                                   _deleteNoteButton.BringToFront(); };

            _newNoteButton.StartAnimation(animNewNoteButton);
            _deleteNoteButton.StartAnimation(animDeleteNoteButton);
        }



        private void HideDeleteButton()
        {
            _newNoteButton.Visibility = ViewStates.Visible;
            _newNoteButton.Alpha = 0;
            _cover.Alpha = 0;

            Animation animNewNoteButton = new RotateAnimation(45, 0, Dimension.RelativeToSelf, .5f, Dimension.RelativeToSelf, .5f) { Duration = 500, FillAfter = true };
            animNewNoteButton.AnimationStart += (object sender, Animation.AnimationStartEventArgs e) =>
            {
                ValueAnimator valueAnim = ValueAnimator.OfFloat(0F, 1F);
                valueAnim.Update += (object anim_sender, ValueAnimator.AnimatorUpdateEventArgs arg) =>
                {
                    float mul = (float)valueAnim.AnimatedValue;
                    _newNoteButton.Alpha = Convert.ToInt32(255 * Convert.ToDouble(mul));
                };
                valueAnim.SetDuration(500);
                valueAnim.RepeatCount = 1;
                valueAnim.Start();
            };
            animNewNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) 
                                           => 
                                              {
                                                  _cover.SetBackgroundResource(Resource.Drawable.plus);
                                                  _cover.Alpha = 255;
                                                  _cover.BringToFront();
                                                  _newNoteButton.BringToFront();
                                              };

            Animation animDeleteNoteButton = new RotateAnimation(45, 0, Dimension.RelativeToSelf, .5F, Dimension.RelativeToSelf, .5F) { Duration = 500, FillAfter = true };
            animDeleteNoteButton.AnimationStart += (object sender, Animation.AnimationStartEventArgs e) =>
            {
                ValueAnimator valueAnim = ValueAnimator.OfFloat(0F, 1F);
                valueAnim.Update += (object anim_sender, ValueAnimator.AnimatorUpdateEventArgs arg) =>
                {
                    float mul = (float)valueAnim.AnimatedValue;
                    _deleteNoteButton.Alpha = 255 - Convert.ToInt32(255 * Convert.ToDouble(mul));
                };
                valueAnim.SetDuration(500);
                valueAnim.RepeatCount = 1;
                valueAnim.Start();
            };
            animDeleteNoteButton.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) => _deleteNoteButton.Visibility = ViewStates.Invisible;

            
            _newNoteButton.StartAnimation(animNewNoteButton);
            _deleteNoteButton.StartAnimation(animDeleteNoteButton);
        }

        private void DeleteNotes()
        {
            foreach (View view in _selectedItems)
                NoteStorage.Notes.Remove(NoteStorage.Notes.Find(note => note.GetHashCode() == (int)view.Tag));
            _selectedItems.Clear();
            SecurityProvider.SaveNotesAsync();
            HideDeleteButton();
            PopulateGrid();

        }

    }
}