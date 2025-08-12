#pragma strict

var cam : Camera;

protected var speed : float = 50.0;
protected var sizeSpeed : float = 10.0;

function Start()
{
	
}

function Update()
{
	
}

function OnGUI()
{
	GUILayout.BeginVertical();
	
		GUILayout.BeginHorizontal();
			if (GUILayout.RepeatButton("Up"))
				cam.transform.localPosition.z -= Time.deltaTime * speed;
		GUILayout.EndHorizontal();	
		
		GUILayout.BeginHorizontal();
		
		if (GUILayout.RepeatButton("Left"))
			cam.transform.localPosition.x += Time.deltaTime * speed;
	
		if (GUILayout.RepeatButton("Right"))
			cam.transform.localPosition.x -= Time.deltaTime * speed;
			
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			if (GUILayout.RepeatButton("Down"))
				cam.transform.localPosition.z += Time.deltaTime * speed;
		GUILayout.EndHorizontal();	
		
		GUILayout.BeginHorizontal();
		if (GUILayout.RepeatButton("Rotate Left"))
			transform.localRotation.eulerAngles.y += Time.deltaTime * speed;
		if (GUILayout.RepeatButton("Rotate Right"))
			transform.localRotation.eulerAngles.y -= Time.deltaTime * speed;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		if (GUILayout.RepeatButton("+"))
			cam.orthographicSize += Time.deltaTime * sizeSpeed;
		if (GUILayout.RepeatButton("-"))
			cam.orthographicSize -= Time.deltaTime * sizeSpeed;
		GUILayout.EndHorizontal();
		
	GUILayout.EndVertical();
}