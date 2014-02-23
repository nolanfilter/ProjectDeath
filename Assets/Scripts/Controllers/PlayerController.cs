﻿using UnityEngine;
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

	private RaycastHit hit;
	private Ray ray;
	private Ray leftRay;
	private Ray rightRay;
	private float leftHeight;
	private float rightHeight;
	private float slopeAngle = 0.3f;

	private Vector3 movementVector;
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
			for( int i = 0; i < teachers.Length; i++ )
				AddRoutine( teachers[i].button, teachers[i].functionName, teachers[i].isRepeatableAction );

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

		controller.Move( movementVector );
		
		movementVector = Vector3.zero;
		applyHeading = false;

		if( controller.isGrounded )
			heading = Mathf.Lerp( heading, startingHeading, 0.5f );
	}

	//for testing purposes only - incredibly inefficent
	void OnGUI()
	{	
		int index = 0;

		foreach( KeyValuePair<InputController.ButtonType, string> kvp in actions )
		{
			GUI.Label( actionRects[ index ], kvp.Value );

			index++;
		}
	}

	//event handlers
	private void OnButtonDown( InputController.ButtonType button )
	{	
		if( !isMobile )
			return;

		if( actions.ContainsKey( button ) )
			StartCoroutine( actions[ button ] );
	}
	
	private void OnButtonHeld( InputController.ButtonType button )
	{	
		if( !isMobile )
			return;

		if( repeatableActions.ContainsKey( button ) )
			StartCoroutine( repeatableActions[ button ] );
	}
	
	private void OnButtonUp( InputController.ButtonType button )
	{

	}
	//end event handlers

	//routines
	private IEnumerator MoveLeft()
	{
		movementVector += Vector3.left * speed * Time.deltaTime;

		slopeCorrection();

		yield break;
	}

	private IEnumerator MoveRight()
	{
		movementVector += Vector3.right * speed * Time.deltaTime;

		slopeCorrection();

		yield break;
	}

	private IEnumerator Jump()
	{
		if( isJumping || !controller.isGrounded )
			yield break;
		
		isJumping = true;

		yield return StartCoroutine( MovementOverTime( Vector3.up, 30f, 0.5f ) );

		while( !controller.isGrounded )
			yield return null;

		yield return new WaitForSeconds( jumpCoolDown );

		isJumping = false;
	}

	private IEnumerator Dash()
	{
		if( isDashing )
			yield break;

		isDashing = true;

		yield return StartCoroutine( MovementOverTime( ( isFacingRight ? Vector3.right : Vector3.left ), 30f, 0.5f ) );

		while( !controller.isGrounded )
			yield return null;

		yield return new WaitForSeconds( dashCoolDown );

		isDashing = false;
	}

	private IEnumerator GravityShift()
	{
		ray = new Ray( transform.position, gravityVector );

		if( !Physics.Raycast( ray, transform.localScale.y * 0.75f ) )
			yield break;

		gravityVector *= -1f;	
	}

	private IEnumerator JetLeft()
	{
		heading -= deltaAngle;
		applyHeading = true;

		yield break;
	}

	private IEnumerator JetRight()
	{
		heading += deltaAngle;
		applyHeading = true;

		yield break;
	}
	//end routines

	private void respawn()
	{
		gravityVector = transform.up * gravity;

		isJumping = false;
		isDashing = false;
		isFacingRight = true;

		heading = startingHeading;
		applyHeading = false;

		transform.position = SpawnerAgent.GetSpawnerPosition();
	}

	private void applyGravity()
	{		
		if( !controller.isGrounded && isMobile ) 
			movementVector += gravityVector * Time.deltaTime;
	}

	private void slopeCorrection()
	{
		if( !controller.isGrounded || isJumping )
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

		respawn();

		yield return new WaitForSeconds( respawnDuration );

		renderer.enabled = true;
		isMobile = true;
	}
}
