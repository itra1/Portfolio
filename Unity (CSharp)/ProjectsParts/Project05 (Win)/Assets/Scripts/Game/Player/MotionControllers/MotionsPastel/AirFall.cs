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
  [MotionName("Air fall")]
  public class AirFall : MotionControllerMotion
  {
	 public const int PHASE_UNKNOWN = 0;
	 public const int PHASE_START = 1840;

	 public int _fallLayer = 1;
	 public int FallLayer
	 {
		get { return _fallLayer; }
		set { _fallLayer = value; }
	 }

	 public AirFall()
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

	 public AirFall(MotionController rController)
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

	 public override bool TestActivate()
	 {

		if (!mIsStartable) return false;

		if (GetDepth() > 1.3f) return true;

		return false;
	 }

	 public float GetDepth()
	 {
		float lDepth = -1f;

		RaycastHit lHitInfo;
		Vector3 lStart = mActorController._Transform.position + (Vector3.up * 10);

		Transform airSurface;

		if (RaycastExt.SafeRaycast(lStart, Vector3.down, out lHitInfo, 10, FallLayer, mActorController._Transform, null, false))
		{
		  airSurface = lHitInfo.collider.gameObject.transform;
		  lDepth = airSurface.position.y - mActorController._Transform.position.y;
		}

		return lDepth;
	 }

	 public override bool TestUpdate()
	 {
		if (mActorController.IsGrounded)
		  return false;

		return true;
	 }


	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		Game.Managers.GameManager.Instance.UserManager.Health.Damage(mMotionController, 1000);

		//CameraBehaviour.Instance.CameraController.Anchor = null;
		var motor = CameraBehaviour.Instance.CameraController.GetMotor<com.ootii.Cameras.LookAtTargetMotor>();
		motor.Position = CameraBehaviour.Instance.transform.position;
		motor.Target = PlayerBehaviour.Instance.transform;
		CameraBehaviour.Instance.CameraController.ActivateMotor(motor);
		DarkTonic.MasterAudio.MasterAudio.PlaySound("PastelVote");
		//DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransform("PastelVote", mActorController.Transform);


		return base.Activate(rPrevMotion);
	 }

#if UNITY_EDITOR
	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;

		int lNewFallLayer = EditorHelper.LayerMaskField(new GUIContent("Fall layer", "Fall layer"), FallLayer);
		if (lNewFallLayer != FallLayer)
		{
		  lIsDirty = true;
		  FallLayer = lNewFallLayer;
		}

		return lIsDirty;
	 }

#endif

  }
}