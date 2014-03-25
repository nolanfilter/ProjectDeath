using UnityEngine;
using System.Collections;

public class CrusherController : MonoBehaviour {

	public Collider deathTrigger;

	public float moveDist;
	public float speed;
	public float pause;
	
	private float rayDistance = 0.2f;
	private bool deathActive = false;
	//private float yMove = -1f;
	private Vector3 origin;
	private Vector3 newY;
	private bool direct = true;
	private bool set = true;
	private float upSpeed;
	private float downSpeed;
	// Use this for initialization
	void Start () {
		origin = transform.position;
		newY = new Vector3 (origin.x, origin.y - moveDist, origin.z);
		downSpeed = ((speed) * 0.6f);
		upSpeed = ((speed) * 0.03f);
	}
		
	// Update is called once per frame
	void Update () {
		if (deathActive == true) {
			deathTrigger.collider.enabled = true;
		} else {
			deathTrigger.collider.enabled = false;
		}

		if (direct == true) {
			transform.position = Vector3.MoveTowards (transform.position, newY, downSpeed);
		} else {
			transform.position = Vector3.Lerp (transform.position, origin, upSpeed);
			set = true;
		}

		if (transform.position.y <= newY.y + 0.1f) {
			if (set == true) {
				StartCoroutine (waitPause(pause));
			}
		} 
		if (transform.position.y >= origin.y - 0.1f) {
			direct = true;
		}
	}

	void FixedUpdate () {

		Ray ray = new Ray (transform.position - Vector3.up * 3.5f, -transform.up);
		RaycastHit rayHit;

		if (Physics.Raycast(ray, out rayHit, rayDistance)) 
		{
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

	IEnumerator waitPause (float waitTime) {
		yield return new WaitForSeconds(waitTime);
		set = false;
		direct = false;

	}
}
