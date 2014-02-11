using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform toFollow = null;
	public float zOffset = -10f;
	public float speed = 0.5f;

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
		transform.position = Vector3.MoveTowards( transform.position, toFollow.position + Vector3.forward * zOffset, speed );
	}
}
