#pragma strict

function Start () {

}

function Update () {
	renderer.material.color = new Color(0,0,0, Mathf.Abs((Mathf.Sin(Time.time * 2)) * .5f + 2f) / 7);
}