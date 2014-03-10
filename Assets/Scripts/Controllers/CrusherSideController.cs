using UnityEngine;
using System.Collections;

public class CrusherSideController : MonoBehaviour {
	/*
	public float speed;
	public float leftX = -1f;
	public float rightX = 1f;
	*/
	private float rayDistance = 1f;
	public Collider deathTriggerSide;
	private bool deathActive = false;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (deathActive == true) {
			deathTriggerSide.collider.enabled = true;
		} else {
			deathTriggerSide.collider.enabled = false;
		}
		/*
		transform.position += Vector3.left * speed * Time.deltaTime;
		
		if (transform.position.x <= leftX) {
			speed = -5f;
		}
		if (transform.position.x >= rightX) {
			speed = 5f;
		}
		*/
		
	}

	void FixedUpdate() {
		Ray ray = new Ray (transform.position, -transform.right);
		RaycastHit rayHit;
		
		if (Physics.Raycast(ray, out rayHit, rayDistance)) {
			deathActive = true;
		} else {
			deathActive = false;
		}
	}
}
