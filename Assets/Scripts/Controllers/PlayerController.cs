using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	private SpriteRenderer[] spriteRenderers;

	private GUIStyle textStyle;

	private CharacterController controller;
	private InputController inputController;

	private float speed = 2.5f;
	private float turningSpeedPercent = 0.4f;
	private float setSpeed = 2.5f;
	private float conveyorSpeedWith = 3.75f;
	private float conveyorSpeedAgainst = 1.25f;

	private float gravity = -2.45f;
	private Vector3 gravityVector;
	private float maxVelocity = 0.0625f;

	private RaycastHit hit;
	private Ray ray;
	private Ray leftRay;
	private Ray rightRay;
	private float leftHeight;
	private float rightHeight;
	private float slopeAngle = 0.5f;
	
	private Vector3 movementVector;

	private bool isFalling;
	private bool wasFalling;
	private float fallBeginTime;
	private float maxFallDuration = 5f;

	private float heading;
	private float startingHeading = 90f;
	private float deltaAngle = 1f;
	private bool applyHeading;

	private bool isJumping;
	private float jumpCoolDown = 0.05f;

	private bool isDashing;
	private float dashCoolDown = 0.05f;

	private bool isFacingRight;
	private bool isMobile;

	private int maxNumRoutines = 4;
	private bool canChooseLoadout = false;
	private int currentlySelectedRoutine;
	private bool displayAllRoutines;
	private int currentlySelectedNewRoutine;
	private bool isSelecting = false;

	private Dictionary<InputController.ButtonType, string> actions;
	private Dictionary<InputController.ButtonType, string> repeatableActions;
	private List<RoutineAgent.RoutineInfo> foundRoutines;

	private string deathTag = "Death";
	private float deathDuration = 0.5f;
	private float relocationDuration = 0.5f;
	private float respawnDuration = 0.25f;

	private string[] currentActions;
	private Rect[] actionRects;
	private List<Rect> allRoutinesRects;

	private bool hasChosenLoadout = false;

	void Awake()
	{
		controller = GetComponent<CharacterController>();
		
		if( controller == null )
		{
			Debug.LogError( "No character controller on " + gameObject.name );
			enabled = false;
			return;
		}

		inputController = GetComponent<InputController>();
		
		if( inputController == null )
		{
			Debug.LogError( "No input controller on " + gameObject.name );
			enabled = false;
			return;
		}

		actions = new Dictionary<InputController.ButtonType, string>();
		repeatableActions = new Dictionary<InputController.ButtonType, string>();
		foundRoutines = new List<RoutineAgent.RoutineInfo>();

		allRoutinesRects = new List<Rect>();
	}

	void Start()
	{	
		currentActions = new string[ maxNumRoutines ];
		actionRects = new Rect[ maxNumRoutines ];

		for( int i = 0; i < maxNumRoutines; i++ )
		{
			currentActions[i] = "";
			actionRects[i] = new Rect( 5f, 20f * (float)i, 100f, 20f );
		}

		isMobile = true;

		textStyle = FontAgent.GetTextStyle();

		spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

		respawn();
	}

	void OnEnable()
	{
		inputController.OnButtonDown += OnButtonDown;
		inputController.OnButtonHeld += OnButtonHeld;
		inputController.OnButtonUp += OnButtonUp;
	}
	
	void OnDisable()
	{
		inputController.OnButtonDown -= OnButtonDown;
		inputController.OnButtonHeld -= OnButtonHeld;
		inputController.OnButtonUp -= OnButtonUp;
	}

	void OnTriggerEnter( Collider collider )
	{
		if( !isMobile )
			return;

		if( collider.tag == deathTag )
		{
			StopAllCoroutines();
			StartCoroutine( "DeathRoutine" );
			return;
		}

		TeachingController[] teachers = collider.gameObject.GetComponents<TeachingController>();

		if( teachers.Length > 0 )
		{
			for( int i = 0; i < teachers.Length; i++ )
				AddRoutine( teachers[i].GetRoutineInfo() );

			Destroy( collider.gameObject );
		}

		CheckpointController checkpoint = collider.gameObject.GetComponent<CheckpointController>();

		if( checkpoint != null )
		{
			SpawnerAgent.SetSpawnerPosition( checkpoint.checkpointPosition );
			Destroy( checkpoint.gameObject );
		}
	}
	/*
	void OnTriggerStay( Collider collider )
	{
		if( collider.tag == "ConveyorLeft" )
			movementVector += Vector3.left * speed * 0.25f * Time.deltaTime;
	}
	*/
	void Update()
	{	
		applyGravity();

		if( movementVector.x > 0f )
			isFacingRight = true;
		else if( movementVector.x < 0f )
			isFacingRight = false;

		if( applyHeading )
		{
			float radian = heading * Mathf.Deg2Rad;

			Vector3 headingVector = new Vector3( Mathf.Cos( radian ), Mathf.Sin( radian ), 0f );

			headingVector *= speed * Time.deltaTime * 1.5f;

			movementVector += headingVector;
		}

		isFalling = !applyHeading && ( movementVector.y == 0f || ( ( gravityVector.y < 0f && movementVector.y < 0f ) || ( gravityVector.y > 0f && movementVector.y > 0f ) ) );

		if( isFalling )
		{
			if( !wasFalling )
			{
				fallBeginTime = Time.time;
				wasFalling = true;
			}
		}
		else
		{
			wasFalling = false;
		}

		controller.Move( movementVector );
		
		movementVector = Vector3.zero;
		applyHeading = false;

		if( isGrounded() )
		{
			heading = Mathf.Lerp( heading, startingHeading, 0.5f );
			fallBeginTime = Time.time;
		}
	}

	void OnGUI()
	{	
		int index = 0;

		for( int i = 0; i < maxNumRoutines; i++ )
		{
			string displayString = currentActions[i];

			if( isSelecting && canChooseLoadout && currentlySelectedRoutine == i )
				displayString = "*" + displayString;

			GUI.Label( actionRects[i], displayString, textStyle );
		}

		if( displayAllRoutines )
		{
			for( int i = 0; i < allRoutinesRects.Count; i++ )
			{
				string displayString = foundRoutines[i].functionName;

				if( currentlySelectedNewRoutine == i )
					displayString = "*" + displayString;
				
				GUI.Label( allRoutinesRects[i], displayString, textStyle );
			}
		}
	}

	//event handlers
	private void OnButtonDown( InputController.ButtonType button )
	{
		if( actions.ContainsKey( button ) )
			StartCoroutine( actions[ button ] );

		if( !isMobile )
			return;

		switch( button )
		{
			case InputController.ButtonType.Left: if( CurrentActionsContains( "MoveLeft" ) ) AnimationAgent.SetLeftBool( true ); break;
			case InputController.ButtonType.Right: if( CurrentActionsContains( "MoveRight" ) ) AnimationAgent.SetRightBool( true ); break;
			case InputController.ButtonType.Jump: if( CurrentActionsContains( "Jump" ) ) AnimationAgent.SetJumpBool( true ); break;
			//case InputController.ButtonType.Sel: AnimationAgent.PrintStates(); break;
		}
	}
	
	private void OnButtonHeld( InputController.ButtonType button )
	{	
		if( repeatableActions.ContainsKey( button ) )
			StartCoroutine( repeatableActions[ button ] );

		if( !isMobile )
			return;

		switch( button )
		{
			case InputController.ButtonType.Left: if( CurrentActionsContains( "MoveLeft" ) ) AnimationAgent.SetLeftBool( true ); break;
			case InputController.ButtonType.Right: if( CurrentActionsContains( "MoveRight" ) ) AnimationAgent.SetRightBool( true ); break;
			case InputController.ButtonType.Jump: if( CurrentActionsContains( "Jump" ) ) AnimationAgent.SetJumpBool( true ); break;
		}
	}
	
	private void OnButtonUp( InputController.ButtonType button )
	{
		if( !isMobile )
			return;

		switch( button )
		{
			case InputController.ButtonType.Left: if( CurrentActionsContains( "MoveLeft" ) ) AnimationAgent.SetLeftBool( false ); break;
			case InputController.ButtonType.Right: if( CurrentActionsContains( "MoveRight" ) ) AnimationAgent.SetRightBool( false ); break;
			case InputController.ButtonType.Jump: if( CurrentActionsContains( "Jump" ) ) AnimationAgent.SetJumpBool( false ); break;
		}

	}
	//end event handlers

	//routines
	private IEnumerator Activate()
	{
		if( isSelecting )
		{
			if( displayAllRoutines )
				SetNewRoutine();
			else
				hasChosenLoadout = true;
		}

		yield break;
	}

	private IEnumerator SelectUp()
	{
		if( isSelecting )
		{
			if( displayAllRoutines )
			{
				currentlySelectedNewRoutine--;
				if( currentlySelectedNewRoutine < 0 )
					currentlySelectedNewRoutine = 0;
			}
			else
			{
				currentlySelectedRoutine--;
				if( currentlySelectedRoutine < 0 )
					currentlySelectedRoutine = 0;
			}
			
			yield break;
		}
	}

	private IEnumerator SelectDown()
	{
		if( isSelecting )
		{
			if( displayAllRoutines )
			{
				currentlySelectedNewRoutine++;
				if( currentlySelectedNewRoutine >= foundRoutines.Count )
					currentlySelectedNewRoutine = foundRoutines.Count - 1;
			}
			else
			{
				currentlySelectedRoutine++;
				if( currentlySelectedRoutine >= maxNumRoutines )
					currentlySelectedRoutine = maxNumRoutines - 1;
			}
			
			yield break;
		}
	}

	private IEnumerator SelectLeft()
	{
		if( isSelecting )
		{
			if( displayAllRoutines )
				SetNewRoutine();
			
			yield break;
		}
	}

	private IEnumerator SelectRight()
	{
		if( isSelecting )
		{
			if( !displayAllRoutines )
				currentlySelectedNewRoutine = 0;
			
			displayAllRoutines = true;
			
			yield break;
		}
	}

	private IEnumerator MoveLeft()
	{
		if( !isMobile )
			yield break;

		if( !isJumping && AnimationAgent.IsAnimationPlaying( "TurnLeft" ) )
			movementVector += Vector3.left * speed * turningSpeedPercent * Time.deltaTime;
		else
			movementVector += Vector3.left * speed * Time.deltaTime;

		slopeCorrection();

		yield break;
	}

	private IEnumerator MoveRight()
	{
		if( !isMobile )
			yield break;

		if( !isJumping && AnimationAgent.IsAnimationPlaying( "TurnRight" ) )
			movementVector += Vector3.right * speed * turningSpeedPercent * Time.deltaTime;
		else
			movementVector += Vector3.right * speed * Time.deltaTime;

		slopeCorrection();

		yield break;
	}

	private IEnumerator Jump()
	{
		if( isJumping || !isGrounded() )
			yield break;

		isJumping = true;

		yield return StartCoroutine( MovementOverTime( gravityVector.normalized * -1f, 10f, 0.5f ) );

		while( !isGrounded() )
			yield return null;

		AnimationAgent.SetJumpBool( false );

		yield return new WaitForSeconds( jumpCoolDown );

		isJumping = false;
	}

	private IEnumerator Dash()
	{
		if( isDashing || !isMobile )
			yield break;

		isDashing = true;

		yield return StartCoroutine( MovementOverTime( ( isFacingRight ? Vector3.right : Vector3.left ), 10f, 0.5f ) );

//		while( !isGrounded() )
//			yield return null;

		yield return new WaitForSeconds( dashCoolDown );

		isDashing = false;
	}

	private IEnumerator GravityShift()
	{
		if( !isGrounded() || !isMobile )
			yield break;

		gravityVector *= -1f;	
	}

	private IEnumerator JetLeft()
	{
		if( !isMobile )
			yield break;

		heading -= deltaAngle;
		applyHeading = true;

		yield break;
	}

	private IEnumerator JetRight()
	{
		if( !isMobile )
			yield break;

		heading += deltaAngle;
		applyHeading = true;

		yield break;
	}
	//end routines

	private bool isGrounded()
	{
		ray = new Ray( transform.position + controller.center, gravityVector );
			
		Physics.Raycast( ray, out hit, controller.height * 0.6f, 1 << 8 );

		if( hit.collider != null && !hit.collider.isTrigger )
			return true;
		else
			return false;
	}

	private void respawn()
	{
		gravityVector = transform.up * gravity;

		isJumping = false;
		isDashing = false;
		isFacingRight = true;
		wasFalling = true;
		fallBeginTime = Time.time;

		heading = startingHeading;
		applyHeading = false;

		transform.position = SpawnerAgent.GetSpawnerPosition();
	}

	private void applyGravity()
	{		
		if( !isGrounded() && isMobile )
		{
			if( isFalling )
				movementVector += gravityVector * ( Time.deltaTime + Mathf.Clamp01( ( Time.time - fallBeginTime ) / maxFallDuration ) * maxVelocity );
			else
				movementVector += gravityVector * Time.deltaTime;
		}
	}

	private void slopeCorrection()
	{
		if( !isGrounded() || isJumping )
			return;

		leftRay = new Ray( transform.position + Vector3.left * transform.localScale.x * 0.5f, gravityVector );

		Physics.Raycast( leftRay, out hit );
		leftHeight = hit.distance;

		rightRay = new Ray( transform.position + Vector3.right * transform.localScale.x * 0.5f, gravityVector );

		Physics.Raycast( rightRay, out hit );
		rightHeight = hit.distance;

		float heightDifference = Mathf.Abs( leftHeight - rightHeight );

		if( heightDifference > 0.01f && heightDifference < slopeAngle )
			movementVector += gravityVector * Time.deltaTime;
	}

	private void AddRoutine( RoutineAgent.RoutineInfo routineInfo )
	{
		if( foundRoutines.Contains( routineInfo ) )
			return;

		foundRoutines.Add( routineInfo );

		allRoutinesRects.Add( new Rect( 85f, 20f * (float)( foundRoutines.Count - 1 ), 100f, 20f ) );

		if( foundRoutines.Count > maxNumRoutines )
		{
			if( !canChooseLoadout )
			{
				//hardcoded
				actions.Add( InputController.ButtonType.Action, "Activate" );
				actions.Add( InputController.ButtonType.Up, "SelectUp" );
				actions.Add( InputController.ButtonType.Down, "SelectDown" );
				actions.Add( InputController.ButtonType.Left2, "SelectLeft" );
				actions.Add( InputController.ButtonType.Right2, "SelectRight" );

				canChooseLoadout = true;
			}

			return;
		}

		if( actions.ContainsKey( routineInfo.button ) )
			actions[ routineInfo.button ] = routineInfo.functionName;
		else
			actions.Add( routineInfo.button, routineInfo.functionName );

		if( routineInfo.isRepeatableAction )
		{
			if( repeatableActions.ContainsKey( routineInfo.button ) )
				repeatableActions[ routineInfo.button ] = routineInfo.functionName;
			else
				repeatableActions.Add( routineInfo.button, routineInfo.functionName );
		}

		currentActions[ foundRoutines.Count - 1 ] = routineInfo.functionName;
	}

	private bool CurrentActionsContains( string functionName )
	{
		bool contains = false;

		for( int i = 0; i < maxNumRoutines; i++ )
			if( currentActions[i] == functionName )
				contains = true;

		return contains;
	}

	private IEnumerator MovementOverTime( Vector3 directionVector, float force, float duration )	
	{
		float beginTime = Time.time;
		float currentTime;
		float lerp;
		
		do
		{
			currentTime = Time.time - beginTime;
			lerp = 1f - ( currentTime / duration );
			
			Vector3 newMovement = directionVector * force * lerp * Time.deltaTime;
			
			movementVector += newMovement;
			
			yield return null;
			
		} while( currentTime < duration );	
	}

	private IEnumerator DeathRoutine()
	{
		isMobile = false;

		for( int i = 0; i < spriteRenderers.Length; i++ )
			spriteRenderers[ i ].enabled = false;

		AnimationAgent.SetLeftBool( false );
		AnimationAgent.SetRightBool( false );
		AnimationAgent.SetJumpBool( false );

		BodyAgent.SpawnBody( transform.position );

		yield return new WaitForSeconds( deathDuration );

		float beginTime = Time.time;
		float currentTime;
		float lerp;

		Vector3 beginPosition = transform.position;
		Vector3 endPosition = SpawnerAgent.GetSpawnerPosition();

		do
		{
			currentTime = Time.time - beginTime;
			lerp = currentTime / relocationDuration;

			transform.position = Vector3.Lerp( beginPosition, endPosition, lerp );

			yield return null;

		} while( currentTime < relocationDuration );

		isSelecting = true;

		currentlySelectedRoutine = 0;

		if( canChooseLoadout )
			while( !hasChosenLoadout )
				yield return null;

		isSelecting = false;

		respawn();

		yield return new WaitForSeconds( respawnDuration );

		for( int i = 0; i < spriteRenderers.Length; i++ )
			spriteRenderers[ i ].enabled = true;

		isMobile = true;

		hasChosenLoadout = false;
	}

	private void SetNewRoutine()
	{
		string oldFunctionName = currentActions[ currentlySelectedRoutine ];
		string newFunctionName = foundRoutines[ currentlySelectedNewRoutine ].functionName;

		if( oldFunctionName == newFunctionName )
		{
			displayAllRoutines = false;
			return;
		}

		int swapToLocation = -1;

		for( int i = 0; i < maxNumRoutines; i++ )
		{
			if( currentActions[i] == newFunctionName )
				swapToLocation = i;
		}

		if( swapToLocation != -1 )
		{
			string temp = currentActions[ swapToLocation ];
			currentActions[ swapToLocation ] = currentActions[ currentlySelectedRoutine ];
			currentActions[ currentlySelectedRoutine ] = temp;
		}
		else
		{
			RoutineAgent.RoutineInfo temp = foundRoutines[ currentlySelectedNewRoutine ];
				
			RemoveByValue( actions, oldFunctionName );
			RemoveByValue( repeatableActions, oldFunctionName );

			actions.Add( temp.button, temp.functionName );

			if( temp.isRepeatableAction )
				repeatableActions.Add( temp.button, temp.functionName );

			currentActions[ currentlySelectedRoutine ] = newFunctionName;
		}

		displayAllRoutines = false;
	}
			                                   
	private void RemoveByValue<TKey,TValue>( Dictionary<TKey, TValue> dictionary, TValue value )
	{
		List<TKey> itemsToRemove = new List<TKey>();
		
		foreach( var pair in dictionary )
		   if( pair.Value.Equals( value ) )
				itemsToRemove.Add( pair.Key );
		
		foreach( TKey item in itemsToRemove )
			dictionary.Remove(item);
	}

	public void AddExternalMovementForce (Vector3 direction) {
		movementVector += direction;
	}
}
