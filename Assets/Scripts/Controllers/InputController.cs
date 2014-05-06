using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoBehaviour {

	public delegate void ButtonDownEventHandler( InputController.ButtonType button );
	public event ButtonDownEventHandler OnButtonDown;
	
	public delegate void ButtonHoldEventHandler( InputController.ButtonType button );
	public event ButtonHoldEventHandler OnButtonHeld;
	
	public delegate void ButtonUpEventHandler( InputController.ButtonType button );
	public event ButtonUpEventHandler OnButtonUp;

	public enum ButtonType
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3,
		FirstPower = 4,
		SecondPower = 5,
		ThirdPower = 6,
		FourthPower = 7,
		Start = 8,
		Sel = 9,
		Triggers = 10,
		Invalid = 11,
	}

	private string verticalAxisString;
	private string horizontalAxisString;
	
	private KeyCode[] codes = new KeyCode[ Enum.GetNames( typeof( ButtonType ) ).Length - 1 ];
	
	private Array buttonTypes = Enum.GetValues( typeof( ButtonType ) );
	
	private Dictionary<ButtonType, bool> currentButtonList;
	private Dictionary<ButtonType, bool> oldButtonList;
	
	void Start()
	{
		verticalAxisString = "Vertical";
		horizontalAxisString = "Horizontal";
		
		currentButtonList = new Dictionary<ButtonType, bool>();
		oldButtonList = new Dictionary<ButtonType, bool>();
		
		foreach( ButtonType button in buttonTypes )
		{
			currentButtonList.Add( button, false );
			oldButtonList.Add( button, false );
		}
	}

	void Update()
	{
		float verticalAxis = Input.GetAxisRaw( verticalAxisString );
		float horizontalAxis = Input.GetAxisRaw( horizontalAxisString );

		if( Input.GetJoystickNames().Length > 0 )
		{
			currentButtonList[ ButtonType.Up ] = verticalAxis > 0f;
			currentButtonList[ ButtonType.Right ] = horizontalAxis > 0f;
			currentButtonList[ ButtonType.Down ] = verticalAxis < 0f;
			currentButtonList[ ButtonType.Left ] = horizontalAxis < 0f;
			currentButtonList[ ButtonType.FirstPower ] = horizontalAxis < 0f;
			currentButtonList[ ButtonType.SecondPower ] = horizontalAxis > 0f;
			currentButtonList[ ButtonType.ThirdPower ] = Input.GetKey( KeyCode.JoystickButton1 ) || Input.GetKey( KeyCode.JoystickButton2 );
			currentButtonList[ ButtonType.FourthPower ] = Input.GetKey( KeyCode.JoystickButton0 ) || Input.GetKey( KeyCode.JoystickButton3 );
			currentButtonList[ ButtonType.Sel ] = Input.GetKey( KeyCode.JoystickButton8 );
			currentButtonList[ ButtonType.Start ] = Input.GetKey( KeyCode.JoystickButton9 );
			currentButtonList[ ButtonType.Triggers ] = Input.GetKey( KeyCode.JoystickButton4 ) && Input.GetKey( KeyCode.JoystickButton5 );
		}
		else
		{
			currentButtonList[ ButtonType.Up ] = Input.GetKey( KeyCode.UpArrow );
			currentButtonList[ ButtonType.Right ] = Input.GetKey( KeyCode.RightArrow );
			currentButtonList[ ButtonType.Down ] = Input.GetKey( KeyCode.DownArrow );
			currentButtonList[ ButtonType.Left ] = Input.GetKey( KeyCode.LeftArrow );
			currentButtonList[ ButtonType.FirstPower ] = Input.GetKey( KeyCode.LeftArrow );
			currentButtonList[ ButtonType.SecondPower ] = Input.GetKey( KeyCode.RightArrow );
			currentButtonList[ ButtonType.ThirdPower ] = Input.GetKey( KeyCode.Space );
			currentButtonList[ ButtonType.FourthPower ] = Input.GetKey( KeyCode.LeftShift );
			currentButtonList[ ButtonType.Sel ] = Input.GetKey( KeyCode.RightShift );
			currentButtonList[ ButtonType.Start ] = Input.GetKey( KeyCode.Return );
			currentButtonList[ ButtonType.Triggers ] = Input.GetKey( KeyCode.LeftCommand ) && Input.GetKey( KeyCode.RightCommand );
		}

		foreach( ButtonType button in buttonTypes )
		{
			if( currentButtonList[ button ] )
			{
				if( oldButtonList[ button ] )
					SendHeldEvent( button );
				else
					SendDownEvent( button );
			}
			else
			{
				if( oldButtonList[ button ] )
					SendUpEvent( button );
			}
			
			oldButtonList[ button ] = currentButtonList[ button ];
		}
	}
	
	private void SendDownEvent( ButtonType button )
	{						
		//Debug.Log( "Button: " + button );

		if( OnButtonDown != null )
			OnButtonDown( button );		
	}
	
	private void SendHeldEvent( ButtonType button )
	{
		if( OnButtonHeld != null )
			OnButtonHeld( button );	
	}
	
	private void SendUpEvent( ButtonType button )
	{
		if( OnButtonUp != null )
			OnButtonUp( button );	
	}
}
