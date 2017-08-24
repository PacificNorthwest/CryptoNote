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
using Android.Graphics;

namespace CryptoTouch
{
    class RecyclerViewItemSpacing : RecyclerView.ItemDecoration
    {
        private readonly int _spacing;

        public RecyclerViewItemSpacing(int spacing) { this._spacing = spacing; }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            outRect.Bottom = outRect.Top = outRect.Left = outRect.Right = _spacing;
        }
    }
}