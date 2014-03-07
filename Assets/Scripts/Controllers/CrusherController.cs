using UnityEngine;
using System.Collections;

public class CrusherController : MonoBehaviour {

	private float rayDistance = 1f;
	public Collider deathTrigger;
	private bool deathActive = false;
	//private float yMove = -1f;

	// Use this for initialization
	void Start () {

	}
		
	// Update is called once per frame
	void Update () {
		if (deathActive = true) {
			deathTrigger.collider.enabled = true;
		} else {
			deathTrigger.collider.enabled = false;
		}

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
			yMove *= -1;
		} 

		if (Physics.Raycast (ray, out rayHit, 4f)) {
			yMove *= -1;
		}
		*/
	}	
}
