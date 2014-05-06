using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public bool teachMoveRightOnStart;
	public bool teachJumpOnStart;
	public bool teachDashOnStart;
	public bool teachGravityShiftOnStart;
	public bool teachThermostatOnStart;
	public bool teachLaserShieldOnStart;

	public SpriteRenderer[] constantSpriteRenderers;
	
	public SpriteRenderer ExhaustSpriteRenderer;
	public SpriteRenderer GravFlipSpriteRenderer;
	public SpriteRenderer JumperCablesSpriteRenderer;
	public SpriteRenderer MagnetSpriteRenderer;
	public SpriteRenderer RocketSpriteRenderer;
	public SpriteRenderer ShieldSpriteRenderer;
	public SpriteRenderer ShieldEffectSpriteRenderer;
	public SpriteRenderer ThermostatSpriteRenderer;
	
	public GameObject ExhaustEffectObject;
	public GameObject RocketEffectObject;
	public GameObject ShieldEffectObject;
	
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
	
	private float additionalForce;
	
	private bool isJumping;
	private float jumpForce = 8f;
	private float jumpDuration = 0.5f;
	private float maxJumpTime = 0.1f;
	private float jumpBeginTime;
	private float addedJumpForce = 6.5f;
	private float jumpCoolDown = 0.05f;
	private bool isHoldingJump = false;
	
	private bool isDashing;
	private float dashForce = 7f;
	private float dashDuration = 0.5f;
	private float dashCoolDown = 0.3f;
	
	private bool isFacingRight;
	private bool isMobile;
	private bool isGrounded;
	private bool isMoving = false;
	
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
	private float deathDuration = 1;
	private float relocationDuration = 1.5f;
	private float respawnDuration = 1f;
	private BodyAgent.DeathType lastDeathType;

	private string[] currentActions;
	private Rect[] actionRects;
	private List<Rect> allRoutinesRects;
	
	private bool hasChosenLoadout = false;
	
	private Transform activePlatform;
	private Vector3 activeLocalPlatformPoint;
	private Vector3 activeGlobalPlatformPoint;
	private Vector3 lastPlatformVelocity;
	
	private Quaternion activeLocalPlatformRotation;
	private Quaternion activeGlobalPlatformRotation;
	
	private RoutineAgent.Routine lastLearnedRoutine = RoutineAgent.Routine.Invalid;
	
	private float temperature;
	private float maxTemperature = 1f;
	//private float temperatureDecayRate = 0.2f;

	private float shieldPower;
	private float beginShieldPower = 5f;
	private float shieldPowerDecayRate = 1f;
	private float shieldPowerGrowthRate = 2f;

	private float selfDestructMeter;
	private float selfDestructDecayRate = 0.5f;
	
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

		AddRoutine( new RoutineAgent.RoutineInfo( InputController.ButtonType.Invalid, RoutineAgent.Routine.MoveLeft, true ) );

		//hardcoded self destruct
		actions.Add( InputController.ButtonType.Triggers, "SelfDestruct" );
		repeatableActions.Add( InputController.ButtonType.Triggers, "SelfDestruct" );
		//end hardcoded

		if( teachMoveRightOnStart )
			AddRoutine( new RoutineAgent.RoutineInfo( InputController.ButtonType.Invalid, RoutineAgent.Routine.MoveRight, true ) );

		if( teachJumpOnStart )
			AddRoutine( new RoutineAgent.RoutineInfo( InputController.ButtonType.Invalid, RoutineAgent.Routine.Jump, true ) );

		if( teachDashOnStart )
			AddRoutine( new RoutineAgent.RoutineInfo( InputController.ButtonType.Invalid, RoutineAgent.Routine.Dash, false ) );

		if( teachGravityShiftOnStart )
			AddRoutine( new RoutineAgent.RoutineInfo( InputController.ButtonType.Invalid, RoutineAgent.Routine.GravShift, false ) );

		if( teachThermostatOnStart )
			AddRoutine( new RoutineAgent.RoutineInfo( InputController.ButtonType.Invalid, RoutineAgent.Routine.Thermostat, false ) );

		if( teachLaserShieldOnStart )
			AddRoutine( new RoutineAgent.RoutineInfo( InputController.ButtonType.Invalid, RoutineAgent.Routine.LaserShield, true ) );

		UpdateSprites();
		
		respawn();

		UpdateRegion();
	}
	
	void OnEnable()
	{
		inputController.OnButtonDown += OnButtonDown;
		inputController.OnButtonHeld += OnButtonHeld;
		inputController.OnButtonUp += OnButtonUp;

		PlayerAgent.RegisterPlayerController( this );
	}
	
	void OnDisable()
	{
		PlayerAgent.UnregisterPlayerController();

		inputController.OnButtonDown -= OnButtonDown;
		inputController.OnButtonHeld -= OnButtonHeld;
		inputController.OnButtonUp -= OnButtonUp;
	}
	
	void OnTriggerEnter( Collider collider )
	{
		ResolveTrigger( collider );
	}

	void OnTriggerStay( Collider collider )
	{
		ResolveTrigger( collider );
	}

	private void ResolveTrigger( Collider collider )
	{
		if( !isMobile )
			return;
		
		if( collider.tag == deathTag )
		{
			StopAllCoroutines();
			
			DeathZoneController deathZoneController = collider.GetComponent<DeathZoneController>();
			
			if( deathZoneController )
				lastDeathType = deathZoneController.deathZoneType;
		
			if( lastDeathType == BodyAgent.DeathType.Laser && ShieldEffectSpriteRenderer && ShieldEffectSpriteRenderer.enabled && shieldPower > 0f  )
				return;
			
			StartCoroutine( "DeathRoutine", false );
			
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
			SpawnerAgent.AddCheckpointPosition( checkpoint.checkpointPosition );
			SpawnerAgent.AddSpawner( new SpawnerAgent.SpawnerInfo( checkpoint.spawnerPosition, checkpoint.GetAnimator(), checkpoint.region ) );
			
			Destroy( checkpoint.gameObject );
		}
	}
	
	void OnControllerColliderHit( ControllerColliderHit hit )
	{
		if( hit.moveDirection.y < -0.9f && hit.normal.y > 0.5f )
			activePlatform = hit.collider.transform;
	}
	
	void Update()
	{	
		if( activePlatform != null )
		{
			Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint( activeLocalPlatformPoint );
			Vector3 moveDistance = newGlobalPlatformPoint - activeGlobalPlatformPoint;
			if( moveDistance != Vector3.zero )
				controller.Move( moveDistance );
			lastPlatformVelocity = ( newGlobalPlatformPoint - activeGlobalPlatformPoint ) / Time.deltaTime;
			
			Quaternion newGlobalPlatformRotation = activePlatform.rotation * activeLocalPlatformRotation;
			Quaternion rotationDiff = newGlobalPlatformRotation * Quaternion.Inverse( activeGlobalPlatformRotation );
			
			rotationDiff = Quaternion.FromToRotation( rotationDiff * transform.up, transform.up ) * rotationDiff;
			
			transform.rotation = rotationDiff * transform.rotation;
		}
		else
		{
			lastPlatformVelocity = Vector3.zero;
		}
		
		activePlatform = null;
		
		applyGravity();
		
		if( movementVector.x > 0f )
			isFacingRight = true;
		else if( movementVector.x < 0f )
			isFacingRight = false;

		if( ExhaustEffectObject )
			ExhaustEffectObject.transform.eulerAngles = Vector3.up * ( isFacingRight ? 270f : 90f );
		
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

		if( controller.enabled )
			controller.Move( movementVector );
		
		movementVector = Vector3.zero;
		applyHeading = false;
		
		if( isGrounded )
		{
			heading = Mathf.Lerp( heading, startingHeading, 0.5f );
			fallBeginTime = Time.time;
		}
		
		if( activePlatform != null )
		{
			activeGlobalPlatformPoint = transform.position;
			activeLocalPlatformPoint = activePlatform.InverseTransformPoint( transform.position );
			
			activeGlobalPlatformRotation = transform.rotation;
			activeLocalPlatformRotation = Quaternion.Inverse( activePlatform.rotation ) * transform.rotation;
		}
	}
	
	void FixedUpdate()
	{
		ray = new Ray( transform.position + controller.center, gravityVector );
		
		Physics.Raycast( ray, out hit, controller.height * 0.6f, 1 << 8 );
		
		bool wasGrounded;
		
		wasGrounded = isGrounded;
		
		isGrounded = ( ( hit.collider != null && !hit.collider.isTrigger ) || activePlatform != null );
		
		if( isMobile && isGrounded && !wasGrounded ) {
			SoundAgent.PlayClip(SoundAgent.SoundEffects.PlayerTouchGround,.15f, false, gameObject);
		}
	}
	
	//event handlers
	private void OnButtonDown( InputController.ButtonType button )
	{
		string functionName = "";
		
		if( actions.ContainsKey( button ) )
		{
			functionName = actions[ button ];
			StartCoroutine( functionName );
		}
		
		if( !isMobile )
			return;
		
		switch( functionName )
		{
			case "MoveLeft": if( CurrentActionsContains( "MoveLeft" ) ) AnimationAgent.SetLeftBool( true ); break;
			case "MoveRight": if( CurrentActionsContains( "MoveRight" ) ) AnimationAgent.SetRightBool( true ); break;
			case "Jump": if( CurrentActionsContains( "Jump" ) ) AnimationAgent.SetJumpBool( true ); break;
		}
	}
	
	private void OnButtonHeld( InputController.ButtonType button )
	{	
		string functionName = "";
		
		if( repeatableActions.ContainsKey( button ) )
		{
			functionName = repeatableActions[ button ];
			StartCoroutine( functionName );
		}
		
		if( !isMobile )
			return;
		
		switch( functionName )
		{
			case "MoveLeft": if( CurrentActionsContains( "MoveLeft" ) ) AnimationAgent.SetLeftBool( true ); break;
			case "MoveRight": if( CurrentActionsContains( "MoveRight" ) ) AnimationAgent.SetRightBool( true ); break;
			case "Jump": if( CurrentActionsContains( "Jump" ) ) AnimationAgent.SetJumpBool( true ); break;
		}
	}
	
	private void OnButtonUp( InputController.ButtonType button )
	{
		string functionName = "";
		
		if( actions.ContainsKey( button ) )
			functionName = actions[ button ];
		
		if( functionName == "Jump" )
			isHoldingJump = false;

		if( functionName == "LaserShield" )
			StartCoroutine( "ShieldPowerGrowthRoutine" );

		if( functionName == "SelfDestruct" )
			selfDestructMeter = 1f;
		
		if( !isMobile )
			return;
		
		switch( functionName )
		{
			case "MoveLeft": if( CurrentActionsContains( "MoveLeft" ) ) AnimationAgent.SetLeftBool( false ); break;
			case "MoveRight": if( CurrentActionsContains( "MoveRight" ) ) AnimationAgent.SetRightBool( false ); break;
			case "Jump": if( CurrentActionsContains( "Jump" ) ) AnimationAgent.SetJumpBool( false ); break;
		}
		
	}
	//end event handlers
	
	//routines
	private IEnumerator Activate()
	{
		if( isSelecting )
		{
			if( displayAllRoutines )
			{
				SetNewRoutine();
				
				SelectionScreenAgent.SetArrow( SelectionScreenAgent.TextType.Invalid );
				UpdateSelectionScreen();
			}
			else
			{
				hasChosenLoadout = true;
			}
		}
		
		yield break;
	}
	
	private IEnumerator SelectUp()
	{
		if( isSelecting )
		{
			if( displayAllRoutines )
			{
				currentlySelectedNewRoutine = ( currentlySelectedNewRoutine + ( foundRoutines.Count - 1 ) )%foundRoutines.Count;
			}
			else
			{
				currentlySelectedRoutine--;
				if( currentlySelectedRoutine < 0 )
					currentlySelectedRoutine = 0;
				
				SelectionScreenAgent.HighlightText( (SelectionScreenAgent.TextType)currentlySelectedRoutine );
			}
			
			UpdateSelectionScreen();
			
			yield break;
		}
	}
	
	private IEnumerator SelectDown()
	{
		if( isSelecting )
		{
			if( displayAllRoutines )
			{
				currentlySelectedNewRoutine = ( currentlySelectedNewRoutine + 1 )%foundRoutines.Count;
			}
			else
			{
				currentlySelectedRoutine++;
				if( currentlySelectedRoutine >= maxNumRoutines )
					currentlySelectedRoutine = maxNumRoutines - 1;
				
				SelectionScreenAgent.HighlightText( (SelectionScreenAgent.TextType)currentlySelectedRoutine );
			}
			
			UpdateSelectionScreen();
			
			yield break;
		}
	}
	
	private IEnumerator SelectLeft()
	{
		if( isSelecting )
		{
			if( displayAllRoutines )
				SetNewRoutine();
			
			SelectionScreenAgent.SetArrow( SelectionScreenAgent.TextType.Invalid );
			UpdateSelectionScreen();
			
			yield break;
		}
	}
	
	private IEnumerator SelectRight()
	{
		if( isSelecting )
		{
			if( !displayAllRoutines )
			{
				for( int i = 0; i < foundRoutines.Count; i++ )
					if( foundRoutines[i].functionName.ToString() == currentActions[ currentlySelectedRoutine ] )
						currentlySelectedNewRoutine = i;
			}
			
			displayAllRoutines = true;
			
			SelectionScreenAgent.SetArrow( (SelectionScreenAgent.TextType)currentlySelectedRoutine );
			
			UpdateSelectionScreen();
			
			yield break;
		}
	}

	private IEnumerator SelfDestruct()
	{
		if( !isMobile )
			yield break;

		selfDestructMeter -= selfDestructDecayRate * Time.deltaTime;

		if( selfDestructMeter <= 0f )
			StartCoroutine( "DeathRoutine", true );
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
		if( !isJumping ) 
		{
			if( !isGrounded )
				yield break;
			SoundAgent.PlayClip (SoundAgent.SoundEffects.PlayerJump, .08f, false, gameObject);
			isJumping = true;
			additionalForce = 0;
			jumpBeginTime = Time.time + maxJumpTime;
			isHoldingJump = true;
			
			StartCoroutine( "WaitAndSetJumpBeginTime" );
		}
		else
		{
			if( isHoldingJump )
				additionalForce = Mathf.Lerp( 0f, addedJumpForce * ( gravityVector.y < 0f ? 1f : -1f ), Mathf.Clamp01( ( Time.time - jumpBeginTime ) / maxJumpTime ) );
			
			yield break;
		}
		
		yield return StartCoroutine( MovementOverTime( gravityVector.normalized * -1f, jumpForce, jumpDuration, true ) );
		
		additionalForce = 0f;
		
		while( !isGrounded )
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

		if( ExhaustEffectObject )
		{
			if( ExhaustEffectObject.activeSelf )
				ExhaustEffectObject.SetActive( false );

			ExhaustEffectObject.SetActive( true );
		}
		
		yield return StartCoroutine( MovementOverTime( ( isFacingRight ? Vector3.right : Vector3.left ), dashForce, dashDuration ) );

		speed *= 0.5f;
		
		yield return new WaitForSeconds( dashCoolDown );
		
		speed = setSpeed;

		isDashing = false;
	}
	
	private IEnumerator GravShift()
	{
		if( !isGrounded || !isMobile )
			yield break;
		
		gravityVector *= -1f;	
	}
	
	private IEnumerator Thermostat()
	{
		if( !isMobile )
			yield break;

		AdjustTemperature( 0f );
	}

	private IEnumerator LaserShield()
	{
		if( !isMobile || shieldPower == 0f )
			yield break;

		StopCoroutine( "ShieldPowerGrowthRoutine" );

		shieldPower -= shieldPowerDecayRate * Time.deltaTime;

		if( shieldPower <= 0f )
			shieldPower = 0f;

		if( ShieldEffectObject )
			ShieldEffectObject.SetActive( true );

		if( ShieldEffectSpriteRenderer )
		{
			Color temp = ShieldEffectSpriteRenderer.color;
			
			if( shieldPower == 0f )
				temp.a = 0f;
			else
				temp.a = Mathf.Lerp( 1f, 0.25f, ( beginShieldPower - shieldPower ) / beginShieldPower );
			
			ShieldEffectSpriteRenderer.color = temp;
			
			ShieldEffectSpriteRenderer.enabled = true;
		}
	}
	//end routines
	
	private void respawn()
	{
		gravityVector = transform.up * gravity;
		
		isJumping = false;
		isDashing = false;
		isFacingRight = true;
		wasFalling = true;
		fallBeginTime = Time.time;
		speed = setSpeed;
		
		heading = startingHeading;
		applyHeading = false;
		
		transform.position = SpawnerAgent.GetNearestCheckpoint( transform.position );
		
		lastLearnedRoutine = RoutineAgent.Routine.Invalid;
		lastDeathType = BodyAgent.DeathType.Invalid;

		AdjustTemperature( 0f );

		shieldPower = beginShieldPower;
		selfDestructMeter = 1f;

		ShieldEffectSpriteRenderer.enabled = false;
	}
	
	private void applyGravity()
	{		
		if( isMobile )
		{
			if( isFalling )
				movementVector += gravityVector * ( Time.deltaTime + Mathf.Clamp01( ( Time.time - fallBeginTime ) / maxFallDuration ) * maxVelocity );
			else
				movementVector += gravityVector * Time.deltaTime;
		}
	}
	
	private void slopeCorrection()
	{
		if( !isGrounded || isJumping )
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
		
		lastLearnedRoutine = routineInfo.functionName;
		
		if( foundRoutines.Count > maxNumRoutines )
		{
			if( !canChooseLoadout )
			{
				//hardcoded
				actions.Add( InputController.ButtonType.Start, "Activate" );
				actions.Add( InputController.ButtonType.Up, "SelectUp" );
				actions.Add( InputController.ButtonType.Down, "SelectDown" );
				actions.Add( InputController.ButtonType.Left, "SelectLeft" );
				actions.Add( InputController.ButtonType.Right, "SelectRight" );
				
				canChooseLoadout = true;
			}
			
			return;
		}
		
		InputController.ButtonType button = GetButtonFromSlot( foundRoutines.Count );
		
		if( actions.ContainsKey( button ) )
			actions[ button ] = routineInfo.functionName.ToString();
		else
			actions.Add( button, routineInfo.functionName.ToString() );
		
		if( routineInfo.isRepeatableAction )
		{
			if( repeatableActions.ContainsKey( button ) )
				repeatableActions[ button ] = routineInfo.functionName.ToString();
			else
				repeatableActions.Add( button, routineInfo.functionName.ToString() );
		}
		
		currentActions[ foundRoutines.Count - 1 ] = routineInfo.functionName.ToString();
	}
	
	private bool CurrentActionsContains( string functionName )
	{
		bool contains = false;
		
		for( int i = 0; i < maxNumRoutines; i++ )
			if( currentActions[i] == functionName )
				contains = true;
		
		return contains;
	}
	
	private InputController.ButtonType GetButtonFromSlot( int slotIndex )
	{
		switch( slotIndex )
		{
		case 1: return InputController.ButtonType.FirstPower; break;
		case 2: return InputController.ButtonType.SecondPower; break;
		case 3: return InputController.ButtonType.ThirdPower; break;
		case 4: return InputController.ButtonType.FourthPower; break;
		}
		
		return InputController.ButtonType.Invalid;
	}
	
	private IEnumerator MovementOverTime( Vector3 directionVector, float force, float duration, bool useAdditionalForce = false )	
	{
		float beginTime = Time.time;
		float currentTime;
		float lerp;
		
		do
		{
			currentTime = Time.time - beginTime;
			lerp = 1f - ( currentTime / duration );
			
			Vector3 newMovement;
			
			if( useAdditionalForce )
				newMovement = directionVector * ( force + additionalForce ) * lerp * Time.deltaTime;
			else
				newMovement = directionVector * force * lerp * Time.deltaTime;
			
			movementVector += newMovement;
			
			yield return null;
			
		} while( currentTime < duration );	
	}
	
	private IEnumerator WaitAndSetJumpBeginTime()
	{
		yield return new WaitForSeconds( 0.05f );
		
		jumpBeginTime = Time.time;
	}

	private IEnumerator ShieldPowerGrowthRoutine()
	{
		if( ShieldEffectObject )
			ShieldEffectObject.SetActive( false );

		if( ShieldEffectSpriteRenderer )
			ShieldEffectSpriteRenderer.enabled = false;

		while( shieldPower < beginShieldPower )
		{
			shieldPower += shieldPowerGrowthRate * Time.deltaTime;
			yield return null;
		}

		shieldPower = beginShieldPower;
	}
	
	private IEnumerator DeathRoutine( bool didSelfDestruct )
	{
		isMobile = false;

		controller.enabled = false;
		
		for( int i = 0; i < constantSpriteRenderers.Length; i++ )
			constantSpriteRenderers[ i ].enabled = false;
		
		UpdateSprites( false );
		
		AnimationAgent.SetLeftBool( false );
		AnimationAgent.SetRightBool( false );
		AnimationAgent.SetJumpBool( false );

		Animator deathAnimationAnimator = null;

		if( didSelfDestruct )
			deathAnimationAnimator = BodyAgent.SpawnBody( transform.position, isFacingRight, BodyAgent.DeathType.Invalid );
		else
			deathAnimationAnimator = BodyAgent.SpawnBody( transform.position, isFacingRight, lastDeathType );
	
		yield return new WaitForSeconds( 0.15f );

		if( deathAnimationAnimator != null && !deathAnimationAnimator.GetCurrentAnimatorStateInfo(0).IsName( "Start" ) )
		{
			while( !deathAnimationAnimator.GetCurrentAnimatorStateInfo(0).IsName( "IsDone" ) )
				yield return null;
		}
		
		yield return new WaitForSeconds( deathDuration );

		BodyAgent.DestroyBodies();

		UpdateRegion();
		
		isMoving = true;
		
		float beginTime;
		float currentTime;
		float lerp;
		
		Vector3 beginPosition;
		Vector3 endPosition;
		
		if( lastLearnedRoutine != RoutineAgent.Routine.Invalid )
		{
			beginTime = Time.time;
			
			beginPosition = transform.position;
			endPosition = SpawnerAgent.GetNearestSpawner( transform.position ).position;
			
			do
			{
				currentTime = Time.time - beginTime;
				lerp = currentTime / relocationDuration;
				
				transform.position = Vector3.Lerp( beginPosition, endPosition, lerp );
				
				yield return null;
				
			} while( currentTime < relocationDuration );
			
			transform.position = endPosition;
			
			
			Animator spawnerAnimator = SpawnerAgent.GetNearestSpawner( transform.position ).animator;
			
			if( spawnerAnimator != null )
			{			
				spawnerAnimator.SetInteger( "routine", (int)lastLearnedRoutine );
				
				yield return new WaitForSeconds( 0.15f );
				
				spawnerAnimator.SetInteger( "routine", -1 );
				
				while( !spawnerAnimator.GetCurrentAnimatorStateInfo(0).IsName( "Blinking" ) )
					yield return null;
			}
			
			yield return new WaitForSeconds( 1f );
		}
		
		beginTime = Time.time;
		
		beginPosition = transform.position;
		endPosition = SpawnerAgent.GetNearestCheckpoint( transform.position );
		
		do
		{
			currentTime = Time.time - beginTime;
			lerp = currentTime / relocationDuration;
			
			transform.position = Vector3.Lerp( beginPosition, endPosition, lerp );
			
			yield return null;
			
		} while( currentTime < relocationDuration );
		
		isMoving = false;
		
		
		isSelecting = true;
		
		currentlySelectedRoutine = 0;
		
		SelectionScreenAgent.HighlightText( (SelectionScreenAgent.TextType)currentlySelectedRoutine );
		UpdateSelectionScreen();
		
		if( canChooseLoadout )
		{
			SelectionScreenAgent.LowerScreen();
			
			while( !hasChosenLoadout )
				yield return null;
			
			SelectionScreenAgent.RaiseScreen();
		}
		
		isSelecting = false;
		
		SelectionScreenAgent.HighlightText( SelectionScreenAgent.TextType.Invalid );

		GameObject spawnerTube = SpawnerAgent.GetSpawnerTube();
		Animator spawnerTubeAnimator = null;

		if( spawnerTube != null )
		{
			spawnerTubeAnimator = spawnerTube.GetComponent<Animator>();

			if( spawnerTubeAnimator != null )
			{
				spawnerTube.transform.position = transform.position;
				spawnerTube.renderer.enabled = true;

				spawnerTubeAnimator.Play( "Respawn" );

				yield return new WaitForSeconds( 0.5f );
			}
		}

		respawn();
		
		yield return new WaitForSeconds( respawnDuration );

		for( int i = 0; i < constantSpriteRenderers.Length; i++ )
			constantSpriteRenderers[i].enabled = true;
		
		UpdateSprites();

		controller.enabled = true;

		isMobile = true;
		
		hasChosenLoadout = false;

		if( spawnerTube != null )
		{
			if( spawnerTubeAnimator != null  )
			{
				while( !spawnerTubeAnimator.GetCurrentAnimatorStateInfo(0).IsName( "IsDone" ) )
					yield return null;
			}
			
			spawnerTube.renderer.enabled = false;
		}
	}
	
	private void SetNewRoutine()
	{
		string oldFunctionName = currentActions[ currentlySelectedRoutine ];
		string newFunctionName = foundRoutines[ currentlySelectedNewRoutine ].functionName.ToString();
		
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
			
			InputController.ButtonType oldButton = GetButtonFromSlot( swapToLocation + 1 );
			InputController.ButtonType newButton = GetButtonFromSlot( currentlySelectedRoutine + 1 );
			
			actions[ oldButton ] = oldFunctionName;
			actions[ newButton ] = newFunctionName;
			
			if( repeatableActions.ContainsKey( oldButton ) && repeatableActions.ContainsKey( newButton ) )
			{
				temp = repeatableActions[ oldButton ];
				repeatableActions[ oldButton ] = repeatableActions[ newButton ];
				repeatableActions[ newButton ] = temp;
			}
			else
			{
				if( repeatableActions.ContainsKey( oldButton ) )
				{
					temp = repeatableActions[ oldButton ];
					repeatableActions.Remove( oldButton );
					repeatableActions.Add( newButton, temp );
				}
				else if( repeatableActions.ContainsKey( newButton ) )
				{
					temp = repeatableActions[ newButton ];
					repeatableActions.Remove( newButton );
					repeatableActions.Add( oldButton, temp );
				}
			}
		}
		else
		{
			RoutineAgent.RoutineInfo temp = foundRoutines[ currentlySelectedNewRoutine ];
			
			RemoveByValue( actions, oldFunctionName );
			RemoveByValue( repeatableActions, oldFunctionName );
			
			InputController.ButtonType button = GetButtonFromSlot( currentlySelectedRoutine + 1 );
			
			if( actions.ContainsKey( button ) )
				actions[ button ] = temp.functionName.ToString();
			else
				actions.Add( button, temp.functionName.ToString() );
			
			if( temp.isRepeatableAction )
				repeatableActions.Add( button, temp.functionName.ToString() );
			
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
	
	private void UpdateSprites( bool display = true )
	{
		if( ExhaustEffectObject )
			ExhaustEffectObject.SetActiveRecursively( false );

		if( RocketEffectObject )
			RocketEffectObject.SetActiveRecursively( false );

		if( ShieldEffectObject )
			ShieldEffectObject.SetActiveRecursively( false );
		
		if( ExhaustSpriteRenderer )
			ExhaustSpriteRenderer.enabled = false;
		
		if( GravFlipSpriteRenderer )
			GravFlipSpriteRenderer.enabled = false;
		
		if( JumperCablesSpriteRenderer )
			JumperCablesSpriteRenderer.enabled = false;
		
		if( MagnetSpriteRenderer )
			MagnetSpriteRenderer.enabled = false;
		
		if( RocketSpriteRenderer )
			RocketSpriteRenderer.enabled = false;
		
		if( ShieldSpriteRenderer )
			ShieldSpriteRenderer.enabled = false;
		
		if( ThermostatSpriteRenderer )
			ThermostatSpriteRenderer.enabled = false;

		if( !display )
			return;

		SelectionScreenController.SlotSprite slotSprite;
		
		for( int i = 0; i < maxNumRoutines; i++ )
		{
			slotSprite = SelectionScreenController.SlotSprite.Invalid;
			
			switch( currentActions[i] )
			{
				case "Dash": slotSprite = SelectionScreenController.SlotSprite.Dash; break;
				case "GravShift": slotSprite = SelectionScreenController.SlotSprite.GravShift; break;
				case "Jump": slotSprite = SelectionScreenController.SlotSprite.Jump; break;
				case "JumperCable": slotSprite = SelectionScreenController.SlotSprite.JumperCable; break;
				case "LaserShield": slotSprite = SelectionScreenController.SlotSprite.LaserShield; break;
				case "Magnet": slotSprite = SelectionScreenController.SlotSprite.Magnet; break;
				case "MoveLeft": slotSprite = SelectionScreenController.SlotSprite.MoveLeft; break;
				case "MoveRight": slotSprite = SelectionScreenController.SlotSprite.MoveRight; break;
				case "Rocket": slotSprite = SelectionScreenController.SlotSprite.Rocket; break;
				case "Thermostat": slotSprite = SelectionScreenController.SlotSprite.Thermostat; break;
			}
			
			SelectionScreenAgent.SetSlotSprite( i+1, slotSprite );
		}

		for( int i = 0; i < currentActions.Length; i++ )
		{
			switch( currentActions[i] )
			{
				case "Dash": ExhaustSpriteRenderer.enabled = true; break;
				case "GravityShift": GravFlipSpriteRenderer.enabled = true; break;
				case "Rocket": RocketSpriteRenderer.enabled = true; break;
				case "LaserShield": ShieldSpriteRenderer.enabled = true; break;
			}
		}
	}

	private void UpdateSelectionScreen()
	{
		for( int i = 0; i < currentActions.Length; i++ )
			SelectionScreenAgent.SetText( (SelectionScreenAgent.TextType)i, currentActions[i] );
		
		int count = foundRoutines.Count;
		
		if( displayAllRoutines && count > 0 )
		{
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPower, foundRoutines[ currentlySelectedNewRoutine ].functionName.ToString() );
			
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPowerPlus1, foundRoutines[ ( currentlySelectedNewRoutine + 1 )%count ].functionName.ToString() );
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPowerPlus2, foundRoutines[ ( currentlySelectedNewRoutine + 2 )%count ].functionName.ToString() );
			
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPowerMinus1, foundRoutines[ ( currentlySelectedNewRoutine + ( count - ( 1 % count ) ) )%count ].functionName.ToString() );
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPowerMinus2, foundRoutines[ ( currentlySelectedNewRoutine + ( count - ( 2 % count ) ) )%count ].functionName.ToString() );
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPowerMinus3, foundRoutines[ ( currentlySelectedNewRoutine + ( count - ( 3 % count )) )%count ].functionName.ToString() );
		}
		else
		{
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPower, "" );
			
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPowerPlus1, "" );
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPowerPlus2, "" );
			
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPowerMinus1, "" );
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPowerMinus2, "" );
			SelectionScreenAgent.SetText( SelectionScreenAgent.TextType.CurrentPowerMinus3, "" );
		}
	}

	private void UpdateRegion()
	{
		RegionAgent.RegionType region = SpawnerAgent.GetNearestSpawner( SpawnerAgent.GetNearestCheckpoint( transform.position ) ).region;

		RegionAgent.DarkenAllRegions();
		RegionAgent.LightenRegion( region );
		RegionAgent.SetVignetteAmount( region );

		SoundAgent.FadeOutAllRegionMusic();
		SoundAgent.FadeInRegionMusic( region );
	}

	public void AddExternalMovementForce (Vector3 direction) {
		movementVector += direction;
	}
	
	public bool GetIsMoving()
	{
		return isMoving;
	}
	
	public void AdjustTemperature( float newtemperature )
	{
		if( CurrentActionsContains( "Thermostat" ) )
		{
			temperature = 0f;
			return;
		}

		temperature = newtemperature;
		
		Color tint = Color.white;
		
		if( temperature > 0f )
			tint = Color.Lerp( Color.white, Color.red, temperature / maxTemperature );
		else if( temperature < 0f )
			tint = Color.Lerp( Color.white, new Color( 0f, 0.4f, 1f, 1f ), Mathf.Abs( temperature ) / maxTemperature );
		
		for( int i = 0; i < constantSpriteRenderers.Length; i++ )
			constantSpriteRenderers[i].color = tint;
		
		if( ExhaustSpriteRenderer )
			ExhaustSpriteRenderer.color = tint;
		
		if( GravFlipSpriteRenderer )
			GravFlipSpriteRenderer.color = tint;
		
		if( JumperCablesSpriteRenderer )
			JumperCablesSpriteRenderer.color = tint;
		
		if( MagnetSpriteRenderer )
			MagnetSpriteRenderer.color = tint;
		
		if( RocketSpriteRenderer )
			RocketSpriteRenderer.color = tint;
		
		if( ShieldSpriteRenderer )
			ShieldSpriteRenderer.color = tint;
		
		if( ThermostatSpriteRenderer )
			ThermostatSpriteRenderer.color = tint;
	}
}