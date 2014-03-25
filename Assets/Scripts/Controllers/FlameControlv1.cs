using UnityEngine;
using System.Collections;

public class FlameControlv1 : MonoBehaviour {

	public float order = 1f;
	public float timing = 5f;
	public Collider flame;
	private bool active = false;
	private bool set = false;

	private bool hasWaited = false;

	public Animator glow;
	public Animator fireBottom;
	public Animator fireMiddle;
	public Animator fireTop;



	// Use this for initialization
	void Start () {
		//Debug.Log (Time.time);
		StartCoroutine (offsetWait(order));
	}
	
	// Update is called once per frame
	void Update () {
		if (active == true) {
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 1f, 1f), .1f);
			if (set == true) {
				//Debug.Log (Time.time);
				glow.SetBool("Fire",true);
				fireBottom.SetBool("Fire",true);
				fireMiddle.SetBool("Fire",true);
				fireTop.SetBool("Fire",true);
				flame.collider.enabled = true;
				StartCoroutine(waitCall (timing));
				set = false;
			}
		} 
		if (active == false) {
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, .2f, 1f), .1f);
			if (set == false) {
				//Debug.Log (Time.time);
				glow.SetBool("Fire",false);
				fireBottom.SetBool("Fire",false);
				fireMiddle.SetBool("Fire",false);
				fireTop.SetBool("Fire",false);
				flame.collider.enabled = false;
				StartCoroutine(waitCall (timing));
				set = true;
			}
		}

	}
	
	IEnumerator waitCall(float waitTime) {
		//Debug.Log ("wait call");
		if(!hasWaited)
		{
			yield return new WaitForSeconds(order/3);
			hasWaited = true;
		}
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
