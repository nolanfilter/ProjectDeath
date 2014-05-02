using UnityEngine;
using System.Collections;

public class LightSwitchController : MonoBehaviour {
	
	public GameObject mainLight;
	private LightController selfLight;

	private LightController lightControl;

	// Use this for initialization
	void Start () {
		lightControl = mainLight.GetComponent<LightController>();
		selfLight = GetComponent<LightController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (selfLight.switchOther) {
			lightControl.enabled = true;
		}
	}
}
