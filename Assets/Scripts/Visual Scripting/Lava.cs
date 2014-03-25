using UnityEngine;
using System.Collections;

public class Lava : MonoBehaviour {

	SpriteRenderer lightSprite;

	bool increase;

	// Use this for initialization
	void Start () {
		lightSprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		lightSprite.color = new Color(1,1,1, (Mathf.Sin(Time.time * 1.5f) * .5f + 2.5f) / 5);
	}
}
