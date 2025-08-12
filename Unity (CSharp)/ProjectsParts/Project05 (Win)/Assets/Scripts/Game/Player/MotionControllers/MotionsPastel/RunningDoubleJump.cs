using System;
using UnityEngine;
using com.ootii.Actors.Navigation;
using com.ootii.Messages;
using com.ootii.Actors.AnimationControllers;
using com.ootii.Helpers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Running Double Jump")]
  [MotionDescription("Повторный прыжок в воздуха")]
  public class RunningDoubleJump : MotionControllerMotion
  {
	 public const int PHASE_START = 273000;
	 public const int PHASE_COMPLETE = 273001;

	 public float _Impulse = 7f;
	 public float Impulse
	 {
		get { return _Impulse; }
		set { _Impulse = value; }
	 }
	 protected Vector3 mLaunchVelocity = Vector3.zero;
	 protected Transform mHipBone = null;
	 public string _HipBoneName = "";
	 public float _MinFallHeight = 2f;
	 public float MinFallHeight
	 {
		get { return _MinFallHeight; }
		set { _MinFallHeight = value; }
	 }
	 public string HipBoneName
	 {
		get { return _HipBoneName; }

		set
		{
		  _HipBoneName = value;
		  if (mMotionController != null)
		  {
			 mHipBone = mMotionController.gameObject.transform.Find(_HipBoneName);
		  }
		}
	 }
	 public bool _ConvertToHipBase = true;
	 public bool ConvertToHipBase
	 {
		get { return _ConvertToHipBase; }
		set { _ConvertToHipBase = value; }
	 }
	 protected Vector3 mLaunchVelocityOverride = Vector3.zero;
	 public Vector3 LaunchVelocityOverride
	 {
		get { return mLaunchVelocityOverride; }
		set { mLaunchVelocityOverride = value; }
	 }
	 protected bool mIsExitTriggered = false;
	 protected float mLastHipDistance = 0f;
	 protected Vector3 mLaunchForward = Vector3.zero;
	 protected bool mIsImpulseApplied = false;
	 protected IWalkRunMotion mWalkRunMotion = null;
	 private float _forwardSpeed = 4;
	 public float ForwardSpeed { get => _forwardSpeed; set => _forwardSpeed = value; }
	 protected MotionControllerMotion mFall = null;

	 public RunningDoubleJump()
		  : base()
	 {
		_Priority = 16;
		_ActionAlias = "Jump";
		mIsStartable = true;
		//mIsGroundedExpected = false;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "RunningDoubleJump"; }
#endif
	 }

	 public RunningDoubleJump(MotionController rController)
		  : base(rController)
	 {
		_Priority = 16;
		_ActionAlias = "Jump";
		mIsStartable = true;
		//mIsGroundedExpected = false;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "RunningDoubleJump"; }
#endif
	 }
	 public override void Initialize()
	 {
		if (mMotionController != null)
		{
		  if (mWalkRunMotion == null) { mWalkRunMotion = mMotionController.GetMotionInterface<IWalkRunMotion>(); }

		  mFall = mMotionController.GetMotion("Fall");
		  if (mFall == null) { mFall = mMotionController.GetMotion<FallPastel>(); }
		}
	 }

	 public override bool TestActivate()
	 {
		if (!mIsStartable)
		{
		  return false;
		}

		if (mActorController.IsGrounded)
		{
		  return false;
		}

		if (Form >= 0 && Form != mMotionController.CurrentForm)
		{
		  return false;
		}

		if (mMotionController._InputSource == null)
		{
		  return false;
		}

		if (!mMotionController._InputSource.IsJustPressed(_ActionAlias))
		{
		  return false;
		}
		if ((mMotionController.ActiveMotion is ISlop))
		{
		  return false;
		}
		if (mActorController.State.Velocity.y < 0 && mActorController.State.GroundSurfaceDistance < 1)
		{
		  return false;
		}

		//if (mMotionController.State.InputForward.magnitude < 0.5f || Mathf.Abs(mMotionController.State.InputFromAvatarAngle) > 10f)
		//{
		//  return false;
		//}

		return true;
	 }

	 public override bool TestUpdate()
	 {
		if (mIsActivatedFrame) { return true; }

		//if (mIsImpulseApplied)
		//{
		//  return false;
		//}

		return true;
	 }

	 public override bool TestInterruption(MotionControllerMotion rMotion)
	 {
		if (rMotion is Fall || rMotion is FallPastel)
		{
		  if (mMotionLayer._AnimatorStateID == STATE_Glide_Complate)
			 return false;

		  if (mActorController.State.GroundSurfaceDistance < MinFallHeight)
		  {
			 return false;
		  }
		}
		if (rMotion is WalkRunPivot && !mActorController.State.IsGrounded)
		  return false;

		if (!mIsExitTriggered)
		  return false;

		return true;
	 }

	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		// Attempt to find the hip bone if we have a name
		if (_ConvertToHipBase)
		{
		  if (mHipBone == null)
		  {
			 if (_HipBoneName.Length > 0)
			 {
				mHipBone = mMotionController._Transform.Find(_HipBoneName);
			 }

			 if (mHipBone == null)
			 {
				mHipBone = mMotionController.Animator.GetBoneTransform(HumanBodyBones.Hips);
				if (mHipBone != null) { _HipBoneName = mHipBone.name; }
			 }
		  }
		}

		// Reset the distance flag for this jump
		mLastHipDistance = 0f;

		// Reset the impulse flag
		mIsImpulseApplied = false;
		mIsExitTriggered = false;

		Vector3 vel = mActorController.AccumulatedVelocity;
		vel.y = 0;
		mActorController.AccumulatedVelocity = vel;
		mLaunchVelocity = mActorController.State.Velocity;
		//mActorController.AccumulatedVelocity = Vector3.zero;
		//mVelocity = mLaunchVelocity;
		mVelocity = Vector3.zero;
		//mActorController.AddImpulse(mActorController._Transform.up * _Impulse);
		mActorController.AddImpulse(mLaunchVelocity*mActorController.Mass + mActorController._Transform.up * _Impulse);

		// Grab the current velocities
		//mLaunchForward = mActorController._Transform.forward;
		//mLaunchVelocity = mLaunchForward * ForwardSpeed * mMotionController.State.InputForward.magnitude;

		// Control whether we're walking or running
		mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START, true);

		// Flag this motion as active
		return base.Activate(rPrevMotion);
	 }

	 /// <summary>
	 /// Raised when we shut the motion down
	 /// </summary>
	 public override void Deactivate()
	 {
		// Continue with the deactivation
		mLaunchVelocityOverride = Vector3.zero;
		Debug.Log("Deactivate");
		base.Deactivate();
	 }
	 public override void UpdateRootMotion(float rDeltaTime, int rUpdateIndex, ref Vector3 rVelocityDelta, ref Quaternion rRotationDelta)
	 {
		rVelocityDelta = Vector3.zero;
		//rVelocityDelta = rVelocityDelta.normalized * (mLaunchVelocity.magnitude * Time.deltaTime);

		// Remove all velocity and rotation since we'll be using our physics to jump
		//rVelocityDelta = Vector3.zero;
		//rRotationDelta = Quaternion.identity;

		//if (mMotionLayer._AnimatorTransitionID == TRANS_RunningJump_RunJump_RunForward ||
		//	 mMotionLayer._AnimatorStateID == STATE_RunJump_RunForward)
		//{
		//  rVelocityDelta = rVelocityDelta.normalized * (mLaunchVelocity.magnitude * Time.deltaTime); // (Time.smoothDeltaTime > 0f ? Time.smoothDeltaTime : Time.deltaTime));
		//}
		//rRotationDelta = Quaternion.identity;

		//if (!IsInLandedState)
		//{
		//rVelocityDelta = Vector3.zero;
		//}
	 }

	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		//mVelocity = Vector3.zero;
		mMovement = Vector3.zero;
		float lHipDistanceDelta = 0f;

		if (rUpdateIndex != 1) { return; }

		if (_ConvertToHipBase && mHipBone != null)
		{
		  float lHipDistance = mHipBone.position.y - mMotionController._Transform.position.y;

		  lHipDistanceDelta = -(lHipDistance - mLastHipDistance);
		  mLastHipDistance = lHipDistance;
		}

		MotionState lState = mMotionController.State;

		int lStateID = mMotionLayer._AnimatorStateID;
		float lStateTime = mMotionLayer._AnimatorStateNormalizedTime;

		// On launch, add the impulse
		if (lStateID == STATE_Glide_Start )
		{
		  // If we haven't applied the impulse, do it now. 
		  if (!mIsImpulseApplied)
		  {
			 mIsImpulseApplied = true;

			 Debug.Log("_Impulse " + _Impulse);
			 return;
		  }

		  // If we're pushing into the ground, end the running jump
		  //if (!mIsExitTriggered && mIsImpulseApplied && lStateTime > 0.2f && lStateTime < 0.5f)
		  //{
		  //if (mActorController.State.IsGrounded)
		  //{
		  //mIsExitTriggered = true;
		  //mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_LAND_IDLE, true);
		  //}
		  //}

		  // As we come to the end, we have a couple of options
		  if (!mIsExitTriggered && lStateTime > 0.8f)
		  {
			 // If we're a long way from the ground, transition to a fall
			 if (mFall != null
				&& mFall.IsEnabled
				&& mActorController.State.GroundSurfaceDistance > _MinFallHeight
				&& lStateTime > 1.5f)
			 {
				mIsExitTriggered = true;
				mMotionController.ActivateMotion(mFall);
			 }
			 // If we're still getting input, keep running
			 else if (lState.InputMagnitudeTrend.Value >= 0.1f) // && Mathf.Abs(lState.InputFromAvatarAngle) < 10f)
			 {
				//Deactivate();
				if (mActorController.State.Velocity.y <= 0 && mActorController.State.GroundSurfaceDistance <= 1f)
				{
				  mIsExitTriggered = true;
				  //mWalkRunMotion.StartInRun = mWalkRunMotion.IsRunActive;
				  //mWalkRunMotion.StartInWalk = !mWalkRunMotion.StartInRun;
				  //mMotionController.ActivateMotion(mWalkRunMotion as MotionControllerMotion);
				  mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_COMPLETE, true);
				}
				//
			 }
			 //// Come to a quick stop
			 else if (mActorController.State.GroundSurfaceDistance <= 1f)
			 {
				mIsExitTriggered = true;
				Deactivate();
				mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_COMPLETE, true);
			 }
		  }
		  else if (mIsExitTriggered && lStateTime > 1.1f)
			 mIsExitTriggered = false;
		  // While in the jump, adjust the displacement
		  else
		  {
			 mMovement = mActorController._Transform.up * lHipDistanceDelta;
		  }
		}
		// Once we get into the run forward, we can transition to the true run
		else if (lStateID == STATE_Glide_Complate)
		{
		  if (lStateTime > 0.1f)
		  {
			 // It may be time to move into the walk/run
			 if (mWalkRunMotion != null && mWalkRunMotion.IsEnabled)
			 {
				mWalkRunMotion.StartInRun = mWalkRunMotion.IsRunActive;
				mWalkRunMotion.StartInWalk = !mWalkRunMotion.StartInRun;
				mMotionController.ActivateMotion(mWalkRunMotion as MotionControllerMotion);
			 }
			 else
			 {
				Deactivate();
			 }
		  }
		}
		// Once we get to the idle pose, we can deactivate
		//else if (lStateID == STATE_LandToIdle)
		//{
		//  mLaunchVelocity = Vector3.zero;
		//}
		else if (lStateID == STATE_IdlePose)
		{
		  Deactivate();
		}

	 }

	 /// <summary>
	 /// Raised by the controller when a message is received
	 /// </summary>
	 public override void OnMessageReceived(IMessage rMessage)
	 {
		if (rMessage == null) { return; }

		NavigationMessage lMessage = rMessage as NavigationMessage;
		if (lMessage != null)
		{
		  // Call for a climb test
		  if (rMessage.ID == NavigationMessage.MSG_NAVIGATE_JUMP)
		  {
			 if (!mIsActive && mMotionController.IsGrounded)
			 {
				if (mActorController.State.Velocity.magnitude >= 5f)
				{
				  rMessage.Recipient = this;
				  rMessage.IsHandled = true;

				  mMotionController.ActivateMotion(this);
				}
			 }
		  }
		}
	 }

#if UNITY_EDITOR

	 /// <summary>
	 /// Allow the motion to render it's own GUI
	 /// </summary>
	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;


		if (EditorHelper.IntField("Form", "Sets the LXMotionForm animator property to determine the animation for the motion. If value is < 0, we use the Actor Core's 'Default Form' state.", Form, mMotionController))
		{
		  lIsDirty = true;
		  Form = EditorHelper.FieldIntValue;
		}

		string lNewActionAlias = EditorGUILayout.TextField(new GUIContent("Action Alias", "Action alias that triggers a climb."), ActionAlias, GUILayout.MinWidth(30));
		if (lNewActionAlias != ActionAlias)
		{
		  lIsDirty = true;
		  ActionAlias = lNewActionAlias;
		}

		bool lNewConvertToHipBase = EditorGUILayout.Toggle(new GUIContent("Convert To Hip Base", "Determines if we apply the physics to the hip bone vs. feet."), ConvertToHipBase);
		if (lNewConvertToHipBase != ConvertToHipBase)
		{
		  lIsDirty = true;
		  ConvertToHipBase = lNewConvertToHipBase;
		}

		string lNewHipBoneName = EditorGUILayout.TextField(new GUIContent("Hip Bone", "Name of the hip bone for adjusting the jump root."), HipBoneName);
		if (lNewHipBoneName != HipBoneName)
		{
		  lIsDirty = true;
		  HipBoneName = lNewHipBoneName;
		}

		float lNewImpulse = EditorGUILayout.FloatField(new GUIContent("Impulse", "Strength of the jump as an instant force."), Impulse);
		if (lNewImpulse != Impulse)
		{
		  lIsDirty = true;
		  Impulse = lNewImpulse;
		}
		float lNewForwardSpeed = EditorGUILayout.FloatField(new GUIContent("Forward speed", "Forward speed"), ForwardSpeed);
		if (lNewForwardSpeed != ForwardSpeed)
		{
		  lIsDirty = true;
		  ForwardSpeed = lNewForwardSpeed;
		}

		float lNewFallDistance = EditorGUILayout.FloatField(new GUIContent("Min Fall Height", "Minimum distance before the jump turns into a fall."), MinFallHeight);
		if (lNewFallDistance != MinFallHeight)
		{
		  lIsDirty = true;
		  MinFallHeight = lNewFallDistance;
		}

		return lIsDirty;
	 }

#endif

	 #region Auto-Generated
	 // ************************************ START AUTO GENERATED ************************************

	 /// <summary>
	 /// These declarations go inside the class so you can test for which state
	 /// and transitions are active. Testing hash values is much faster than strings.
	 /// </summary>
	 public int STATE_Start = -1;
	 public int STATE_Glide_Start = -1;
	 public int STATE_IdlePose = -1;
	 public int STATE_Glide_Complate = -1;
	 public int TRANS_AnyState_Glide_Start = -1;
	 public int TRANS_EntryState_Glide_Start = -1;
	 public int TRANS_Glide_Start_Glide_Complate = -1;
	 public int TRANS_Glide_Complate_IdlePose = -1;

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
			 if (lStateID == STATE_Start) { return true; }
			 if (lStateID == STATE_Glide_Start) { return true; }
			 if (lStateID == STATE_IdlePose) { return true; }
			 if (lStateID == STATE_Glide_Complate) { return true; }
		  }

		  if (lTransitionID == TRANS_AnyState_Glide_Start) { return true; }
		  if (lTransitionID == TRANS_EntryState_Glide_Start) { return true; }
		  if (lTransitionID == TRANS_Glide_Start_Glide_Complate) { return true; }
		  if (lTransitionID == TRANS_Glide_Complate_IdlePose) { return true; }
		  return false;
		}
	 }

	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsMotionState(int rStateID)
	 {
		if (rStateID == STATE_Start) { return true; }
		if (rStateID == STATE_Glide_Start) { return true; }
		if (rStateID == STATE_IdlePose) { return true; }
		if (rStateID == STATE_Glide_Complate) { return true; }
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
		  if (rStateID == STATE_Start) { return true; }
		  if (rStateID == STATE_Glide_Start) { return true; }
		  if (rStateID == STATE_IdlePose) { return true; }
		  if (rStateID == STATE_Glide_Complate) { return true; }
		}

		if (rTransitionID == TRANS_AnyState_Glide_Start) { return true; }
		if (rTransitionID == TRANS_EntryState_Glide_Start) { return true; }
		if (rTransitionID == TRANS_Glide_Start_Glide_Complate) { return true; }
		if (rTransitionID == TRANS_Glide_Complate_IdlePose) { return true; }
		return false;
	 }

	 /// <summary>
	 /// Preprocess any animator data so the motion can use it later
	 /// </summary>
	 public override void LoadAnimatorData()
	 {
		string lLayer = mMotionController.Animator.GetLayerName(mMotionLayer._AnimatorLayerIndex);
		TRANS_AnyState_Glide_Start = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".RunningDoubleJump.Glide_Start");
		TRANS_EntryState_Glide_Start = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".RunningDoubleJump.Glide_Start");
		STATE_Start = mMotionController.AddAnimatorName("" + lLayer + ".Start");
		STATE_Glide_Start = mMotionController.AddAnimatorName("" + lLayer + ".RunningDoubleJump.Glide_Start");
		TRANS_Glide_Start_Glide_Complate = mMotionController.AddAnimatorName("" + lLayer + ".RunningDoubleJump.Glide_Start -> " + lLayer + ".RunningDoubleJump.Glide_Complate");
		STATE_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".RunningDoubleJump.IdlePose");
		STATE_Glide_Complate = mMotionController.AddAnimatorName("" + lLayer + ".RunningDoubleJump.Glide_Complate");
		TRANS_Glide_Complate_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".RunningDoubleJump.Glide_Complate -> " + lLayer + ".RunningDoubleJump.IdlePose");
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

		UnityEditor.Animations.AnimatorStateMachine lSSM_N518624 = MotionControllerMotion.EditorFindSSM(lLayerStateMachine, "RunningDoubleJump");
		if (lSSM_N518624 == null) { lSSM_N518624 = lLayerStateMachine.AddStateMachine("RunningDoubleJump", new Vector3(390, -100, 0)); }

		UnityEditor.Animations.AnimatorState lState_N2263372 = MotionControllerMotion.EditorFindState(lSSM_N518624, "Glide_Start");
		if (lState_N2263372 == null) { lState_N2263372 = lSSM_N518624.AddState("Glide_Start", new Vector3(430, 90, 0)); }
		lState_N2263372.speed = 1f;
		lState_N2263372.mirror = false;
		lState_N2263372.tag = "";
		lState_N2263372.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "Glide_Start");

		UnityEditor.Animations.AnimatorState lState_N2322496 = MotionControllerMotion.EditorFindState(lSSM_N518624, "IdlePose");
		if (lState_N2322496 == null) { lState_N2322496 = lSSM_N518624.AddState("IdlePose", new Vector3(920, -30, 0)); }
		lState_N2322496.speed = 1f;
		lState_N2322496.mirror = false;
		lState_N2322496.tag = "";
		lState_N2322496.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/IdleWalkRun.fbx", "Idle");

		UnityEditor.Animations.AnimatorState lState_N2470432 = MotionControllerMotion.EditorFindState(lSSM_N518624, "Glide_Complate");
		if (lState_N2470432 == null) { lState_N2470432 = lSSM_N518624.AddState("Glide_Complate", new Vector3(670, 90, 0)); }
		lState_N2470432.speed = 1f;
		lState_N2470432.mirror = false;
		lState_N2470432.tag = "";
		lState_N2470432.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "Glide_Complate");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_N2263386 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_N2263372, 0);
		if (lAnyTransition_N2263386 == null) { lAnyTransition_N2263386 = lLayerStateMachine.AddAnyStateTransition(lState_N2263372); }
		lAnyTransition_N2263386.isExit = false;
		lAnyTransition_N2263386.hasExitTime = false;
		lAnyTransition_N2263386.hasFixedDuration = true;
		lAnyTransition_N2263386.exitTime = 0.75f;
		lAnyTransition_N2263386.duration = 0.25f;
		lAnyTransition_N2263386.offset = 0f;
		lAnyTransition_N2263386.mute = false;
		lAnyTransition_N2263386.solo = false;
		lAnyTransition_N2263386.canTransitionToSelf = false;
		lAnyTransition_N2263386.orderedInterruption = true;
		lAnyTransition_N2263386.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_N2263386.conditions.Length - 1; i >= 0; i--) { lAnyTransition_N2263386.RemoveCondition(lAnyTransition_N2263386.conditions[i]); }
		lAnyTransition_N2263386.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 273000f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_N2470452 = MotionControllerMotion.EditorFindTransition(lState_N2263372, lState_N2470432, 0);
		if (lTransition_N2470452 == null) { lTransition_N2470452 = lState_N2263372.AddTransition(lState_N2470432); }
		lTransition_N2470452.isExit = false;
		lTransition_N2470452.hasExitTime = false;
		lTransition_N2470452.hasFixedDuration = true;
		lTransition_N2470452.exitTime = 0.8453608f;
		lTransition_N2470452.duration = 0.03434348f;
		lTransition_N2470452.offset = 0f;
		lTransition_N2470452.mute = false;
		lTransition_N2470452.solo = false;
		lTransition_N2470452.canTransitionToSelf = true;
		lTransition_N2470452.orderedInterruption = true;
		lTransition_N2470452.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_N2470452.conditions.Length - 1; i >= 0; i--) { lTransition_N2470452.RemoveCondition(lTransition_N2470452.conditions[i]); }
		lTransition_N2470452.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 273001f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_N2489406 = MotionControllerMotion.EditorFindTransition(lState_N2470432, lState_N2322496, 0);
		if (lTransition_N2489406 == null) { lTransition_N2489406 = lState_N2470432.AddTransition(lState_N2322496); }
		lTransition_N2489406.isExit = false;
		lTransition_N2489406.hasExitTime = true;
		lTransition_N2489406.hasFixedDuration = true;
		lTransition_N2489406.exitTime = 0.4642859f;
		lTransition_N2489406.duration = 0.04708204f;
		lTransition_N2489406.offset = 0f;
		lTransition_N2489406.mute = false;
		lTransition_N2489406.solo = false;
		lTransition_N2489406.canTransitionToSelf = true;
		lTransition_N2489406.orderedInterruption = true;
		lTransition_N2489406.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_N2489406.conditions.Length - 1; i >= 0; i--) { lTransition_N2489406.RemoveCondition(lTransition_N2489406.conditions[i]); }


		// Run any post processing after creating the state machine
		OnStateMachineCreated();
	 }

#endif

	 // ************************************ END AUTO GENERATED ************************************
	 #endregion


  }
}