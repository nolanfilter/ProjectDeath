using UnityEngine;
using System.Collections;

public class FlipController : MonoBehaviour {

	public float order;
	public float timing;
	public Transform mainPos;

	private bool spin = false;
	private bool set = false;
	private bool hasWaited = false;
	private bool setTarget = true;

	private float rotation;
	private Quaternion target;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (spin == true) {
			//transform.Rotate(0f, 0f, (180f * Time.deltaTime / timing));
			mainPos.rotation = Quaternion.Lerp(transform.rotation, target, 7.5f * Time.deltaTime); //edit flip rate here (recommend increments of common angles)
			//platPos.rotation = Quaternion.Lerp(transform.rotation, target, 7.5f * Time.deltaTime);
			if (set == true) {
				//Debug.Log (Time.time);
				StartCoroutine(platFlipWait(timing));
				set = false;
			}
		} 
		if (spin == false) {
			if (set == false) {
				//Debug.Log (Time.time);
				if (setTarget == true) {
					target = Quaternion.Euler (0f, 0f, 180f);
				} else {
					target = Quaternion.Euler (0f, 0f, 0f);
				}
				StartCoroutine(platFlipWait(timing));
				if (setTarget == true) {
					setTarget = false;
				} else {
					setTarget = true;
				}
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
