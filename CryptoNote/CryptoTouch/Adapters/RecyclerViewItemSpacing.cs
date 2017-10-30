using Android.Views;
using Android.Support.V7.Widget;
using Android.Graphics;

namespace CryptoNote.Adapters
{
    class RecyclerViewItemSpacing : RecyclerView.ItemDecoration
    {
        private readonly int _spacing;

        public RecyclerViewItemSpacing(int spacing) { this._spacing = spacing; }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            outRect.Bottom = outRect.Right = _spacing;
        }
    }
}