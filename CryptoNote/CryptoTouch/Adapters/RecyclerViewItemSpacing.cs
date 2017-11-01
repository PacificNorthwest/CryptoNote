using Android.Views;
using Android.Support.V7.Widget;
using Android.Graphics;

namespace CryptoNote.Adapters
{
    /// <summary>
    /// RecyclerView item decorator
    /// </summary>
    class RecyclerViewItemSpacing : RecyclerView.ItemDecoration
    {
        private readonly int _spacing;

        public RecyclerViewItemSpacing(int spacing) { this._spacing = spacing; }

        /// <summary>
        /// Add extra spacing to a grid item
        /// </summary>
        /// <param name="outRect"></param>
        /// <param name="view"></param>
        /// <param name="parent"></param>
        /// <param name="state"></param>
        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        { outRect.Bottom = outRect.Right = _spacing; }
    }
}