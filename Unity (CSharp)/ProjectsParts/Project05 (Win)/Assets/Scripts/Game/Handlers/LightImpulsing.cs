using UnityEngine;
using System.Collections;
namespace it.Game.Handles
{
  public class LightImpulsing : HandlersBase
  {

	 public float TimeMultipler = 0.2f;
	 public float RangeMultipler = 0.2f;
	 public bool isSinusoid;
	 Light _light;

	 private float _startIntensity;
	 Vector3 RndArg1;
	 void Start()
	 {
		_light = GetComponent<Light>();
		_startIntensity = _light.intensity;
		RndArg1 = Random.insideUnitSphere * 2;
	 }

	 protected override void OnUpdate()
	 {
		base.OnUpdate();
		var x = Time.time * TimeMultipler;
		float xPos = 0;
		if (isSinusoid)
		  xPos = Mathf.Sin(Mathf.Sin(x));
		else
		  xPos = Mathf.Sin(Mathf.Sin(x * RndArg1.x) + x * RndArg1.y) + Mathf.Cos(Mathf.Cos(x) * RndArg1.z + x);


		_light.intensity = _startIntensity + xPos * RangeMultipler;
	 }
  }
}