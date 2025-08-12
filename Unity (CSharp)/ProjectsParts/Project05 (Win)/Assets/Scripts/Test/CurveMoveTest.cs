using System;
using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy.Generator;
using FluffyUnderware.DevTools;
using FluffyUnderware.Curvy.Utils;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using FluffyUnderware.Curvy.Controllers;
using FluffyUnderware.Curvy;

namespace Test
{
  public class CurveMoveTest : MonoBehaviourBase
  {

	 public CurvySpline Spline;
	 private float nearestTF = 0;
	 public Vector3 offset;

	 private void Start()
	 {
		var lookupPos = Spline.transform.InverseTransformPoint(transform.position);
		nearestTF = Spline.GetNearestPointTF(lookupPos);
	 }

	 private void Update()
	 {
		//if (nearestTF == 1)
		//  return;

		//nearestTF += 0.03f * Time.deltaTime;
		transform.localPosition = offset;

		//transform.position = Spline.Interpolate(nearestTF);
		//transform.position = Spline.transform.TransformPoint(Spline.Interpolate(nearestTF));
		//transform.rotation = Quaternion.Euler(Spline.GetTangentFast(nearestTF));
		//com.ootii.Graphics.GraphicsManager.DrawLine(transform.position, tanget, Color.green, null, 0.1f);
	 }

  }


}