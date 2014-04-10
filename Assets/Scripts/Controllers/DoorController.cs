using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {

	public GameObject block1; //top block
	public GameObject block2; //middle block
	public GameObject block3; //bottom block
	public float speed;
	public bool openSaysMe;  //have this be modified by outside changes

	private Vector3 origin;
	private Vector3 topPoint;
	private SpriteRenderer sprite1;
	private SpriteRenderer sprite2;
	private SpriteRenderer sprite3;
	//private bool openSaysMe;

	// Use this for initialization
	void Start () {
		origin = transform.position;
		topPoint = new Vector3 (origin.x, origin.y + 1.3f, origin.z);
		sprite1 = block1.GetComponent<SpriteRenderer>();
		sprite2 = block2.GetComponent<SpriteRenderer>();
		sprite3 = block3.GetComponent<SpriteRenderer>();
		speed = (speed * 0.03f);

	}
	
	// Update is called once per frame
	void Update () {


		if (openSaysMe == true) {
			if (block3.transform.position.y <= topPoint.y) {
				block1.transform.position += new Vector3(0f, speed, 0f);
				block2.transform.position += new Vector3(0f, speed, 0f);
				block3.transform.position += new Vector3(0f, speed, 0f);
			}
		}

		if (block1.transform.position.y >= topPoint.y) {
			sprite1.enabled = false;
		}
		if (block2.transform.position.y >= topPoint.y) {
			sprite2.enabled = false;
		}
	}
}
