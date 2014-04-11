using UnityEngine;
using System.Collections;

public class CameraSystemHookController : MonoBehaviour {

	private Rect hook;

	void Start()
	{
		hook = new Rect( transform.position.x - transform.localScale.x * 0.5f, transform.position.y - transform.localScale.y * 0.5f, transform.localScale.x, transform.localScale.y );
		CameraSystemAgent.RegisterHook( hook );
	}

	void OnDestroy()
	{
		CameraSystemAgent.UnregisterHook( hook );
	}
}
