package md5c8c2619c18903f3e54bbe5474b0a63e0;


public class AuthCallback
	extends android.support.v4.hardware.fingerprint.FingerprintManagerCompat.AuthenticationCallback
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAuthenticationSucceeded:(Landroid/support/v4/hardware/fingerprint/FingerprintManagerCompat$AuthenticationResult;)V:GetOnAuthenticationSucceeded_Landroid_support_v4_hardware_fingerprint_FingerprintManagerCompat_AuthenticationResult_Handler\n" +
			"n_onAuthenticationFailed:()V:GetOnAuthenticationFailedHandler\n" +
			"";
		mono.android.Runtime.register ("CryptoNote.Security.AuthCallback, CryptoTouch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", AuthCallback.class, __md_methods);
	}


	public AuthCallback () throws java.lang.Throwable
	{
		super ();
		if (getClass () == AuthCallback.class)
			mono.android.TypeManager.Activate ("CryptoNote.Security.AuthCallback, CryptoTouch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public AuthCallback (md502d6e4510324cbd3241d51deea888945.LoginActivity p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == AuthCallback.class)
			mono.android.TypeManager.Activate ("CryptoNote.Security.AuthCallback, CryptoTouch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "CryptoNote.Activities.LoginActivity, CryptoTouch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void onAuthenticationSucceeded (android.support.v4.hardware.fingerprint.FingerprintManagerCompat.AuthenticationResult p0)
	{
		n_onAuthenticationSucceeded (p0);
	}

	private native void n_onAuthenticationSucceeded (android.support.v4.hardware.fingerprint.FingerprintManagerCompat.AuthenticationResult p0);


	public void onAuthenticationFailed ()
	{
		n_onAuthenticationFailed ();
	}

	private native void n_onAuthenticationFailed ();

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
