using UnityEngine;
using System.Collections;

public class MovingPlatController : MonoBehaviour {

	public float speed;
	public float distance;
	public float buffer; //keep below 1f, around 0.1f if possible

	private float moveSpeed;
	private Vector3 origin;
	private Vector3 newPos;
	private bool moveRight;
	private bool moveLeft;
	private Vector3 farRight;
	private Vector3 farLeft;

	// Use this for initialization
	void Start () {
		moveSpeed = ((speed) * 0.03f);
		origin = transform.position;
		newPos = new Vector3 (origin.x + distance, origin.y, origin.z);
		farRight = new Vector3(newPos.x - buffer, newPos.y, newPos.z);
		farLeft = new Vector3(origin.x + buffer, origin.y, origin.z);
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x <= farLeft.x) {
			moveRight = true;
			moveLeft = false;
		} 
		if (transform.position.x >= farRight.x) {
			moveLeft = true;
			moveRight = false;
		}

		if (moveRight == true) {
			transform.position = Vector3.Lerp (transform.position, newPos, moveSpeed);
		}
		if (moveLeft == true) {
			transform.position = Vector3.Lerp (transform.position, origin, moveSpeed);
		}

	}
}
