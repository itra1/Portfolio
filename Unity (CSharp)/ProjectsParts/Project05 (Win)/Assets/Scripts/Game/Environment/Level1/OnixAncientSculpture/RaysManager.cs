using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.Level1.OnixAncientSculpture
{
  public class RaysManager : MonoBehaviourBase
  {

	 [SerializeField]
	 private Game.Environment.Handlers.RayTripleDrawner[] _hornRays = new Game.Environment.Handlers.RayTripleDrawner[0];
	 [SerializeField]
	 private GameObject[] _hornSphere = new GameObject[0];
	 [SerializeField]
	 private Game.Environment.Handlers.RayTripleDrawner _gateRay = null;
	 [SerializeField]
	 private GameObject _gateSphere = null;

	 [SerializeField]
	 private GameObject _lightPoint = null;

	 [SerializeField]
	 private AnimationCurve _sphereScalingAnimate;
	 [SerializeField]
	 private AnimationCurve _sphereCenterScalingAnimate;

	 private void Start()
	 {
		SetInvisible();
	 }

	 public void Show()
	 {
		SetVisible();
	 }

	 public void Hide()
	 {
		SetInvisible();
	 }

	 public void SetVisible(bool force = false)
	 {
		if (force)
		{
		  ForceActivate();
		  return;
		}

		for (int i = 0; i < _hornSphere.Length; i++)
		{
		  int index = i;
		  _hornSphere[index].gameObject.SetActive(true);
		  Game.Utils.ScalingAnimatedToCurve.Play(_hornSphere[index], 3, _sphereScalingAnimate, () =>
		  {
			 _hornRays[index].gameObject.SetActive(true);
			 _hornRays[index].StartVisualLine();

		  });
		}

		_lightPoint.SetActive(true);


		InvokeSeconds(() =>
		{
		  _gateSphere.gameObject.SetActive(true);
		  Game.Utils.ScalingAnimatedToCurve.Play(_gateSphere, 3, _sphereCenterScalingAnimate, () =>
		  {
			 _gateRay.gameObject.SetActive(true);
			 _gateRay.StartVisualLine();
		  });
		}, 3);


	 }
	 private void ForceActivate()
	 {
		for (int i = 0; i < _hornSphere.Length; i++)
		{
		  int index = i;
		  _hornSphere[index].gameObject.SetActive(true);
		  _hornRays[index].gameObject.SetActive(true);
		  _hornRays[index].StartVisualLine();
		  Game.Utils.ScalingAnimatedToCurve.Play(_hornSphere[index], 0.1f, _sphereScalingAnimate, null);
		}
		_lightPoint.SetActive(true);
		_gateSphere.gameObject.SetActive(true);
		Game.Utils.ScalingAnimatedToCurve.Play(_gateSphere, 0.1f, _sphereCenterScalingAnimate, null);
		_gateRay.gameObject.SetActive(true);
		_gateRay.StartVisualLine();
	 }

	 private void SetInvisible()
	 {
		foreach (var elem in _hornSphere)
		  elem.gameObject.SetActive(false);
		foreach (var elem in _hornRays)
		  elem.gameObject.SetActive(false);
		_lightPoint.SetActive(false);
		_gateRay.gameObject.SetActive(false);
		_gateSphere.gameObject.SetActive(false);
	 }

  }
}