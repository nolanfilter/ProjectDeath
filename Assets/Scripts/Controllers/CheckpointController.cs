using UnityEngine;
using System.Collections;

public class CheckpointController : MonoBehaviour {

	public Vector3 checkpointPosition;
	public Vector3 spawnerPosition;
	public Animator animator;
	public GameObject areaCoverObject;

	void Start()
	{
		animator = transform.parent.parent.FindChild( "Screen" ).GetComponent<Animator>();
	}
}
