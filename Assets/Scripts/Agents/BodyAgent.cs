using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyAgent : MonoBehaviour {

	public enum DeathType
	{
		Laser = 0,
		Fire = 1,
		Crusher = 2,
		Cold = 3,
		Heat = 4,
		Invalid = 5,
	}

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

	public static Animator SpawnBody( Vector3 position, bool isFacingRight, DeathType type = DeathType.Invalid )
	{
		if( instance )
			return instance.internalSpawnBody( position, isFacingRight, type );

		return null;
	}

	private Animator internalSpawnBody( Vector3 position, bool isFacingRight, DeathType type )
	{
		if( body == null )
			return null;

		GameObject temp = Instantiate( body, position, Quaternion.identity ) as GameObject;

		temp.transform.parent = transform;

		if( isFacingRight )
			temp.transform.localScale = new Vector3( temp.transform.localScale.x * -1f, temp.transform.localScale.y, temp.transform.localScale.z );

		bodies.Add( temp );

		if( bodies.Count > maxNumBodies )
			bodies.RemoveAt( 0 );

		if( type == DeathType.Invalid )
			return null;

		Animator animator = temp.GetComponentInChildren<Animator>();

		if( animator == null )
			return null;

		animator.SetInteger( "deathType", (int)type );

		return animator;
	}
}


