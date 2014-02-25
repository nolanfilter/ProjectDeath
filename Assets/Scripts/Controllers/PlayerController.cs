using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	private CharacterController controller;
	private InputController inputController;

	private float speed = 10f;
	private float setSpeed = 10f;
	private float conveyorSpeedWith = 15f;
	private float conveyorSpeedAgainst = 5f;

	private float gravity = -9.8f;
	private Vector3 gravityVector;
	private float maxVelocity = 0.25f;

	private RaycastHit hit;
	private Ray ray;
	private Ray leftRay;
	private Ray rightRay;
	private float leftHeight;
	private float rightHeight;
	private float slopeAngle = 0.3f;
	
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

	private Dictionary<InputController.ButtonType, string> actions;
	private Dictionary<InputController.ButtonType, string> repeatableActions;

	private string deathTag = "Death";
	private float deathDuration = 0.5f;
	private float relocationDuration = 0.5f;
	private float respawnDuration = 0.25f;

	private List<Rect> actionRects;

	//TO DO: Generalize
	private bool hasJumpCategory = false;
	private bool hasChosenJump = false;
	private int currentJumpIndex = 0;
	private float lastJumpIndexChangeTime;
	private float jumpIndexChangeBuffer = 0.25f;
	private enum JumpCategory
	{
		Jump = 0,
		Jet = 1,
		Shift = 2,
	}
	private bool[] jumpMovesActivated;

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

		actionRects = new List<Rect>();

		//TO DO: Generalize
		int length = Enum.GetNames( typeof( JumpCategory ) ).Length;

		jumpMovesActivated = new bool[ length ];

		for( int i = 0; i < length; i++ )
			jumpMovesActivated[ i ] = false;
	}

	void Start()
	{	
		isMobile = true;

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
			//TO DO: Generalize
			if( teachers[0].isJumpCategory )
			{
				switch( teachers[0].functionName )
				{
					case "Jump": jumpMovesActivated[ (int)JumpCategory.Jump ] = true; break;
					case "GravityShift": jumpMovesActivated[ (int)JumpCategory.Shift ] = true; break;
					case "JetLeft": case "JetRight": jumpMovesActivated[ (int)JumpCategory.Jet ] = true; break;
				}
				
				if( !hasJumpCategory )
				{
					int jumpMovesCount = 0;
					
					for( int i = 0; i < jumpMovesActivated.Length; i++ )
						if( jumpMovesActivated[ i ] )
							jumpMovesCount++;
					
					hasJumpCategory = jumpMovesCount > 1;
				}

				if( !hasJumpCategory )
				{
					for( int i = 0; i < teachers.Length; i++ )
						AddRoutine( teachers[i].button, teachers[i].functionName, teachers[i].isRepeatableAction );
				}
				else
				{
					AddRoutine( InputController.ButtonType.Action, "Activate", false );
				}
			}
			else
			{
				for( int i = 0; i < teachers.Length; i++ )
					AddRoutine( teachers[i].button, teachers[i].functionName, teachers[i].isRepeatableAction );
			}

			Destroy( collider.gameObject );
		}

		CheckpointController checkpoint = collider.gameObject.GetComponent<CheckpointController>();

		if( checkpoint != null )
		{
			SpawnerAgent.SetSpawnerPosition( checkpoint.checkpointPosition );
			Destroy( checkpoint.gameObject );
		}
	}

	void OnTriggerStay( Collider collider )
	{
		if( collider.tag == "ConveyorLeft" )
			movementVector += Vector3.left * speed * 0.25f * Time.deltaTime;
	}

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

	//for testing purposes only - incredibly inefficent
	void OnGUI()
	{	
		int index = 0;

		foreach( KeyValuePair<InputController.ButtonType, string> kvp in actions )
		{
			if( kvp.Value != "Activate" )
			{
				GUI.Label( actionRects[ index ], kvp.Value );

				index++;
			}
		}
	}

	//event handlers
	private void OnButtonDown( InputController.ButtonType button )
	{
		if( actions.ContainsKey( button ) )
			StartCoroutine( actions[ button ] );
	}
	
	private void OnButtonHeld( InputController.ButtonType button )
	{	
		if( repeatableActions.ContainsKey( button ) )
			StartCoroutine( repeatableActions[ button ] );
	}
	
	private void OnButtonUp( InputController.ButtonType button )
	{

	}
	//end event handlers

	//routines
	private IEnumerator Activate()
	{
		if( !isMobile )
		{
			if( hasJumpCategory )
				hasChosenJump = true;
		}

		yield break;
	}

	private IEnumerator MoveLeft()
	{
		if( !isMobile )
		{
			ChangeJumpIndex( false );

			yield break;
		}

		movementVector += Vector3.left * speed * Time.deltaTime;

		slopeCorrection();

		yield break;
	}

	private IEnumerator MoveRight()
	{
		if( !isMobile )
		{
			ChangeJumpIndex( true );
			
			yield break;
		}

		movementVector += Vector3.right * speed * Time.deltaTime;

		slopeCorrection();

		yield break;
	}

	private IEnumerator Jump()
	{
		if( isJumping || !isGrounded() )
			yield break;
		
		isJumping = true;

		yield return StartCoroutine( MovementOverTime( Vector3.up, 30f, 0.5f ) );

		while( !isGrounded() )
			yield return null;

		yield return new WaitForSeconds( jumpCoolDown );

		isJumping = false;
	}

	private IEnumerator Dash()
	{
		if( isDashing || !isMobile )
			yield break;

		isDashing = true;

		yield return StartCoroutine( MovementOverTime( ( isFacingRight ? Vector3.right : Vector3.left ), 30f, 0.5f ) );

		while( !isGrounded() )
			yield return null;

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
		ray = new Ray( transform.position, gravityVector );
			
		Physics.Raycast( ray, out hit );

		if( hit.distance <= transform.localScale.y * 0.6f && hit.collider != null && !hit.collider.isTrigger )
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

		leftRay = new Ray( transform.position + Vector3.left * transform.localScale.x * 0.5f, Vector3.down );

		Physics.Raycast( leftRay, out hit );
		leftHeight = hit.distance;

		rightRay = new Ray( transform.position + Vector3.right * transform.localScale.x * 0.5f, Vector3.down );

		Physics.Raycast( rightRay, out hit );
		rightHeight = hit.distance;

		float heightDifference = Mathf.Abs( leftHeight - rightHeight );

		if( heightDifference > 0.01f && heightDifference < slopeAngle )
			movementVector += gravityVector * speed * Time.deltaTime;
	}

	private void AddRoutine( InputController.ButtonType button, string functionName, bool isRepeatableAction )
	{
		if( actions.ContainsKey( button ) )
			actions[ button ] = functionName;
		else
		{
			actions.Add( button, functionName );
			actionRects.Add( new Rect( 5f, 20f * actionRects.Count, 100f, 20f ) );
		}

		if( isRepeatableAction )
		{
			if( repeatableActions.ContainsKey( button ) )
				repeatableActions[ button ] = functionName;
			else
				repeatableActions.Add( button, functionName );
		}
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
		renderer.enabled = false;

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

		if( hasJumpCategory )
			while( !hasChosenJump )
				yield return null;

		respawn();

		yield return new WaitForSeconds( respawnDuration );

		renderer.enabled = true;
		isMobile = true;

		hasChosenJump = false;
	}

	//TO DO: Generalize
	private void ChangeJumpIndex( bool increment )
	{
		if( !hasJumpCategory || ( Time.time - lastJumpIndexChangeTime < jumpIndexChangeBuffer ) )
			return;

		lastJumpIndexChangeTime = Time.time;

		do
		{
			if( increment )
				currentJumpIndex = ( currentJumpIndex + 1 )%jumpMovesActivated.Length;
			else
				currentJumpIndex = ( currentJumpIndex + ( jumpMovesActivated.Length - 1 ) )%jumpMovesActivated.Length;

		} while( !jumpMovesActivated[ currentJumpIndex ] );

		if( actions.ContainsKey( InputController.ButtonType.Jump ) )
		   actions.Remove( InputController.ButtonType.Jump );

		if( repeatableActions.ContainsKey( InputController.ButtonType.Jump ) )
			repeatableActions.Remove( InputController.ButtonType.Jump );

		if( actions.ContainsKey( InputController.ButtonType.Sel ) )
		   actions.Remove( InputController.ButtonType.Sel );

		if( repeatableActions.ContainsKey( InputController.ButtonType.Sel ) )
			repeatableActions.Remove( InputController.ButtonType.Sel );

		if( actions.ContainsKey( InputController.ButtonType.Start ) )
		   actions.Remove( InputController.ButtonType.Start );

		if( repeatableActions.ContainsKey( InputController.ButtonType.Start ) )
			repeatableActions.Remove( InputController.ButtonType.Start );

		switch( currentJumpIndex )
		{
			case (int)JumpCategory.Jump:
			{
				AddRoutine( InputController.ButtonType.Jump, "Jump", true );
			} break;

			case (int)JumpCategory.Shift:
			{
				AddRoutine( InputController.ButtonType.Jump, "GravityShift", false );
			} break;

			case (int)JumpCategory.Jet:
			{
				AddRoutine( InputController.ButtonType.Sel, "JetLeft", true );
				AddRoutine( InputController.ButtonType.Start, "JetRight", true );
			} break;
		}
	}
}
