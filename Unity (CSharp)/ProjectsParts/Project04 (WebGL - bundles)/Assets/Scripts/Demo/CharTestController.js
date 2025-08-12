#pragma strict

var from : GameObject;
var to : GameObject;

var prefabs : GameObject[];

var objects : GameObject[];
var positions : Vector3[];

var count : int = 50;

protected var speed : float = 7;

function Start()
{
	InitializeObjects();
}

protected function GenerateNewPosition() : Vector3
{
	var from : Vector3 = from.transform.position;
	
	var to : Vector3 = to.transform.position;
	
	return Vector3(Random.Range(from.x, to.x), 0, Random.Range(from.z, to.z));
}

protected function InitializeObjects()
{
	objects = new GameObject[count];
	
	positions = new Vector3[count];
	
	for (var i = 0; i < count; i++)
	{
		var newObject : GameObject = Instantiate(prefabs[Random.Range(0, prefabs.length)]);
		
		newObject.transform.parent = transform;
		newObject.transform.position = GenerateNewPosition();
		
		positions[i] = GenerateNewPosition();
		
		objects[i] = newObject;
	}
}

function Update()
{
	for (var i = 0; i < count; i++)
	{
		var curObject : GameObject = objects[i];
		
		curObject.transform.position = Vector3.MoveTowards(curObject.transform.position, positions[i], Time.deltaTime * speed);
		
		if (Vector3.Distance(curObject.transform.position, positions[i]) < 0.001)
			positions[i] = GenerateNewPosition();
		else
			curObject.transform.LookAt(positions[i]);
	}
}