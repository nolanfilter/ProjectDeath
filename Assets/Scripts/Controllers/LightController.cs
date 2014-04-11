using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour {

	public float timing;
	public float distance = 1f;
	public float spread = 1f;
	public Vector3 point0;
	public Vector3 point1;
	public Vector3 point2;
	public Vector3 point3;
	public Vector3 point4;

	private bool active = false;
	private bool set = false;
	private Vector3 dir0;
	private Vector3 dir1;
	private Vector3 dir2;
	private Vector3 dir3;
	private Vector3 dir4;

	private bool flash0 = false;
	private bool flash1 = false;
	private bool flash2 = false;
	private bool flash3 = false;
	private bool flash4 = false;

	// Use this for initialization
	void Start () {
		dir0 = new Vector3(point0.x * spread, point0.y * distance, 0f);
		dir1 = new Vector3(point1.x * spread, point1.y * distance, 0f);
		dir2 = new Vector3(point2.x * spread, point2.y * distance, 0f);
		dir3 = new Vector3(point3.x * spread, point3.y * distance, 0f);
		dir4 = new Vector3(point4.x * spread, point4.y * distance, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray0 = new Ray(transform.position, dir0);
		Ray ray1 = new Ray(transform.position, dir1);
		Ray ray2 = new Ray(transform.position, dir2);
		Ray ray3 = new Ray(transform.position, dir3);
		Ray ray4 = new Ray(transform.position, dir4);
		// debug/visualize rays
		Debug.DrawRay (transform.position,dir0,Color.green);
		Debug.DrawRay (transform.position,dir1,Color.green);
		Debug.DrawRay (transform.position,dir2,Color.green);
		Debug.DrawRay (transform.position,dir3,Color.green);
		Debug.DrawRay (transform.position,dir4,Color.green);
		//
		RaycastHit rayHit0;
		RaycastHit rayHit1;
		RaycastHit rayHit2;
		RaycastHit rayHit3;
		RaycastHit rayHit4;
	
		if (Physics.Raycast(ray0, out rayHit0, distance)) {
			//Debug.Log (rayHit0.collider);
			if (rayHit0.collider.tag == "Player") {
				//Debug.Log("Flash");
				flash0 = true;
			} else {
				flash0 = false;
			}
		}
		if (Physics.Raycast(ray1, out rayHit1, distance)) {
			//Debug.Log (rayHit1.collider);
			if (rayHit1.collider.tag == "Player") {
				//Debug.Log("Flash");
				flash1 = true;
			} else {
				flash1 = false;
			}
		}
		if (Physics.Raycast(ray2, out rayHit2, distance)) {
			//Debug.Log (rayHit2.collider);
			if (rayHit2.collider.tag == "Player") {
				//Debug.Log("Flash");
				flash2 = true;
			} else {
				flash2 = false;
			}
		}
		if (Physics.Raycast(ray3, out rayHit3, distance)) {
			//Debug.Log (rayHit3.collider);
			if (rayHit3.collider.tag == "Player") {
				//Debug.Log("Flash");
				flash3 = true;
			} else {
				flash3 = false;
		}
		}
		if (Physics.Raycast(ray4, out rayHit4, distance)) {
			//Debug.Log (rayHit4.collider);
			if (rayHit4.collider.tag == "Player") {
				//Debug.Log("Flash");
				flash4 = true;
			} else {
				flash4 = false;
			}
		}


		/*
		if (Physics.Linecast(transform.position, dir0, out rayHit0)) {
			//Debug.Log (rayHit0.collider);
			if (rayHit0.collider.tag == "Player") {
				//Debug.Log("Flash");
				flash0 = true;
			} else {
				flash0 = false;
			}
		}
		if (Physics.Linecast(transform.position, dir1, out rayHit1)) {
			//Debug.Log (rayHit1.collider);
			if (rayHit1.collider.tag == "Player") {
				//Debug.Log("Flash");
				flash1 = true;
			} else {
				flash1 = false;
			}
		}
		if (Physics.Linecast(transform.position, dir2, out rayHit2)) {
			//Debug.Log (rayHit2.collider);
			if (rayHit2.collider.tag == "Player") {
				//Debug.Log("Flash");
				flash2 = true;
			} else {
				flash2 = false;
			}
		}
		if (Physics.Linecast(transform.position, dir3, out rayHit3)) {
			//Debug.Log (rayHit3.collider);
			if (rayHit3.collider.tag == "Player") {
				//Debug.Log("Flash");
				flash3 = true;
			} else {
				flash3 = false;
			}
		}
		if (Physics.Linecast(transform.position, dir4, out rayHit4)) {
			//Debug.Log (rayHit4.collider);
			if (rayHit4.collider.tag == "Player") {
				//Debug.Log("Flash");
				flash4 = true;
			} else {
				flash4 = false;
			}
		}
		*/

		if (active == true) {
			if (set == true) {
				//set up rays
				/*
				Ray ray0 = new Ray(transform.position, dir0);
				Ray ray1 = new Ray(transform.position, dir1);
				Ray ray2 = new Ray(transform.position, dir2);
				Ray ray3 = new Ray(transform.position, dir3);
				Ray ray4 = new Ray(transform.position, dir4);
				// debug/visualize rays
				Debug.DrawRay (transform.position,dir0,Color.green);
				Debug.DrawRay (transform.position,dir1,Color.green);
				Debug.DrawRay (transform.position,dir2,Color.green);
				Debug.DrawRay (transform.position,dir3,Color.green);
				Debug.DrawRay (transform.position,dir4,Color.green);
				//
				RaycastHit rayHit0;
				RaycastHit rayHit1;
				RaycastHit rayHit2;
				RaycastHit rayHit3;
				RaycastHit rayHit4;
				*/

				//activate raycasts
				if (flash0 || flash1 || flash2 || flash3 || flash4) {
					Debug.Log ("Flash");
				}

				//put visual blocking code with object either above in if-statements or down here after activation

				StartCoroutine(waitCall (timing));
				set = false;
			}
		} 
		if (active == false) {
			if (set == false) {
				//deactivate rays
				StartCoroutine(waitCall (timing));
				set = true;
			}
		}
		
	}

	
	IEnumerator waitCall(float waitTime) {
		//Debug.Log ("wait call");
		yield return new WaitForSeconds(waitTime);
		if (active == true) {
			active = false;
		} else {
			active = true;
		}
		//Debug.Log ("end wait");
		//Debug.Log (Time.time);
	}
}
