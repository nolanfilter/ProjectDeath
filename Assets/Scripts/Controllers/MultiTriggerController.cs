using UnityEngine;
using System.Collections;

public class MultiTriggerController : MonoBehaviour {

	public GameObject[] objects;

	private bool switches = false;
		
	private MultiTriggerReceivers itemScript;

	public Animator anim;

	//public bool activate = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider collider) {


		if(objects[0].gameObject.tag == "Death")
		{
			if(objects[0].activeSelf)
			{
				objects[0].SetActive(false);
			}
			else if(!objects[0].activeSelf)
			{
				objects[0].SetActive(true);
			}
		}

		if (collider.gameObject.tag == "Player") {
			anim.SetBool("Pressed", true);
			//Debug.Log ("Player");
			//switches = true;
			foreach (GameObject item in objects) {
				itemScript = item.GetComponent<MultiTriggerReceivers>();
				if ( itemScript != null && itemScript.activate == false) {
					itemScript.activate = true;
					itemScript.inactive = false;
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
				if ( itemScript != null && itemScript.rise == true) {
					itemScript.rise = false;
					itemScript.inactive = false;
				}
				/*
				if (itemScript.lockGate == true) {
					Debug.Log ("Unlocking");
					itemScript.lockGate = false;
				}
				*/
				if ( itemScript != null && itemScript.holdButton == false) {
					itemScript.holdButton = true;
					itemScript.inactive = false;
				}
			}
		}
	}

	void OnTriggerExit (Collider collider) {
		if (collider.gameObject.tag == "Player") {
			anim.SetBool("Pressed", false);
		}
	}
}
