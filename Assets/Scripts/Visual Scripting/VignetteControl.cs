﻿using UnityEngine;
using System.Collections;

public class VignetteControl : MonoBehaviour {
	
	public Color amount;

	private Color change;
	// Use this for initialization
	void Start () {
		change = new Color(1f,1f,1f,.3f);
	}
	
	// Update is called once per frame
	void Update () {

		change = Color.Lerp(renderer.material.color, amount, Time.deltaTime * .3f);

		renderer.material.color = change;
	}
}
