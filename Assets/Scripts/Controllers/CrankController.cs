using UnityEngine;
using System.Collections;

public class CrankController : MonoBehaviour {

	public float order;
	public float timing;
	public Transform platPos;
	
	private bool spin = false;
	private bool set = false;
	private bool hasWaited = false;
	
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (spin == true) {
			//transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 1f, 1f), .1f);
			platPos.RotateAround(transform.position, Vector3.up, 45 * Time.deltaTime);
			if (set == true) {
				//Debug.Log (Time.time);
				StartCoroutine(platFlipWait(timing));
				set = false;
			}
		} 
		if (spin == false) {
			//transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, .2f, 1f), .1f);
			platPos.RotateAround(transform.position, Vector3.up, 45 * Time.deltaTime);
			if (set == false) {
				//Debug.Log (Time.time);
				StartCoroutine(platFlipWait(timing));
				set = true;
			}
		}
	}
	
	
	IEnumerator platFlipWait(float timeWait) {
		if(!hasWaited)
		{
			yield return new WaitForSeconds(order/3);
			hasWaited = true;
		}
		yield return new WaitForSeconds(timeWait);
		if (spin == true) {
			spin = false;
		} else {
			spin = true;
		}
		
	}
}
