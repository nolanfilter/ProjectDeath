using UnityEngine;
using System.Collections;

public class ElevatorController : MonoBehaviour {
	
	public GameObject elevatorBlock;
	public GameObject player;
	public GameObject[] levels; 
	public float delay;
	public float speed;


	private bool playerOn = false;
	private bool set = false;
	private bool hold = false;
	private bool active = true;
	private bool noMove = false;
	private Vector3 origin;

	// Use this for initialization
	void Start () {
		speed = (speed * 0.03f);
		origin = new Vector3(elevatorBlock.transform.position.x, elevatorBlock.transform.position.y,elevatorBlock.transform.position.z);
		//Debug.Log (levels.Length);
	}
	
	// Update is called once per frame
	void Update () {
		if (playerOn == false) {
			elevatorBlock.transform.position = Vector3.MoveTowards(elevatorBlock.transform.position, origin, (speed * 2));
			/*
			for (int i = 0; i < levels.Length; i++) {
				if ((levels[i].transform.position.y - player.transform.position.y) <= 1f) {
					Debug.Log (i);
					elevatorBlock.transform.position = Vector3.MoveTowards(elevatorBlock.transform.position, levels[i].transform.position, speed);
				}
			}
			*/
		}
	}

	void OnTriggerStay (Collider collider) {
		if (collider.gameObject.tag == "Player") {
			playerOn = true;
			for (int i = 0; i<levels.Length; i++) {
				//Debug.Log (i);
				if (active == true) {
					if (noMove == false) {
						elevatorBlock.transform.position = Vector3.MoveTowards (elevatorBlock.transform.position, levels[i].transform.position, speed);
						hold = false;
					}
					if (elevatorBlock.transform.position.y >= levels[i].transform.position.y) {
						noMove = true;
						//Debug.Log (set);
						if (set == false) {
							if (hold == false) {
								StartCoroutine (waitCall(delay));
							}
						}
						//Debug.Log (set);
						if (set == true) {
							//Debug.Log ("reset");
							set = false;
							hold = true;
							noMove = false;
							i++;
						}
					}

				}
			}
		}
	}

	void OnTriggerExit (Collider collider) {
		playerOn = false;
	}

	IEnumerator waitCall (float waitTime) {
		yield return new WaitForSeconds(waitTime);
		//Debug.Log ("waited");
		if (set == false) {
			set = true;
		}
	}
}
