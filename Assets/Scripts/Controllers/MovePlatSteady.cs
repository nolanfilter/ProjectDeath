using UnityEngine;
using System.Collections;

public class MovePlatSteady : MonoBehaviour {

	public float speed;
	public float distance;
	public float wait;

	private Vector3 origin;
	private Vector3 target;
	private float moveSpeed;
	private bool noStop;
	//private Vector3 reTarget;

	private bool okayGo;

	// Use this for initialization
	void Start () {
		origin = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		target = new Vector3(transform.position.x + distance, transform.position.y, transform.position.z);
		moveSpeed = (speed * 0.03f);
		StartCoroutine (waitCall());
	}
	
	// Update is called once per frame
	void Update () {
		if (noStop) {
			if (okayGo) {
				transform.Translate (target * moveSpeed);
				//Vector3.MoveTowards(transform.position, target, 0.1f);
				//Debug.Log (target);
				if (transform.position.x >= target.x - 0.1f) {
					noStop = false;
					okayGo = false;
				}
				//reTarget = new Vector3 (target.x - distance, target.y, target.z);
			}
			if (!okayGo) {
				//Debug.Log ("go back");
				//transform.Translate (Vector3.left * (distance/2) * Time.deltaTime);
				transform.Translate (Vector3.left * (distance * .85f) * moveSpeed);
				//Vector3.MoveTowards(transform.position, origin, 0.1f);
				//Debug.Log ("moving back");
				//transform.Translate(reTarget * Time.deltaTime);
				//Debug.Log (origin);
				if (transform.position.x <= origin.x + 0.1f) {
					//Debug.Log ("set true");
					noStop = false;
					okayGo = true;
				}

			}
		}
		//Debug.Log (okayGo);
		//Debug.Log (noStop);
	}

	IEnumerator waitCall() {
		while (true) {
			//Debug.Log ("wait call");
			noStop = true;
			yield return new WaitForSeconds (wait * 2f);
		}
	}
}
