using UnityEngine;
using System;
using System.Collections;

public class SelectionScreenController : MonoBehaviour {

	public TextMesh Ability1TextMesh;
	public TextMesh Ability2TextMesh;
	public TextMesh Ability3TextMesh;
	public TextMesh Ability4TextMesh;
	public TextMesh CurrentPowerTextMesh;
	public TextMesh CurrentPowerPlus1TextMesh;
	public TextMesh CurrentPowerPlus2TextMesh;
	public TextMesh CurrentPowerMinus1TextMesh;
	public TextMesh CurrentPowerMinus2TextMesh;
	public TextMesh CurrentPowerMinus3TextMesh;

	public SpriteRenderer Arrow1;
	public SpriteRenderer Arrow2;
	public SpriteRenderer Arrow3;
	public SpriteRenderer Arrow4;
	public SpriteRenderer Highlight;

	public enum SlotSprite
	{
		Dash = 0,
		GravShift = 1,
		JumperCable = 2,
		LaserShield = 3,
		Magnet = 4,
		MoveLeft = 5,
		MoveRight = 6,
		Rocket = 7,
		Thermostat = 8,
		Invalid = 9,
	}
	
	public SpriteRenderer[] slot1Sprites = new SpriteRenderer[ Enum.GetNames( typeof( SlotSprite ) ).Length - 1 ];
	public SpriteRenderer[] slot2Sprites = new SpriteRenderer[ Enum.GetNames( typeof( SlotSprite ) ).Length - 1 ];
	public SpriteRenderer[] slot3Sprites = new SpriteRenderer[ Enum.GetNames( typeof( SlotSprite ) ).Length - 1 ];
	public SpriteRenderer[] slot4Sprites = new SpriteRenderer[ Enum.GetNames( typeof( SlotSprite ) ).Length - 1 ];
}