using UnityEngine;
using System.Collections;
namespace it.Game.Handles
{
  public class SinusoidMoves : HandlersBase
  {
	 public float TimeMultipler = 0.2f;
	 public float RangeMultiplerX = 0.2f;
	 public float RangeMultiplerY = 0.2f;
	 public float RangeMultiplerZ = 0.2f;

	 Transform t;
	 Vector3 StartPosition;
	 Vector3 RndArg1;
	 Vector3 RndArg2;
	 Vector3 RndArg3; 
	 
	 void Start()
	 {
		t = GetComponent<Transform>();
		StartPosition = t.localPosition;
		//RndArg1 = Random.insideUnitSphere * 2;
		//RndArg2 = Random.insideUnitSphere * 2;
		//RndArg3 = Random.insideUnitSphere * 2;
	 }
	 protected override void OnUpdate()
	 {
		base.OnUpdate();
		var x = Time.time * TimeMultipler;
		var xPos = Mathf.Sin(Mathf.Sin(x));
		var yPos = Mathf.Sin(Mathf.Sin(x));
		var zPos = Mathf.Sin(Mathf.Sin(x));
		t.localPosition = StartPosition + new Vector3(xPos* RangeMultiplerX, yPos* RangeMultiplerY, zPos* RangeMultiplerZ);
	 }
  }
}