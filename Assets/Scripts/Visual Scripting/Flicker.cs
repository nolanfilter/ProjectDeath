using UnityEngine;
using System.Collections;

public class Flicker : MonoBehaviour {

	float flickerSpeed;
	float newBrightness;
	SpriteRenderer lightSprite;
	public Light lightSource;

	// Use this for initialization
	void Start () {
		flickerSpeed = Random.Range(.05f,.25f);
		newBrightness = Random.Range(.5f,1f);
		lightSprite = GetComponent<SpriteRenderer>();
		if(lightSprite != null && lightSource != null)
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
			lightSource.intensity = newBrightness * 2;
			newBrightness = Random.Range(.2f,1f);
			flickerSpeed = Random.Range(.03f,.3f);
			yield return new WaitForSeconds (flickerSpeed);
			
			lightSprite.color = new Color(1,1,1,newBrightness);
			lightSource.intensity = newBrightness * 2;
			newBrightness = Random.Range(.5f,1f);
			flickerSpeed = Random.Range(.01f,.1f);
			yield return new WaitForSeconds (flickerSpeed);

		}
	}
}
