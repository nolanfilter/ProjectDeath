using UnityEngine;
using System.Collections;

public class SoundAgent : MonoBehaviour {
	
	public enum SoundEffects
	{
		
	}
	
	public GameObject audioObject;
	
	//clips go here
	
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
	
	public static void PlayClip( SoundEffects soundEffect )
	{
		if( instance )
			instance.internalPlayClip( soundEffect );
	}
	
	private void internalPlayClip( SoundEffects soundEffect )
	{
		if( audioObject == null )
			return;
		
		AudioSource audioSource = audioObject.AddComponent<AudioSource>();
		
		audioSource.loop = false;
		audioSource.volume = (float)globalVolume;
		
		switch( soundEffect )
		{
			//put clips here
		}
		
		
		if( audioSource.clip == null )
		{
			Destroy( audioSource );
			return;
		}
		
		audioSource.Play();
		
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
