using UnityEngine;
using System.Collections;

public class MenuGlow : MonoBehaviour {

	SpriteRenderer lightSprite;

	bool increase;

	// Use this for initialization
	void Start () {
		lightSprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		lightSprite.color = new Color(1,1,1, (Mathf.Sin(Time.time * 3) * .8f + 4f) / 5);
	}
}
