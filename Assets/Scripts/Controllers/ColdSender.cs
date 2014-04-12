using UnityEngine;
using System.Collections;

public class ColdSender : MonoBehaviour {

	//public float rate;
	public bool active;
	public bool kill = false;
	public GameObject killBlock;

	// Use this for initialization
	void Start () {
		//StartCoroutine(switchKill());
		active = false;
		//rate *= 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (kill == true) {
			gameObject.tag = "Death";
		} else {
			gameObject.tag = "Untagged";
		}
		*/

		if (kill == true) {
			killBlock.collider.enabled = true;
		} else {
			killBlock.collider.enabled = false;
		}

	}

	void OnTriggerStay (Collider collider) {
		if (collider.gameObject.tag == "Player") {
			active = true;
		}
		/*
		if (kill == true) {
			gameObject.tag = "Death";
		} else {
			gameObject.tag = "Untagged";
		}
		*/
	}

	void OnTriggerExit (Collider collider) {
		if (collider.gameObject.tag == "Player") {
			active = false;
		}

	}
	/*
	IEnumerator switchKill () {
		yield return new WaitForSeconds(2f);
		if (kill == true) {
			kill = false;
		}
	}
	*/
}
