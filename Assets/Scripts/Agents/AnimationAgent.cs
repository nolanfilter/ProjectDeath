using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationAgent : MonoBehaviour {

	private List<Animator> animators;

	private static AnimationAgent mInstance = null;
	public static AnimationAgent instance
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
			Debug.LogError( string.Format( "Only one instance of AnimationAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;
		
		animators = new List<Animator>();
	}

	public static void RegisterAnimator( Animator animator )
	{
		if( instance )
			instance.internalRegisterAnimator( animator );
	}

	private void internalRegisterAnimator( Animator animator )
	{
		if( animator == null )
			return;

		if( !animators.Contains( animator ) )
			animators.Add( animator );
	}

	public static void UnregisterAnimator( Animator animator )
	{
		if( instance )
			instance.internalUnregisterAnimator( animator );
	}

	private void internalUnregisterAnimator( Animator animator )
	{
		if( animator == null )
			return;

		if( animators.Contains( animator ) )
			animators.Remove( animator );
	}

	public static void SetLeftBool( bool newValue )
	{
		if( instance )
			instance.internalSetLeftBool( newValue );
	}

	private void internalSetLeftBool( bool newValue )
	{
		int count = animators.Count;

		for( int i = 0; i < count; i++ )
			animators[i].SetBool( "Left", newValue ); 
	}

	public static void SetRightBool( bool newValue )
	{
		if( instance )
			instance.internalSetRightBool( newValue );
	}

	private void internalSetRightBool( bool newValue )
	{
		int count = animators.Count;
		
		for( int i = 0; i < count; i++ )
			animators[i].SetBool( "Right", newValue ); 
	}

	public static void SetJumpBool( bool newValue )
	{
		if( instance )
			instance.internalSetJumpBool( newValue );
	}

	private void internalSetJumpBool( bool newValue )
	{
		int count = animators.Count;
		
		for( int i = 0; i < count; i++ )
			animators[i].SetBool( "Jump", newValue ); 
	}

	public static void PrintStates()
	{
		if( instance )
			instance.internalPrintStates();
	}

	private void internalPrintStates()
	{
		int count = animators.Count;

		for( int i = 0; i < count; i++ )
			Debug.Log( animators[i].GetCurrentAnimatorStateInfo(0).nameHash );
	}

	public static bool IsAnimationPlaying( string name )
	{
		if( instance )
			return instance.internalIsAnimationPlaying( name );

		return false;
	}

	private bool internalIsAnimationPlaying( string name )
	{
		bool isPlaying = false;

		int count = animators.Count;
		
		for( int i = 0; i < count; i++ )
			if( animators[i].GetCurrentAnimatorStateInfo(0).IsName( name ) )
				isPlaying = true;

		return isPlaying;
	}
}
