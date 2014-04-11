using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

	public Transform toFollow = null;
	public float zOffset = -10f;

	private PlayerController controller;
	private List<Rect> containingHooks;
	
	void Start()
	{
		if( toFollow == null )
		{
			enabled = false;
			return;
		}

		controller = toFollow.GetComponent<PlayerController>();

		if( controller == null )
		{
			enabled = false;
			return;
		}
	}

	void Update()
	{
		transform.position = toFollow.position + Vector3.forward * zOffset;

		Vector2 point = new Vector2( transform.position.x, transform.position.y );

		containingHooks = CameraSystemAgent.GetContainingHooks( point );

		if( containingHooks.Count > 0f && !controller.GetIsMoving() )
		{
			float left = Mathf.Infinity;
			float right = -Mathf.Infinity;
			float top = -Mathf.Infinity;
			float bottom = Mathf.Infinity;

			for( int i = 0; i < containingHooks.Count; i++ )
			{
				float temp = containingHooks[i].x;

				if( temp < left )
					left = temp;

				temp = containingHooks[i].x + containingHooks[i].width;

				if( temp > right )
					right = temp;

				temp = containingHooks[i].y;

				if( temp < bottom )
					bottom = temp;

				temp = containingHooks[i].y + containingHooks[i].height;

				if( temp > top )
					top = temp;
			}

			Vector3 bottomLeft = camera.ScreenToWorldPoint( Vector3.zero );
			Vector3 topRight = camera.ScreenToWorldPoint( new Vector3( Screen.width, Screen.height, 0f ) );

			if( ( right - left ) > ( topRight.x - bottomLeft.x ) )
			{
				if( bottomLeft.x < left )
					transform.position -= Vector3.right * ( bottomLeft.x - left );

				if( topRight.x > right )
					transform.position -= Vector3.right * ( topRight.x - right );
			}

			if( ( top - bottom ) > ( topRight.y - bottomLeft.y ) )
			{
				if( bottomLeft.y < bottom )
					transform.position -= Vector3.up * ( bottomLeft.y - bottom );

				if( topRight.y > top )
					transform.position -= Vector3.up * ( topRight.y - top );
			}
		}
	}
}
