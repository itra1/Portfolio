using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using com.ootii.Cameras;

namespace it.Game.PlayMaker.Camera
{
  [ActionCategory("Camera controller")]
  [HutongGames.PlayMaker.Tooltip("Переключание на фиксированное положение камеры")]
  public class CameraToTargetPosition : CameraControllerBase
  {
	 public FsmGameObject _target;
	 public FsmBool _rotation = true;

	 private CopyTransformFromTarget _motion;

	 public override void OnEnter()
	 {
		base.OnEnter();
		_motion = _controller.GetMotor<CopyTransformFromTarget>("FixedPosition");
		_motion.Target = _target.Value.transform;
		Confirm(_motion);
	 }


  }
}