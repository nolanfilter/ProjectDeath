using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraSystemAgent : MonoBehaviour {

	private List<Rect> hooks;
	
	private static CameraSystemAgent mInstance = null;
	public static CameraSystemAgent instance
	{
		get
		{
			return mInstance;
		}
	}
	
	void Awake()
	{
		if( mInstance != null )
		{
			Debug.LogError( string.Format( "Only one instance of CameraSystemAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;

		hooks = new List<Rect>();
	}

	public static void RegisterHook( Rect hook )
	{
		if( instance )
			instance.internalRegisterHook( hook );
	}

	private void internalRegisterHook( Rect hook )
	{
		if( !hooks.Contains( hook ) )
			hooks.Add( hook );
	}

	public static void UnregisterHook( Rect hook )
	{
		if( instance )
			instance.internalUnregisterHook( hook );
	}

	private void internalUnregisterHook( Rect hook )
	{
		if( hooks.Contains( hook ) )
			hooks.Remove( hook );
	}

	public static List<Rect> GetContainingHooks( Vector2 point )
	{
		if( instance )
			return instance.internalGetContainingHooks( point );

		return new List<Rect>();
	}

	private List<Rect> internalGetContainingHooks( Vector2 point )
	{
		List<Rect> containingHooks = new List<Rect>();

		for( int i = 0; i < hooks.Count; i++ )
			if( hooks[i].Contains( point ) )
				containingHooks.Add( hooks[i] );

		return containingHooks;
	}
}
