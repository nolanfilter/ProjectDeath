using UnityEngine;
using System.Collections;

public class EngineController : MonoBehaviour {

	public GameObject[] deathBlocks;
	public float waittime;

	private EngineDeathCubeController blockScript;
	private bool turnOn;

	// Use this for initialization
	void Start () {
		StartCoroutine (WaitLoop ());
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject block in deathBlocks) {
			blockScript = block.GetComponent<EngineDeathCubeController>();
			if (blockScript.active == false && turnOn == true) {
				blockScript.active = true;
			}
		}				 
	}

	private IEnumerator WaitLoop() {
		while (true) {
			turnOn = !turnOn;
			foreach (GameObject block in deathBlocks) {
				blockScript = block.GetComponent<EngineDeathCubeController>();
				blockScript.active = false;
			}	
			yield return new WaitForSeconds(waittime);
		}
	}
}
