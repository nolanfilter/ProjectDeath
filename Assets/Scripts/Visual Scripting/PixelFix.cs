using UnityEngine;
using System.Collections;

public class PixelFix : MonoBehaviour {


	public void Awake()
		
	{
		camera.orthographicSize = (Screen.height / 100f / 2.0f); // 100f is the PixelPerUnit that you have set on your sprite. Default is 100.
		
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
