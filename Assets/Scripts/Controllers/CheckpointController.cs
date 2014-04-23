using UnityEngine;
using System.Collections;

public class CheckpointController : MonoBehaviour {

	public Vector3 checkpointPosition;
	public Vector3 spawnerPosition;
	public RegionAgent.RegionType region;
	private Animator animator;

	void Start()
	{
		animator = transform.parent.parent.FindChild( "Screen" ).GetComponent<Animator>();
	}

	public Animator GetAnimator()
	{
		return animator;
	}
}
