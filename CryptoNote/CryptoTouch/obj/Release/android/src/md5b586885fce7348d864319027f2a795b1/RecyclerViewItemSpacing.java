package md5b586885fce7348d864319027f2a795b1;


public class RecyclerViewItemSpacing
	extends android.support.v7.widget.RecyclerView.ItemDecoration
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getItemOffsets:(Landroid/graphics/Rect;Landroid/view/View;Landroid/support/v7/widget/RecyclerView;Landroid/support/v7/widget/RecyclerView$State;)V:GetGetItemOffsets_Landroid_graphics_Rect_Landroid_view_View_Landroid_support_v7_widget_RecyclerView_Landroid_support_v7_widget_RecyclerView_State_Handler\n" +
			"";
		mono.android.Runtime.register ("CryptoNote.Adapters.RecyclerViewItemSpacing, CryptoTouch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", RecyclerViewItemSpacing.class, __md_methods);
	}


	public RecyclerViewItemSpacing () throws java.lang.Throwable
	{
		super ();
		if (getClass () == RecyclerViewItemSpacing.class)
			mono.android.TypeManager.Activate ("CryptoNote.Adapters.RecyclerViewItemSpacing, CryptoTouch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public RecyclerViewItemSpacing (int p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == RecyclerViewItemSpacing.class)
			mono.android.TypeManager.Activate ("CryptoNote.Adapters.RecyclerViewItemSpacing, CryptoTouch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0 });
	}


	public void getItemOffsets (android.graphics.Rect p0, android.view.View p1, android.support.v7.widget.RecyclerView p2, android.support.v7.widget.RecyclerView.State p3)
	{
		n_getItemOffsets (p0, p1, p2, p3);
	}

	private native void n_getItemOffsets (android.graphics.Rect p0, android.view.View p1, android.support.v7.widget.RecyclerView p2, android.support.v7.widget.RecyclerView.State p3);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
