using UnityEngine;
using System.Collections;

public class ColdController : MonoBehaviour {

	public float rate;
	public GameObject[] coldBlocks;	
	public float maxVal;

	private float count;
	private ColdSender blockScript;
	//private bool check;
	//private float match = 0f;


	// Use this for initialization
	void Start () {
		rate *= 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject block in coldBlocks) {
			blockScript = block.GetComponent<ColdSender>();
			if (blockScript.active == true) {
				count += rate * 2f; //alter temp increase rate here
				//Debug.Log ("Active");
			} else {
				//Debug.Log ("inactive");
				if (count > 0) {
					count -= rate * 1.5f * Time.deltaTime; //increase drop of temperature here
				}
			}
			if (count >= maxVal) {
				blockScript.kill = true;
				StartCoroutine (resetCount());
			}
			if (count < maxVal) {
				blockScript.kill = false;
			}
		}
		/*
		if (match > 0) {
			check = true;
		}
		if (match <= 0) {
			check = false;
		}
		if (check) {
			count += rate;
		} else {
			if (count > 0) {
				count -= 15f * Time.deltaTime;
			}
		}
		*/
		//Debug.Log(count); //watch rates of increase and decrease here
	}

	IEnumerator resetCount() {
		yield return new WaitForSeconds (.5f);
		count = 0f;
	}

}
