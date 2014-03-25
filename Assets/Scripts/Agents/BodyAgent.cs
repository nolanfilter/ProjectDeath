using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyAgent : MonoBehaviour {

	public GameObject body;
	private List<GameObject> bodies;

	private int maxNumBodies = 1000;

	private static BodyAgent mInstance = null;
	public static BodyAgent instance
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
			Debug.LogError( string.Format( "Only one instance of BodyAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;

		bodies = new List<GameObject>();
	}

	public static void SpawnBody( Vector3 position )
	{
		if( instance )
			instance.internalSpawnBody( position );
	}

	private void internalSpawnBody( Vector3 position )
	{
		if( body == null )
			return;

		GameObject temp = Instantiate( body, position, Quaternion.identity ) as GameObject;

		temp.transform.parent = transform;

		bodies.Add( temp );

		if( bodies.Count > maxNumBodies )
			bodies.RemoveAt( 0 );
	}
}
