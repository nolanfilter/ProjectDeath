using UnityEngine;
using System.Collections;

public class CrusherController : MonoBehaviour {

	public Collider deathTrigger;

	private float rayDistance = 1f;
	private bool deathActive = false;
	//private float yMove = -1f;
	//private Vector3 moveVector = new Vector3 (0f,yMove, 0f);

	// Use this for initialization
	void Start () {

	}
		
	// Update is called once per frame
	void Update () {
		if (deathActive == true) {
			deathTrigger.collider.enabled = true;
		} else {
			deathTrigger.collider.enabled = false;
		}

		//transform.position += moveVector;
	}

	void FixedUpdate () {

		Ray ray = new Ray (transform.position, -transform.up);
		RaycastHit rayHit;

		if (Physics.Raycast(ray, out rayHit, rayDistance)) {
			deathActive = true;
		} else {
			deathActive = false;
		}
		/*
		if (Physics.Raycast (ray, out rayHit, 0.1f)) {
			yMove *= 1;
		} 

		if (Physics.Raycast (ray, out rayHit, 4f)) {
			yMove *= -1;
		}
		*/
	}	
}
