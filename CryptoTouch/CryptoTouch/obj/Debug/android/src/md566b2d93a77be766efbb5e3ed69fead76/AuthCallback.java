package md566b2d93a77be766efbb5e3ed69fead76;


public class AuthCallback
	extends android.hardware.fingerprint.FingerprintManager.AuthenticationCallback
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAuthenticationSucceeded:(Landroid/hardware/fingerprint/FingerprintManager$AuthenticationResult;)V:GetOnAuthenticationSucceeded_Landroid_hardware_fingerprint_FingerprintManager_AuthenticationResult_Handler\n" +
			"n_onAuthenticationFailed:()V:GetOnAuthenticationFailedHandler\n" +
			"";
		mono.android.Runtime.register ("CryptoTouch.AuthCallback, CryptoTouch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", AuthCallback.class, __md_methods);
	}


	public AuthCallback () throws java.lang.Throwable
	{
		super ();
		if (getClass () == AuthCallback.class)
			mono.android.TypeManager.Activate ("CryptoTouch.AuthCallback, CryptoTouch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public AuthCallback (md5d619d79349270a1d3e86c8aac97c2862.LoginActivity p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == AuthCallback.class)
			mono.android.TypeManager.Activate ("CryptoTouch.AuthCallback, CryptoTouch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "CryptoTouch.Activities.LoginActivity, CryptoTouch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void onAuthenticationSucceeded (android.hardware.fingerprint.FingerprintManager.AuthenticationResult p0)
	{
		n_onAuthenticationSucceeded (p0);
	}

	private native void n_onAuthenticationSucceeded (android.hardware.fingerprint.FingerprintManager.AuthenticationResult p0);


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
