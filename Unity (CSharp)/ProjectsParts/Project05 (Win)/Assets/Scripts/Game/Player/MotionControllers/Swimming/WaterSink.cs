using System;
using UnityEngine;
using com.ootii.Actors.AnimationControllers;
using com.ootii.Cameras;
using com.ootii.Helpers;
using com.ootii.Geometry;
using com.ootii.Timing;
using InputManagerEntry = com.ootii.Helpers.InputManagerEntry;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Water Sink")]
  public class WaterSink : MotionControllerMotion
  {

	 public const int PHASE_UNKNOWN = 0;
	 public const int PHASE_START = 1840;

	 protected SwimInfo mSwimmerInfo = null;
	 private Game.Player.PlayerBehaviour _playerBehaviour;

	 public WaterSink()
	 : base()
	 {
		_Pack = Idle.GroupName();
		_Category = EnumMotionCategories.DEATH;

		_Priority = 99;
		_ActionAlias = "";
		_OverrideLayers = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Utilities-SM"; }
#endif
	 }

	 public WaterSink(MotionController rController)
		  : base(rController)
	 {
		_Pack = Idle.GroupName();
		_Category = EnumMotionCategories.DEATH;

		_Priority = 99;
		_ActionAlias = "";

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Utilities-SM"; }
#endif
	 }

	 public override void Awake()
	 {
		base.Awake();
		_playerBehaviour = mMotionController.GetComponent<PlayerBehaviour>();
	 }

	 public override bool TestActivate()
	 {
		if (it.Game.Managers.GameManager.Instance.LocationManager.IsSwim)
		  return false;

		if (!mIsStartable)
		{
		  return false;
		}
		if (mSwimmerInfo == null) { mSwimmerInfo = SwimInfo.GetSwimmerInfo(mMotionController._Transform); }

		if (mSwimmerInfo.GetDepth() > 1.3f)
		  return true;

		return false;
	 }

	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		Game.Managers.GameManager.Instance.UserManager.Health.Damage(mMotionController, 1000);

		//CameraBehaviour.Instance.CameraController.Anchor = null;
		var motor = CameraBehaviour.Instance.CameraController.GetMotor<com.ootii.Cameras.LookAtTargetMotor>();
		motor.Position = CameraBehaviour.Instance.transform.position;
		motor.Target = PlayerBehaviour.Instance.transform;
		CameraBehaviour.Instance.CameraController.ActivateMotor(motor);


		return base.Activate(rPrevMotion);
	 }

  }
}