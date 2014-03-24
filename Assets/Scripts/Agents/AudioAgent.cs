using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioAgent : MonoBehaviour
{
	public static float audioBlendTime = 0.3f;
	public static float audioBlendInterval = 0.01f;
	
	public static string useBackgroundMusicString = "UseBackgroundMusic";
	public static string useSFXString = "UseSFX";
	
	public enum AudioType
	{
		SoundEffect = 0,
		BackgroundMusic = 1,
		Invalid,
	}
	
	//Default audio layer volumes you can set in inspector - TO DO: write custom editor to expose layer names
	public float[] AudioLayerVolumes = new float[ Enum.GetNames( typeof( AudioType ) ).Length - 1 ];
	
	private static Dictionary< AudioType, float > InternalAudioLayerVolumes = new Dictionary< AudioType, float >();
	
	private static AudioAgent mInstance;
	public static AudioAgent instance
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
			Debug.LogError( "Only one instance of AudioAgent allowed. Destroying " + gameObject + " and leaving " + mInstance.gameObject );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;
		
		int numAudioLayers = AudioLayerVolumes.Length;
		
		for( int i = 0; i < numAudioLayers; i++ )
		{
			if( i == 0 )
			{
				if( PlayerPrefs.HasKey( useSFXString ) )
					AudioLayerVolumes[i] = (float)PlayerPrefs.GetInt( useSFXString );
				else
					AudioLayerVolumes[i] = Mathf.Clamp01( AudioLayerVolumes[i] );
			}
			else if( i == 1 )
			{
				if( PlayerPrefs.HasKey( useBackgroundMusicString ) )
					AudioLayerVolumes[i] = (float)PlayerPrefs.GetInt( useBackgroundMusicString );
				else
					AudioLayerVolumes[i] = Mathf.Clamp01( AudioLayerVolumes[i] );
			}
			
			AddAudioLayerVolume( (AudioType) i );
		}
	}
	
	public static void AudioCrossFade( AudioSource audioSource, AudioType audioType )
	{
		if( instance )
			instance.InternalAudioCrossFade( audioSource, audioType, audioBlendTime );
	}
	
	public static void AudioCrossFade( AudioSource audioSource, AudioType audioType, float crossFadeTime )
	{
		if( instance )
			instance.InternalAudioCrossFade( audioSource, audioType, crossFadeTime );
	}
	
	public void InternalAudioCrossFade( AudioSource audioSource, AudioType audioType, float crossFadeTime )
	{	
		StartCoroutine( PerformAudioCrossFade( audioSource, audioType, crossFadeTime ) );
	}
	
	private IEnumerator PerformAudioCrossFade( AudioSource audioSource, AudioType audioType, float crossFadeTime )
	{
		if( audioSource == null || audioSource.clip == null )
		{
			Debug.LogError( "No proper audio source found" );
			yield break;
		}
		
		if( audioType == AudioType.Invalid )
		{
			Debug.LogError( "Audio type is invalid" );
		}
		
		GameObject audioLayer = GetAudioLayer( audioType );
		
		if( audioRoot == null )
		{
			Debug.LogError( "No audio layer found" );
			yield break;
		}
		
		bool hasFromAudio = true;
		AudioSource fromAudio = GetLatestAudioSource( audioType );
		
		if( fromAudio == null || !fromAudio.isPlaying)
		{
			hasFromAudio = false;
		}
		
		AudioSource toAudio = audioLayer.AddComponent("AudioSource") as AudioSource;
		
		toAudio.clip = audioSource.clip;
		toAudio.loop = audioSource.loop;
		toAudio.time = audioSource.time;
		toAudio.volume = 0f;
		
		float beginTime = Time.time;
		
		float lerp;
		float currentTime; 
		
		toAudio.Play();
		
		float volume;
		
		do
		{
			currentTime = Time.time - beginTime;
			lerp = currentTime/crossFadeTime;
			
			volume = GetAudioLayerVolume( audioType );
			
			toAudio.volume = lerp * volume;
			
			//could be deleted by another audio crossfade
			if( fromAudio == null )
				hasFromAudio = false;
			
			if( hasFromAudio )
				fromAudio.volume = ( 1f - lerp ) * volume;
			
			yield return new WaitForSeconds( audioBlendInterval );
			
		} while( currentTime < crossFadeTime );
		
		toAudio.volume = 1f * volume;
		if( fromAudio != null )
			DestroyObject( fromAudio );
	}
	
	public static AudioSource GetLatestAudioSource( AudioType audioType )
	{
		GameObject audioLayer = GetAudioLayer( audioType );
		
		AudioSource[] audioSources = audioLayer.GetComponents<AudioSource>() as AudioSource[];
		
		if( audioSources.Length > 0 )
			return audioSources[ audioSources.Length - 1 ];
		
		return audioLayer.AddComponent("AudioSource") as AudioSource;
	}
	
	private static GameObject GetAudioLayer( AudioType audioType )
	{
		if( audioType == AudioType.Invalid )
			return null;
		
		string audioLayerName = audioType.ToString() + "Layer";
		
		GameObject audioLayer;
		
		Transform audioLayerTransform = audioRoot.transform.Find( audioLayerName );
		
		if( audioLayerTransform == null )
			audioLayer = AddAudioLayer( audioLayerName );
		else
			audioLayer = audioLayerTransform.gameObject;
		
		return audioLayer;
	}
	
	private static GameObject AddAudioLayer( string name )
	{
		GameObject audioLayer = new GameObject( name );
		audioLayer.transform.parent = audioRoot.transform;
		audioLayer.transform.localPosition = Vector3.zero;
		audioLayer.transform.localRotation = Quaternion.identity;
		audioLayer.transform.localScale = Vector3.one;
		
		return audioLayer;
	}
	
	public static void MuteAllAudio( bool mute )
	{
		var keys = new List<AudioType>( InternalAudioLayerVolumes.Keys );
		
		foreach( var key in keys )
			SetAudioLayerVolume( key , ( mute ) ? 0f : instance.AudioLayerVolumes[ (int) key ] );
	}
	
	public static float GetAudioLayerVolume( AudioType audioType )
	{
		if( audioType == AudioType.Invalid )
			return -1f;
		
		if( !InternalAudioLayerVolumes.ContainsKey( audioType ) )
		{
			Debug.LogError( "Cannot find audio layer volume for " + audioType );
			return -1f;
		}
		
		return InternalAudioLayerVolumes[ audioType ];
	}
	
	public static void SetAudioLayerVolume( AudioType audioType, float volume )
	{
		if( audioType == AudioType.Invalid )
			return;
		
		float currentVolume = GetAudioLayerVolume( audioType );
		
		if( currentVolume == -1f || currentVolume == volume )
			return;
		
		InternalAudioLayerVolumes[ audioType ] = volume;
		
		GameObject audioLayer = GetAudioLayer( audioType );
		
		if( audioLayer != null )
		{
			AudioSource[] audioSources = audioLayer.GetComponents<AudioSource>() as AudioSource[];
			
			foreach( AudioSource audioSource in audioSources )
				audioSource.volume = volume;
		}
	}
	
	private static void AddAudioLayerVolume( AudioType audioType )
	{
		if( !InternalAudioLayerVolumes.ContainsKey( audioType ) )
			InternalAudioLayerVolumes.Add( audioType, instance.AudioLayerVolumes[ (int)audioType ] );
	}
	
	public static GameObject audioRoot
	{
		get
		{
			return Camera.mainCamera.gameObject;
		}
	}
}
