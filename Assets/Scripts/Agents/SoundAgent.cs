using UnityEngine;
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
	
	public AudioClip backgroundMusicClip;
	private AudioSource backgroundMusicSource;
	
	private float backgroundMusicVolume = 0.75f;
	
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
		
		backgroundMusicSource = audioObject.AddComponent<AudioSource>();
		backgroundMusicSource.loop = true;
		backgroundMusicSource.volume = backgroundMusicVolume * (float)globalVolume;
		backgroundMusicSource.clip = backgroundMusicClip;
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
	
	public static void MuteSounds()
	{
		if( instance )
			instance.setGlobalVolume( 0 );
	}
	
	public static void UnmuteSounds()
	{
		if( instance )
			instance.setGlobalVolume( 1 );
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
			if( audioSources[i] != backgroundMusicSource )
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
			if( audioSources[i] != backgroundMusicSource )
				audioSources[i].Play();
	}
	
	private void setGlobalVolume( int newVolume )
	{
		if( globalVolume == newVolume )
			return;
		
		globalVolume = newVolume;
		updateGlobalVolumePref();
		
		AudioSource[] audioSources = audioObject.GetComponents<AudioSource>();
		
		for( int i = 0; i < audioSources.Length; i++ )
			if( audioSources[i] != backgroundMusicSource )
				audioSources[i].volume = (float)globalVolume;
		
		if( backgroundMusicSource )
			backgroundMusicSource.volume = backgroundMusicVolume * (float)globalVolume;
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
		if( backgroundMusicClip == null || backgroundMusicSource.isPlaying )
			return;
		
		if( reset )
			backgroundMusicSource.time = 0f;
		
		backgroundMusicSource.Play();
	}
	
	public static void PauseBackgroundMusic()
	{
		if( instance )
			instance.internalPauseBackgroundMusic();
	}
	
	private void internalPauseBackgroundMusic()
	{
		if( backgroundMusicClip == null || backgroundMusicSource == null || !backgroundMusicSource.isPlaying )
			return;
		
		backgroundMusicSource.Pause();
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
}
