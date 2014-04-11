using UnityEngine;
using System.Collections;

public class MultiTriggerController : MonoBehaviour {

	public GameObject[] objects;

	private bool switches = false;
		
	private MultiTriggerReceivers itemScript;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "Player") {
			//Debug.Log ("Player");
			//switches = true;
			foreach (GameObject item in objects) {
				itemScript = item.GetComponent<MultiTriggerReceivers>();
				if (itemScript.activate == false) {
					itemScript.activate = true;
				}/* 
				else {
					itemScript.activate = false;
				}
				*/
			}
		}
	}

	void OnTriggerStay (Collider collider) {
		//Debug.Log ("OnTriggerStay");
		if (collider.gameObject.tag == "Player") {
			foreach (GameObject item in objects) {
				itemScript = item.GetComponent<MultiTriggerReceivers>();
				if (itemScript.rise == true) {
					itemScript.rise = false;
				}
				/*
				if (itemScript.lockGate == true) {
					Debug.Log ("Unlocking");
					itemScript.lockGate = false;
				}
				*/
				if (itemScript.holdButton == false) {
					itemScript.holdButton = true;
				}
			}
		}
	}
}
