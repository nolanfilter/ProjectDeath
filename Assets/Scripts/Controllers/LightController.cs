using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour {

	//take note: the angle of the raycast is not completely dependent on the angle
	//use the debug to find your direction while placing and match accordingly

	public float timing;
	public float distance = 1f;
	public float angle;
	public float rays;
	public bool stationary = true;
	public float moveAngle;
	public float angleTiming;
	public GameObject camBlock;
	public float fadeRate = 5f;
	public bool switchOther = false;


	private bool flash = false;
	private bool active = false;
	private bool set = false;
	private bool activeM = false;
	private bool setM = false;
	private float rad;
	private Vector3 rayVector;
	private float moveRad;
	private float rayAngle;
	private float alphaNum = 0f;
	private Color blockColor;
	//private LightContControl lightCont;
	//private Shader blockShader;
	//private Vector3 eulerOrigin;
	//private Vector3 eulerTarget;



	// Use this for initialization
	void Start () {
		rad = angle * Mathf.Deg2Rad;
		moveRad = moveAngle * Mathf.Deg2Rad;
		if (!stationary) {
			StartCoroutine (RotateWaitLoop ());
		}
		camBlock.renderer.material.shader = Shader.Find("Transparent/Diffuse");
		//lightCont = GetComponent<LightContControl>();
	}
	
	// Update is called once per frame
	void Update () {
		if (setM == false) {
			//Debug.Log ("rotate");
			transform.Rotate (Vector3.forward * Time.deltaTime * moveAngle);
		}

		rayAngle = (transform.eulerAngles.z * Mathf.Deg2Rad);
		for (int i = 0; i < rays; i++) 
		{
			rayVector = new Vector3(Mathf.Cos(rayAngle + (rad/rays) * i),Mathf.Sin(rayAngle + (rad/rays) * i), 0f);
			//rayVector = new Vector3(Mathf.Cos((transform.eulerAngles.z * Mathf.Deg2Rad) + (rad / rays) * i), Mathf.Sin((transform.eulerAngles.z * Mathf.Deg2Rad) + (rad / rays) * i), 0f);
			//Debug.Log (rayVector);
			Ray ray = new Ray(transform.position, rayVector);
			RaycastHit rayHit;
			//Debug.DrawRay (transform.position,rayVector); //comment this out when ready to show
			if (Physics.Raycast (ray,out rayHit, distance)) {
				if (rayHit.collider.tag == "Player") {
					switchOther = true;
					flash = true;
				}
			}
		}

		if (alphaNum > 0f) {
			alphaNum -= (fadeRate * 0.01f);
		}
		//Debug.Log (alphaNum);
		Color tempColor = camBlock.renderer.material.color;
		tempColor.a = alphaNum;
		camBlock.renderer.material.color = tempColor;
		//Debug.Log (camBlock.renderer.material.color.a);

		if (active == true) {
			if (set == true) {
				if (flash) {
					//Debug.Log ("Flash");
					//put visual blocking code with object either above in if-statements or down here after activation
					alphaNum = 1f;
				}
				StartCoroutine(waitCall (timing));
				set = false;
			}
		} 
		if (active == false) {
			if (set == false) {
				StartCoroutine(waitCall (timing));
				set = true;
			}
		}

	}

	
	IEnumerator waitCall(float waitTime) {
		//Debug.Log ("wait call");
		//Debug.Log ("About to wait " + waitTime);
		yield return new WaitForSeconds(waitTime);
		//Debug.Log ("Waited " + waitTime);
		if (active == true) {
			active = false;
		} else {
			active = true;
		}

		flash = false;

		//Debug.Log ("end wait");
		//Debug.Log (Time.time);
	}

	private IEnumerator RotateWaitLoop()
	{
		while (true) {
			//Debug.Log ("wait Called");
			//float beginTime = Time.time;
			if (activeM == false) {
				//Debug.Log ("set false");
				moveAngle *= -1f;
				setM = false;
			} else {
				//Debug.Log ("set true");
				moveAngle = moveAngle;
				setM = true;
			}
			//transform.Rotate(Vector3.forward * Time.deltaTime * moveAngle);
			yield return new WaitForSeconds(angleTiming);
			activeM = !activeM;
		}

	}
}
