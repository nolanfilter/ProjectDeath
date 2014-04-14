﻿using UnityEngine;
using System.Collections;

public class AreaEffects : MonoBehaviour {

	public GameObject vignetteScreen;
	public float vignettingAmount;
	public float tintR;
	public float tintG;
	public float tintB;

	public GameObject leaving;
	public GameObject entering;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerExit (Collider collider) {
		vignetteScreen.GetComponent<VignetteControl>().amount = new Color(tintR, tintG, tintB, vignettingAmount);
		leaving.GetComponent<AreaCoverControl>().on = true;
		entering.GetComponent<AreaCoverControl>().on = false;
	}
}
