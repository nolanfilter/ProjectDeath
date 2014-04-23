using UnityEngine;
using System.Collections;

public class CheckpointBehavior : MonoBehaviour {

	float timer;
	public Animator animLeft;
	public Animator animRight;
	public GameObject checkpoint;

	// Use this for initialization
	void Start () {
		checkpoint.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(timer > 2.5)
		{
			animLeft.SetBool("checkSet", true);
			animRight.SetBool("checkSet", true);
			checkpoint.SetActive(true);
			enabled = false;
		}
	}

	void OnTriggerStay (Collider collider) {
		if(collider.tag == "Player")
		{
			animRight.SetBool("press", true);
			timer += Time.deltaTime;
		}
	}

	void OnTriggerExit (Collider collider) {
		if(collider.tag == "Player")
		{
			animRight.SetBool("press", false);
			timer = 0;
		}
	}
}
