using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	private CharacterController controller;
	private InputController inputController;

	private Vector3 startPosition = new Vector3( 0f, 10f, 0f );

	private float speed = 10f;

	private float gravity = -9.8f;
	private Vector3 gravityVector;

	private Vector3 movementVector;

	private bool isJumping;
	private float jumpCoolDown = 0.25f;

	private Dictionary<InputController.ButtonType, string> actions;
	private Dictionary<InputController.ButtonType, string> repeatableActions;

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
		TeachingController teach = collider.gameObject.GetComponent<TeachingController>();

		if( teach != null )
		{
			AddRoutine( teach.button, teach.functionName, teach.isRepeatableAction );
			Destroy( teach.gameObject );
		}
	}

	void OnTriggerStay( Collider collider )
	{
		if( collider.tag == "Death" )
			respawn();
	}

	void Update()
	{	
		applyGravity ();
		
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

	private IEnumerator Hop()
	{
		if( isJumping || !controller.isGrounded )
			yield break;

		isJumping = true;

		yield return StartCoroutine( MovementOverTime( Vector3.up, 12.5f, 0.5f ) );

		while( !controller.isGrounded )
			yield return null;

		yield return new WaitForSeconds( jumpCoolDown );

		isJumping = false;
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
	//end routines

	private void respawn()
	{
		StopAllCoroutines();

		isJumping = false;

		transform.position = startPosition;
	}

	private void applyGravity()
	{		
		if( !controller.isGrounded ) 
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
}
