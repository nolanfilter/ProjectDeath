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

	private bool up = true;

	private int currentLevel;

	// Use this for initialization
	void Start () {
		currentLevel = 0;
		speed = (speed * 0.03f);
		origin = new Vector3(elevatorBlock.transform.position.x, elevatorBlock.transform.position.y,elevatorBlock.transform.position.z);
		//Debug.Log (levels.Length);
	}
	
	// Update is called once per frame
	void Update () {
		if (!playerOn) {
			elevatorBlock.transform.position = Vector3.MoveTowards(elevatorBlock.transform.position, origin, (speed * 2));

			//SoundAgent.PlayClip(SoundAgent.SoundEffects.Elevator,1f, false,gameObject);
			/*
			for (int i = 0; i < levels.Length; i++) {
				if ((levels[i].transform.position.y - player.transform.position.y) <= 1f) {
					Debug.Log (i);
					elevatorBlock.transform.position = Vector3.MoveTowards(elevatorBlock.transform.position, levels[i].transform.position, speed);
				}
			}
			*/
		}
		if(playerOn)
		{

		}
	}

	void OnTriggerStay (Collider collider) {
		if (collider.gameObject.tag == "Player") {
			playerOn = true;


			if(currentLevel == levels.Length)
			{
				up = false;
			}

				if(up)
			{
				if (noMove == false) {
					elevatorBlock.transform.position = Vector3.MoveTowards (elevatorBlock.transform.position, levels[currentLevel].transform.position, speed);
					//SoundAgent.PlayClip(SoundAgent.SoundEffects.Elevator,1f, false,gameObject);
				}
				if (elevatorBlock.transform.position.y >= levels[currentLevel].transform.position.y) {
					noMove = true;
					if (set == false) {
							StartCoroutine (waitCall(delay));
					}
					if (set == true) {
						currentLevel ++;
						noMove = false;
						set = false;
					}
				}
				if(currentLevel == levels.Length)
				{
					up = false;
					currentLevel--;
				}
			}


			if(!up)
			{
				if (noMove == false) {
					elevatorBlock.transform.position = Vector3.MoveTowards (elevatorBlock.transform.position, levels[currentLevel].transform.position, speed);
					//SoundAgent.PlayClip(SoundAgent.SoundEffects.Elevator,1f, false,gameObject);
					hold = false;
					
				}
				if (elevatorBlock.transform.position.y <= levels[currentLevel].transform.position.y) {
					noMove = true;
					if (set == false) {
						if (hold == false) {
							StartCoroutine (waitCall(delay));
							
						}
					}
					if (set == true) {
						currentLevel --;
						hold = true;
						noMove = false;
						set = false;
					}
				}
				if(currentLevel == -1)
				{
					up = true;
					currentLevel++;
				}
			}



		}
	}

	void OnTriggerExit (Collider collider) {
		playerOn = false;
		set = false;
	 	hold = false;
		active = true;
		noMove = false;
	}

	IEnumerator waitCall (float waitTime) {
		yield return new WaitForSeconds(waitTime);

		if (set == false) {
			set = true;
		}
	}
}
