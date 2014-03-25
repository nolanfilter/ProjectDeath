using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	float flickerSpeed;
	float newBrightness;
	SpriteRenderer lightSprite;

	// Use this for initialization
	void Start () {
		flickerSpeed = Random.Range(.05f,.1f);
		newBrightness = Random.Range(.5f,1f);
		lightSprite = GetComponent<SpriteRenderer>();
		if(lightSprite != null)
		{
			StartCoroutine("flickerSprite");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator flickerSprite(){	
		while(true)
		{
			lightSprite.color = new Color(1,1,1,newBrightness);
			newBrightness = Random.Range(.3f,.5f);
			flickerSpeed = Random.Range(.04f,.06f);
			yield return new WaitForSeconds (flickerSpeed);

		}
	}
}
