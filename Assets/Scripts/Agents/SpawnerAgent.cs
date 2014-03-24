using UnityEngine;
using System.Collections;

public class SpawnerAgent : MonoBehaviour {

	private Vector3 spawnerPosition;

	private static SpawnerAgent mInstance = null;
	public static SpawnerAgent instance
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
			Debug.LogError( string.Format( "Only one instance of SpawnerAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;
	}

	void Start()
	{
		SetSpawnerPosition( Vector3.zero );
	}

	public static Vector3 GetSpawnerPosition()
	{
		if( instance )
			return instance.spawnerPosition;

		return Vector3.zero;
	}

	public static void SetSpawnerPosition( Vector3 newPosition )
	{
		if( instance )
			instance.spawnerPosition = newPosition;
	}
}
