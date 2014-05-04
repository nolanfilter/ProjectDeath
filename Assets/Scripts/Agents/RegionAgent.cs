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

	public float BridgeRegionVignetteAmount;
	public float CryoRegionVignetteAmount;
	public float DiscoRegionVignetteAmount;
	public float EngineRegionVignetteAmount;
	public float HubRegionVignetteAmount;
	public float JanitorRegionVignetteAmount;
	public float SpaceRegionVignetteAmount;

	public GameObject VignetteScreen;
	private VignetteControl vignetteControl = null;

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

	void Start()
	{
		if( VignetteScreen )
			vignetteControl = VignetteScreen.GetComponent<VignetteControl>();
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

	public static void SetVignetteAmount( RegionType region )
	{
		if( instance )
			instance.internalSetVignetteAmount( region ); 
	}

	private void internalSetVignetteAmount( RegionType region )
	{
		if( vignetteControl == null )
			return;

		Color newAmount = Color.white;

		switch( region )
		{
			case RegionType.BridgeRegion: newAmount.a = BridgeRegionVignetteAmount; break;
			case RegionType.CryoRegion: newAmount.a = CryoRegionVignetteAmount; break;
			case RegionType.DiscoRegion: newAmount.a = DiscoRegionVignetteAmount; break;
			case RegionType.EngineRegion: newAmount.a = EngineRegionVignetteAmount; break;
			case RegionType.HubRegion: newAmount.a = HubRegionVignetteAmount; break;
			case RegionType.JanitorRegion: newAmount.a = JanitorRegionVignetteAmount; break;
			case RegionType.SpaceRegion: newAmount.a = SpaceRegionVignetteAmount; break;
		}

		vignetteControl.amount = newAmount;
	}
}
