﻿using UnityEngine;
using System.Collections;

public class CrusherController : MonoBehaviour {

	public float speed = 5f;
	public float topY = -3.5f;
	public float bottomY = -10.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.position += Vector3.up * speed * Time.deltaTime;

		if (transform.position.y >= topY) {
			speed = -5f;
		}
		if (transform.position.y <= bottomY) {
			speed = 5f;
		}

	}
}