using UnityEngine;
using System.Collections;

public class BodyController : MonoBehaviour {

	private float waitTime = 0.25f;
	private CharacterController controller;

	private float gravity = -1.45f;
	private Vector3 gravityVector;

	void Start()
	{
		controller = GetComponent<CharacterController>();

		if( controller == null )
		{
			enabled = false;
			return;
		}

		controller.enabled = false;

		gravityVector = Vector3.up * gravity;

		StartCoroutine( "BodyRoutine" );
	}

	private IEnumerator BodyRoutine()
	{
		yield return new WaitForSeconds( waitTime );
		controller.enabled = true;

		while( !controller.isGrounded )
		{
			controller.Move( gravityVector * Time.deltaTime );

			yield return null;
		}

		controller.enabled = false;
	}
}
