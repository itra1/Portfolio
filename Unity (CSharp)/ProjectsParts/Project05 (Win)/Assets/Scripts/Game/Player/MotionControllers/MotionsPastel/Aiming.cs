using UnityEngine;
using com.ootii.Actors.AnimationControllers;
using com.ootii.Actors.Combat;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Messages;
using com.ootii.MotionControllerPacks;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Aiming")]
  public class Aiming : MotionControllerMotion
  {

	 public const int PHASE_UNKNOWN = 0;

	 private bool _isAiming;

	 public bool IsAiming => _isAiming;

	 private HoldItem _holdMotion;

	 /// <summary>
	 /// Default constructor
	 /// </summary>
	 public Aiming()
		  : base()
	 {
		_Pack = Idle.GroupName();
		_Category = EnumMotionCategories.AIMING;

		_Priority = 100;
		_ActionAlias = "Aiming";
		_OverrideLayers = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = ""; }
#endif
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public Aiming(MotionController rController)
		  : base(rController)
	 {
		_Pack = Idle.GroupName();
		_Category = EnumMotionCategories.AIMING;

		_Priority = 100;
		_ActionAlias = "Aiming";

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = ""; }
#endif
	 }

	 public override bool TestUpdate()
	 {
		if (!mMotionController.IsGrounded)
		  return false;

		if (mMotionController._InputSource.IsPressed(_ActionAlias))
		  return true;

		return false;
	 }


	 public override bool TestInterruption(MotionControllerMotion rMotion)
	 {
		if (rMotion is HoldItem)
		  return false;

		return false;
	 }
	 public override bool TestActivate()
	 {
		if (!mIsStartable) { return false; }
		if (!mMotionController.IsGrounded) { return false; }

		if (_holdMotion == null)
		  _holdMotion = mMotionController.GetMotion<HoldItem>();

		if (_holdMotion.HoldenItem == null || _holdMotion.HoldenItem.HoldStey == 0)
		  return false;

		if (mMotionController._InputSource.IsPressed(_ActionAlias))
		  return true;

		return false;
	 }
	 public override void Deactivate()
	 {
		// Finish the deactivation process
		base.Deactivate();
		_isAiming = false;
		mMotionController.Animator.SetBool("Aiming", _isAiming);
	 }

	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		_isAiming = true;
		mMotionController.Animator.SetBool("Aiming", _isAiming);

		// Return
		return base.Activate(rPrevMotion);
	 }
#if UNITY_EDITOR
	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;


		string lNewActionAlias = EditorGUILayout.TextField(new GUIContent("Action Alias", "Action alias that triggers a climb."), ActionAlias, GUILayout.MinWidth(30));
		if (lNewActionAlias != ActionAlias)
		{
		  lIsDirty = true;
		  ActionAlias = lNewActionAlias;
		}

		return lIsDirty;
	 }
#endif

  }
}