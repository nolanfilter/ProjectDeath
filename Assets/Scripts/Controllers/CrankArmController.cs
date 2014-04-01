using UnityEngine;
using System.Collections;

public class CrankArmController : MonoBehaviour {

	public float order;
	public float timing;
	public Transform mainPos;
	
	private bool spin = false;
	private bool set = false;
	private bool hasWaited = false;

	private Vector3 setTarget;

	private float newY;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		/*
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
		}*/
	
		transform.RotateAround(mainPos.position, Vector3.forward, 90f * Time.deltaTime); //orbit code
		/*
		newY = transform.position.y + 10f;
		setTarget = new Vector3 (transform.position.x, transform.position.y, transform.position.z-1); //makes platform face up (flips axis but just scale appropriately if need to change
		transform.LookAt (setTarget);
		*/

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
