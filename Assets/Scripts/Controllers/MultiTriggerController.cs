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
		/*
		if (switches == true) {
			foreach (GameObject item in objects) {
				item.activate = true;
			}
		}
		*/
	}

	void OnTriggerEnter(Collider Collider) {
		if (collider.gameObject.tag == "Player") {
			//switches = true;
			foreach (GameObject item in objects) {
				itemScript = item.GetComponent<MultiTriggerReceivers>();
				itemScript.activate = true;
			}
		}
	}
	
}
