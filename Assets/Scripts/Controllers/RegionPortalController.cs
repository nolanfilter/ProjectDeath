using UnityEngine;
using System.Collections;

public class RegionPortalController : MonoBehaviour {

	public RegionAgent.RegionType leaving;
	public RegionAgent.RegionType entering;
	
	void OnTriggerExit (Collider collider) 
	{
		RegionAgent.DarkenRegion( leaving );
		RegionAgent.LightenRegion( entering );
		RegionAgent.SetVignetteAmount( entering );
	}
}
