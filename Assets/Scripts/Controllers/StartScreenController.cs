using UnityEngine;
using System.Collections;

public class StartScreenController : MonoBehaviour {

	public GameObject startText;
	public GameObject creditsText;
	public GameObject titleText;
	public GameObject creditsPrefab;

	private TextMesh start;
	private TextMesh credits;
	private TextMesh title;

	private int select = 0;
	private bool creditsShown = false;
	private GameObject creditsSet;

	// Use this for initialization
	void Start () {
		start = startText.GetComponent<TextMesh>();
		credits = creditsText.GetComponent<TextMesh>();
		title = titleText.GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.RightArrow) && select < 1) {
			select++;
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow) && select > 0) {
			select--;
		}

		if (!creditsShown) {
			if (select == 0) {
				start.characterSize = 1.2f;
				credits.characterSize = 1f;
				if (Input.GetKeyDown (KeyCode.KeypadEnter)) {
					Application.LoadLevel("MaybeTheGameMaybe");
				}
			}
			if (select == 1) {
				credits.characterSize = 1.2f;
				start.characterSize = 1f;
				if (Input.GetKeyDown (KeyCode.Return)) {
					creditsShown = true;
					start.text = "";
					credits.text = "";
					title.text = "";
					creditsSet = Instantiate(creditsPrefab, new Vector3 (0f, 0f, 0f), Quaternion.identity) as GameObject; //instantiate credits object
				}
				
			}
		}

		if (creditsShown) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				Destroy(creditsSet); //remove credits object
				creditsShown = false;
				start.text = "Start";
				credits.text = "Credits";
				title.text = "Die, Robot";
			}
		}
	}
}
