using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy;
using it.Game.Player.MotionControllers.Motions;

namespace it.Game.Environment.All.FlightCorridor
{
  /// <summary>
  /// Коридор полета
  /// </summary>
  public class FlightCorridor : Environment
  {
	 [SerializeField]
	 private CurvySpline Spline = null;

	 [SerializeField]
	 private float _radiusMove = 0;

	 [SerializeField]
	 private FluffyUnderware.Curvy.Controllers.SplineController _splineController = null;
	 [SerializeField]
	 private float _speedMove = 1;
	 [SerializeField]
	 private float _speedOffset = 1;

	 private bool _isActiveFly = false;

	 /// <summary>
	 /// Игрок входит в корридор
	 /// </summary>
	 public void PlayerEnter()
	 {
		if (_isActiveFly)
		  return;

		_isActiveFly = true;
		var lookupPos = Spline.transform.InverseTransformPoint(Game.Player.PlayerBehaviour.Instance.transform.position);
		float nearestTF = Spline.GetNearestPointTF(lookupPos);

		_splineController.Position = nearestTF;

		_splineController.MovementDirection = nearestTF > 0.5f
		  ? FluffyUnderware.Curvy.Controllers.MovementDirection.Backward
		  : FluffyUnderware.Curvy.Controllers.MovementDirection.Forward;

		Game.Player.PlayerBehaviour.Instance.MotionController.GetMotion<FlyingPath>().Start(_splineController, _radiusMove, _speedOffset);
		_splineController.Speed = _speedMove;
	 }

	 /// <summary>
	 /// Плеер выходит из корридора
	 /// </summary>
	 public void PlayerExit()
	 {
		Game.Player.PlayerBehaviour.Instance.MotionController.GetMotion<FlyingPath>().Stop();
		_isActiveFly = false;
	 }

  }
}