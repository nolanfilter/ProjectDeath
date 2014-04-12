using UnityEngine;
using System;
using System.Collections;

public class SelectionScreenAgent : MonoBehaviour {
	
	public enum TextType
	{
		Ability1 = 0,
		Ability2 = 1,
		Ability3 = 2,
		Ability4 = 3,
		CurrentPower = 4,
		CurrentPowerPlus1 = 5,
		CurrentPowerPlus2 = 6,
		CurrentPowerMinus1 = 7,
		CurrentPowerMinus2 = 8,
		CurrentPowerMinus3 = 9,
		Invalid = 10,
	}

	public GameObject selectionScreenPrefab = null;
	private GameObject selectionScreenObject = null;
	private SelectionScreenController controller = null;

	private Color regularColor = new Color( 0.753f, 0.914f, 0.929f, 1f );
	private Color highlightColor = Color.white;

	private Camera selectionScreenCamera;

	private float offscreenYValue = 5.25f;
	private float onscreenYValue = 0f;
	private float zOffset = 10f;

	private float speed = 7f;

	private static SelectionScreenAgent mInstance = null;
	public static SelectionScreenAgent instance
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
			Debug.LogError( string.Format( "Only one instance of SelectionScreenAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;
	}

	void Start()
	{
		GameObject go = new GameObject();
		go.name = "SelectionScreenCamera";
		selectionScreenCamera = go.AddComponent<Camera>();
		selectionScreenCamera.clearFlags = CameraClearFlags.Depth;
		selectionScreenCamera.isOrthoGraphic = true;
		selectionScreenCamera.orthographicSize = 3;
		selectionScreenCamera.cullingMask = ( 1 << LayerMask.NameToLayer( "SelectionScreen" ) );

		if( selectionScreenPrefab )
		{
			selectionScreenObject = Instantiate( selectionScreenPrefab ) as GameObject;
			selectionScreenObject.transform.parent = go.transform;
			selectionScreenObject.transform.localPosition = Vector3.up * offscreenYValue + Vector3.forward * zOffset;
		}

		if( selectionScreenObject )
			controller = selectionScreenObject.GetComponent<SelectionScreenController>();

		SetArrow( TextType.Invalid );

		int length = Enum.GetNames( typeof( TextType ) ).Length - 1;

		for( int i = 0; i < length; i++ )
			SetText( (TextType)i, "" );

		RaiseScreen();
	}

	public static void SetText( TextType type, string newText )
	{
		if( instance && instance.enabled )
			instance.internalSetText( type, newText );
	}

	private void internalSetText( TextType type, string newText )
	{
		if( controller == null )
			return;

		switch( type )
		{
			case TextType.Ability1: if( controller.Ability1TextMesh ) controller.Ability1TextMesh.text = newText; break;
			case TextType.Ability2: if( controller.Ability2TextMesh ) controller.Ability2TextMesh.text = newText; break;
			case TextType.Ability3: if( controller.Ability3TextMesh ) controller.Ability3TextMesh.text = newText; break;
			case TextType.Ability4: if( controller.Ability4TextMesh ) controller.Ability4TextMesh.text = newText; break;
			case TextType.CurrentPower: if( controller.CurrentPowerTextMesh ) controller.CurrentPowerTextMesh.text = newText; break;
			case TextType.CurrentPowerPlus1: if( controller.CurrentPowerPlus1TextMesh ) controller.CurrentPowerPlus1TextMesh.text = newText; break;
			case TextType.CurrentPowerPlus2: if( controller.CurrentPowerPlus2TextMesh ) controller.CurrentPowerPlus2TextMesh.text = newText; break;
			case TextType.CurrentPowerMinus1: if( controller.CurrentPowerMinus1TextMesh ) controller.CurrentPowerMinus1TextMesh.text = newText; break;
			case TextType.CurrentPowerMinus2: if( controller.CurrentPowerMinus2TextMesh ) controller.CurrentPowerMinus2TextMesh.text = newText; break;
			case TextType.CurrentPowerMinus3: if( controller.CurrentPowerMinus3TextMesh ) controller.CurrentPowerMinus3TextMesh.text = newText; break;
		}
	}

	public static void HighlightText( TextType type )
	{
		if( instance && instance.enabled )
			instance.internalHighlightText( type );
	}

	private void internalHighlightText( TextType type )
	{
		if( controller.Ability1TextMesh )
			controller.Ability1TextMesh.color = ( type == TextType.Ability1 ) ? highlightColor : regularColor;
			
		if( controller.Ability2TextMesh )
			controller.Ability2TextMesh.color = ( type == TextType.Ability2 ) ? highlightColor : regularColor;

		if( controller.Ability3TextMesh )
			controller.Ability3TextMesh.color = ( type == TextType.Ability3 ) ? highlightColor : regularColor;

		if( controller.Ability4TextMesh )
			controller.Ability4TextMesh.color = ( type == TextType.Ability4 ) ? highlightColor : regularColor;
	}

	public static void SetArrow( TextType type )
	{
		if( instance && instance.enabled )
			instance.internalSetArrow( type );
	}

	private void internalSetArrow( TextType type )
	{
		if( controller.Arrow1 )
			controller.Arrow1.enabled = ( type == TextType.Ability1 );
		
		if( controller.Arrow2 )
			controller.Arrow2.enabled = ( type == TextType.Ability2 );
		
		if( controller.Arrow3 )
			controller.Arrow3.enabled = ( type == TextType.Ability3 );
		
		if( controller.Arrow4 )
			controller.Arrow4.enabled = ( type == TextType.Ability4 );

		if( controller.Highlight )
			controller.Highlight.enabled = ( type != TextType.Invalid );
	}

	public static void SetSlotSprite( int slotNumber, SelectionScreenController.SlotSprite slotSprite )
	{
		if( instance )
			instance.internalSetSlotSprite( slotNumber, slotSprite );
	}

	private void internalSetSlotSprite( int slotNumber, SelectionScreenController.SlotSprite slotSprite )
	{
		switch( slotNumber )
		{
			case 1:
			{
				for( int i = 0; i < controller.slot1Sprites.Length; i++ )
					controller.slot1Sprites[i].enabled = ( i == (int)slotSprite );
			} break;

			case 2:
			{
				for( int i = 0; i < controller.slot2Sprites.Length; i++ )
					controller.slot2Sprites[i].enabled = ( i == (int)slotSprite );
			} break;

			case 3:
			{
				for( int i = 0; i < controller.slot3Sprites.Length; i++ )
					controller.slot3Sprites[i].enabled = ( i == (int)slotSprite );
			} break;

			case 4:
			{
				for( int i = 0; i < controller.slot4Sprites.Length; i++ )
					controller.slot4Sprites[i].enabled = ( i == (int)slotSprite );
			} break;
		}
	}

	public static void RaiseScreen()
	{
		if( instance )
			instance.internalRaiseScreen();
	}

	private void internalRaiseScreen()
	{
		StopAllCoroutines();
		StartCoroutine( "DoRaiseScreen" );
	}

	private IEnumerator DoRaiseScreen()
	{
		if( selectionScreenObject.transform.localPosition.y == offscreenYValue )
			yield break;
		
		while( selectionScreenObject.transform.localPosition.y < ( offscreenYValue - speed * Time.deltaTime ) )
		{
			selectionScreenObject.transform.localPosition += Vector3.up * speed * Time.deltaTime;
			yield return null;
		}
		
		selectionScreenObject.transform.localPosition = Vector3.up * offscreenYValue + Vector3.forward * zOffset;
	}

	public static void LowerScreen()
	{
		if( instance )
			instance.internalLowerScreen();
	}

	private void internalLowerScreen()
	{
		StopAllCoroutines();
		StartCoroutine( "DoLowerScreen" );
	}

	private IEnumerator DoLowerScreen()
	{
		if( selectionScreenObject.transform.localPosition.y == onscreenYValue )
			yield break;
		
		while( selectionScreenObject.transform.localPosition.y > ( onscreenYValue + speed * Time.deltaTime ) )
		{
			selectionScreenObject.transform.localPosition -= Vector3.up * speed * Time.deltaTime;
			yield return null;
		}
		
		selectionScreenObject.transform.localPosition = Vector3.up * onscreenYValue + Vector3.forward * zOffset;
	}
}
