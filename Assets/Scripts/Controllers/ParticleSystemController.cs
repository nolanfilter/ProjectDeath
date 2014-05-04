using UnityEngine;
using System.Collections;

public class ParticleSystemController : MonoBehaviour {

	private ParticleSystem particleSystem = null;

	void Awake()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}

	void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine( "PlayParticleSystem" );
	}

	private IEnumerator PlayParticleSystem()
	{
		if( particleSystem == null )
			yield break;

		particleSystem.Stop();
		particleSystem.Clear();
		particleSystem.Play();

		yield return new WaitForSeconds( particleSystem.duration );

		particleSystem.Stop();

		gameObject.SetActive( false );
	}
}
