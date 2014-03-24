using UnityEngine;
using System.Collections;

public class FlameControlv2 : MonoBehaviour {

	public float order = 1f;
	public float timing = 5f;
	public Collider flameTop;
	public Collider flame2;
	private bool active = false;
	private bool set = false;

	// Use this for initialization
	void Start () {
		//Debug.Log (Time.time);
		StartCoroutine (offsetWait(order));
	}
	
	// Update is called once per frame
	void Update () {
		if (active == true) {
			if (set == true) {
				//Debug.Log (Time.time);
				flameTop.collider.enabled = true;
				flame2.collider.enabled = true;
				StartCoroutine(waitCall (timing));
				set = false;
			}
		} 
		if (active == false) {
			if (set == false) {
				//Debug.Log (Time.time);
				flameTop.collider.enabled = false;
				flame2.collider.enabled = false;
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

	IEnumerator offsetWait(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		//Debug.Log (Time.time);
	}
}
