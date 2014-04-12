using UnityEngine;
using System.Collections;

public class EngineDeathCubeController : MonoBehaviour {

	//public GameObject engine;
	public bool active;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (active == false) {
			transform.collider.enabled = false;
		} else {
			transform.collider.enabled = true;
		}
	}
}
