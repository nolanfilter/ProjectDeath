using UnityEngine;
using System.Collections;

public class AnimatorController : MonoBehaviour {

	public Animator anim;
	
	void OnEnable()
	{
		AnimationAgent.RegisterAnimator( anim );
	}

	void OnDisable()
	{
		AnimationAgent.RegisterAnimator( anim );
	}
}
