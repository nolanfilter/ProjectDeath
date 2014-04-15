using UnityEngine;
using System.Collections;

public class CrusherController : MonoBehaviour {

	public Collider deathTrigger;

	public float moveDist;
	public float speed;
	public float pause;
	public float order;
	
	private float rayDistance = 1f;
	private bool deathActive = false;
	//private float yMove = -1f;
	private Vector3 origin;
	private Vector3 newY;
	private bool direct = true;
	private bool set = true;
	private float upSpeed;
	private float downSpeed;
	private bool done;
	// Use this for initialization
	void Start () {
		origin = transform.position;
		newY = new Vector3 (origin.x, origin.y - moveDist, origin.z);
		downSpeed = ((speed) * 0.6f);
		upSpeed = ((speed) * 0.03f);
		done = false;
		StartCoroutine(waitBegin(order));
	}
		
	// Update is called once per frame
	void Update () {
		if(!done)
		{
			return;
		}
		if (deathActive == true) {
			deathTrigger.collider.enabled = true;
		} else {
			deathTrigger.collider.enabled = false;
		}

		if (direct == true) {
			transform.position = Vector3.MoveTowards (transform.position, newY, downSpeed);
			//SoundAgent.PlayClip (SoundAgent.SoundEffects.CrusherSwoosh,1f, false, gameObject);
		} else {
			transform.position = Vector3.Lerp (transform.position, origin, upSpeed);
			//SoundAgent.PlayClip (SoundAgent.SoundEffects.CrusherRise,1f, false, gameObject);
			set = true;
			deathActive = false;
		}
		if(moveDist > 0)
		{
			if (transform.position.y <= newY.y + 0.1f) {
				//SoundAgent.PlayClip(SoundAgent.SoundEffects.CrusherHit,1f, false, gameObject);
				deathActive = true;
				if (set == true) {
					StartCoroutine (waitPause(pause));
				}
			}
			if (transform.position.y >= origin.y - 0.1f) {
				direct = true;
			}
		}

		if(moveDist < 0)
		{
			if (transform.position.y >= newY.y - 0.1f) {
				deathActive = true;
				if (set == true) {
					StartCoroutine (waitPause(pause));
				}
			}
			if (transform.position.y <= origin.y + 0.1f) {
				direct = true;
			}
		}
	}

	/*
	void FixedUpdate () {

		if (gameObject.collider.tag != "OtherCollider") {
			Ray ray = new Ray (transform.position - (Vector3.up * 2.5f), -transform.up);
			RaycastHit rayHit;
			if (!Physics.Raycast(ray, out rayHit, rayDistance)) 
			{
				deathActive = false;
			} else {
				deathActive = true;
			}
		} else {
			rayDistance = 5f;
			Ray ray = new Ray (transform.position - (Vector3.up * 3f), -transform.up);
			RaycastHit rayHit;
			if (Physics.Raycast(ray, out rayHit, rayDistance)) 
			{
				deathActive = false;
			} else {
				deathActive = true;
			}
		}
	}	
	*/
	IEnumerator waitPause (float waitTime) {
		yield return new WaitForSeconds(waitTime);
		set = false;
		direct = false;

	}
	IEnumerator waitBegin (float waitTime) {
		yield return new WaitForSeconds(waitTime);
		done = true;
	}
}
