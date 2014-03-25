using UnityEngine;
using System.Collections;

public class RoutineAgent : MonoBehaviour {

	public struct RoutineInfo
	{
		public InputController.ButtonType button { get; private set; }
		public string functionName { get; private set; }
		
		public bool isRepeatableAction { get; private set; }

		public RoutineInfo( InputController.ButtonType newButton = InputController.ButtonType.Invalid, string newFunctionName = "", bool newIsRepeatableAction = false )
		{
			button = newButton;
			functionName = newFunctionName;
			isRepeatableAction = newIsRepeatableAction;
		}
	}

	private static RoutineAgent mInstance = null;
	public static RoutineAgent instance
	{
		get
		{
			return mInstance;
		}
	}
	
	void Awake()
	{
		if( mInstance != null )
		{
			Debug.LogError( string.Format( "Only one instance of RoutineAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;
	}
}
