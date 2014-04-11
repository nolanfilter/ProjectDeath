using UnityEngine;
using System.Collections;

public class FanController : MonoBehaviour {

	public float speed;
	public PlayerController player;
	public string direction; //type in "Left" or "Right"
	
	private Vector3 newForce;

	// Use this for initialization
	void Start () {
		if (direction == "Left") {
			speed *= -1f;
			newForce = new Vector3(speed, 0f, 0f);
		}
		if (direction == "Right") {
			speed = speed;
			newForce = new Vector3(speed, 0f, 0f);
		}
		if (direction == "Up") {
			speed = speed;
			newForce = new Vector3(0f, speed, 0f);
		}
		if (direction == "Down") {
			speed *= -1f;
			newForce = new Vector3(0f, speed, 0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay (Collider collider) {
		if (collider.gameObject.tag == "Player") {

			player.AddExternalMovementForce(newForce);
		}

	}
}
