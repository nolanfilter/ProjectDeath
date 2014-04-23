using UnityEngine;
using System;
using System.Collections;

public class RegionAgent : MonoBehaviour {

	public enum RegionType
	{
		BridgeRegion = 0,
		CryoRegion = 1,
		DiscoRegion = 2,
		EngineRegion = 3,
		HubRegion = 4,
		JanitorRegion = 5,
		SpaceRegion = 6,
		Invalid = 7,
	}

	public GameObject BridgeRegionCoverObject;
	public GameObject CryoRegionCoverObject;
	public GameObject DiscoRegionCoverObject;
	public GameObject EngineRegionCoverObject;
	public GameObject HubRegionCoverObject;
	public GameObject JanitorRegionCoverObject;
	public GameObject SpaceRegionCoverObject;

	private static RegionAgent mInstance = null;
	public static RegionAgent instance
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
			Debug.LogError( string.Format( "Only one instance of RegionAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;
	}
	
	public static GameObject GetRegionAreaCover( RegionType region )
	{
		if( instance )
			return instance.internalGetRegionAreaCover( region );

		return null;
	}

	private GameObject internalGetRegionAreaCover( RegionType region )
	{
		switch( region )
		{
			case RegionType.BridgeRegion: return BridgeRegionCoverObject;
			case RegionType.CryoRegion: return CryoRegionCoverObject;
			case RegionType.DiscoRegion: return DiscoRegionCoverObject;
			case RegionType.EngineRegion: return EngineRegionCoverObject;
			case RegionType.HubRegion: return HubRegionCoverObject;
			case RegionType.JanitorRegion: return JanitorRegionCoverObject;
			case RegionType.SpaceRegion: return SpaceRegionCoverObject;
		}

		return null;
	}

	public static void DarkenAllRegions()
	{
		if( instance )
			instance.internalDarkenAllRegions();
	}

	private void internalDarkenAllRegions()
	{
		int numRegions = Enum.GetNames( typeof( RegionType ) ).Length - 1;

		for( int i = 0; i < numRegions; i++ )
			DarkenRegion( (RegionType)i );
	}

	public static void DarkenRegion( RegionType region )
	{
		if( instance )
			instance.internalDarkenRegion( region );
	}

	private void internalDarkenRegion( RegionType region )
	{
		switch( region )
		{
			case RegionType.BridgeRegion: if( BridgeRegionCoverObject != null ) BridgeRegionCoverObject.GetComponent<AreaCoverControl>().on = true; break;
			case RegionType.CryoRegion: if( CryoRegionCoverObject != null ) CryoRegionCoverObject.GetComponent<AreaCoverControl>().on = true; break;
			case RegionType.DiscoRegion: if( DiscoRegionCoverObject != null ) DiscoRegionCoverObject.GetComponent<AreaCoverControl>().on = true; break;
			case RegionType.EngineRegion: if( EngineRegionCoverObject != null ) EngineRegionCoverObject.GetComponent<AreaCoverControl>().on = true; break;
			case RegionType.HubRegion: if( HubRegionCoverObject != null ) HubRegionCoverObject.GetComponent<AreaCoverControl>().on = true; break;
			case RegionType.JanitorRegion: if( JanitorRegionCoverObject != null ) JanitorRegionCoverObject.GetComponent<AreaCoverControl>().on = true; break;
			case RegionType.SpaceRegion: if( SpaceRegionCoverObject != null ) SpaceRegionCoverObject.GetComponent<AreaCoverControl>().on = true; break;
		}
	}

	public static void LightenRegion( RegionType region )
	{
		if( instance )
			instance.internalLightenRegion( region );
	}
	
	private void internalLightenRegion( RegionType region )
	{
		switch( region )
		{
			case RegionType.BridgeRegion: if( BridgeRegionCoverObject != null ) BridgeRegionCoverObject.GetComponent<AreaCoverControl>().on = false; break;
			case RegionType.CryoRegion: if( CryoRegionCoverObject != null ) CryoRegionCoverObject.GetComponent<AreaCoverControl>().on = false; break;
			case RegionType.DiscoRegion: if( DiscoRegionCoverObject != null ) DiscoRegionCoverObject.GetComponent<AreaCoverControl>().on = false; break;
			case RegionType.EngineRegion: if( EngineRegionCoverObject != null ) EngineRegionCoverObject.GetComponent<AreaCoverControl>().on = false; break;
			case RegionType.HubRegion: if( HubRegionCoverObject != null ) HubRegionCoverObject.GetComponent<AreaCoverControl>().on = false; break;
			case RegionType.JanitorRegion: if( JanitorRegionCoverObject != null ) JanitorRegionCoverObject.GetComponent<AreaCoverControl>().on = false; break;
			case RegionType.SpaceRegion: if( SpaceRegionCoverObject != null ) SpaceRegionCoverObject.GetComponent<AreaCoverControl>().on = false; break;
		}
	}
}
