using UnityEngine;
using System.Collections;

public class ColdController : MonoBehaviour {

	public float rate;
	public GameObject[] coldBlocks;	
	public float maxVal;

	private float count;
	private ColdSender blockScript;
	private bool check;


	// Use this for initialization
	void Start () {
		rate *= 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject block in coldBlocks) {
			blockScript = block.GetComponent<ColdSender>();
			if (blockScript.active == true) {
				check = true;
			} else {
				if (count > 0) {
					check = false;
					count -= 15f * Time.deltaTime;
				}
			}
			if (count >= maxVal) {
				blockScript.kill = true;
			}
			if (count < maxVal) {
				blockScript.kill = false;
			}
		}
		if (check) {
			count += rate;
		}
		Debug.Log(count);
	}

}
