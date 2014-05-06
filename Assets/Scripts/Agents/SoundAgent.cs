using UnityEngine;
using System;
using System.Collections;

public class SoundAgent : MonoBehaviour {
	
	public enum SoundEffects
	{
		Elevator = 0,
		PlayerJump = 1,
		PlayerTouchGround = 2,
		LaserSound = 3,
		CrusherSwoosh = 4,
		CrusherHit = 5,
		CrusherRise = 6,
		TeachSound = 7,
		DoorSound = 8,
		ButtonClick = 9,

		Invalid = 10,
	}
	
	public GameObject audioObject;
	
	public AudioClip ElevatorClip;
	public AudioClip PlayerJumpClip;
	public AudioClip PlayerTouchGroundClip;
	public AudioClip LaserClip;
	public AudioClip CrusherSwooshClip;
	public AudioClip CrusherHitClip;
	public AudioClip CrusherRiseClip;
	public AudioClip TeachClip;
	public AudioClip DoorClip;
	public AudioClip ButtonClip;

	public AudioClip BridgeRegionMusicClip;
	public AudioClip CryoRegionMusicClip;
	public AudioClip DiscoRegionMusicClip;
	public AudioClip EngineRegionMusicClip;
	public AudioClip HubRegionMusicClip;
	public AudioClip JanitorRegionMusicClip;
	public AudioClip SpaceRegionMusicClip;

	public float BridgeRegionVolume = 1f;
	public float CryoRegionVolume = 1f;
	public float DiscoRegionVolume = 1f;
	public float EngineRegionVolume = 1f;
	public float HubRegionVolume = 1f;
	public float JanitorRegionVolume = 1f;
	public float SpaceRegionVolume = 1f;

	private AudioSource BridgeRegionSource;
	private AudioSource CryoRegionSource;
	private AudioSource DiscoRegionSource;
	private AudioSource EngineRegionSource;
	private AudioSource HubRegionSource;
	private AudioSource JanitorRegionSource;
	private AudioSource SpaceRegionSource;

	private bool BridgeRegionSourceRequested = false;
	private bool CryoRegionSourceRequested = false;
	private bool DiscoRegionSourceRequested = false;
	private bool EngineRegionSourceRequested = false;
	private bool HubRegionSourceRequested = false;
	private bool JanitorRegionSourceRequested = false;
	private bool SpaceRegionSourceRequested = false;

	public float FadeSpeed = 0.25f;

	private int globalVolume;	
	private string globalVolumeString = "GlobalVolume";
	
	private static SoundAgent mInstance = null;
	public static SoundAgent instance
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
			Debug.LogError( string.Format( "Only one instance of SoundAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;
	}
	
	void Start()
	{
		if( audioObject == null )
			audioObject = Camera.main.gameObject;
		
		if( PlayerPrefs.HasKey( globalVolumeString ) )
		{
			globalVolume = PlayerPrefs.GetInt( globalVolumeString );
		}
		else
		{
			globalVolume = 1;
			updateGlobalVolumePref();	
		}

		BridgeRegionSource = audioObject.AddComponent<AudioSource>();
		BridgeRegionSource.loop = true;
		BridgeRegionSource.volume = 0f;
		BridgeRegionSource.clip = BridgeRegionMusicClip;

		CryoRegionSource = audioObject.AddComponent<AudioSource>();
		CryoRegionSource.loop = true;
		CryoRegionSource.volume = 0f;
		CryoRegionSource.clip = CryoRegionMusicClip;

		DiscoRegionSource = audioObject.AddComponent<AudioSource>();
		DiscoRegionSource.loop = true;
		DiscoRegionSource.volume = 0f;
		DiscoRegionSource.clip = DiscoRegionMusicClip;

		EngineRegionSource = audioObject.AddComponent<AudioSource>();
		EngineRegionSource.loop = true;
		EngineRegionSource.volume = 0f;
		EngineRegionSource.clip = EngineRegionMusicClip;

		HubRegionSource = audioObject.AddComponent<AudioSource>();
		HubRegionSource.loop = true;
		HubRegionSource.volume = 0f;
		HubRegionSource.clip = HubRegionMusicClip;

		JanitorRegionSource = audioObject.AddComponent<AudioSource>();
		JanitorRegionSource.loop = true;
		JanitorRegionSource.volume = 0f;
		JanitorRegionSource.clip = JanitorRegionMusicClip;

		SpaceRegionSource = audioObject.AddComponent<AudioSource>();
		SpaceRegionSource.loop = true;
		SpaceRegionSource.volume = 0f;
		SpaceRegionSource.clip = SpaceRegionMusicClip;

		PlayBackgroundMusic();
	}
	
	public static void PlayClip( SoundEffects soundEffect, float volume, bool shouldLoop = false, GameObject singleAudioObject = null )
	{
		if( instance )
			instance.internalPlayClip( soundEffect, volume, shouldLoop, singleAudioObject );
	}
	
	private void internalPlayClip( SoundEffects soundEffect, float volume, bool shouldLoop, GameObject singleAudioObject )
	{
		if( singleAudioObject != null )
			audioObject = singleAudioObject;

		if( audioObject == null )
			return;
		
		if( Vector3.Distance( audioObject.transform.position, Camera.main.transform.position ) > 20f )
			return;

		AudioSource audioSource = audioObject.AddComponent<AudioSource>();
		
		audioSource.loop = shouldLoop;
		audioSource.volume = volume;
		
		switch( soundEffect )
		{
			//add clips here
			case SoundEffects.Elevator: audioSource.clip = ElevatorClip; break;
			case SoundEffects.PlayerJump: audioSource.clip = PlayerJumpClip; break;
			case SoundEffects.PlayerTouchGround: audioSource.clip = PlayerTouchGroundClip; break;
			case SoundEffects.LaserSound: audioSource.clip = LaserClip; break;
			case SoundEffects.CrusherSwoosh: audioSource.clip = CrusherSwooshClip; break;
			case SoundEffects.CrusherHit: audioSource.clip = CrusherHitClip; break;
			case SoundEffects.CrusherRise: audioSource.clip = CrusherRiseClip; break;
			case SoundEffects.TeachSound: audioSource.clip = TeachClip; break;
			case SoundEffects.DoorSound: audioSource.clip = DoorClip; break;
			case SoundEffects.ButtonClick: audioSource.clip = ButtonClip; break;

		}
		
		
		if( audioSource.clip == null )
		{
			Destroy( audioSource );
			return;
		}
		
		audioSource.Play();

		if( !shouldLoop )
			StartCoroutine( DestroyOnFinish( audioSource, audioSource.clip.length ) );
	}
	
	public static void PlayBackgroundMusic( bool reset = false )
	{
		if( instance )
			instance.internalPlayBackgroundMusic( reset );
	}

	
	public static void PauseSounds()
	{
		if( instance )
			instance.internalPauseSounds();
	}
	
	private void internalPauseSounds()
	{
		AudioSource[] audioSources = audioObject.GetComponents<AudioSource>();
		
		for( int i = 0; i < audioSources.Length; i++ )
			audioSources[i].Pause();
	}
	
	public static void UnpauseSounds()
	{
		if( instance )
			instance.internalUnpauseSounds();
	}
	
	private void internalUnpauseSounds()
	{
		AudioSource[] audioSources = audioObject.GetComponents<AudioSource>();
		
		for( int i = 0; i < audioSources.Length; i++ )
			audioSources[i].Play();
	}
	
	public static bool isMuted()
	{
		if( instance )
			return instance.internalIsMuted();
		
		return false;
	}
	
	private bool internalIsMuted()
	{		
		return ( globalVolume == 0 );
	}
	
	private void internalPlayBackgroundMusic( bool reset )
	{
		if( reset )
		{
			BridgeRegionSource.time = 0f;
			CryoRegionSource.time = 0f;
			DiscoRegionSource.time = 0f;
			EngineRegionSource.time = 0f;
			HubRegionSource.time = 0f;
			JanitorRegionSource.time = 0f;
			SpaceRegionSource.time = 0f;
		}
		
		BridgeRegionSource.Play();
		CryoRegionSource.Play();
		DiscoRegionSource.Play();
		EngineRegionSource.Play();
		HubRegionSource.Play();
		JanitorRegionSource.Play();
		SpaceRegionSource.Play();
	}
	
	public static void PauseBackgroundMusic()
	{
		if( instance )
			instance.internalPauseBackgroundMusic();
	}
	
	private void internalPauseBackgroundMusic()
	{
		BridgeRegionSource.Pause();
		CryoRegionSource.Pause();
		DiscoRegionSource.Pause();
		EngineRegionSource.Pause();
		HubRegionSource.Pause();
		JanitorRegionSource.Pause();
		SpaceRegionSource.Pause();
	}
	
	private void updateGlobalVolumePref()
	{
		PlayerPrefs.SetInt( globalVolumeString, globalVolume );
	}
	
	private IEnumerator DestroyOnFinish( AudioSource source, float duration )
	{
		float currentTime = 0f;
		
		do
		{
			if( source.isPlaying )
				currentTime += Time.deltaTime;
			
			yield return null;
			
		} while( currentTime < duration );
		
		Destroy( source );
	}

	public static void FadeOutAllRegionMusic()
	{
		if( instance )
			instance.internalFadeOutAllRegionMusic();
	}

	private void internalFadeOutAllRegionMusic()
	{
		int numRegions = Enum.GetNames( typeof( RegionAgent.RegionType ) ).Length - 1;
		
		for( int i = 0; i < numRegions; i++ )
			FadeOutRegionMusic( (RegionAgent.RegionType)i );
	}

	public static void FadeOutRegionMusic( RegionAgent.RegionType region )
	{
		if( instance )
			instance.internalFadeOutRegionMusic( region );
	}

	private void internalFadeOutRegionMusic( RegionAgent.RegionType region )
	{
		StartCoroutine( RegionFade( region, false ) );
	}

	public static void FadeInRegionMusic( RegionAgent.RegionType region )
	{
		if( instance )
			instance.internalFadeInRegionMusic( region );
	}

	private void internalFadeInRegionMusic( RegionAgent.RegionType region )
	{
		StartCoroutine( RegionFade( region, true ) );
	}

	private IEnumerator RegionFade( RegionAgent.RegionType region, bool fadeIn )
	{
		AudioSource regionSource = null;
		float regionVolume = 0f;


		switch( region )
		{
			case RegionAgent.RegionType.BridgeRegion: regionSource = BridgeRegionSource; regionVolume = BridgeRegionVolume; BridgeRegionSourceRequested = true; break;
			case RegionAgent.RegionType.CryoRegion: regionSource = CryoRegionSource; regionVolume = CryoRegionVolume; CryoRegionSourceRequested = true; break;
			case RegionAgent.RegionType.DiscoRegion: regionSource = DiscoRegionSource; regionVolume = DiscoRegionVolume; DiscoRegionSourceRequested = true; break;
			case RegionAgent.RegionType.EngineRegion: regionSource = EngineRegionSource; regionVolume = EngineRegionVolume; EngineRegionSourceRequested = true; break;
			case RegionAgent.RegionType.HubRegion: regionSource = HubRegionSource; regionVolume = HubRegionVolume; HubRegionSourceRequested = true; break;
			case RegionAgent.RegionType.JanitorRegion: regionSource = JanitorRegionSource; regionVolume = JanitorRegionVolume; JanitorRegionSourceRequested = true; break;
			case RegionAgent.RegionType.SpaceRegion: regionSource = SpaceRegionSource; regionVolume = SpaceRegionVolume; SpaceRegionSourceRequested = true; break;
		} 

		//ensure this coroutine has sole control of region source
		yield return null;
		yield return null;

		switch( region )
		{
			case RegionAgent.RegionType.BridgeRegion: BridgeRegionSourceRequested = false; break;
			case RegionAgent.RegionType.CryoRegion: CryoRegionSourceRequested = false; break;
			case RegionAgent.RegionType.DiscoRegion: DiscoRegionSourceRequested = false; break;
			case RegionAgent.RegionType.EngineRegion: EngineRegionSourceRequested = false; break;
			case RegionAgent.RegionType.HubRegion: HubRegionSourceRequested = false; break;
			case RegionAgent.RegionType.JanitorRegion: JanitorRegionSourceRequested = false; break;
			case RegionAgent.RegionType.SpaceRegion: SpaceRegionSourceRequested = false; break;
		} 

		if( regionSource == null )
			yield break;

		float toValue = ( fadeIn ? regionVolume : 0f );
		float sign = ( fadeIn ? 1f : -1f );

		while( regionSource.volume != toValue )
		{
			regionSource.volume += FadeSpeed * Time.deltaTime * sign;
			regionSource.volume = Mathf.Clamp( regionSource.volume, 0f, regionVolume );

			bool regionRequested = false;

			switch( region )
			{
				case RegionAgent.RegionType.BridgeRegion: regionRequested = BridgeRegionSourceRequested; break;
				case RegionAgent.RegionType.CryoRegion: regionRequested = CryoRegionSourceRequested; break;
				case RegionAgent.RegionType.DiscoRegion: regionRequested = DiscoRegionSourceRequested; break;
				case RegionAgent.RegionType.EngineRegion: regionRequested = EngineRegionSourceRequested; break;
				case RegionAgent.RegionType.HubRegion: regionRequested = HubRegionSourceRequested; break;
				case RegionAgent.RegionType.JanitorRegion: regionRequested = JanitorRegionSourceRequested; break;
				case RegionAgent.RegionType.SpaceRegion: regionRequested = SpaceRegionSourceRequested; break;
			} 

			if( regionRequested )
				yield break;

			yield return null;
		}
	}
}
