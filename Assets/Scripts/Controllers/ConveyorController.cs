using UnityEngine;
using System.Collections;

public class ConveyorController : MonoBehaviour {

	public float speed;
	public PlayerController player;
	public string direction; //type in "Left" or "Right"
	public float intNum;

	private Vector3 newForce;
	private bool active = false;
	private bool prevent = false;
	private bool set = false;
	private bool run = false;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	/*
	void OnCollisionStay(Collision collision) {
		if (collision.gameObject.tag != "Player") {
			Debug.Log ("Not Player");
			return;
		}
		Debug.Log ("Player");
		if (direction == "Left") {
			speed *= -1f;
		}
		if (direction == "Right") {
			speed = speed;
		}

		newForce = new Vector3(speed, 0f, 0f);
		Debug.Log (newForce);
		player.AddExternalMovementForce(newForce);
		Debug.Log ("added");
	}
	*/

	void OnTriggerStay(Collider collider) {
		if (collider.gameObject.tag != "Player") {
			//Debug.Log ("Not Player");
			return;
		} else {
			active = true;
		}

		//Debug.Log ("Player");

		if (active == true && prevent == false) {
			if (direction == "Left") {
				speed *= -1f;
				prevent = true;
			}
			if (direction == "Right") {
				speed = speed;
				prevent = true;
			}
		}

		newForce = new Vector3(speed, 0f, 0f);

		if (intNum == 0) {
			//Debug.Log (newForce);
			player.AddExternalMovementForce(newForce);
			//Debug.Log ("added");
		} else {
			if (run == false) {
				if (set == false) {
					StartCoroutine(waitCall(intNum));
					set = true;
				}
			}
			if (run == true) {
				if (set == true) {
					//Debug.Log (newForce);
					player.AddExternalMovementForce(newForce);
					//Debug.Log ("added");
					set = false;
					run = false;
				}
			}
		}

	}

	IEnumerator waitCall (float waitTime) {
		yield return new WaitForSeconds(waitTime);
		Debug.Log ("WAITED");
		if (run == true) {
			run = false;
		} else {
			run = true;
		}
	}
}
