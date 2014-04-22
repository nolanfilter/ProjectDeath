using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnerAgent : MonoBehaviour {

	public struct SpawnerInfo
	{
		public Vector3 position { get; private set; }
		public Animator animator { get; private set; }
		public GameObject areaCoverObject { get; private set; }

		public SpawnerInfo( Vector3 newPosition, Animator newAnimator, GameObject newAreaCoverObject )
		{
			position = newPosition;
			animator = newAnimator;
			areaCoverObject = newAreaCoverObject;
		}
	}

	private List<SpawnerInfo> spawners;

	public Vector3 beginCheckpointPosition = Vector3.zero;
	public Vector3 beginSpawnerPosition = Vector3.zero;
	public Animator beginSpawnerAnimator = null;
	public GameObject beginAreaCoverObject = null;

	private List<Vector3> checkpointPositions;
	private Vector3 spawnerPosition;
	private Animator spawnerAnimator;
	private GameObject areaCoverObject;

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

		spawners = new List<SpawnerInfo>();
		checkpointPositions = new List<Vector3>();
	}

	void Start()
	{
		AddCheckpointPosition( beginCheckpointPosition );
		AddSpawner( new SpawnerInfo( beginSpawnerPosition, beginSpawnerAnimator, beginAreaCoverObject ) );
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

	public static Vector3 GetNearestCheckpoint( Vector3 currentPosition )
	{
		if( instance )
			return instance.internalGetNearestCheckpoint( currentPosition );

		return Vector3.zero;
	}

	private Vector3 internalGetNearestCheckpoint( Vector3 currentPosition )
	{
		if( checkpointPositions.Count == 0 )
			return Vector3.zero;

		Vector3 nearestCheckpoint = checkpointPositions[0];

		for( int i = 1; i < checkpointPositions.Count; i++ )
			if( Vector3.Distance( currentPosition, checkpointPositions[i] ) < Vector3.Distance( currentPosition, checkpointPositions[i] ) )
				nearestCheckpoint = checkpointPositions[i];

		return nearestCheckpoint;
	}

	public static SpawnerInfo GetNearestSpawner( Vector3 currentPosition )
	{
		if( instance )
			return instance.internalGetNearestSpawner( currentPosition );

		return new SpawnerInfo( Vector3.zero, null, null );
	}

	private SpawnerInfo internalGetNearestSpawner( Vector3 currentPosition )
	{
		if( spawners.Count == 0 )
			return new SpawnerInfo( Vector3.zero, null, null );

		SpawnerInfo nearestSpawner = spawners[0];

		for( int i = 1; i < spawners.Count; i++ )
			if( Vector3.Distance( currentPosition, spawners[i].position ) < Vector3.Distance( currentPosition, nearestSpawner.position ) )
				nearestSpawner = spawners[i];

		return nearestSpawner;
	}

	public static void AddSpawner( SpawnerInfo newSpawner )
	{
		if( instance )
			instance.internalAddSpanwer( newSpawner );
	}

	private void internalAddSpanwer( SpawnerInfo newSpawner )
	{
		if( !spawners.Contains( newSpawner ) )
			spawners.Add( newSpawner );
	}
}
