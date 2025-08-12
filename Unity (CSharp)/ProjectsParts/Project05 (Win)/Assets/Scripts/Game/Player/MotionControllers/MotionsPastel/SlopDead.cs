using System.Collections;

using com.ootii.Actors.Navigation;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Messages;
using com.ootii.Actors.AnimationControllers;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using com.ootii.Graphics;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Slop Dead Pastel")]
  [MotionDescription("Смерть при ударе об предмет при скольжении")]
  public class SlopDead : MotionControllerMotion, ISlop
  {

	 public const int PHASE_UNKNOWN = 0;

	 public int _BarrierLayers = 1;
	 public int BarrierLayers
	 {
		get { return _BarrierLayers; }
		set { _BarrierLayers = value; }
	 }

	 public SlopDead() : base()
	 {
		_Priority = 51;
		mIsStartable = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Slide-SM"; }
#endif
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public SlopDead(MotionController rController) : base(rController)
	 {
		_Priority = 51;
		mIsStartable = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Slide-SM"; }
#endif
	 }


	 public override bool TestActivate()
	 {
		// запускаемый
		if (!mIsStartable)
		  return false;

		// активно скольжение
		if (!(mMotionController.ActiveMotion is ISlop))
		  return false;

		return TestBarrier();
	 }

	 /// <summary>
	 /// Тесте на марьер
	 /// </summary>
	 /// <returns></returns>
	 private bool TestBarrier()
	 {
		// выстрел чуть выше земли
		Vector3 lRayStart = mActorController._Transform.position + (mActorController._Transform.up * 0.1f);
		Vector3 lRayDirection = mActorController._Transform.forward;
		float lRayDistance = 1;

		RaycastHit _hit;

		if (RaycastExt.SafeRaycast(lRayStart, lRayDirection, out _hit, lRayDistance, BarrierLayers, mActorController._Transform))
		{
		  bool colBool = DamageCollision(_hit);
		  if(colBool)	 return true;
		}

		lRayStart = mActorController._Transform.position + (mActorController._Transform.up * 0.5f);

		if (RaycastExt.SafeRaycast(lRayStart, lRayDirection, out _hit, lRayDistance, BarrierLayers, mActorController._Transform))
		{
		  bool colBool = DamageCollision(_hit);
		  if (colBool) return true;
		}

		lRayStart = mActorController._Transform.position + (mActorController._Transform.up * 1f);

		if (RaycastExt.SafeRaycast(lRayStart, lRayDirection, out _hit, lRayDistance, BarrierLayers, mActorController._Transform))
		{
		  bool colBool = DamageCollision(_hit);
		  if (colBool) return true;
		}

		lRayStart = mActorController._Transform.position + (mActorController._Transform.up * (mActorController.Height-0.1f));

		if (RaycastExt.SafeRaycast(lRayStart, lRayDirection, out _hit, lRayDistance, BarrierLayers, mActorController._Transform))
		{
		  bool colBool = DamageCollision(_hit);
		  if (colBool) return true;
		}

		return false;

	 }

	 public bool DamageCollision(RaycastHit hitData)
	 {
		float dot = mActorController._Transform.forward.Dot(hitData.normal);

		Debug.Log("Dot " + dot);
		return dot < -0.4;

	 }

	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		Debug.Log("Damage");

		SlopSlide_v2 ss = mMotionController.GetMotion<SlopSlide_v2>();
		ss.IsEnabled = false;
		SlopSlideJump sj = mMotionController.GetMotion<SlopSlideJump>();
		ss.IsEnabled = false;

		PlayerBehaviour.Damage(100);

		return false;
		//return base.Activate(rPrevMotion);
	 }

#if UNITY_EDITOR

	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;
		int lNewBarrierLayers = EditorHelper.LayerMaskField(new GUIContent("Barrier layers", "Барьер"), BarrierLayers);
		if (lNewBarrierLayers != BarrierLayers)
		{
		  lIsDirty = true;
		  BarrierLayers = lNewBarrierLayers;
		}
		EditorGUI.indentLevel--;

		return lIsDirty;
	 }

#endif

	 }
}