using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnerAgent : MonoBehaviour {

	public Vector3 beginCheckpointPosition = Vector3.zero;
	public Vector3 beginSpawnerPosition = Vector3.zero;
	public Animator beginSpawnerAnimator = null;

	private List<Vector3> checkpointPositions;
	private Vector3 spawnerPosition;
	private Animator spawnerAnimator;

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

		checkpointPositions = new List<Vector3>();
	}

	void Start()
	{
		AddCheckpointPosition( beginCheckpointPosition );
		SetSpawnerPosition( beginSpawnerPosition );
		SetSpawnerAnimator( beginSpawnerAnimator );
	}

	public static void AddCheckpointPosition( Vector3 newPosition )
	{
		if( instance )
			instance.internalAddCheckpointPosition( newPosition );
	}

	private void internalAddCheckpointPosition( Vector3 newPosition )
	{
		if( !checkpointPositions.Contains( newPosition ) )
			checkpointPositions.Add( newPosition );
	}

	public static Vector3 GetCheckpointPosition()
	{
		if( instance )
			return instance.internalGetCheckpointPosition();

		return Vector3.zero;
	}

	private Vector3 internalGetCheckpointPosition()
	{
		if( checkpointPositions.Count == 0 )
			return Vector3.zero;

		return checkpointPositions[ checkpointPositions.Count - 1 ];
	}

	public static Animator GetSpawnerAnimator()
	{
		if( instance )
			return instance.spawnerAnimator;
		
		return null;
	}

	public static void SetSpawnerAnimator( Animator newSpawnerAnimator )
	{
		if( instance )
			instance.spawnerAnimator = newSpawnerAnimator;
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
