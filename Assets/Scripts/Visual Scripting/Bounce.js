#pragma strict



function Start () {

}

function Update () {
	transform.position.y += Mathf.Sin(Time.time) * .005;
}