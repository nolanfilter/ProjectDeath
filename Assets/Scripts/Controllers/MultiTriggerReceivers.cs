using UnityEngine;
using System.Collections;

public class MultiTriggerReceivers : MonoBehaviour {

	public bool activate; //receiving bool for the main controller

	public float speed;


	private Vector3 origin;
	
	//door variables
	public GameObject block1; //top block
	public GameObject block2; //middle block
	public GameObject block3; //bottom block
	private Vector3 topPoint;
	private SpriteRenderer sprite1;
	private SpriteRenderer sprite2;
	private SpriteRenderer sprite3;
	private bool doorMoved = false;
	//end door variables

	//button variables
	private Vector3 lowPoint;
	public bool rise = true;
	public float delay;
	private bool delayPassed = true;
	private bool delayCalled = true;
	//private bool dropped = false;
	//public bool lockGate = false;
	public bool holdButton = true;
	//end button variables


	// Use this for initialization
	void Start () {
		//Debug.Log (gameObject.tag);
		if (gameObject.tag == "Door") { //door start
			origin = transform.position;
			topPoint = new Vector3 (origin.x, origin.y + 1.3f, origin.z);
			sprite1 = block1.GetComponent<SpriteRenderer>();
			sprite2 = block2.GetComponent<SpriteRenderer>();
			sprite3 = block3.GetComponent<SpriteRenderer>();
			speed = (speed * 0.03f); //modify door speed
		}

		if (gameObject.tag == "Button") { //button start
			origin = transform.position;
			lowPoint = new Vector3 (origin.x, origin.y - 0.2f, origin.z);
			speed = (speed * 0.03f); //modify button speed
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.tag == "Door") {
			if (activate == true) {
				if (doorMoved == false) {
					//Debug.Log("Going Up");
					if (block3.transform.position.y <= topPoint.y) {
						block1.transform.position += new Vector3(0f, speed, 0f);
						block2.transform.position += new Vector3(0f, speed, 0f);
						block3.transform.position += new Vector3(0f, speed, 0f);
					}
				} else {
					//Debug.Log("Going Down");
					if (block2.transform.position.y >= origin.y) {
						block1.transform.position -= new Vector3(0f, speed, 0f);
						block2.transform.position -= new Vector3(0f, speed, 0f);
						block3.transform.position -= new Vector3(0f, speed, 0f);
					}
				}

				if (block3.transform.position.y >= topPoint.y) {
					//Debug.Log ("resetting");
					doorMoved = true;
					activate = false;
					//Debug.Log (activate);
				}
				if (block2.transform.position.y <= origin.y) {
					//Debug.Log ("resetting a");
					doorMoved = false;
					activate = false;
					//Debug.Log (activate);
				}
			}
			if (block1.transform.position.y >= topPoint.y) {
				sprite1.enabled = false;
			}
			if (block2.transform.position.y >= topPoint.y) {
				sprite2.enabled = false;
			}
			
			if (block1.transform.position.y <= topPoint.y) {
				sprite1.enabled = true;
			}
			if (block2.transform.position.y <= topPoint.y) {
				sprite2.enabled = true;
			}

		} //end door script


		if (gameObject.tag == "Button") {
			if (rise == false) {
					if (transform.position.y >= lowPoint.y){
						transform.position -= new Vector3(0f,speed,0f);
						//Debug.Log ("Falling");
					}
				rise = true;
				delayCalled = false;
				delayPassed = false;
				//Debug.Log (holdButton);
				//}
			}	
			else { // this makes it so the button won't jump up and down
				//Debug.Log (rise);
				if (transform.position.y <= origin.y) { //makes the button rise
					if (delayCalled == false) {
						if (delayPassed == false) {
							StartCoroutine(waitCall (delay)); //delay coroutine so it doesn't pop up immediately
							Debug.Log ("Waited");
						}
					}
					if (delayCalled == true) {
						Debug.Log ("change");
						holdButton = false;
						delayPassed = true;
					}
				}
				if (holdButton == false) {
					//Debug.Log ("up");
					if (transform.position.y < origin.y) { //resets boolean variables for next press
						transform.position += new Vector3(0f, (speed/2), 0f);
					}
				}
			}
		}// end button script
	}

	IEnumerator waitCall (float waitTime) {
		//Debug.Log ("Wait Call");
		yield return new WaitForSeconds(waitTime);
		if (delayCalled == false) {
			delayCalled = true;
			Debug.Log (delayCalled);
		}
	}
}
