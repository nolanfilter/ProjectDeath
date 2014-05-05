#pragma strict

var colors:Color[];

var current:int;

var timer:float;

function Start () {
	current = 0;
	timer = 0;
}

function Update () {
	timer += Time.deltaTime;
	
	if(timer > .2)
	{
		renderer.material.color = colors[current];
		if(current < colors.Length - 1)
		{
			current ++;
		}
		else
		{
			current = 0;
		}
		timer = 0;
	}
}