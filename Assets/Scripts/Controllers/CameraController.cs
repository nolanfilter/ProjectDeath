using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform toFollow = null;
	public float zOffset = -10f;

	void Start()
	{
		if( toFollow == null )
		{
			enabled = false;
			return;
		}
	}

	void Update()
	{
		transform.position = toFollow.position + Vector3.forward * zOffset;
	}
}
