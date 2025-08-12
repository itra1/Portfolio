using System.Collections.Generic;
using UnityEngine;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Graphics;
using com.ootii.Utilities.Debug;
using com.ootii.Actors.AnimationControllers;
using it.Game.Items;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Hold Item")]
  [MotionDescription("Item hold")]
  public class HoldItem : MotionControllerMotion
  {

	 public int PHASE_UNKNOWN = 0;
	 public int PHASE_START = 5000;
	 public int PHASE_IDLE = 5001;
	 public int PHASE_LEFT_UP = 5010;

	 private it.Game.Items.HoldenItem _holdenItem;
	 public HoldenItem HoldenItem { get => _holdenItem; set => _holdenItem = value; }

	 public bool IsHold { get => _holdenItem != null; }

	 public void SetItem(HoldenItem item)
	 {
		_holdenItem = item;
		mMotionController.Animator.SetInteger("HoldItem", _holdenItem.HoldStey);
		//Activate(mMotionController.ActiveMotion);
	 }

	 private void RemoveItem()
	 {

	 }

	 public HoldItem()
				: base()
	 {
		_Category = EnumMotionCategories.IMPACT;

		_Priority = 5;
		_Form = 0;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "HoldItem-SM"; }
#endif
	 }

	 public HoldItem(MotionController rController)
				: base(rController)
	 {
		_Category = EnumMotionCategories.IMPACT;

		_Priority = 5;
		_Form = 0;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "HoldItem-SM"; }
#endif
	 }

	 public override void Awake()
	 {
		base.Awake();


		//_PlayerHoldItem = mMotionController.GetComponent<GPlayer.PlayerHoldItem>();
	 }
	 public override bool TestActivate()
	 {
		if (!mIsStartable) { return false; }

		if (IsHold)
		  return true;

		return false;
	 }

	 public override bool TestUpdate()
	 {
		if (IsHold) return true;

		return false;
	 }
	 public void SetCustomHold(it.Game.Items.HoldenItem targetHoldItem)
	 {
		if (_holdenItem == targetHoldItem)
		  return;

		var target = targetHoldItem.GetComponentInChildren<it.Game.Player.Interactions.InteractionTarget>();
		var effector = mMotionController.GetComponent<PlayerBehaviour>().FullBodyBipedIK.solver.GetEffector(target.effectorType);

		Vector3 parentPosition = effector.bone.position;
		Quaternion rotation = effector.bone.rotation;

		effector.bone.transform.position = target.transform.position;
		effector.bone.transform.rotation = target.transform.rotation;
		targetHoldItem.transform.SetParent(effector.bone.transform);
		effector.bone.transform.position = parentPosition;
		effector.bone.transform.rotation = rotation;
		var poser = effector.bone.GetComponent<RootMotion.FinalIK.HandPoser>();
		poser.poseRoot = target.transform;
		poser.weight = 1;
		SetItem(targetHoldItem);
	 }

	 public void DropItem()
	 {
		_holdenItem.transform.SetParent(null);
		_holdenItem.Drop();
		PlayerBehaviour.Instance.InteractionSystem.ResumeAll();
		_holdenItem = null;
	 }

	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {

		_holdenItem.Hold();
		mMotionController.SetAnimatorMotionParameter(mMotionLayer.AnimatorLayerIndex, 2);
		if (rPrevMotion is RightHandUp)
		{
		  mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_IDLE, _Form, mParameter, true);
		}
		else
		{
		  mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_START, _Form, mParameter, true);
		}

		//var anim = _holdenItem.HoldenAnimation;

		//switch (anim)
		//{
		//  case HoldenItem.HoldenAnimationType.leftUp:
		//	 mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_LEFT_UP, _Form, mParameter, true);
		//	 break;
		//}

		// Finalize the activation
		return base.Activate(rPrevMotion);
	 }

	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		base.Update(rDeltaTime, rUpdateIndex);

		int lStateID = mMotionLayer._AnimatorStateID;
		float lStateTime = mMotionLayer._AnimatorStateNormalizedTime;

		int lTransitionID = mMotionLayer._AnimatorTransitionID;
		float lTransitionTime = mMotionLayer._AnimatorTransitionNormalizedTime;

		if (lStateID == STATE_HoldItem && lStateTime > 0.1f)
		{
		  mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_IDLE, _Form, mParameter, true);
		}

	 }

	 public override bool TestInterruption(MotionControllerMotion rMotion)
	 {
		if (rMotion is Aiming)
		  return true;
		if (rMotion is RightHandUp)
		  return true;

		if (_holdenItem != null)
		  return false;
		return true;
	 }

	 #region Auto-Generated
	 // ************************************ START AUTO GENERATED ************************************

	 /// <summary>
	 /// These declarations go inside the class so you can test for which state
	 /// and transitions are active. Testing hash values is much faster than strings.
	 /// </summary>
	 public int STATE_DefaultEmpty = -1;
	 public int STATE_LeftHandHold = -1;
	 public int STATE_HoldItem = -1;
	 public int STATE_HoldItemIdle = -1;
	 public int TRANS_AnyState_LeftHandHold = -1;
	 public int TRANS_EntryState_LeftHandHold = -1;
	 public int TRANS_AnyState_HoldItem = -1;
	 public int TRANS_EntryState_HoldItem = -1;
	 public int TRANS_HoldItem_HoldItemIdle = -1;

	 /// <summary>
	 /// Determines if we're using auto-generated code
	 /// </summary>
	 public override bool HasAutoGeneratedCode
	 {
		get { return true; }
	 }

	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsInMotionState
	 {
		get
		{
		  int lStateID = mMotionLayer._AnimatorStateID;
		  int lTransitionID = mMotionLayer._AnimatorTransitionID;

		  if (lTransitionID == 0)
		  {
			 if (lStateID == STATE_DefaultEmpty) { return true; }
			 if (lStateID == STATE_LeftHandHold) { return true; }
			 if (lStateID == STATE_HoldItem) { return true; }
			 if (lStateID == STATE_HoldItemIdle) { return true; }
		  }

		  if (lTransitionID == TRANS_AnyState_LeftHandHold) { return true; }
		  if (lTransitionID == TRANS_EntryState_LeftHandHold) { return true; }
		  if (lTransitionID == TRANS_AnyState_HoldItem) { return true; }
		  if (lTransitionID == TRANS_EntryState_HoldItem) { return true; }
		  if (lTransitionID == TRANS_HoldItem_HoldItemIdle) { return true; }
		  return false;
		}
	 }

	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsMotionState(int rStateID)
	 {
		if (rStateID == STATE_DefaultEmpty) { return true; }
		if (rStateID == STATE_LeftHandHold) { return true; }
		if (rStateID == STATE_HoldItem) { return true; }
		if (rStateID == STATE_HoldItemIdle) { return true; }
		return false;
	 }

	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsMotionState(int rStateID, int rTransitionID)
	 {
		if (rTransitionID == 0)
		{
		  if (rStateID == STATE_DefaultEmpty) { return true; }
		  if (rStateID == STATE_LeftHandHold) { return true; }
		  if (rStateID == STATE_HoldItem) { return true; }
		  if (rStateID == STATE_HoldItemIdle) { return true; }
		}

		if (rTransitionID == TRANS_AnyState_LeftHandHold) { return true; }
		if (rTransitionID == TRANS_EntryState_LeftHandHold) { return true; }
		if (rTransitionID == TRANS_AnyState_HoldItem) { return true; }
		if (rTransitionID == TRANS_EntryState_HoldItem) { return true; }
		if (rTransitionID == TRANS_HoldItem_HoldItemIdle) { return true; }
		return false;
	 }

	 /// <summary>
	 /// Preprocess any animator data so the motion can use it later
	 /// </summary>
	 public override void LoadAnimatorData()
	 {
		string lLayer = mMotionController.Animator.GetLayerName(mMotionLayer._AnimatorLayerIndex);
		TRANS_AnyState_LeftHandHold = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".HoldItem-SM.LeftHandHold");
		TRANS_EntryState_LeftHandHold = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".HoldItem-SM.LeftHandHold");
		TRANS_AnyState_HoldItem = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".HoldItem-SM.HoldItem");
		TRANS_EntryState_HoldItem = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".HoldItem-SM.HoldItem");
		STATE_DefaultEmpty = mMotionController.AddAnimatorName("" + lLayer + ".Default Empty");
		STATE_LeftHandHold = mMotionController.AddAnimatorName("" + lLayer + ".HoldItem-SM.LeftHandHold");
		STATE_HoldItem = mMotionController.AddAnimatorName("" + lLayer + ".HoldItem-SM.HoldItem");
		TRANS_HoldItem_HoldItemIdle = mMotionController.AddAnimatorName("" + lLayer + ".HoldItem-SM.HoldItem -> " + lLayer + ".HoldItem-SM.HoldItemIdle");
		STATE_HoldItemIdle = mMotionController.AddAnimatorName("" + lLayer + ".HoldItem-SM.HoldItemIdle");
	 }

#if UNITY_EDITOR

	 /// <summary>
	 /// New way to create sub-state machines without destroying what exists first.
	 /// </summary>
	 protected override void CreateStateMachine()
	 {
		int rLayerIndex = mMotionLayer._AnimatorLayerIndex;
		MotionController rMotionController = mMotionController;

		UnityEditor.Animations.AnimatorController lController = null;

		Animator lAnimator = rMotionController.Animator;
		if (lAnimator == null) { lAnimator = rMotionController.gameObject.GetComponent<Animator>(); }
		if (lAnimator != null) { lController = lAnimator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController; }
		if (lController == null) { return; }

		while (lController.layers.Length <= rLayerIndex)
		{
		  UnityEditor.Animations.AnimatorControllerLayer lNewLayer = new UnityEditor.Animations.AnimatorControllerLayer();
		  lNewLayer.name = "Layer " + (lController.layers.Length + 1);
		  lNewLayer.stateMachine = new UnityEditor.Animations.AnimatorStateMachine();
		  lController.AddLayer(lNewLayer);
		}

		UnityEditor.Animations.AnimatorControllerLayer lLayer = lController.layers[rLayerIndex];

		UnityEditor.Animations.AnimatorStateMachine lLayerStateMachine = lLayer.stateMachine;

		UnityEditor.Animations.AnimatorStateMachine lSSM_867614 = MotionControllerMotion.EditorFindSSM(lLayerStateMachine, "HoldItem-SM");
		if (lSSM_867614 == null) { lSSM_867614 = lLayerStateMachine.AddStateMachine("HoldItem-SM", new Vector3(420, 40, 0)); }

		UnityEditor.Animations.AnimatorState lState_868046 = MotionControllerMotion.EditorFindState(lSSM_867614, "LeftHandHold");
		if (lState_868046 == null) { lState_868046 = lSSM_867614.AddState("LeftHandHold", new Vector3(380, 140, 0)); }
		lState_868046.speed = 1f;
		lState_868046.mirror = false;
		lState_868046.tag = "";
		lState_868046.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/HoldenItem.fbx", "LeftHandHold");

		UnityEditor.Animations.AnimatorState lState_N3103424 = MotionControllerMotion.EditorFindState(lSSM_867614, "HoldItem");
		if (lState_N3103424 == null) { lState_N3103424 = lSSM_867614.AddState("HoldItem", new Vector3(290, -30, 0)); }
		lState_N3103424.speed = 1f;
		lState_N3103424.mirror = false;
		lState_N3103424.tag = "";
		lState_N3103424.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/NineAnimations/HoldItems/HoldItem.anim", "HoldItem");

		UnityEditor.Animations.AnimatorState lState_N3103440 = MotionControllerMotion.EditorFindState(lSSM_867614, "HoldItemIdle");
		if (lState_N3103440 == null) { lState_N3103440 = lSSM_867614.AddState("HoldItemIdle", new Vector3(400, 40, 0)); }
		lState_N3103440.speed = 1f;
		lState_N3103440.mirror = false;
		lState_N3103440.tag = "";
		lState_N3103440.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/NineAnimations/HoldItems/HoldItemIdle.anim", "HoldItemIdle");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_867618 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_868046, 0);
		if (lAnyTransition_867618 == null) { lAnyTransition_867618 = lLayerStateMachine.AddAnyStateTransition(lState_868046); }
		lAnyTransition_867618.isExit = false;
		lAnyTransition_867618.hasExitTime = false;
		lAnyTransition_867618.hasFixedDuration = true;
		lAnyTransition_867618.exitTime = 0.75f;
		lAnyTransition_867618.duration = 0.25f;
		lAnyTransition_867618.offset = 0f;
		lAnyTransition_867618.mute = false;
		lAnyTransition_867618.solo = false;
		lAnyTransition_867618.canTransitionToSelf = false;
		lAnyTransition_867618.orderedInterruption = true;
		lAnyTransition_867618.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_867618.conditions.Length - 1; i >= 0; i--) { lAnyTransition_867618.RemoveCondition(lAnyTransition_867618.conditions[i]); }
		lAnyTransition_867618.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 5010f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_N3103460 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_N3103424, 0);
		if (lAnyTransition_N3103460 == null) { lAnyTransition_N3103460 = lLayerStateMachine.AddAnyStateTransition(lState_N3103424); }
		lAnyTransition_N3103460.isExit = false;
		lAnyTransition_N3103460.hasExitTime = false;
		lAnyTransition_N3103460.hasFixedDuration = true;
		lAnyTransition_N3103460.exitTime = 0.75f;
		lAnyTransition_N3103460.duration = 0.25f;
		lAnyTransition_N3103460.offset = 0f;
		lAnyTransition_N3103460.mute = false;
		lAnyTransition_N3103460.solo = false;
		lAnyTransition_N3103460.canTransitionToSelf = true;
		lAnyTransition_N3103460.orderedInterruption = true;
		lAnyTransition_N3103460.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_N3103460.conditions.Length - 1; i >= 0; i--) { lAnyTransition_N3103460.RemoveCondition(lAnyTransition_N3103460.conditions[i]); }
		lAnyTransition_N3103460.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 5000f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_N3103492 = MotionControllerMotion.EditorFindTransition(lState_N3103424, lState_N3103440, 0);
		if (lTransition_N3103492 == null) { lTransition_N3103492 = lState_N3103424.AddTransition(lState_N3103440); }
		lTransition_N3103492.isExit = false;
		lTransition_N3103492.hasExitTime = true;
		lTransition_N3103492.hasFixedDuration = true;
		lTransition_N3103492.exitTime = 0.5f;
		lTransition_N3103492.duration = 0.25f;
		lTransition_N3103492.offset = 0f;
		lTransition_N3103492.mute = false;
		lTransition_N3103492.solo = false;
		lTransition_N3103492.canTransitionToSelf = true;
		lTransition_N3103492.orderedInterruption = true;
		lTransition_N3103492.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_N3103492.conditions.Length - 1; i >= 0; i--) { lTransition_N3103492.RemoveCondition(lTransition_N3103492.conditions[i]); }


		// Run any post processing after creating the state machine
		OnStateMachineCreated();
	 }

#endif

	 // ************************************ END AUTO GENERATED ************************************
	 #endregion

  }

}