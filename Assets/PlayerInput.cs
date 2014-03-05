using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public Animator anim;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey("left"))
		{
			anim.SetBool("Left", true);
		}
		if(Input.GetKey("right"))
		{
			anim.SetBool("Right", true);
		}
		if(Input.GetKey("space"))
		{
			anim.SetBool("Jump", true);
		}

		if(Input.GetKeyUp("left"))
		{
			anim.SetBool("Left", false);
		}
		if(Input.GetKeyUp("right"))
		{
			anim.SetBool("Right", false);
		}
		if(Input.GetKeyUp("space"))
		{
			anim.SetBool("Jump", false);
		}
	}
}
