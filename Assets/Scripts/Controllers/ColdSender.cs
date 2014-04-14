using UnityEngine;
using System.Collections;

public class ColdSender : MonoBehaviour {

	//public float rate;
	public bool active;
	public bool kill = false;
	public GameObject killBlock;

	private Vector3 vibPlusX;
	private Vector3 vibMinusX;

	// Use this for initialization
	void Start () {
		//StartCoroutine(switchKill());
		active = false;
		//inactive = true;
		//rate *= 0.5f;
		vibPlusX = new Vector3(killBlock.transform.position.x + 0.02f, killBlock.transform.position.y, killBlock.transform.position.z);
		vibMinusX = new Vector3(killBlock.transform.position.x - 0.04f, killBlock.transform.position.y, killBlock.transform.position.z);
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
			killBlock.transform.position = vibPlusX; 
			killBlock.transform.position = vibMinusX;
			killBlock.transform.position = vibPlusX;
		} else {
			killBlock.collider.enabled = false;
		}
		active = false;

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

	/*
	void OnTriggerExit (Collider collider) {
		if (collider.gameObject.tag == "Player") {
			//Debug.Log ("Exit");
			active = false;
		}

	}
	*/
	/*
	IEnumerator switchKill () {
		yield return new WaitForSeconds(2f);
		if (kill == true) {
			kill = false;
		}
	}
	*/
}
