using UnityEngine;
using System.Collections;

public class FlameController : MonoBehaviour {

	public float interTime = 14f; //make this double goTime
	public float goTime = 7f; // 14,7 makes 5 seconds on and off timers
	public Collider flame0;
	public Collider flame1;
	public Collider flame2;
	public Collider flame3;
	public Collider flame4;
	
	private float checkTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		checkTime = Time.time;
		if ((checkTime % interTime) >= goTime) {
			Debug.Log ("flames active");
			flame0.collider.enabled = true;
			flame1.collider.enabled = true;
			flame2.collider.enabled = true;
			flame3.collider.enabled = true;
			flame4.collider.enabled = true;
			StartCoroutine(waitCall());
		} else {
			Debug.Log ("flames down");
			flame0.collider.enabled = false;
			flame1.collider.enabled = false;
			flame2.collider.enabled = false;
			flame3.collider.enabled = false;
			flame4.collider.enabled = false;
		}
	}

	IEnumerator waitCall() {
		Debug.Log ("waiting (active)");
		yield return new WaitForSeconds(5);
	}
}
