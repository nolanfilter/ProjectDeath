using UnityEngine;
using System.Collections;

public class TeachingController : MonoBehaviour {
	
	public InputController.ButtonType button = InputController.ButtonType.Invalid;
	public string functionName = "";

	public bool isRepeatableAction = false;

	private RoutineAgent.RoutineInfo routineInfo;

	void Start()
	{
		routineInfo = new RoutineAgent.RoutineInfo( button, functionName, isRepeatableAction );
	}

	public RoutineAgent.RoutineInfo GetRoutineInfo()
	{
		return routineInfo;
	}
}
