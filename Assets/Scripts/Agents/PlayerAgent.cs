using UnityEngine;
using System.Collections;

public class PlayerAgent : MonoBehaviour {

	private PlayerController playerController;

	private static PlayerAgent mInstance = null;
	public static PlayerAgent instance
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
			Debug.LogError( string.Format( "Only one instance of PlayerAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;
	}

	public static void RegisterPlayerController( PlayerController newPlayerController )
	{
		if( instance )
			instance.internalRegisterPlayerController( newPlayerController );
	}

	private void internalRegisterPlayerController( PlayerController newPlayerController )
	{
		playerController = newPlayerController;
	}

	public static void UnregisterPlayerController()
	{
		if( instance )
			instance.internalUnregisterPlayerController();
	}

	private void internalUnregisterPlayerController()
	{
		playerController = null;
	}

	public static PlayerController GetPlayerController()
	{
		if( instance )
			return instance.internalGetPlayerController();

		return null;
	}

	private PlayerController internalGetPlayerController()
	{
		return playerController;
	}
}
