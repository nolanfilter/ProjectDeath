using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraSystemHookController)), CanEditMultipleObjects]
public class DrawRectEditor : Editor {

	void OnSceneGUI()
	{
		Transform targetTransform = ((CameraSystemHookController)target).transform;

		Vector3[] verts = new Vector3[] {
			targetTransform.position - Vector3.right * targetTransform.localScale.x * 0.5f + Vector3.up * targetTransform.localScale.y * 0.5f, 
			targetTransform.position + Vector3.right * targetTransform.localScale.x * 0.5f + Vector3.up * targetTransform.localScale.y * 0.5f,  
			targetTransform.position + Vector3.right * targetTransform.localScale.x * 0.5f - Vector3.up * targetTransform.localScale.y * 0.5f, 
			targetTransform.position - Vector3.right * targetTransform.localScale.x * 0.5f - Vector3.up * targetTransform.localScale.y * 0.5f,
		};

		Handles.DrawSolidRectangleWithOutline( verts, Color.clear, Color.cyan );
	}
}