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
		Invalid = 10,
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
		
		//hardcoded for PS3 controller, PS4 controller, and PC
		if( Input.GetJoystickNames().Length > 0 )
		{	
			switch( Input.GetJoystickNames()[0] )
			{
				/*
				//NES
				case " USB Gamepad ":
				{
					Debug.Log( "Set to NES" );

					codes[ (int)ButtonType.Up ] = (KeyCode)( (int)KeyCode.Joystick1Button0 + 20 );
					codes[ (int)ButtonType.Down ] = (KeyCode)( (int)KeyCode.Joystick1Button1 + 20 );
					codes[ (int)ButtonType.Left ] = (KeyCode)( (int)KeyCode.Joystick1Button2 + 20 );
					codes[ (int)ButtonType.Right ] = (KeyCode)( (int)KeyCode.Joystick1Button3 + 20 );
					codes[ (int)ButtonType.Sel ] = (KeyCode)( (int)KeyCode.Joystick1Button4 + 20 );
					codes[ (int)ButtonType.Start ] = (KeyCode)( (int)KeyCode.Joystick1Button5 + 20 );
					codes[ (int)ButtonType.Jump ] = (KeyCode)( (int)KeyCode.Joystick1Button6 + 20 );
					codes[ (int)ButtonType.Jump2 ] = (KeyCode)( (int)KeyCode.Joystick1Button7 + 20 );
					codes[ (int)ButtonType.Action ] = (KeyCode)( (int)KeyCode.Joystick1Button8 + 20 );
					codes[ (int)ButtonType.Action2 ] = (KeyCode)( (int)KeyCode.Joystick1Button9 + 20 );
				} break;

				//PS3
				case "Sony PLAYSTATION(R)3 Controller":
				{
					codes[ (int)ButtonType.Up ] = (KeyCode)( (int)KeyCode.Joystick1Button4 + 20 );
					codes[ (int)ButtonType.Down ] = (KeyCode)( (int)KeyCode.Joystick1Button6 + 20 );
					codes[ (int)ButtonType.Left ] = (KeyCode)( (int)KeyCode.Joystick1Button7 + 20 );
					codes[ (int)ButtonType.Right ] = (KeyCode)( (int)KeyCode.Joystick1Button5 + 20 );
					codes[ (int)ButtonType.Sel ] = (KeyCode)( (int)KeyCode.Joystick1Button0 + 20 );
					codes[ (int)ButtonType.Start ] = (KeyCode)( (int)KeyCode.Joystick1Button3 + 20 );
					codes[ (int)ButtonType.Jump ] = (KeyCode)( (int)KeyCode.Joystick1Button14 + 20 );
					codes[ (int)ButtonType.Jump2 ] = (KeyCode)( (int)KeyCode.Joystick1Button12 + 20 );
					codes[ (int)ButtonType.Action ] = (KeyCode)( (int)KeyCode.Joystick1Button13 + 20 );
					codes[ (int)ButtonType.Action2 ] = (KeyCode)( (int)KeyCode.Joystick1Button15 + 20 );
				} break;
				
				//PS4
				case "Sony Computer Entertainment Wireless Controller":
				{
					codes[ (int)ButtonType.Up ] = (KeyCode)( (int)KeyCode.Joystick1Button16 + 20 );
					codes[ (int)ButtonType.Down ] = (KeyCode)( (int)KeyCode.Joystick1Button17 + 20 );
					codes[ (int)ButtonType.Left ] = (KeyCode)( (int)KeyCode.Joystick1Button18 + 20 );
					codes[ (int)ButtonType.Right ] = (KeyCode)( (int)KeyCode.Joystick1Button19 + 20 );
					codes[ (int)ButtonType.Sel ] = (KeyCode)( (int)KeyCode.Joystick1Button9 + 20 );
					codes[ (int)ButtonType.Start ] = (KeyCode)( (int)KeyCode.Joystick1Button13 + 20 );
					codes[ (int)ButtonType.Jump ] = (KeyCode)( (int)KeyCode.Joystick1Button1 + 20 );
					codes[ (int)ButtonType.Jump2 ] = (KeyCode)( (int)KeyCode.Joystick1Button3 + 20 );
					codes[ (int)ButtonType.Action ] = (KeyCode)( (int)KeyCode.Joystick1Button2 + 20 );
					codes[ (int)ButtonType.Action2 ] = (KeyCode)( (int)KeyCode.Joystick1Button0 + 20 );
				} break;
				*/

				//SNES
				case " 2Axes 11Keys Game  Pad":
				{
					Debug.Log( "Set to SNES" );

					codes[ (int)ButtonType.Up ] = (KeyCode)( (int)KeyCode.Joystick1Button0 + 20 );
					codes[ (int)ButtonType.Down ] = (KeyCode)( (int)KeyCode.Joystick1Button1 + 20 );
					codes[ (int)ButtonType.Left ] = (KeyCode)( (int)KeyCode.Joystick1Button2 + 20 );
					codes[ (int)ButtonType.Right ] = (KeyCode)( (int)KeyCode.Joystick1Button3 + 20 );
					codes[ (int)ButtonType.FirstPower ] = (KeyCode)( (int)KeyCode.Joystick1Button2 + 20 );
					codes[ (int)ButtonType.SecondPower ] = (KeyCode)( (int)KeyCode.Joystick1Button19 + 20 );
					codes[ (int)ButtonType.ThirdPower ] = (KeyCode)( (int)KeyCode.Joystick1Button19 + 20 );
					codes[ (int)ButtonType.FourthPower ] = (KeyCode)( (int)KeyCode.Joystick1Button19 + 20 );
					codes[ (int)ButtonType.Sel ] = (KeyCode)( (int)KeyCode.Joystick1Button4 + 20 );
					codes[ (int)ButtonType.Start ] = (KeyCode)( (int)KeyCode.Joystick1Button5 + 20 );
				} break;

				/*
				//XBOX 360
				case "":
				{
					codes[ (int)ButtonType.Up ] = (KeyCode)( (int)KeyCode.Joystick1Button5 + 20 );
					codes[ (int)ButtonType.Down ] = (KeyCode)( (int)KeyCode.Joystick1Button6 + 20 );
					codes[ (int)ButtonType.Left ] = (KeyCode)( (int)KeyCode.Joystick1Button7 + 20 );
					codes[ (int)ButtonType.Right ] = (KeyCode)( (int)KeyCode.Joystick1Button8 + 20 );
					codes[ (int)ButtonType.Sel ] = (KeyCode)( (int)KeyCode.Joystick1Button10 + 20 );
					codes[ (int)ButtonType.Start ] = (KeyCode)( (int)KeyCode.Joystick1Button9 + 20 );
					codes[ (int)ButtonType.Jump ] = (KeyCode)( (int)KeyCode.Joystick1Button16 + 20 );
					codes[ (int)ButtonType.Jump2 ] = (KeyCode)( (int)KeyCode.Joystick1Button19 + 20 );
					codes[ (int)ButtonType.Action ] = (KeyCode)( (int)KeyCode.Joystick1Button17 + 20 );
					codes[ (int)ButtonType.Action2 ] = (KeyCode)( (int)KeyCode.Joystick1Button18 + 20 );
				} break;
				*/
			}
		}
		else
		{
			codes[ (int)ButtonType.Up ] = KeyCode.UpArrow;
			codes[ (int)ButtonType.Down ] = KeyCode.DownArrow;
			codes[ (int)ButtonType.Left ] = KeyCode.LeftArrow;
			codes[ (int)ButtonType.Right ] = KeyCode.RightArrow;
			codes[ (int)ButtonType.FirstPower ] = KeyCode.LeftArrow;
			codes[ (int)ButtonType.SecondPower ] = KeyCode.RightArrow;
			codes[ (int)ButtonType.ThirdPower ] = KeyCode.Space;
			codes[ (int)ButtonType.FourthPower ] = KeyCode.LeftShift;
			codes[ (int)ButtonType.Sel ] = KeyCode.RightShift;
			codes[ (int)ButtonType.Start ] = KeyCode.Return;
		}
		//end hardcoded
		
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
		
		currentButtonList[ ButtonType.Up ] = verticalAxis > 0f;
		currentButtonList[ ButtonType.Right ] = horizontalAxis > 0f;
		currentButtonList[ ButtonType.Down ] = verticalAxis < 0f;
		currentButtonList[ ButtonType.Left ] = horizontalAxis < 0f;
		
		for( int i = 0; i < codes.Length; i++ )
		{
			if( i == (int)ButtonType.Up || i == (int)ButtonType.Right || i == (int)ButtonType.Down || i == (int)ButtonType.Left )
				currentButtonList[ (ButtonType)i ] = currentButtonList[ (ButtonType)i ] || Input.GetKey( codes[ i ] );
			//else if( i == (int)ButtonType.Action2 )
			//	currentButtonList[ ButtonType.Action ] = currentButtonList[ ButtonType.Action ] || Input.GetKey( codes[ i ] );
			//else if( i == (int)ButtonType.Jump2 )
			//	currentButtonList[ ButtonType.Jump ] = currentButtonList[ ButtonType.Jump ] || Input.GetKey( codes[ i ] );
			else
				currentButtonList[ (ButtonType)i ] = Input.GetKey( codes[ i ] );


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
