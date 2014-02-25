using UnityEngine;
using System.Collections;

public class CrusherSideController : MonoBehaviour {

	public float speed;
	public float leftX = -1f;
	public float rightX = 1f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.position += Vector3.left * speed * Time.deltaTime;
		
		if (transform.position.x <= leftX) {
			speed = -5f;
		}
		if (transform.position.x >= rightX) {
			speed = 5f;
		}
		
	}
}
