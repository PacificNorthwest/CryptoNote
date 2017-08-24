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

namespace CryptoTouch
{
    class NoteEditText : EditText
    {
        public Activities.NoteActivity RootActivity;

        public NoteEditText(Context context) : base(context) { }
        public NoteEditText(Context context, Android.Util.IAttributeSet set) : base(context, set) { }

        public override bool OnKeyPreIme([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (e.Action == KeyEventActions.Up && keyCode == Keycode.Back)
            {
                if (RootActivity.SaveButtonMargin > 10)
                    RootActivity.SaveButtonMargin = 10;
                return base.OnKeyPreIme(keyCode, e);
            }
            return base.OnKeyPreIme(keyCode, e);
        }
    }
}