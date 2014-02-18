using UnityEngine;
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

	private Vector3 movementVector;

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
		gravityVector = transform.up * gravity;
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

		TeachingController teach = collider.gameObject.GetComponent<TeachingController>();

		if( teach != null )
		{
			AddRoutine( teach.button, teach.functionName, teach.isRepeatableAction );
			Destroy( teach.gameObject );
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

		controller.Move( movementVector );
		
		movementVector = Vector3.zero;
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

		yield break;
	}

	private IEnumerator MoveRight()
	{
		movementVector += Vector3.right * speed * Time.deltaTime;

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
	//end routines

	private void respawn()
	{
		isJumping = false;
		isDashing = false;
		isFacingRight = true;

		transform.position = SpawnerAgent.GetSpawnerPosition();
	}

	private void applyGravity()
	{		
		if( !controller.isGrounded && isMobile ) 
			movementVector += gravityVector * Time.deltaTime;
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
