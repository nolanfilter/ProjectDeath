using UnityEngine;
using System.Collections;

public class AreaCoverControl : MonoBehaviour {

	public bool on;

	private Color newColor;

	/*
	// Use this for initialization
	void Start () {
		on = true;
		if(transform.name == "Janitor")
		{
			on = false;
		}
	}
	*/
	
	// Update is called once per frame
	void Update () {
		if(!on)
		{
			newColor = Color.Lerp(renderer.material.color, new Color(1,1,1,1), Time.deltaTime * .3f);
		}
		if(on)
		{
			newColor = Color.Lerp(renderer.material.color, new Color(0,0,0,0), Time.deltaTime * .8f);
		}

		renderer.material.color = newColor;
	}
}
