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
	//end door variables

	//button variables
	private Vector3 lowPoint;
	public bool rise = true;
	public float delay;
	private bool delayPassed = false;
	private bool dropped = false;
	public bool lockGate = false;
	public bool holdButton = false;
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
				if (block3.transform.position.y <= topPoint.y) {
					block1.transform.position += new Vector3(0f, speed, 0f);
					block2.transform.position += new Vector3(0f, speed, 0f);
					block3.transform.position += new Vector3(0f, speed, 0f);
				}
			} else {
				if (block2.transform.position.y >= origin.y) {
					block1.transform.position -= new Vector3(0f, speed, 0f);
					block2.transform.position -= new Vector3(0f, speed, 0f);
					block3.transform.position -= new Vector3(0f, speed, 0f);
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

		holdButton = false;

		if (gameObject.tag == "Button") {
			if (activate == true) {
				if (lockGate == false) { // if lockGate is false, then the button will sink
					if ((transform.position.y >= lowPoint.y) && (dropped == false)) {
						transform.position -= new Vector3(0f,speed,0f);
						Debug.Log ("Falling");
					}
					//dropped = true;
					//Debug.Log (lockGate);
				}
			}
			if (holdButton == false) { // this makes it so the button won't jump up and down
				//Debug.Log ("past Hold");
				if ((rise = true) && (transform.position.y < origin.y)) { //makes the button rise
					//Debug.Log ("Calling wait");
					if (delayPassed == false) {
						Debug.Log (lockGate);
						StartCoroutine(waitCall(delay)); //delay coroutine so it doesn't pop up immediately
					} else {
						Debug.Log ("Rising");
						transform.position += new Vector3(0f,(speed/2),0f); //causes button to rise slower than it dropped
					}
					lockGate = true; //this seems to be the cause of the bug
							//moving this around will either cause the button to not rise up for only work intermittenly on passes
				}
				if (transform.position.y == origin.y) { //resets boolean variables for next press
					delayPassed = false;
					rise = false;
				}
			} // end button script
		}
	}

	IEnumerator waitCall (float waitTime) {
		//Debug.Log ("Wait Call");
		yield return new WaitForSeconds(waitTime);
		delayPassed = true;
		//Debug.Log ("Waited");
	}
}
