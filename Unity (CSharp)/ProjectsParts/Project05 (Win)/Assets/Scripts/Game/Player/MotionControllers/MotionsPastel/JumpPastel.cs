using UnityEngine;
using com.ootii.Actors.Navigation;
using com.ootii.Cameras;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Messages;
using com.ootii.Actors.AnimationControllers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  /// <summary>
  /// Standing or running jump. The jump allows for control
  /// while in the air.
  /// </summary>
  [MotionName("Jump")]
  [MotionDescription("A physics based multi-part jump that allows the player to launch into the " +
					  "air and recover into the idle pose or a run. The jump is created so the avatar " +
					  "can jump as high as mass, gravity, and impulse allow.")]
  public class JumpPastel : MotionControllerMotion
  {
	 // Enum values for the motion
	 public const int PHASE_UNKNOWN = 0;

	 public const int PHASE_START = 251;
	 public const int PHASE_START_FALL = 250;
	 public const int PHASE_START_SHORT = 252;

	 public const int PHASE_LAUNCH = 201;
	 public const int PHASE_RISE = 202;
	 public const int PHASE_RISE_TO_TOP = 203;
	 public const int PHASE_TOP = 204;
	 public const int PHASE_TOP_TO_FALL = 205;
	 public const int PHASE_FALL = 206;
	 public const int PHASE_LAND = 207;
	 public const int PHASE_RECOVER_TO_IDLE = 208;
	 public const int PHASE_RECOVER_TO_MOVE = 209;
	 public const int PHASE_RUN = 210;
	 public const int PHASE_WALK = 211;

	 public const int PHASE_RUN_STOP = 272553;

	 /// <summary>
	 /// Stores hit info so we don't have to keep allocating
	 /// </summary>
	 //private static RaycastHit sHitInfo;

	 /// <summary>
	 /// The amount of force caused by the player jumping
	 /// </summary>
	 protected float _Impulse = 10f;
	 public float Impulse
	 {
		get { return _Impulse; }
		set { _Impulse = value; }
	 }

	 protected bool isFaling = false;
	 protected float _startFallHeight;
	 public float StartFallHeight
	 {
		get
		{
		  return _startFallHeight;
		}
		set
		{
		  isFaling = true;
		  _startFallHeight = value;
		}
	 }


	 /// <summary>
	 /// The physics jump creates a parabola that is typically based on the
	 /// feet. However, if the animation has the feet move... that could be an issue.
	 /// </summary>
	 public bool _ConvertToHipBase = false;
	 public bool ConvertToHipBase
	 {
		get { return _ConvertToHipBase; }
		set { _ConvertToHipBase = value; }
	 }

	 /// <summary>
	 /// Allows us to assign a hip bone for adjusting the jump height off of
	 /// the foot position
	 /// </summary>
	 public string _HipBoneName = "";
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

	 /// <summary>
	 /// Use the launch velocity throughout the jump
	 /// </summary>
	 public bool _IsMomentumEnabled = true;
	 public bool IsMomentumEnabled
	 {
		get { return _IsMomentumEnabled; }
		set { _IsMomentumEnabled = value; }
	 }

	 /// <summary>
	 /// Determines if the player can control the avatar movement
	 /// and rotation while in the air.
	 /// </summary>
	 public bool _IsControlEnabled = true;
	 public bool IsControlEnabled
	 {
		get { return _IsControlEnabled; }
		set { _IsControlEnabled = value; }
	 }

	 /// <summary>
	 /// When in air, the player can still move the avatar. This
	 /// value is the max speed the player can move the avatar by.
	 /// </summary>
	 public float _ControlSpeed = 2f;
	 public float ControlSpeed
	 {
		get { return _ControlSpeed; }
		set { _ControlSpeed = value; }
	 }

	 /// <summary>
	 /// If the value is great than 0, we'll do a check to see if there
	 /// is enough room to even attempt a jump. While in a jump, we'll cancel it
	 /// if there isn't enough room
	 /// </summary>
	 public float _RequiredOverheadDistance = 0f;
	 public float RequiredOverheadDistance
	 {
		get { return _RequiredOverheadDistance; }
		set { _RequiredOverheadDistance = value; }
	 }

	 /// <summary>
	 /// Maximum amount of time will stay in the jump
	 /// </summary>
	 public float _MaxJumpTime = 5f;
	 public float MaxJumpTime
	 {
		get { return _MaxJumpTime; }
		set { _MaxJumpTime = value; }
	 }

	 /// <summary>
	 /// Allows the caller to explicitly set the launch velocity to be used during the next activation
	 /// </summary>
	 protected Vector3 mLaunchVelocityOverride = Vector3.zero;
	 public Vector3 LaunchVelocityOverride
	 {
		get { return mLaunchVelocityOverride; }
		set { mLaunchVelocityOverride = value; }
	 }

	 /// <summary>
	 /// Determines if we rotate to match the camera
	 /// </summary>
	 public bool _RotateWithCamera = false;
	 public bool RotateWithCamera
	 {
		get { return _RotateWithCamera; }

		set
		{
		  _RotateWithCamera = value;

		  // Register this motion with the camera
		  if (mMotionController != null && mMotionController.CameraRig is BaseCameraRig)
		  {
			 ((BaseCameraRig)mMotionController.CameraRig).OnPostLateUpdate -= OnCameraUpdated;
			 if (_RotateWithCamera) { ((BaseCameraRig)mMotionController.CameraRig).OnPostLateUpdate += OnCameraUpdated; }
		  }
		}
	 }

	 /// <summary>
	 /// Input alias to determine if we rotate withthe camera
	 /// </summary>
	 public string _RotateWithCameraAlias = "ActivateRotation";
	 public string RotateWithCameraAlias
	 {
		get { return _RotateWithCameraAlias; }
		set { _RotateWithCameraAlias = value; }
	 }

	 /// <summary>
	 /// Desired degrees of rotation per second
	 /// </summary>
	 public float _RotationToCameraSpeed = 360f;
	 public float RotationToCameraSpeed
	 {
		get { return _RotationToCameraSpeed; }
		set { _RotationToCameraSpeed = value; }
	 }

	 /// <summary>
	 /// Forward the player was facing when they launched. It helps
	 /// us control the total rotation that can happen in the air.
	 /// </summary>
	 protected Vector3 mLaunchForward = Vector3.zero;

	 /// <summary>
	 /// Velocity at the time the character launches. This helps us with momentum
	 /// </summary>
	 protected Vector3 mLaunchVelocity = Vector3.zero;

	 /// <summary>
	 /// Transform for the hip to help adjust the height
	 /// </summary>
	 protected Transform mHipBone = null;

	 /// <summary>
	 /// Distance between the base and the hips
	 /// </summary>
	 protected float mLastHipDistance = 0f;

	 /// <summary>
	 /// Determines if the impulse has been applied or not
	 /// </summary>
	 protected bool mIsImpulseApplied = false;

	 /// <summary>
	 /// Connect to the move motion if we can
	 /// </summary>
	 protected IWalkRunMotion mWalkRunMotion = null;

	 /// <summary>
	 /// Frame level rotation test
	 /// </summary>
	 protected bool mRotateWithCamera = false;

	 /// <summary>
	 /// Determines if the actor rotation should be linked to the camera
	 /// </summary>
	 protected bool mLinkRotation = false;

	 protected bool _isStartFall;
	 protected bool _toIdle;
	 protected bool _toRun;

	 protected readonly float _timeJump = 0.4f;
	 protected float _isJumpTime;

	 protected bool _noExistJump;


	 /// <summary>
	 /// Default constructor
	 /// </summary>
	 public JumpPastel()
		  : base()
	 {
		_Form = -1;
		_Priority = 15;
		_ActionAlias = "Jump";
		mIsStartable = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Jump-SM"; }
#endif
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public JumpPastel(MotionController rController)
		  : base(rController)
	 {
		_Form = -1;
		_Priority = 15;
		_ActionAlias = "Jump";
		mIsStartable = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Jump-SM"; }
#endif
	 }

	 /// <summary>
	 /// Initialize is called after all the motions have been initialized. This allow us time to
	 /// create references before the motions start working
	 /// </summary>
	 public override void Initialize()
	 {
		if (mMotionController != null)
		{
		  if (mWalkRunMotion == null) { mWalkRunMotion = mMotionController.GetMotionInterface<IWalkRunMotion>(); }
		  //if (mWalkRunPivot == null) { mWalkRunPivot = mMotionController.GetMotion<WalkRunPivot>(); }
		  //if (mWalkRunPivot_v2 == null) { mWalkRunPivot_v2 = mMotionController.GetMotion<WalkRunPivot_v2>(); }
		  //if (mWalkRunStrafe == null) { mWalkRunStrafe = mMotionController.GetMotion<WalkRunStrafe>(); }
		  //if (mWalkRunStrafe_v2 == null) { mWalkRunStrafe_v2 = mMotionController.GetMotion<WalkRunStrafe_v2>(); }
		  //if (mWalkRunRotate == null) { mWalkRunRotate = mMotionController.GetMotion<WalkRunRotate>(); }
		  //if (mWalkRunRotate_v2 == null) { mWalkRunRotate_v2 = mMotionController.GetMotion<WalkRunRotate_v2>(); }
		}
	 }

	 /// <summary>
	 /// Awake is called after all objects are initialized so you can safely speak to other objects. This is where
	 /// reference can be associated.
	 /// </summary>
	 public override void Awake()
	 {
		base.Awake();
	 }

	 public override bool TestInterruption(MotionControllerMotion rMotion)
	 {
		return base.TestInterruption(rMotion);
	 }

	 /// <summary>
	 /// Tests if this motion should be started. However, the motion
	 /// isn't actually started.
	 /// </summary>
	 /// <returns></returns>
	 public override bool TestActivate()
	 {
		// If we're not startable, this is easy
		if (!mIsStartable)
		{
		  return false;
		}

		// Test to see if the form condition matches our current default form
		if (_Form >= 0 && mMotionController.CurrentForm != _Form)
		{
		  return false;
		}

		// If we're working as an NPC, this changes things a bit. We're being controlled
		// by our velocity. So, use that to determine if we're heading up.
		if (mActorController.UseTransformPosition)
		{
		  if (!mActorController.IsGrounded)
		  {
			 // If we're moving up, we're probably jumping
			 Vector3 lVerticalVelocity = Vector3.Project(mActorController.State.Velocity, mActorController._Transform.up);
			 if (Vector3.Dot(lVerticalVelocity, mActorController._Transform.up) > 0f)
			 {
				if (mMotionLayer.ActiveMotion == null ||
					 mMotionLayer.ActiveMotion.Category != EnumMotionCategories.CLIMB)
				{
				  return true;
				}
			 }
		  }

		  return false;
		}

		// If we're not grounded, this is easy
		if (!mActorController.IsGrounded)
		{
		  return false;
		}

		// Ensure we have input to test
		if (mMotionController._InputSource == null)
		{
		  return false;
		}

		if (mMotionController._InputSource.IsJustPressed(_ActionAlias))
		{
		  if (RaycastExt.SafeSphereCast(mActorController.transform.position + mActorController.transform.up * mActorController.Height, mActorController.transform.up, mActorController.BodyShapes[0].Radius/2, 0.2f, mActorController.CollisionLayers, mActorController._Transform))
			 return false;

			 // Perform an upward raycast to determine if something is overhead. If it is, we need
			 // to prevent or stop a jump
		  if (_RequiredOverheadDistance > 0)
			 {
				if (RaycastExt.SafeRaycast(mActorController._Transform.position, mActorController._Transform.up, _RequiredOverheadDistance, mActorController._CollisionLayers, mActorController._Transform))
				{
				  return false;
				}
			 }

			 // If we're not in the middle of a jump, let it happen
			 if (IsInLandedState || !IsInMotionState)
			 {
				return true;
			 }

		}

		return false;
	 }

	 /// <summary>
	 /// Tests if the motion should continue. If it shouldn't, the motion
	 /// is typically disabled
	 /// </summary>
	 /// <returns></returns>
	 public override bool TestUpdate()
	 {
		if (mIsActivatedFrame)
		{
		  return true;
		}

		if (_MaxJumpTime > 0f && mMotionLayer.ActiveMotionDuration > _MaxJumpTime)
		{
		  return false;
		}

		// If we're not in a jump motion stake, we need to get out
		if (mIsAnimatorActive && !IsInMotionState)
		{
		  // We also need to ensure we're not currently in a previous transition
		  if (mMotionLayer._AnimatorTransitionID == 0)
		  {
			 return false;
		  }
		}

		// Perform an upward raycast to determine if something is overhead. If it is, we need
		// to prevent or stop a jump
		if (_RequiredOverheadDistance > 0)
		{
		  if (RaycastExt.SafeRaycast(mMotionController.transform.position, Vector3.up, _RequiredOverheadDistance))
		  {
			 return false;
		  }
		}

		return true;
	 }

	 /// <summary>
	 /// Called to start the specific motion. If the motion
	 /// were something like 'jump', this would start the jumping process
	 /// </summary>
	 /// <param name="rPrevMotion">Motion that this motion is taking over from</param>
	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		// Flag the motion as active
		mIsActivatedFrame = true;
		mIsStartable = false;
		_isStartFall = false;

		

		// Store the current active Walk Motion (if it is)
		if (rPrevMotion is IWalkRunMotion)
		{
		  mWalkRunMotion = rPrevMotion as IWalkRunMotion;
		}

		// Force the camera to the default mode
		if (mMotionController.CameraRig != null)
		{
		  // TRT 10/13/16: Removed as not needed with CC
		  //mMotionController.CameraRig.Mode = 0;
		}

		// Attempt to find the hip bone if we have a name
		if (_ConvertToHipBase)
		{
		  if (mHipBone == null)
		  {
			 if (_HipBoneName.Length > 0)
			 {
				mHipBone = mActorController._Transform.Find(_HipBoneName);
			 }

			 if (mHipBone == null)
			 {
				Animator lAnimator = mMotionController.Animator;
				if (lAnimator != null)
				{
				  mHipBone = lAnimator.GetBoneTransform(HumanBodyBones.Hips);
				  if (mHipBone != null) { _HipBoneName = mHipBone.name; }
				}
			 }
		  }
		}

		// Reset the distance flag for this jump
		mLastHipDistance = 0f;

		// Clear out the impulse
		mIsImpulseApplied = true;

		// Grab the current velocities
		mLaunchForward = mActorController._Transform.forward;
		mLaunchVelocity = (mLaunchVelocityOverride.sqrMagnitude > 0f ? mLaunchVelocityOverride : mActorController.State.Velocity);

		// Initialize the jump
		//mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START, true);
		mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START_SHORT, true);
		mActorController.AccumulatedVelocity = Vector3.zero;
		mActorController.AddImpulse(mActorController._Transform.up * _Impulse);

		// Register this motion with the camera
		if (_RotateWithCamera && mMotionController.CameraRig is BaseCameraRig)
		{
		  ((BaseCameraRig)mMotionController.CameraRig).OnPostLateUpdate -= OnCameraUpdated;
		  ((BaseCameraRig)mMotionController.CameraRig).OnPostLateUpdate += OnCameraUpdated;
		}

		// Report that we're good to enter the jump
		return base.Activate(rPrevMotion);
	 }

	 /// <summary>
	 /// Called to stop the motion. If the motion is stopable. Some motions
	 /// like jump cannot be stopped early
	 /// </summary>
	 public override void Deactivate()
	 {
		mLaunchVelocityOverride = Vector3.zero;

		// Register this motion with the camera
		if (mMotionController.CameraRig is BaseCameraRig)
		{
		  ((BaseCameraRig)mMotionController.CameraRig).OnPostLateUpdate -= OnCameraUpdated;
		}

		base.Deactivate();
	 }

	 /// <summary>
	 /// Allows the motion to modify the root-motion velocities before they are applied. 
	 /// 
	 /// NOTE:
	 /// Be careful when removing rotations as some transitions will want rotations even 
	 /// if the state they are transitioning from don't.
	 /// </summary>
	 /// <param name="rDeltaTime">Time since the last frame (or fixed update call)</param>
	 /// <param name="rUpdateIndex">Index of the update to help manage dynamic/fixed updates. [0: Invalid update, >=1: Valid update]</param>
	 /// <param name="rVelocityDelta">Root-motion linear velocity relative to the actor's forward</param>
	 /// <param name="rRotationDelta">Root-motion rotational velocity</param>
	 /// <returns></returns>
	 public override void UpdateRootMotion(float rDeltaTime, int rUpdateIndex, ref Vector3 rVelocityDelta, ref Quaternion rRotationDelta)
	 {
		rRotationDelta = Quaternion.identity;

		if (!IsInLandedState)
		{
		  rVelocityDelta = Vector3.zero;
		}
	 }

	 public Vector3 velocityTarget;

	 /// <summary>
	 /// Updates the motion over time. This is called by the controller
	 /// every update cycle so animations and stages can be updated.
	 /// </summary>
	 /// <param name="rDeltaTime">Time since the last frame (or fixed update call)</param>
	 /// <param name="rUpdateIndex">Index of the update to help manage dynamic/fixed updates. [0: Invalid update, >=1: Valid update]</param>
	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		mActorController.State.Velocity = mLaunchVelocity;
		mMovement = Vector3.zero;

		bool lAllowSlide = false;
		float lHipDistanceDelta = 0f;

		// Since we're not doing any lerping or physics based stuff here,
		// we'll only process once per cyle even if we're running slow.
		if (rUpdateIndex != 1) { return; }

		// Grab the state info once
		MotionState lState = mMotionController.State;

		int lStateID = mMotionLayer._AnimatorStateID;
		float lStateTime = mMotionLayer._AnimatorStateNormalizedTime;

		int lTransitionID = mMotionLayer._AnimatorTransitionID;
		float lTransitionTime = mMotionLayer._AnimatorTransitionNormalizedTime;

		// We do the inverse tilt so we calculate the rotation in "natural up" space vs. "actor up" space. 
		Quaternion lInvTilt = QuaternionExt.FromToRotation(mActorController._Transform.up, Vector3.up);

		Vector3 lVelocity = mActorController.State.Velocity;
		if (Time.deltaTime > Time.fixedDeltaTime) { lVelocity = (mActorController.State.Velocity * Time.deltaTime) / Time.fixedDeltaTime; }
		lVelocity = lInvTilt * lVelocity;

		// If we have a hip bone, we'll adjust the jump based on the distance
		// that changes between the foot and the hips. This way, the jump is
		// "hip based" and not "foot based".
		if (_ConvertToHipBase && mHipBone != null)
		{
		  Vector3 lLocalPosition = -mHipBone.InverseTransformPoint(mActorController._Transform.position);
		  float lHipDistance = lLocalPosition.y;

		  // As the distance gets smaller, we increase the shift
		  lHipDistanceDelta = -(lHipDistance - mLastHipDistance);
		  mLastHipDistance = lHipDistance;
		}

		// Blend that occurs before we jump
		if (lTransitionID == TRANS_EntryState_JumpRise)
		{
		  if (!mIsImpulseApplied && lTransitionTime > 0.5f)
		  {
			 mIsImpulseApplied = true;
			 mActorController.AddImpulse(mActorController._Transform.up * _Impulse);
		  }
		}
		// This is the start of the jump. The animator will automatically move on after the node
		// has finished. However, it could move to the "JumpRisePose" or "JumpRiseToTop"
		else if (lStateID == STATE_JumpRise)
		{
		  // We really shouldn't get here, but if the transition time is 
		  // too short, we'll end up skipping past the previous impluse. This
		  // happens with something like the idle pose
		  if (!mIsImpulseApplied)
		  {
			 mIsImpulseApplied = true;
			 mActorController.AddImpulse(mActorController._Transform.up * _Impulse);
		  }
		  // If our velocity is trailing off, move to the top position
		  else if (lVelocity.y < 1.5f)
		  {
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RISE_TO_TOP);
		  }
		}
		// This is the holding position for a super high jump. The pose gives us time
		// before the top occurs.
		else if (lStateID == STATE_JumpRisePose)
		{
		  // If our velocity is trailing off, move to the top position
		  if (lVelocity.y < 2.5f)
		  {
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RISE_TO_TOP);
		  }

		  //mMovement = (mActorController._Transform.rotation * new Vector3(0f, lHipDistanceDelta, 0f));
		  mMovement = mActorController._Transform.up * lHipDistanceDelta;
		}
		// At this point, we're close to the peak of the jump and we need to transition
		// into the top position.
		else if (lStateID == STATE_JumpTopPose)
		{
		  if (mActorController.AccumulatedVelocity.y < -1.5f) // || lState.GroundDistance < 0.45f)
		  {
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_TOP_TO_FALL);
		  }
		}
		else if (lStateID == STATE_JumpRiseToTop)
		{
		  if (lVelocity.y < 0.1f)
		  {
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_TOP);
		  }
		  // If we slow down, start moving to the fall position
		  //if (lVelocity.y < -1.5f) // || lState.GroundDistance < 0.45f)
		  if (mActorController.AccumulatedVelocity.y < -1.5f) // || lState.GroundDistance < 0.45f)
			 {
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_TOP_TO_FALL);
		  }

		  //mMovement = (mActorController._Transform.rotation * new Vector3(0f, lHipDistanceDelta, 0f));
		  mMovement = mActorController._Transform.up * lHipDistanceDelta;
		}
		// We should be at the peak of the jump. We don't expect to wait here
		// long, but this gives us a "pose" to hold onto if needed
		else if (lStateID == STATE_JumpTopPose)
		{
		  // We may have moved over something during the jump. If so, 
		  // we can move straight to the recover
		  if (mActorController.State.GroundSurfaceDistance < 0.5f)
		  {
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_IDLE);
		  }
		  // If we've reached the fall speed, transition
		  else if (lVelocity.y < -1.5f)
		  {
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_TOP_TO_FALL);
			 StartFallHeight = mActorController._Transform.position.y;
		  }
		  // Otherwise, ensure we're in the right phase
		  else
		  {
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_TOP);
		  }

		  mMovement = mActorController._Transform.up * lHipDistanceDelta;
		}
		// Here we come out of the top pose and start moving into the fall pose
		else if (lStateID == STATE_JumpTopToFall)
		{
		  // If we got ontop of something, we may need to recover.
		  if (mActorController.State.GroundSurfaceDistance < 0.15f)
		  {
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_IDLE);
		  }
		  // Look for the ground and prepare to transition
		  else if (mActorController.State.GroundSurfaceDistance < 0.35f)
		  {
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_LAND);
		  }

		  mMovement = mActorController._Transform.up * lHipDistanceDelta;
		}
		else if (lTransitionID == TRANS_JumpRunLeftStart_JumpRunLeftStop 
		  || lTransitionID == TRANS_JumpRunRightStart_JumpRunRightStop
		  || lTransitionID == TRANS_GlideStart_GlideStop)
		{
		  if (!_noExistJump)
			 TestReactivate();
		  if (!_noExistJump)
		  {
			 if (mMotionController.State.InputForward.magnitude <= 0.1f)
			 {
				mLaunchVelocity = Vector3.zero;
				mActorController.AccumulatedVelocity = Vector3.zero;
			 }
		  }
		}
		else if(lStateID == STATE_GlideStart)
		{

		  //if (lStateTime > 0.8f)
		  //{
			 _noExistJump = false;
			 if (lStateTime > 1f && mActorController.State.GroundSurfaceDistance > 2f)
			 {
				mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_FALL);
			 }
			 else if (lState.InputMagnitudeTrend.Value >= 0.1f)
			 {
				if (mActorController.State.GroundSurfaceDistance <= 0.6f)
				{
				  _toRun = true;
				  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RUN_STOP);
				}
			 }
			 else if (mActorController.State.GroundSurfaceDistance <= 0.6f)
			 {
				_toIdle = true;
				mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RUN_STOP);
			 }

		  //}
		  mMovement = mActorController._Transform.up * lHipDistanceDelta;
		}
		// Старт бега
		else if(lStateID == STATE_JumpRunLeftStart || lStateID == STATE_JumpRunRightStart)
		{
		  if(lStateTime > 0.8f)
		  {
			 _noExistJump = false;
			 if (mActorController.State.GroundSurfaceDistance > 2f)
			 {
				mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_FALL);
			 }else if(lState.InputMagnitudeTrend.Value >= 0.1f)
			 {
				if (mActorController.State.GroundSurfaceDistance <= 0.4f)
				{
				  _toRun = true;
				  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RUN_STOP);
				}
			 }
			 else if (mActorController.State.GroundSurfaceDistance <= 0.4f)
			 {
				_toIdle = true;
				mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RUN_STOP);
			 }

		  }
		  mMovement = mActorController._Transform.up * lHipDistanceDelta;
		}
		else if(lStateID == STATE_JumpRunLeftStop 
		  || lStateID == STATE_JumpRunRightStop
		  || lStateID == STATE_GlideStop)
		{

		  if (!_noExistJump)
			 TestReactivate();
		  if (!_noExistJump)
		  {
			 if (lStateTime > 0.2f)
			 {
				if (_toRun)
				{
				  _toRun = false;
				  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_MOVE);
				}
				if (_toIdle)
				{
				  _toIdle = false;
				  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_IDLE);
				}
			 }
			 mMovement = mActorController._Transform.up * lHipDistanceDelta;
		  }
		}
		// We could be falling for a while. This animation allows us to 
		// hold in the falling state until we hit the ground.
		else if (lStateID == STATE_JumpFallPose)
		{
		  if (!_isStartFall)
		  {
			 StartFallHeight = mActorController._Transform.position.y;
			 _isStartFall = true;
		  }
		  //mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, Jump_old.PHASE_FALL);

		  // Look for the ground. In this case, we want a 
		  // value that is slightly greater than our collider radius
		  if (mActorController.State.GroundSurfaceDistance < 0.35f)
		  {
			 if (isFaling)
			 {
				isFaling = false;
				float fallHeight = StartFallHeight - mActorController._Transform.position.y;

				if (fallHeight > 6)
				{
				  Game.Managers.GameManager.Instance.UserManager.Health.Damage(mMotionController, 1000);
				  return;
				}
			 }


			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_LAND);
		  }

		  mMovement = mActorController._Transform.up * lHipDistanceDelta;
		}
		// This is the first state in the jump where we hit the ground
		else if (lStateID == STATE_JumpLand)
		{
		  if (mActorController.State.IsGrounded)
		  {
			 // If there is no controller input, we can stop
			 if (lState.InputMagnitudeTrend.Value < 0.03f)
			 {
				mLaunchVelocity = Vector3.zero;
				mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_IDLE);
			 }
			 // If the player is messing with the input, we need to think about
			 // what to transition to
			 else
			 {
				//if ((mWalkRunPivot != null && mWalkRunPivot.IsEnabled && mWalkRunPivot.IsRunActive) ||
				//    (mWalkRunPivot_v2 != null && mWalkRunPivot_v2.IsEnabled && mWalkRunPivot_v2.IsRunActive) ||
				//    (mWalkRunStrafe != null && mWalkRunStrafe.IsEnabled && mWalkRunStrafe.IsRunActive) ||
				//    (mWalkRunStrafe_v2 != null && mWalkRunStrafe_v2.IsEnabled && mWalkRunStrafe_v2.IsRunActive) ||
				//    (mWalkRunRotate != null && mWalkRunRotate.IsEnabled && mWalkRunRotate.IsRunActive) ||
				//    (mWalkRunRotate_v2 != null && mWalkRunRotate_v2.IsEnabled && mWalkRunRotate_v2.IsRunActive))
				if (mWalkRunMotion != null && mWalkRunMotion.IsEnabled && mWalkRunMotion.IsRunActive)
				{
				  if (Mathf.Abs(lState.InputFromAvatarAngle) > 140) { mLaunchVelocity = Vector3.zero; }
				  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_MOVE);
				}
				else
				{
				  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_IDLE);
				}
			 }
		  }
		  else
		  {
			 mMovement = mActorController._Transform.up * lHipDistanceDelta;
		  }
		}
		// Called when the avatar starts to come out of the impact
		else if (lStateID == STATE_JumpRecoverIdle)
		{
		  mIsStartable = true;

		  // If we're moving forward, transition into the run/walk
		  if (lState.InputMagnitudeTrend.Value >= 0.1f && Mathf.Abs(lState.InputFromAvatarAngle) < 20f)
		  {
			 // Allow us to keep moving so we blend into the run
			 lAllowSlide = true;

			 // It may be time to move into the walk/run
			 if (lStateTime > 0.3f)
			 {
				if (mWalkRunMotion != null && mWalkRunMotion.IsEnabled)
				{
				  mWalkRunMotion.StartInRun = mWalkRunMotion.IsRunActive;
				  mWalkRunMotion.StartInWalk = !mWalkRunMotion.StartInRun;
				  mMotionController.ActivateMotion(mWalkRunMotion as MotionControllerMotion);
				}

				//if (mWalkRunPivot != null && mWalkRunPivot.IsEnabled)
				//{
				//    mWalkRunPivot.StartInRun = mWalkRunPivot.IsRunActive;
				//    mWalkRunPivot.StartInWalk = !mWalkRunPivot.StartInRun;
				//    mMotionController.ActivateMotion(mWalkRunPivot);
				//}
				//else if (mWalkRunPivot_v2 != null && mWalkRunPivot_v2.IsEnabled)
				//{
				//    mWalkRunPivot_v2.StartInRun = mWalkRunPivot_v2.IsRunActive;
				//    mWalkRunPivot_v2.StartInWalk = !mWalkRunPivot_v2.StartInRun;
				//    mMotionController.ActivateMotion(mWalkRunPivot_v2);
				//}
				//else if (mWalkRunStrafe != null && mWalkRunStrafe.IsEnabled)
				//{
				//    mWalkRunStrafe.StartInRun = mWalkRunStrafe.IsRunActive;
				//    mWalkRunStrafe.StartInWalk = !mWalkRunStrafe.StartInRun;
				//    mMotionController.ActivateMotion(mWalkRunStrafe);
				//}
				//else if (mWalkRunStrafe_v2 != null && mWalkRunStrafe_v2.IsEnabled)
				//{
				//    mWalkRunStrafe_v2.StartInRun = mWalkRunStrafe_v2.IsRunActive;
				//    mWalkRunStrafe_v2.StartInWalk = !mWalkRunStrafe_v2.StartInRun;
				//    mMotionController.ActivateMotion(mWalkRunStrafe_v2);
				//}
				//else if (mWalkRunRotate != null && mWalkRunRotate.IsEnabled)
				//{
				//    mWalkRunRotate.StartInRun = mWalkRunRotate.IsRunActive;
				//    mWalkRunRotate.StartInWalk = !mWalkRunRotate.StartInRun;
				//    mMotionController.ActivateMotion(mWalkRunRotate);
				//}
				//else if (mWalkRunRotate_v2 != null && mWalkRunRotate_v2.IsEnabled)
				//{
				//    mWalkRunRotate_v2.StartInRun = mWalkRunRotate_v2.IsRunActive;
				//    mWalkRunRotate_v2.StartInWalk = !mWalkRunRotate_v2.StartInRun;
				//    mMotionController.ActivateMotion(mWalkRunRotate_v2);
				//}
			 }
		  }
		}
		// Called when the avatar starts to come out of the impact
		else if (lStateID == STATE_JumpRecoverRun || lStateID == STATE_LandRun)
		{
		  if(!_noExistJump)
			 TestReactivate();

		  if (!_noExistJump)
		  {
			 // Allow the animation to control movement again
			 mLaunchVelocity = Vector3.zero;

			 if (mWalkRunMotion != null && mWalkRunMotion.IsEnabled)
			 {
				// Allow us to keep moving so we blend into the run
				lAllowSlide = true;

				if (lStateTime > 0.2f)
				{
				  mWalkRunMotion.StartInRun = mWalkRunMotion.IsRunActive;
				  mWalkRunMotion.StartInWalk = !mWalkRunMotion.StartInRun;
				  mMotionController.ActivateMotion(mWalkRunMotion as MotionControllerMotion);
				}
			 }
			 //// It may be time to move into the walk/run
			 //if (mWalkRunPivot != null && mWalkRunPivot.IsEnabled)
			 //{
			 //    // Allow us to keep moving so we blend into the run
			 //    lAllowSlide = true;

			 //    if (lStateTime > 0.2f)
			 //    {
			 //        mWalkRunPivot.StartInRun = mWalkRunPivot.IsRunActive;
			 //        mWalkRunPivot.StartInWalk = !mWalkRunPivot.StartInRun;
			 //        mMotionController.ActivateMotion(mWalkRunPivot);
			 //    }
			 //}
			 //else if (mWalkRunPivot_v2 != null && mWalkRunPivot_v2.IsEnabled)
			 //{
			 //    // Allow us to keep moving so we blend into the run
			 //    lAllowSlide = true;

			 //    if (lStateTime > 0.2f)
			 //    {
			 //        mWalkRunPivot_v2.StartInRun = mWalkRunPivot_v2.IsRunActive;
			 //        mWalkRunPivot_v2.StartInWalk = !mWalkRunPivot_v2.StartInRun;
			 //        mMotionController.ActivateMotion(mWalkRunPivot_v2);
			 //    }
			 //}
			 //else if (mWalkRunStrafe != null && mWalkRunStrafe.IsEnabled)
			 //{
			 //    // Allow us to keep moving so we blend into the run
			 //    lAllowSlide = true;

			 //    if (lStateTime > 0.2f)
			 //    {
			 //        mWalkRunStrafe.StartInRun = mWalkRunStrafe.IsRunActive;
			 //        mWalkRunStrafe.StartInWalk = !mWalkRunStrafe.StartInRun;
			 //        mMotionController.ActivateMotion(mWalkRunStrafe);
			 //    }
			 //}
			 //else if (mWalkRunStrafe_v2 != null && mWalkRunStrafe_v2.IsEnabled)
			 //{
			 //    // Allow us to keep moving so we blend into the run
			 //    lAllowSlide = true;

			 //    if (lStateTime > 0.2f)
			 //    {
			 //        mWalkRunStrafe_v2.StartInRun = mWalkRunStrafe_v2.IsRunActive;
			 //        mWalkRunStrafe_v2.StartInWalk = !mWalkRunStrafe_v2.StartInRun;
			 //        mMotionController.ActivateMotion(mWalkRunStrafe_v2);
			 //    }
			 //}
			 //else if (mWalkRunRotate != null && mWalkRunRotate.IsEnabled)
			 //{
			 //    // Allow us to keep moving so we blend into the run
			 //    lAllowSlide = true;

			 //    if (lStateTime > 0.2f)
			 //    {
			 //        mWalkRunRotate.StartInRun = mWalkRunRotate.IsRunActive;
			 //        mWalkRunRotate.StartInWalk = !mWalkRunRotate.StartInRun;
			 //        mMotionController.ActivateMotion(mWalkRunRotate);
			 //    }
			 //}
			 //else if (mWalkRunRotate_v2 != null && mWalkRunRotate_v2.IsEnabled)
			 //{
			 //    // Allow us to keep moving so we blend into the run
			 //    lAllowSlide = true;

			 //    if (lStateTime > 0.2f)
			 //    {
			 //        mWalkRunRotate_v2.StartInRun = mWalkRunRotate_v2.IsRunActive;
			 //        mWalkRunRotate_v2.StartInWalk = !mWalkRunRotate_v2.StartInRun;
			 //        mMotionController.ActivateMotion(mWalkRunRotate_v2);
			 //    }
			 //}
			 else
			 {
				Deactivate();
			 }
		  }
		}
		// If there's no movement, we're done
		else if (lStateID == STATE_IdlePose && mAge > 0.05f)
		{
		  Deactivate();
		}

		// Check if we're rotating with the camera
		mRotateWithCamera = false;
		if (_RotateWithCamera && mMotionController._CameraTransform != null)
		{
		  if (_RotateWithCameraAlias.Length == 0 || mMotionController._InputSource.IsPressed(_RotateWithCameraAlias)) { mRotateWithCamera = true; }
		}

		// If we're meant to rotate with the camera (and OnCameraUpdate isn't already attached), do it here
		if (mRotateWithCamera && !(mMotionController.CameraRig is BaseCameraRig))
		{
		  OnCameraUpdated(rDeltaTime, rUpdateIndex, null);
		}

		// Set the controller state with the modified values
		mMotionController.State = lState;

		// Determine the resulting velocity of this update
		mVelocity = DetermineVelocity(lAllowSlide);
	 }

	 protected virtual bool TestReactivate()
	 {
		return false;
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
				if (mActorController.State.Velocity.magnitude < 5f)
				{
				  rMessage.Recipient = this;
				  rMessage.IsHandled = true;

				  mMotionController.ActivateMotion(this);
				}
			 }
		  }
		}
	 }

	 /// <summary>
	 /// Returns the current velocity of the motion
	 /// </summary>
	 protected Vector3 DetermineVelocity(bool rAllowSlide)
	 {
		Vector3 lVelocity = Vector3.zero;
		int lStateID = mMotionLayer._AnimatorStateID;

		// TRT 11/20/15: If we're colliding with an object, we won't allow
		// any velocity. This helps prevent sliding while jumping
		// against an object.
		if (mActorController.State.IsColliding)
		{
		  return lVelocity;
		}

		// If were in the midst of jumping, we want to add velocity based on 
		// the magnitude of the controller. 
		if ((lStateID != STATE_JumpRecoverIdle || rAllowSlide) &&
			 (lStateID != STATE_JumpRecoverRun || rAllowSlide) &&
			 IsInMotionState)
		{
		  MotionState lState = mMotionController.State;

		  // Speed that comes from momenum
		  Vector3 lMomentum = mLaunchVelocity;
		  float lMomentumSpeed = (_IsMomentumEnabled ? lMomentum.magnitude : 0f);

		  // Speed that comes from the user
		  float lControlSpeed = (_IsControlEnabled ? _ControlSpeed * lState.InputMagnitudeTrend.Value : 0f);

		  // Speed we'll use as the character is jumping
		  float lAirSpeed = Mathf.Max(lMomentumSpeed, lControlSpeed);

		  // If we allow control, let the player determine the direction
		  if (_IsControlEnabled)
		  {
			 Vector3 lBaseForward = mActorController._Transform.forward;
			 if (mMotionController._InputSource != null && mMotionController._InputSource.IsEnabled)
			 {
				if (mMotionController._CameraTransform != null)
				{
				  lBaseForward = mMotionController._CameraTransform.forward;
				}
			 }

			 // Create a quaternion that gets us from our world-forward to our actor/camera direction.
			 // FromToRotation creates a quaternion using the shortest method which can sometimes
			 // flip the angle. LookRotation will attempt to keep the "up" direction "up".
			 Quaternion lToBaseForward = Quaternion.LookRotation(lBaseForward, mActorController._Transform.up);

			 // Determine the avatar displacement direction. This isn't just
			 // normal movement forward, but includes movement to the side
			 Vector3 lMoveDirection = lToBaseForward * lState.InputForward;

			 // Apply the direction and speed
			 lVelocity = lVelocity + (lMoveDirection * lAirSpeed);
		  }

		  // If momementum is enabled, add it to keep the player moving in the direction of the jump
		  if (_IsMomentumEnabled)
		  {
			 lVelocity = lVelocity + lMomentum;
		  }

		  // Don't exceed our air speed
		  if (lVelocity.magnitude > lAirSpeed)
		  {
			 lVelocity = lVelocity.normalized * lAirSpeed;
		  }
		}

		return lVelocity;
	 }

	 /// <summary>
	 /// When we want to rotate based on the camera direction, we need to tweak the actor
	 /// rotation AFTER we process the camera. Otherwise, we can get small stutters during camera rotation. 
	 /// 
	 /// This is the only way to keep them totally in sync. It also means we can't run any of our AC processing
	 /// as the AC already ran. So, we do minimal work here
	 /// </summary>
	 /// <param name="rDeltaTime"></param>
	 /// <param name="rUpdateCount"></param>
	 /// <param name="rCamera"></param>
	 private void OnCameraUpdated(float rDeltaTime, int rUpdateIndex, BaseCameraRig rCamera)
	 {
		if (!mRotateWithCamera) { return; }
		if (mMotionController._CameraTransform == null) { return; }

		float lToCameraAngle = Vector3Ext.HorizontalAngleTo(mMotionController._Transform.forward, mMotionController._CameraTransform.forward, mMotionController._Transform.up);
		if (!mLinkRotation && Mathf.Abs(lToCameraAngle) <= _RotationToCameraSpeed * rDeltaTime) { mLinkRotation = true; }

		if (!mLinkRotation)
		{
		  float lRotationAngle = Mathf.Abs(lToCameraAngle);
		  float lRotationSign = Mathf.Sign(lToCameraAngle);
		  lToCameraAngle = lRotationSign * Mathf.Min(_RotationToCameraSpeed * rDeltaTime, lRotationAngle);
		}

		Quaternion lRotation = Quaternion.AngleAxis(lToCameraAngle, Vector3.up);
		mActorController.Yaw = mActorController.Yaw * lRotation;
		mActorController._Transform.rotation = mActorController.Tilt * mActorController.Yaw;
	 }

	 /// <summary>
	 /// Test to see if we're currently in a jump state prior to landing
	 /// </summary>
	 protected bool IsInMidJumpState
	 {
		get
		{
		  int lStateID = mMotionLayer._AnimatorStateID;
		  if (lStateID == STATE_JumpRise) { return true; }
		  if (lStateID == STATE_JumpRisePose) { return true; }
		  if (lStateID == STATE_JumpRiseToTop) { return true; }
		  if (lStateID == STATE_JumpTopPose) { return true; }
		  if (lStateID == STATE_JumpTopToFall) { return true; }
		  if (lStateID == STATE_JumpFallPose) { return true; }
		  if (lStateID == STATE_JumpLand) { return true; }
		  if (lStateID == STATE_JumpRunRightStart) { return true; }
		  if (lStateID == STATE_JumpRunLeftStart) { return true; }
		  if (lStateID == STATE_GlideStart) { return true; }
		  if (lStateID == STATE_GlideStop) { return true; }

		  int lTransitionID = mMotionLayer._AnimatorTransitionID;
		  if (lTransitionID == TRANS_EntryState_JumpRise) { return true; }
		  if (lTransitionID == TRANS_AnyState_JumpRise) { return true; }
		  if (lTransitionID == TRANS_EntryState_JumpFallPose) { return true; }
		  if (lTransitionID == TRANS_AnyState_JumpFallPose) { return true; }

		  if (lTransitionID == TRANS_AnyState_JumpRunRightStart) { return true; }
		  if (lTransitionID == TRANS_EntryState_JumpRunRightStart) { return true; }
		  if (lTransitionID == TRANS_AnyState_JumpRunLeftStart) { return true; }
		  if (lTransitionID == TRANS_EntryState_JumpRunLeftStart) { return true; }

		  if (lTransitionID == TRANS_AnyState_GlideStart) { return true; }
		  if (lTransitionID == TRANS_EntryState_GlideStart) { return true; }

		  return false;
		}
	 }

	 /// <summary>
	 /// Determines if we're in one of the landed states. Mostly this is so we
	 /// can stop adding movement.
	 /// </summary>
	 protected bool IsInLandedState
	 {
		get
		{
		  int lStateID = mMotionLayer._AnimatorStateID;
		  //if (lStateID == STATE_JumpRecoverIdle) { return true; }
		  if (lStateID == STATE_JumpRecoverRun) { return true; }
		  if (lStateID == STATE_IdlePose) { return true; }
		  if (lStateID == STATE_LandRun) { return true; }
		  if (mMotionController.State.InputForward.magnitude > 0.1f && lStateID == STATE_JumpRunLeftStop) { return true; }
		  if (mMotionController.State.InputForward.magnitude > 0.1f && lStateID == STATE_JumpRunRightStop) { return true; }

		  return false;
		}
	 }

#if UNITY_EDITOR

	 /// <summary>
	 /// Allow the motion to render it's own GUI
	 /// </summary>
	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;

		if (EditorHelper.IntField("Form Condition", "Optional condition used to only activate this motion if the value matches the current Default Form of the MC. Set to -1 to disable.", Form, mMotionController))
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

		if (EditorHelper.FloatField("Max Jump Time", "Maximum amount of time we'll remain in the jump or fall.", MaxJumpTime, mMotionController))
		{
		  lIsDirty = true;
		  MaxJumpTime = EditorHelper.FieldFloatValue;
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

		bool lNewIsMomentumEnabled = EditorGUILayout.Toggle(new GUIContent("Is Momentum Enabled", "Determines if the avatar's speed and direction before the jump are used to propel the avatar while in the air."), IsMomentumEnabled);
		if (lNewIsMomentumEnabled != IsMomentumEnabled)
		{
		  lIsDirty = true;
		  IsMomentumEnabled = lNewIsMomentumEnabled;
		}

		bool lNewIsControlEnabled = EditorGUILayout.Toggle(new GUIContent("Is Control Enabled", "Determines if the player can control the avatar while in the air."), IsControlEnabled);
		if (lNewIsControlEnabled != IsControlEnabled)
		{
		  lIsDirty = true;
		  IsControlEnabled = lNewIsControlEnabled;
		}

		float lNewControlSpeed = EditorGUILayout.FloatField(new GUIContent("Control Speed", "Speed of the avatar when in the air. This should roughly match the ground speed of the avatar."), ControlSpeed);
		if (lNewControlSpeed != ControlSpeed)
		{
		  lIsDirty = true;
		  ControlSpeed = lNewControlSpeed;
		}

		float lNewRequiredOverheadDistance = EditorGUILayout.FloatField(new GUIContent("Required Overhead Distance", "When greater than 0, a test will be made to determine if we can jump or can continue a jump."), RequiredOverheadDistance);
		if (lNewRequiredOverheadDistance != RequiredOverheadDistance)
		{
		  lIsDirty = true;
		  RequiredOverheadDistance = lNewRequiredOverheadDistance;
		}

		GUILayout.Space(5f);

		if (EditorHelper.BoolField("Rotate With Camera", "Determines if we rotate to match the camera.", RotateWithCamera, mMotionController))
		{
		  lIsDirty = true;
		  RotateWithCamera = EditorHelper.FieldBoolValue;
		}

		if (EditorHelper.TextField("Rotate Action Alias", "Action alias determines if rotation is activated. This typically matches the input source's View Activator.", RotateWithCameraAlias, mMotionController))
		{
		  lIsDirty = true;
		  RotateWithCameraAlias = EditorHelper.FieldStringValue;
		}

		if (EditorHelper.FloatField("Rotation Speed", "Degrees per second to rotate to the camera's direction.", RotationToCameraSpeed, mMotionController))
		{
		  lIsDirty = true;
		  RotationToCameraSpeed = EditorHelper.FieldFloatValue;
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
	 public int STATE_JumpRise = -1;
	 public int STATE_JumpLand = -1;
	 public int STATE_JumpRisePose = -1;
	 public int STATE_JumpFallPose = -1;
	 public int STATE_JumpTopToFall = -1;
	 public int STATE_JumpRiseToTop = -1;
	 public int STATE_JumpTopPose = -1;
	 public int STATE_JumpRecoverIdle = -1;
	 public int STATE_JumpRecoverRun = -1;
	 public int STATE_IdlePose = -1;
	 public int STATE_JumpRunRightStart = -1;
	 public int STATE_JumpRunLeftStart = -1;
	 public int STATE_JumpRunLeftStop = -1;
	 public int STATE_JumpRunRightStop = -1;
	 public int STATE_LandRun = -1;
	 public int STATE_GlideStart = -1;
	 public int STATE_GlideStop = -1;
	 public int TRANS_AnyState_JumpRise = -1;
	 public int TRANS_EntryState_JumpRise = -1;
	 public int TRANS_AnyState_JumpFallPose = -1;
	 public int TRANS_EntryState_JumpFallPose = -1;
	 public int TRANS_AnyState_JumpRiseToTop = -1;
	 public int TRANS_EntryState_JumpRiseToTop = -1;
	 public int TRANS_AnyState_JumpRunRightStart = -1;
	 public int TRANS_EntryState_JumpRunRightStart = -1;
	 public int TRANS_AnyState_JumpRunLeftStart = -1;
	 public int TRANS_EntryState_JumpRunLeftStart = -1;
	 public int TRANS_AnyState_GlideStart = -1;
	 public int TRANS_EntryState_GlideStart = -1;
	 public int TRANS_JumpRise_JumpRiseToTop = -1;
	 public int TRANS_JumpRise_JumpRisePose = -1;
	 public int TRANS_JumpLand_JumpRecoverRun = -1;
	 public int TRANS_JumpLand_JumpRecoverIdle = -1;
	 public int TRANS_JumpRisePose_JumpRiseToTop = -1;
	 public int TRANS_JumpFallPose_JumpLand = -1;
	 public int TRANS_JumpTopToFall_JumpLand = -1;
	 public int TRANS_JumpTopToFall_JumpFallPose = -1;
	 public int TRANS_JumpTopToFall_JumpRecoverIdle = -1;
	 public int TRANS_JumpTopToFall_JumpRecoverRun = -1;
	 public int TRANS_JumpRiseToTop_JumpTopToFall = -1;
	 public int TRANS_JumpRiseToTop_JumpTopPose = -1;
	 public int TRANS_JumpRiseToTop_JumpRecoverIdle = -1;
	 public int TRANS_JumpRiseToTop_JumpRecoverRun = -1;
	 public int TRANS_JumpTopPose_JumpTopToFall = -1;
	 public int TRANS_JumpTopPose_JumpRecoverIdle = -1;
	 public int TRANS_JumpRecoverIdle_IdlePose = -1;
	 public int TRANS_JumpRunRightStart_JumpRunRightStop = -1;
	 public int TRANS_JumpRunRightStart_JumpFallPose = -1;
	 public int TRANS_JumpRunLeftStart_JumpRunLeftStop = -1;
	 public int TRANS_JumpRunLeftStart_JumpFallPose = -1;
	 public int TRANS_JumpRunLeftStop_LandRun = -1;
	 public int TRANS_JumpRunLeftStop_IdlePose = -1;
	 public int TRANS_JumpRunRightStop_LandRun = -1;
	 public int TRANS_JumpRunRightStop_IdlePose = -1;
	 public int TRANS_GlideStart_GlideStop = -1;
	 public int TRANS_GlideStop_LandRun = -1;
	 public int TRANS_GlideStop_IdlePose = -1;

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
			 if (lStateID == STATE_JumpRise) { return true; }
			 if (lStateID == STATE_JumpLand) { return true; }
			 if (lStateID == STATE_JumpRisePose) { return true; }
			 if (lStateID == STATE_JumpFallPose) { return true; }
			 if (lStateID == STATE_JumpTopToFall) { return true; }
			 if (lStateID == STATE_JumpRiseToTop) { return true; }
			 if (lStateID == STATE_JumpTopPose) { return true; }
			 if (lStateID == STATE_JumpRecoverIdle) { return true; }
			 if (lStateID == STATE_JumpRecoverRun) { return true; }
			 if (lStateID == STATE_IdlePose) { return true; }
			 if (lStateID == STATE_JumpRunRightStart) { return true; }
			 if (lStateID == STATE_JumpRunLeftStart) { return true; }
			 if (lStateID == STATE_JumpRunLeftStop) { return true; }
			 if (lStateID == STATE_JumpRunRightStop) { return true; }
			 if (lStateID == STATE_LandRun) { return true; }
			 if (lStateID == STATE_GlideStart) { return true; }
			 if (lStateID == STATE_GlideStop) { return true; }
		  }

		  if (lTransitionID == TRANS_AnyState_JumpRise) { return true; }
		  if (lTransitionID == TRANS_EntryState_JumpRise) { return true; }
		  if (lTransitionID == TRANS_AnyState_JumpFallPose) { return true; }
		  if (lTransitionID == TRANS_EntryState_JumpFallPose) { return true; }
		  if (lTransitionID == TRANS_AnyState_JumpRiseToTop) { return true; }
		  if (lTransitionID == TRANS_EntryState_JumpRiseToTop) { return true; }
		  if (lTransitionID == TRANS_AnyState_JumpRunRightStart) { return true; }
		  if (lTransitionID == TRANS_EntryState_JumpRunRightStart) { return true; }
		  if (lTransitionID == TRANS_AnyState_JumpRunLeftStart) { return true; }
		  if (lTransitionID == TRANS_EntryState_JumpRunLeftStart) { return true; }
		  if (lTransitionID == TRANS_AnyState_GlideStart) { return true; }
		  if (lTransitionID == TRANS_EntryState_GlideStart) { return true; }
		  if (lTransitionID == TRANS_JumpRise_JumpRiseToTop) { return true; }
		  if (lTransitionID == TRANS_JumpRise_JumpRisePose) { return true; }
		  if (lTransitionID == TRANS_JumpLand_JumpRecoverRun) { return true; }
		  if (lTransitionID == TRANS_JumpLand_JumpRecoverIdle) { return true; }
		  if (lTransitionID == TRANS_JumpRisePose_JumpRiseToTop) { return true; }
		  if (lTransitionID == TRANS_JumpFallPose_JumpLand) { return true; }
		  if (lTransitionID == TRANS_JumpTopToFall_JumpLand) { return true; }
		  if (lTransitionID == TRANS_JumpTopToFall_JumpFallPose) { return true; }
		  if (lTransitionID == TRANS_JumpTopToFall_JumpRecoverIdle) { return true; }
		  if (lTransitionID == TRANS_JumpTopToFall_JumpRecoverRun) { return true; }
		  if (lTransitionID == TRANS_JumpRiseToTop_JumpTopToFall) { return true; }
		  if (lTransitionID == TRANS_JumpRiseToTop_JumpTopPose) { return true; }
		  if (lTransitionID == TRANS_JumpRiseToTop_JumpRecoverIdle) { return true; }
		  if (lTransitionID == TRANS_JumpRiseToTop_JumpRecoverRun) { return true; }
		  if (lTransitionID == TRANS_JumpTopPose_JumpTopToFall) { return true; }
		  if (lTransitionID == TRANS_JumpTopPose_JumpRecoverIdle) { return true; }
		  if (lTransitionID == TRANS_JumpRecoverIdle_IdlePose) { return true; }
		  if (lTransitionID == TRANS_JumpRunRightStart_JumpRunRightStop) { return true; }
		  if (lTransitionID == TRANS_JumpRunRightStart_JumpFallPose) { return true; }
		  if (lTransitionID == TRANS_JumpRunLeftStart_JumpRunLeftStop) { return true; }
		  if (lTransitionID == TRANS_JumpRunLeftStart_JumpFallPose) { return true; }
		  if (lTransitionID == TRANS_JumpRunLeftStop_LandRun) { return true; }
		  if (lTransitionID == TRANS_JumpRunLeftStop_IdlePose) { return true; }
		  if (lTransitionID == TRANS_JumpRunRightStop_LandRun) { return true; }
		  if (lTransitionID == TRANS_JumpRunRightStop_IdlePose) { return true; }
		  if (lTransitionID == TRANS_GlideStart_GlideStop) { return true; }
		  if (lTransitionID == TRANS_GlideStop_LandRun) { return true; }
		  if (lTransitionID == TRANS_GlideStop_IdlePose) { return true; }
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
		if (rStateID == STATE_JumpRise) { return true; }
		if (rStateID == STATE_JumpLand) { return true; }
		if (rStateID == STATE_JumpRisePose) { return true; }
		if (rStateID == STATE_JumpFallPose) { return true; }
		if (rStateID == STATE_JumpTopToFall) { return true; }
		if (rStateID == STATE_JumpRiseToTop) { return true; }
		if (rStateID == STATE_JumpTopPose) { return true; }
		if (rStateID == STATE_JumpRecoverIdle) { return true; }
		if (rStateID == STATE_JumpRecoverRun) { return true; }
		if (rStateID == STATE_IdlePose) { return true; }
		if (rStateID == STATE_JumpRunRightStart) { return true; }
		if (rStateID == STATE_JumpRunLeftStart) { return true; }
		if (rStateID == STATE_JumpRunLeftStop) { return true; }
		if (rStateID == STATE_JumpRunRightStop) { return true; }
		if (rStateID == STATE_LandRun) { return true; }
		if (rStateID == STATE_GlideStart) { return true; }
		if (rStateID == STATE_GlideStop) { return true; }
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
		  if (rStateID == STATE_JumpRise) { return true; }
		  if (rStateID == STATE_JumpLand) { return true; }
		  if (rStateID == STATE_JumpRisePose) { return true; }
		  if (rStateID == STATE_JumpFallPose) { return true; }
		  if (rStateID == STATE_JumpTopToFall) { return true; }
		  if (rStateID == STATE_JumpRiseToTop) { return true; }
		  if (rStateID == STATE_JumpTopPose) { return true; }
		  if (rStateID == STATE_JumpRecoverIdle) { return true; }
		  if (rStateID == STATE_JumpRecoverRun) { return true; }
		  if (rStateID == STATE_IdlePose) { return true; }
		  if (rStateID == STATE_JumpRunRightStart) { return true; }
		  if (rStateID == STATE_JumpRunLeftStart) { return true; }
		  if (rStateID == STATE_JumpRunLeftStop) { return true; }
		  if (rStateID == STATE_JumpRunRightStop) { return true; }
		  if (rStateID == STATE_LandRun) { return true; }
		  if (rStateID == STATE_GlideStart) { return true; }
		  if (rStateID == STATE_GlideStop) { return true; }
		}

		if (rTransitionID == TRANS_AnyState_JumpRise) { return true; }
		if (rTransitionID == TRANS_EntryState_JumpRise) { return true; }
		if (rTransitionID == TRANS_AnyState_JumpFallPose) { return true; }
		if (rTransitionID == TRANS_EntryState_JumpFallPose) { return true; }
		if (rTransitionID == TRANS_AnyState_JumpRiseToTop) { return true; }
		if (rTransitionID == TRANS_EntryState_JumpRiseToTop) { return true; }
		if (rTransitionID == TRANS_AnyState_JumpRunRightStart) { return true; }
		if (rTransitionID == TRANS_EntryState_JumpRunRightStart) { return true; }
		if (rTransitionID == TRANS_AnyState_JumpRunLeftStart) { return true; }
		if (rTransitionID == TRANS_EntryState_JumpRunLeftStart) { return true; }
		if (rTransitionID == TRANS_AnyState_GlideStart) { return true; }
		if (rTransitionID == TRANS_EntryState_GlideStart) { return true; }
		if (rTransitionID == TRANS_JumpRise_JumpRiseToTop) { return true; }
		if (rTransitionID == TRANS_JumpRise_JumpRisePose) { return true; }
		if (rTransitionID == TRANS_JumpLand_JumpRecoverRun) { return true; }
		if (rTransitionID == TRANS_JumpLand_JumpRecoverIdle) { return true; }
		if (rTransitionID == TRANS_JumpRisePose_JumpRiseToTop) { return true; }
		if (rTransitionID == TRANS_JumpFallPose_JumpLand) { return true; }
		if (rTransitionID == TRANS_JumpTopToFall_JumpLand) { return true; }
		if (rTransitionID == TRANS_JumpTopToFall_JumpFallPose) { return true; }
		if (rTransitionID == TRANS_JumpTopToFall_JumpRecoverIdle) { return true; }
		if (rTransitionID == TRANS_JumpTopToFall_JumpRecoverRun) { return true; }
		if (rTransitionID == TRANS_JumpRiseToTop_JumpTopToFall) { return true; }
		if (rTransitionID == TRANS_JumpRiseToTop_JumpTopPose) { return true; }
		if (rTransitionID == TRANS_JumpRiseToTop_JumpRecoverIdle) { return true; }
		if (rTransitionID == TRANS_JumpRiseToTop_JumpRecoverRun) { return true; }
		if (rTransitionID == TRANS_JumpTopPose_JumpTopToFall) { return true; }
		if (rTransitionID == TRANS_JumpTopPose_JumpRecoverIdle) { return true; }
		if (rTransitionID == TRANS_JumpRecoverIdle_IdlePose) { return true; }
		if (rTransitionID == TRANS_JumpRunRightStart_JumpRunRightStop) { return true; }
		if (rTransitionID == TRANS_JumpRunRightStart_JumpFallPose) { return true; }
		if (rTransitionID == TRANS_JumpRunLeftStart_JumpRunLeftStop) { return true; }
		if (rTransitionID == TRANS_JumpRunLeftStart_JumpFallPose) { return true; }
		if (rTransitionID == TRANS_JumpRunLeftStop_LandRun) { return true; }
		if (rTransitionID == TRANS_JumpRunLeftStop_IdlePose) { return true; }
		if (rTransitionID == TRANS_JumpRunRightStop_LandRun) { return true; }
		if (rTransitionID == TRANS_JumpRunRightStop_IdlePose) { return true; }
		if (rTransitionID == TRANS_GlideStart_GlideStop) { return true; }
		if (rTransitionID == TRANS_GlideStop_LandRun) { return true; }
		if (rTransitionID == TRANS_GlideStop_IdlePose) { return true; }
		return false;
	 }

	 /// <summary>
	 /// Preprocess any animator data so the motion can use it later
	 /// </summary>
	 public override void LoadAnimatorData()
	 {
		string lLayer = mMotionController.Animator.GetLayerName(mMotionLayer._AnimatorLayerIndex);
		TRANS_AnyState_JumpRise = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Jump-SM.JumpRise");
		TRANS_EntryState_JumpRise = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Jump-SM.JumpRise");
		TRANS_AnyState_JumpFallPose = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Jump-SM.JumpFallPose");
		TRANS_EntryState_JumpFallPose = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Jump-SM.JumpFallPose");
		TRANS_AnyState_JumpRiseToTop = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Jump-SM.JumpRiseToTop");
		TRANS_EntryState_JumpRiseToTop = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Jump-SM.JumpRiseToTop");
		TRANS_AnyState_JumpRunRightStart = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Jump-SM.JumpRunRightStart");
		TRANS_EntryState_JumpRunRightStart = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Jump-SM.JumpRunRightStart");
		TRANS_AnyState_JumpRunLeftStart = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Jump-SM.JumpRunLeftStart");
		TRANS_EntryState_JumpRunLeftStart = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Jump-SM.JumpRunLeftStart");
		TRANS_AnyState_GlideStart = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Jump-SM.GlideStart");
		TRANS_EntryState_GlideStart = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Jump-SM.GlideStart");
		STATE_Start = mMotionController.AddAnimatorName("" + lLayer + ".Start");
		STATE_JumpRise = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRise");
		TRANS_JumpRise_JumpRiseToTop = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRise -> " + lLayer + ".Jump-SM.JumpRiseToTop");
		TRANS_JumpRise_JumpRisePose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRise -> " + lLayer + ".Jump-SM.JumpRisePose");
		STATE_JumpLand = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpLand");
		TRANS_JumpLand_JumpRecoverRun = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpLand -> " + lLayer + ".Jump-SM.JumpRecoverRun");
		TRANS_JumpLand_JumpRecoverIdle = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpLand -> " + lLayer + ".Jump-SM.JumpRecoverIdle");
		STATE_JumpRisePose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRisePose");
		TRANS_JumpRisePose_JumpRiseToTop = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRisePose -> " + lLayer + ".Jump-SM.JumpRiseToTop");
		STATE_JumpFallPose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpFallPose");
		TRANS_JumpFallPose_JumpLand = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpFallPose -> " + lLayer + ".Jump-SM.JumpLand");
		STATE_JumpTopToFall = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpTopToFall");
		TRANS_JumpTopToFall_JumpLand = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpTopToFall -> " + lLayer + ".Jump-SM.JumpLand");
		TRANS_JumpTopToFall_JumpFallPose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpTopToFall -> " + lLayer + ".Jump-SM.JumpFallPose");
		TRANS_JumpTopToFall_JumpRecoverIdle = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpTopToFall -> " + lLayer + ".Jump-SM.JumpRecoverIdle");
		TRANS_JumpTopToFall_JumpRecoverRun = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpTopToFall -> " + lLayer + ".Jump-SM.JumpRecoverRun");
		STATE_JumpRiseToTop = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRiseToTop");
		TRANS_JumpRiseToTop_JumpTopToFall = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRiseToTop -> " + lLayer + ".Jump-SM.JumpTopToFall");
		TRANS_JumpRiseToTop_JumpTopPose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRiseToTop -> " + lLayer + ".Jump-SM.JumpTopPose");
		TRANS_JumpRiseToTop_JumpRecoverIdle = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRiseToTop -> " + lLayer + ".Jump-SM.JumpRecoverIdle");
		TRANS_JumpRiseToTop_JumpRecoverRun = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRiseToTop -> " + lLayer + ".Jump-SM.JumpRecoverRun");
		STATE_JumpTopPose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpTopPose");
		TRANS_JumpTopPose_JumpTopToFall = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpTopPose -> " + lLayer + ".Jump-SM.JumpTopToFall");
		TRANS_JumpTopPose_JumpRecoverIdle = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpTopPose -> " + lLayer + ".Jump-SM.JumpRecoverIdle");
		STATE_JumpRecoverIdle = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRecoverIdle");
		TRANS_JumpRecoverIdle_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRecoverIdle -> " + lLayer + ".Jump-SM.IdlePose");
		STATE_JumpRecoverRun = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRecoverRun");
		STATE_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.IdlePose");
		STATE_JumpRunRightStart = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunRightStart");
		TRANS_JumpRunRightStart_JumpRunRightStop = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunRightStart -> " + lLayer + ".Jump-SM.JumpRunRightStop");
		TRANS_JumpRunRightStart_JumpFallPose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunRightStart -> " + lLayer + ".Jump-SM.JumpFallPose");
		STATE_JumpRunLeftStart = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunLeftStart");
		TRANS_JumpRunLeftStart_JumpRunLeftStop = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunLeftStart -> " + lLayer + ".Jump-SM.JumpRunLeftStop");
		TRANS_JumpRunLeftStart_JumpFallPose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunLeftStart -> " + lLayer + ".Jump-SM.JumpFallPose");
		STATE_JumpRunLeftStop = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunLeftStop");
		TRANS_JumpRunLeftStop_LandRun = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunLeftStop -> " + lLayer + ".Jump-SM.LandRun");
		TRANS_JumpRunLeftStop_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunLeftStop -> " + lLayer + ".Jump-SM.IdlePose");
		STATE_JumpRunRightStop = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunRightStop");
		TRANS_JumpRunRightStop_LandRun = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunRightStop -> " + lLayer + ".Jump-SM.LandRun");
		TRANS_JumpRunRightStop_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.JumpRunRightStop -> " + lLayer + ".Jump-SM.IdlePose");
		STATE_LandRun = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.LandRun");
		STATE_GlideStart = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.GlideStart");
		TRANS_GlideStart_GlideStop = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.GlideStart -> " + lLayer + ".Jump-SM.GlideStop");
		STATE_GlideStop = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.GlideStop");
		TRANS_GlideStop_LandRun = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.GlideStop -> " + lLayer + ".Jump-SM.LandRun");
		TRANS_GlideStop_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".Jump-SM.GlideStop -> " + lLayer + ".Jump-SM.IdlePose");
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

		UnityEditor.Animations.AnimatorStateMachine lSSM_53030 = MotionControllerMotion.EditorFindSSM(lLayerStateMachine, "Jump-SM");
		if (lSSM_53030 == null) { lSSM_53030 = lLayerStateMachine.AddStateMachine("Jump-SM", new Vector3(390, -200, 0)); }

		UnityEditor.Animations.AnimatorState lState_53050 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpRise");
		if (lState_53050 == null) { lState_53050 = lSSM_53030.AddState("JumpRise", new Vector3(-30, 160, 0)); }
		lState_53050.speed = 1f;
		lState_53050.mirror = false;
		lState_53050.tag = "";
		lState_53050.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/StayJumpAll.fbx", "IdleToRise");

		UnityEditor.Animations.AnimatorState lState_53052 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpLand");
		if (lState_53052 == null) { lState_53052 = lSSM_53030.AddState("JumpLand", new Vector3(900, 130, 0)); }
		lState_53052.speed = 1.2f;
		lState_53052.mirror = false;
		lState_53052.tag = "";
		lState_53052.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/StayJumpAll.fbx", "FallToLand");

		UnityEditor.Animations.AnimatorState lState_53054 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpRisePose");
		if (lState_53054 == null) { lState_53054 = lSSM_53030.AddState("JumpRisePose", new Vector3(120, -10, 0)); }
		lState_53054.speed = 1f;
		lState_53054.mirror = false;
		lState_53054.tag = "";
		lState_53054.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/StayJumpAll.fbx", "RisePose");

		UnityEditor.Animations.AnimatorState lState_53056 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpFallPose");
		if (lState_53056 == null) { lState_53056 = lSSM_53030.AddState("JumpFallPose", new Vector3(640, -130, 0)); }
		lState_53056.speed = 0.8f;
		lState_53056.mirror = false;
		lState_53056.tag = "";
		lState_53056.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/StayJumpAll.fbx", "FallPose");

		UnityEditor.Animations.AnimatorState lState_53058 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpTopToFall");
		if (lState_53058 == null) { lState_53058 = lSSM_53030.AddState("JumpTopToFall", new Vector3(552, 132, 0)); }
		lState_53058.speed = 1f;
		lState_53058.mirror = false;
		lState_53058.tag = "";
		lState_53058.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/StayJumpAll.fbx", "TopToFall");

		UnityEditor.Animations.AnimatorState lState_53060 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpRiseToTop");
		if (lState_53060 == null) { lState_53060 = lSSM_53030.AddState("JumpRiseToTop", new Vector3(252, 132, 0)); }
		lState_53060.speed = 1f;
		lState_53060.mirror = false;
		lState_53060.tag = "";
		lState_53060.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/StayJumpAll.fbx", "RiseToTop");

		UnityEditor.Animations.AnimatorState lState_53062 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpTopPose");
		if (lState_53062 == null) { lState_53062 = lSSM_53030.AddState("JumpTopPose", new Vector3(396, 12, 0)); }
		lState_53062.speed = 1f;
		lState_53062.mirror = false;
		lState_53062.tag = "";
		lState_53062.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/StayJumpAll.fbx", "TopPose");

		UnityEditor.Animations.AnimatorState lState_53064 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpRecoverIdle");
		if (lState_53064 == null) { lState_53064 = lSSM_53030.AddState("JumpRecoverIdle", new Vector3(948, -84, 0)); }
		lState_53064.speed = 1.5f;
		lState_53064.mirror = false;
		lState_53064.tag = "";
		lState_53064.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/StayJumpAll.fbx", "LandToIdle");

		UnityEditor.Animations.AnimatorState lState_53066 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpRecoverRun");
		if (lState_53066 == null) { lState_53066 = lSSM_53030.AddState("JumpRecoverRun", new Vector3(936, 252, 0)); }
		lState_53066.speed = 1f;
		lState_53066.mirror = false;
		lState_53066.tag = "";
		lState_53066.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/StayJumpAll.fbx", "LandToRun2");

		UnityEditor.Animations.AnimatorState lState_53068 = MotionControllerMotion.EditorFindState(lSSM_53030, "IdlePose");
		if (lState_53068 == null) { lState_53068 = lSSM_53030.AddState("IdlePose", new Vector3(1176, -84, 0)); }
		lState_53068.speed = 1f;
		lState_53068.mirror = false;
		lState_53068.tag = "";
		lState_53068.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/IdleWalkRun.fbx", "Idle");

		UnityEditor.Animations.AnimatorState lState_53070 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpRunRightStart");
		if (lState_53070 == null) { lState_53070 = lSSM_53030.AddState("JumpRunRightStart", new Vector3(440, -400, 0)); }
		lState_53070.speed = 0.7f;
		lState_53070.mirror = false;
		lState_53070.tag = "";
		lState_53070.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunRightStartFull");

		UnityEditor.Animations.AnimatorState lState_53072 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpRunLeftStart");
		if (lState_53072 == null) { lState_53072 = lSSM_53030.AddState("JumpRunLeftStart", new Vector3(440, -340, 0)); }
		lState_53072.speed = 0.7f;
		lState_53072.mirror = false;
		lState_53072.tag = "";
		lState_53072.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunLeftStartFull");

		UnityEditor.Animations.AnimatorState lState_53074 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpRunLeftStop");
		if (lState_53074 == null) { lState_53074 = lSSM_53030.AddState("JumpRunLeftStop", new Vector3(700, -340, 0)); }
		lState_53074.speed = 1f;
		lState_53074.mirror = false;
		lState_53074.tag = "";
		lState_53074.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunLeftStop");

		UnityEditor.Animations.AnimatorState lState_53076 = MotionControllerMotion.EditorFindState(lSSM_53030, "JumpRunRightStop");
		if (lState_53076 == null) { lState_53076 = lSSM_53030.AddState("JumpRunRightStop", new Vector3(700, -400, 0)); }
		lState_53076.speed = 1f;
		lState_53076.mirror = false;
		lState_53076.tag = "";
		lState_53076.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunRightStop");

		UnityEditor.Animations.AnimatorState lState_53078 = MotionControllerMotion.EditorFindState(lSSM_53030, "LandRun");
		if (lState_53078 == null) { lState_53078 = lSSM_53030.AddState("LandRun", new Vector3(1090, -370, 0)); }
		lState_53078.speed = 1f;
		lState_53078.mirror = false;
		lState_53078.tag = "";
		lState_53078.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/Run3New.fbx", "Run");

		UnityEditor.Animations.AnimatorState lState_N8222 = MotionControllerMotion.EditorFindState(lSSM_53030, "GlideStart");
		if (lState_N8222 == null) { lState_N8222 = lSSM_53030.AddState("GlideStart", new Vector3(440, -520, 0)); }
		lState_N8222.speed = 1f;
		lState_N8222.mirror = false;
		lState_N8222.tag = "";
		lState_N8222.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "GlideStart");

		UnityEditor.Animations.AnimatorState lState_N8394 = MotionControllerMotion.EditorFindState(lSSM_53030, "GlideStop");
		if (lState_N8394 == null) { lState_N8394 = lSSM_53030.AddState("GlideStop", new Vector3(700, -520, 0)); }
		lState_N8394.speed = 1f;
		lState_N8394.mirror = false;
		lState_N8394.tag = "";
		lState_N8394.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "GlideStop");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_53202 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_53050, 0);
		if (lAnyTransition_53202 == null) { lAnyTransition_53202 = lLayerStateMachine.AddAnyStateTransition(lState_53050); }
		lAnyTransition_53202.isExit = false;
		lAnyTransition_53202.hasExitTime = false;
		lAnyTransition_53202.hasFixedDuration = true;
		lAnyTransition_53202.exitTime = 0.5f;
		lAnyTransition_53202.duration = 0.1f;
		lAnyTransition_53202.offset = 0f;
		lAnyTransition_53202.mute = false;
		lAnyTransition_53202.solo = false;
		lAnyTransition_53202.canTransitionToSelf = true;
		lAnyTransition_53202.orderedInterruption = true;
		lAnyTransition_53202.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_53202.conditions.Length - 1; i >= 0; i--) { lAnyTransition_53202.RemoveCondition(lAnyTransition_53202.conditions[i]); }
		lAnyTransition_53202.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 251f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_53204 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_53056, 0);
		if (lAnyTransition_53204 == null) { lAnyTransition_53204 = lLayerStateMachine.AddAnyStateTransition(lState_53056); }
		lAnyTransition_53204.isExit = false;
		lAnyTransition_53204.hasExitTime = false;
		lAnyTransition_53204.hasFixedDuration = true;
		lAnyTransition_53204.exitTime = 0.9f;
		lAnyTransition_53204.duration = 0.2f;
		lAnyTransition_53204.offset = 0f;
		lAnyTransition_53204.mute = false;
		lAnyTransition_53204.solo = false;
		lAnyTransition_53204.canTransitionToSelf = true;
		lAnyTransition_53204.orderedInterruption = true;
		lAnyTransition_53204.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_53204.conditions.Length - 1; i >= 0; i--) { lAnyTransition_53204.RemoveCondition(lAnyTransition_53204.conditions[i]); }
		lAnyTransition_53204.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 250f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_53330 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_53060, 0);
		if (lAnyTransition_53330 == null) { lAnyTransition_53330 = lLayerStateMachine.AddAnyStateTransition(lState_53060); }
		lAnyTransition_53330.isExit = false;
		lAnyTransition_53330.hasExitTime = false;
		lAnyTransition_53330.hasFixedDuration = true;
		lAnyTransition_53330.exitTime = 0.7500001f;
		lAnyTransition_53330.duration = 0.1582668f;
		lAnyTransition_53330.offset = 0f;
		lAnyTransition_53330.mute = false;
		lAnyTransition_53330.solo = false;
		lAnyTransition_53330.canTransitionToSelf = true;
		lAnyTransition_53330.orderedInterruption = true;
		lAnyTransition_53330.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_53330.conditions.Length - 1; i >= 0; i--) { lAnyTransition_53330.RemoveCondition(lAnyTransition_53330.conditions[i]); }
		lAnyTransition_53330.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 252f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_53344 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_53070, 0);
		if (lAnyTransition_53344 == null) { lAnyTransition_53344 = lLayerStateMachine.AddAnyStateTransition(lState_53070); }
		lAnyTransition_53344.isExit = false;
		lAnyTransition_53344.hasExitTime = false;
		lAnyTransition_53344.hasFixedDuration = true;
		lAnyTransition_53344.exitTime = 0.7499999f;
		lAnyTransition_53344.duration = 0.1340237f;
		lAnyTransition_53344.offset = 0f;
		lAnyTransition_53344.mute = false;
		lAnyTransition_53344.solo = false;
		lAnyTransition_53344.canTransitionToSelf = true;
		lAnyTransition_53344.orderedInterruption = true;
		lAnyTransition_53344.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_53344.conditions.Length - 1; i >= 0; i--) { lAnyTransition_53344.RemoveCondition(lAnyTransition_53344.conditions[i]); }
		lAnyTransition_53344.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272552f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_53346 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_53072, 0);
		if (lAnyTransition_53346 == null) { lAnyTransition_53346 = lLayerStateMachine.AddAnyStateTransition(lState_53072); }
		lAnyTransition_53346.isExit = false;
		lAnyTransition_53346.hasExitTime = false;
		lAnyTransition_53346.hasFixedDuration = true;
		lAnyTransition_53346.exitTime = 0.7499999f;
		lAnyTransition_53346.duration = 0.1082513f;
		lAnyTransition_53346.offset = 0f;
		lAnyTransition_53346.mute = false;
		lAnyTransition_53346.solo = false;
		lAnyTransition_53346.canTransitionToSelf = true;
		lAnyTransition_53346.orderedInterruption = true;
		lAnyTransition_53346.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_53346.conditions.Length - 1; i >= 0; i--) { lAnyTransition_53346.RemoveCondition(lAnyTransition_53346.conditions[i]); }
		lAnyTransition_53346.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272551f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_N8764 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_N8222, 0);
		if (lAnyTransition_N8764 == null) { lAnyTransition_N8764 = lLayerStateMachine.AddAnyStateTransition(lState_N8222); }
		lAnyTransition_N8764.isExit = false;
		lAnyTransition_N8764.hasExitTime = false;
		lAnyTransition_N8764.hasFixedDuration = true;
		lAnyTransition_N8764.exitTime = 0.75f;
		lAnyTransition_N8764.duration = 0.25f;
		lAnyTransition_N8764.offset = 0f;
		lAnyTransition_N8764.mute = false;
		lAnyTransition_N8764.solo = false;
		lAnyTransition_N8764.canTransitionToSelf = true;
		lAnyTransition_N8764.orderedInterruption = true;
		lAnyTransition_N8764.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_N8764.conditions.Length - 1; i >= 0; i--) { lAnyTransition_N8764.RemoveCondition(lAnyTransition_N8764.conditions[i]); }
		lAnyTransition_N8764.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272560f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_53966 = MotionControllerMotion.EditorFindTransition(lState_53050, lState_53060, 0);
		if (lTransition_53966 == null) { lTransition_53966 = lState_53050.AddTransition(lState_53060); }
		lTransition_53966.isExit = false;
		lTransition_53966.hasExitTime = false;
		lTransition_53966.hasFixedDuration = false;
		lTransition_53966.exitTime = 0.9427966f;
		lTransition_53966.duration = 0.07627118f;
		lTransition_53966.offset = 0f;
		lTransition_53966.mute = false;
		lTransition_53966.solo = false;
		lTransition_53966.canTransitionToSelf = true;
		lTransition_53966.orderedInterruption = true;
		lTransition_53966.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53966.conditions.Length - 1; i >= 0; i--) { lTransition_53966.RemoveCondition(lTransition_53966.conditions[i]); }
		lTransition_53966.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 203f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_53968 = MotionControllerMotion.EditorFindTransition(lState_53050, lState_53054, 0);
		if (lTransition_53968 == null) { lTransition_53968 = lState_53050.AddTransition(lState_53054); }
		lTransition_53968.isExit = false;
		lTransition_53968.hasExitTime = true;
		lTransition_53968.hasFixedDuration = false;
		lTransition_53968.exitTime = 0.9455966f;
		lTransition_53968.duration = 0.05858077f;
		lTransition_53968.offset = 0f;
		lTransition_53968.mute = false;
		lTransition_53968.solo = false;
		lTransition_53968.canTransitionToSelf = true;
		lTransition_53968.orderedInterruption = true;
		lTransition_53968.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53968.conditions.Length - 1; i >= 0; i--) { lTransition_53968.RemoveCondition(lTransition_53968.conditions[i]); }

		UnityEditor.Animations.AnimatorStateTransition lTransition_53972 = MotionControllerMotion.EditorFindTransition(lState_53052, lState_53066, 0);
		if (lTransition_53972 == null) { lTransition_53972 = lState_53052.AddTransition(lState_53066); }
		lTransition_53972.isExit = false;
		lTransition_53972.hasExitTime = true;
		lTransition_53972.hasFixedDuration = false;
		lTransition_53972.exitTime = 0.100004f;
		lTransition_53972.duration = 0.3956454f;
		lTransition_53972.offset = 0.01488612f;
		lTransition_53972.mute = false;
		lTransition_53972.solo = false;
		lTransition_53972.canTransitionToSelf = true;
		lTransition_53972.orderedInterruption = true;
		lTransition_53972.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53972.conditions.Length - 1; i >= 0; i--) { lTransition_53972.RemoveCondition(lTransition_53972.conditions[i]); }
		lTransition_53972.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 209f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_53974 = MotionControllerMotion.EditorFindTransition(lState_53052, lState_53064, 0);
		if (lTransition_53974 == null) { lTransition_53974 = lState_53052.AddTransition(lState_53064); }
		lTransition_53974.isExit = false;
		lTransition_53974.hasExitTime = true;
		lTransition_53974.hasFixedDuration = false;
		lTransition_53974.exitTime = 0.207459f;
		lTransition_53974.duration = 0.169278f;
		lTransition_53974.offset = 0f;
		lTransition_53974.mute = false;
		lTransition_53974.solo = false;
		lTransition_53974.canTransitionToSelf = true;
		lTransition_53974.orderedInterruption = true;
		lTransition_53974.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53974.conditions.Length - 1; i >= 0; i--) { lTransition_53974.RemoveCondition(lTransition_53974.conditions[i]); }
		lTransition_53974.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 208f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_53978 = MotionControllerMotion.EditorFindTransition(lState_53054, lState_53060, 0);
		if (lTransition_53978 == null) { lTransition_53978 = lState_53054.AddTransition(lState_53060); }
		lTransition_53978.isExit = false;
		lTransition_53978.hasExitTime = false;
		lTransition_53978.hasFixedDuration = false;
		lTransition_53978.exitTime = 0.9f;
		lTransition_53978.duration = 0f;
		lTransition_53978.offset = 0f;
		lTransition_53978.mute = false;
		lTransition_53978.solo = false;
		lTransition_53978.canTransitionToSelf = true;
		lTransition_53978.orderedInterruption = true;
		lTransition_53978.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53978.conditions.Length - 1; i >= 0; i--) { lTransition_53978.RemoveCondition(lTransition_53978.conditions[i]); }
		lTransition_53978.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 203f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_53982 = MotionControllerMotion.EditorFindTransition(lState_53056, lState_53052, 0);
		if (lTransition_53982 == null) { lTransition_53982 = lState_53056.AddTransition(lState_53052); }
		lTransition_53982.isExit = false;
		lTransition_53982.hasExitTime = false;
		lTransition_53982.hasFixedDuration = false;
		lTransition_53982.exitTime = 0.02201979f;
		lTransition_53982.duration = 0.01773792f;
		lTransition_53982.offset = 0f;
		lTransition_53982.mute = false;
		lTransition_53982.solo = false;
		lTransition_53982.canTransitionToSelf = true;
		lTransition_53982.orderedInterruption = true;
		lTransition_53982.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53982.conditions.Length - 1; i >= 0; i--) { lTransition_53982.RemoveCondition(lTransition_53982.conditions[i]); }
		lTransition_53982.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 207f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_53986 = MotionControllerMotion.EditorFindTransition(lState_53058, lState_53052, 0);
		if (lTransition_53986 == null) { lTransition_53986 = lState_53058.AddTransition(lState_53052); }
		lTransition_53986.isExit = false;
		lTransition_53986.hasExitTime = false;
		lTransition_53986.hasFixedDuration = false;
		lTransition_53986.exitTime = 0.9f;
		lTransition_53986.duration = 0f;
		lTransition_53986.offset = 0f;
		lTransition_53986.mute = false;
		lTransition_53986.solo = false;
		lTransition_53986.canTransitionToSelf = true;
		lTransition_53986.orderedInterruption = true;
		lTransition_53986.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53986.conditions.Length - 1; i >= 0; i--) { lTransition_53986.RemoveCondition(lTransition_53986.conditions[i]); }
		lTransition_53986.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 207f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_53988 = MotionControllerMotion.EditorFindTransition(lState_53058, lState_53056, 0);
		if (lTransition_53988 == null) { lTransition_53988 = lState_53058.AddTransition(lState_53056); }
		lTransition_53988.isExit = false;
		lTransition_53988.hasExitTime = true;
		lTransition_53988.hasFixedDuration = false;
		lTransition_53988.exitTime = 0.8359196f;
		lTransition_53988.duration = 0.159396f;
		lTransition_53988.offset = 0f;
		lTransition_53988.mute = false;
		lTransition_53988.solo = false;
		lTransition_53988.canTransitionToSelf = true;
		lTransition_53988.orderedInterruption = true;
		lTransition_53988.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53988.conditions.Length - 1; i >= 0; i--) { lTransition_53988.RemoveCondition(lTransition_53988.conditions[i]); }

		UnityEditor.Animations.AnimatorStateTransition lTransition_53990 = MotionControllerMotion.EditorFindTransition(lState_53058, lState_53064, 0);
		if (lTransition_53990 == null) { lTransition_53990 = lState_53058.AddTransition(lState_53064); }
		lTransition_53990.isExit = false;
		lTransition_53990.hasExitTime = false;
		lTransition_53990.hasFixedDuration = false;
		lTransition_53990.exitTime = 0.9f;
		lTransition_53990.duration = 0.4982873f;
		lTransition_53990.offset = 0f;
		lTransition_53990.mute = false;
		lTransition_53990.solo = false;
		lTransition_53990.canTransitionToSelf = true;
		lTransition_53990.orderedInterruption = true;
		lTransition_53990.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53990.conditions.Length - 1; i >= 0; i--) { lTransition_53990.RemoveCondition(lTransition_53990.conditions[i]); }
		lTransition_53990.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 208f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_53992 = MotionControllerMotion.EditorFindTransition(lState_53058, lState_53066, 0);
		if (lTransition_53992 == null) { lTransition_53992 = lState_53058.AddTransition(lState_53066); }
		lTransition_53992.isExit = false;
		lTransition_53992.hasExitTime = false;
		lTransition_53992.hasFixedDuration = false;
		lTransition_53992.exitTime = 0.9f;
		lTransition_53992.duration = 0.5029359f;
		lTransition_53992.offset = 0f;
		lTransition_53992.mute = false;
		lTransition_53992.solo = false;
		lTransition_53992.canTransitionToSelf = true;
		lTransition_53992.orderedInterruption = true;
		lTransition_53992.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53992.conditions.Length - 1; i >= 0; i--) { lTransition_53992.RemoveCondition(lTransition_53992.conditions[i]); }
		lTransition_53992.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 209f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_53996 = MotionControllerMotion.EditorFindTransition(lState_53060, lState_53058, 0);
		if (lTransition_53996 == null) { lTransition_53996 = lState_53060.AddTransition(lState_53058); }
		lTransition_53996.isExit = false;
		lTransition_53996.hasExitTime = false;
		lTransition_53996.hasFixedDuration = false;
		lTransition_53996.exitTime = 0.903662f;
		lTransition_53996.duration = 0.1926761f;
		lTransition_53996.offset = 0f;
		lTransition_53996.mute = false;
		lTransition_53996.solo = false;
		lTransition_53996.canTransitionToSelf = true;
		lTransition_53996.orderedInterruption = true;
		lTransition_53996.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53996.conditions.Length - 1; i >= 0; i--) { lTransition_53996.RemoveCondition(lTransition_53996.conditions[i]); }
		lTransition_53996.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 205f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_53998 = MotionControllerMotion.EditorFindTransition(lState_53060, lState_53062, 0);
		if (lTransition_53998 == null) { lTransition_53998 = lState_53060.AddTransition(lState_53062); }
		lTransition_53998.isExit = false;
		lTransition_53998.hasExitTime = true;
		lTransition_53998.hasFixedDuration = false;
		lTransition_53998.exitTime = 1f;
		lTransition_53998.duration = 0f;
		lTransition_53998.offset = 0f;
		lTransition_53998.mute = false;
		lTransition_53998.solo = false;
		lTransition_53998.canTransitionToSelf = true;
		lTransition_53998.orderedInterruption = true;
		lTransition_53998.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_53998.conditions.Length - 1; i >= 0; i--) { lTransition_53998.RemoveCondition(lTransition_53998.conditions[i]); }
		lTransition_53998.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 203f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54000 = MotionControllerMotion.EditorFindTransition(lState_53060, lState_53064, 0);
		if (lTransition_54000 == null) { lTransition_54000 = lState_53060.AddTransition(lState_53064); }
		lTransition_54000.isExit = false;
		lTransition_54000.hasExitTime = true;
		lTransition_54000.hasFixedDuration = false;
		lTransition_54000.exitTime = 0.07359297f;
		lTransition_54000.duration = 1.948052f;
		lTransition_54000.offset = 0.005134339f;
		lTransition_54000.mute = false;
		lTransition_54000.solo = false;
		lTransition_54000.canTransitionToSelf = true;
		lTransition_54000.orderedInterruption = true;
		lTransition_54000.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54000.conditions.Length - 1; i >= 0; i--) { lTransition_54000.RemoveCondition(lTransition_54000.conditions[i]); }
		lTransition_54000.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 208f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54002 = MotionControllerMotion.EditorFindTransition(lState_53060, lState_53066, 0);
		if (lTransition_54002 == null) { lTransition_54002 = lState_53060.AddTransition(lState_53066); }
		lTransition_54002.isExit = false;
		lTransition_54002.hasExitTime = true;
		lTransition_54002.hasFixedDuration = false;
		lTransition_54002.exitTime = 0f;
		lTransition_54002.duration = 2.5f;
		lTransition_54002.offset = 0f;
		lTransition_54002.mute = false;
		lTransition_54002.solo = false;
		lTransition_54002.canTransitionToSelf = true;
		lTransition_54002.orderedInterruption = true;
		lTransition_54002.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54002.conditions.Length - 1; i >= 0; i--) { lTransition_54002.RemoveCondition(lTransition_54002.conditions[i]); }
		lTransition_54002.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 209f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54006 = MotionControllerMotion.EditorFindTransition(lState_53062, lState_53058, 0);
		if (lTransition_54006 == null) { lTransition_54006 = lState_53062.AddTransition(lState_53058); }
		lTransition_54006.isExit = false;
		lTransition_54006.hasExitTime = false;
		lTransition_54006.hasFixedDuration = false;
		lTransition_54006.exitTime = 0.9f;
		lTransition_54006.duration = 0.2013423f;
		lTransition_54006.offset = 0f;
		lTransition_54006.mute = false;
		lTransition_54006.solo = false;
		lTransition_54006.canTransitionToSelf = true;
		lTransition_54006.orderedInterruption = true;
		lTransition_54006.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54006.conditions.Length - 1; i >= 0; i--) { lTransition_54006.RemoveCondition(lTransition_54006.conditions[i]); }
		lTransition_54006.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 205f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54008 = MotionControllerMotion.EditorFindTransition(lState_53062, lState_53064, 0);
		if (lTransition_54008 == null) { lTransition_54008 = lState_53062.AddTransition(lState_53064); }
		lTransition_54008.isExit = false;
		lTransition_54008.hasExitTime = false;
		lTransition_54008.hasFixedDuration = false;
		lTransition_54008.exitTime = 0.9f;
		lTransition_54008.duration = 1.25f;
		lTransition_54008.offset = 0f;
		lTransition_54008.mute = false;
		lTransition_54008.solo = false;
		lTransition_54008.canTransitionToSelf = true;
		lTransition_54008.orderedInterruption = true;
		lTransition_54008.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54008.conditions.Length - 1; i >= 0; i--) { lTransition_54008.RemoveCondition(lTransition_54008.conditions[i]); }
		lTransition_54008.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 208f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54012 = MotionControllerMotion.EditorFindTransition(lState_53064, lState_53068, 0);
		if (lTransition_54012 == null) { lTransition_54012 = lState_53064.AddTransition(lState_53068); }
		lTransition_54012.isExit = false;
		lTransition_54012.hasExitTime = true;
		lTransition_54012.hasFixedDuration = true;
		lTransition_54012.exitTime = 0.4549774f;
		lTransition_54012.duration = 0.2560845f;
		lTransition_54012.offset = 0f;
		lTransition_54012.mute = false;
		lTransition_54012.solo = false;
		lTransition_54012.canTransitionToSelf = true;
		lTransition_54012.orderedInterruption = true;
		lTransition_54012.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54012.conditions.Length - 1; i >= 0; i--) { lTransition_54012.RemoveCondition(lTransition_54012.conditions[i]); }

		UnityEditor.Animations.AnimatorStateTransition lTransition_54018 = MotionControllerMotion.EditorFindTransition(lState_53070, lState_53076, 0);
		if (lTransition_54018 == null) { lTransition_54018 = lState_53070.AddTransition(lState_53076); }
		lTransition_54018.isExit = false;
		lTransition_54018.hasExitTime = false;
		lTransition_54018.hasFixedDuration = true;
		lTransition_54018.exitTime = 0.9156361f;
		lTransition_54018.duration = 0.03471392f;
		lTransition_54018.offset = 0f;
		lTransition_54018.mute = false;
		lTransition_54018.solo = false;
		lTransition_54018.canTransitionToSelf = true;
		lTransition_54018.orderedInterruption = true;
		lTransition_54018.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54018.conditions.Length - 1; i >= 0; i--) { lTransition_54018.RemoveCondition(lTransition_54018.conditions[i]); }
		lTransition_54018.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272553f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54020 = MotionControllerMotion.EditorFindTransition(lState_53070, lState_53056, 0);
		if (lTransition_54020 == null) { lTransition_54020 = lState_53070.AddTransition(lState_53056); }
		lTransition_54020.isExit = false;
		lTransition_54020.hasExitTime = true;
		lTransition_54020.hasFixedDuration = true;
		lTransition_54020.exitTime = 0.9163325f;
		lTransition_54020.duration = 0.03652212f;
		lTransition_54020.offset = 0f;
		lTransition_54020.mute = false;
		lTransition_54020.solo = false;
		lTransition_54020.canTransitionToSelf = true;
		lTransition_54020.orderedInterruption = true;
		lTransition_54020.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54020.conditions.Length - 1; i >= 0; i--) { lTransition_54020.RemoveCondition(lTransition_54020.conditions[i]); }
		lTransition_54020.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 206f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54024 = MotionControllerMotion.EditorFindTransition(lState_53072, lState_53074, 0);
		if (lTransition_54024 == null) { lTransition_54024 = lState_53072.AddTransition(lState_53074); }
		lTransition_54024.isExit = false;
		lTransition_54024.hasExitTime = false;
		lTransition_54024.hasFixedDuration = true;
		lTransition_54024.exitTime = 0.8503405f;
		lTransition_54024.duration = 0.08721232f;
		lTransition_54024.offset = 0f;
		lTransition_54024.mute = false;
		lTransition_54024.solo = false;
		lTransition_54024.canTransitionToSelf = true;
		lTransition_54024.orderedInterruption = true;
		lTransition_54024.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54024.conditions.Length - 1; i >= 0; i--) { lTransition_54024.RemoveCondition(lTransition_54024.conditions[i]); }
		lTransition_54024.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272553f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54026 = MotionControllerMotion.EditorFindTransition(lState_53072, lState_53056, 0);
		if (lTransition_54026 == null) { lTransition_54026 = lState_53072.AddTransition(lState_53056); }
		lTransition_54026.isExit = false;
		lTransition_54026.hasExitTime = true;
		lTransition_54026.hasFixedDuration = true;
		lTransition_54026.exitTime = 0.9358018f;
		lTransition_54026.duration = 0.03316906f;
		lTransition_54026.offset = 0f;
		lTransition_54026.mute = false;
		lTransition_54026.solo = false;
		lTransition_54026.canTransitionToSelf = true;
		lTransition_54026.orderedInterruption = true;
		lTransition_54026.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54026.conditions.Length - 1; i >= 0; i--) { lTransition_54026.RemoveCondition(lTransition_54026.conditions[i]); }
		lTransition_54026.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 206f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54030 = MotionControllerMotion.EditorFindTransition(lState_53074, lState_53078, 0);
		if (lTransition_54030 == null) { lTransition_54030 = lState_53074.AddTransition(lState_53078); }
		lTransition_54030.isExit = false;
		lTransition_54030.hasExitTime = true;
		lTransition_54030.hasFixedDuration = true;
		lTransition_54030.exitTime = 0.02913063f;
		lTransition_54030.duration = 0.07748188f;
		lTransition_54030.offset = 5.995386E-08f;
		lTransition_54030.mute = false;
		lTransition_54030.solo = false;
		lTransition_54030.canTransitionToSelf = true;
		lTransition_54030.orderedInterruption = true;
		lTransition_54030.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54030.conditions.Length - 1; i >= 0; i--) { lTransition_54030.RemoveCondition(lTransition_54030.conditions[i]); }
		lTransition_54030.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 209f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54032 = MotionControllerMotion.EditorFindTransition(lState_53074, lState_53068, 0);
		if (lTransition_54032 == null) { lTransition_54032 = lState_53074.AddTransition(lState_53068); }
		lTransition_54032.isExit = false;
		lTransition_54032.hasExitTime = true;
		lTransition_54032.hasFixedDuration = true;
		lTransition_54032.exitTime = 0f;
		lTransition_54032.duration = 0.25f;
		lTransition_54032.offset = 0f;
		lTransition_54032.mute = false;
		lTransition_54032.solo = false;
		lTransition_54032.canTransitionToSelf = true;
		lTransition_54032.orderedInterruption = true;
		lTransition_54032.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54032.conditions.Length - 1; i >= 0; i--) { lTransition_54032.RemoveCondition(lTransition_54032.conditions[i]); }
		lTransition_54032.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 208f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54036 = MotionControllerMotion.EditorFindTransition(lState_53076, lState_53078, 0);
		if (lTransition_54036 == null) { lTransition_54036 = lState_53076.AddTransition(lState_53078); }
		lTransition_54036.isExit = false;
		lTransition_54036.hasExitTime = true;
		lTransition_54036.hasFixedDuration = true;
		lTransition_54036.exitTime = 0.06032767f;
		lTransition_54036.duration = 0.04871098f;
		lTransition_54036.offset = 0.1903456f;
		lTransition_54036.mute = false;
		lTransition_54036.solo = false;
		lTransition_54036.canTransitionToSelf = true;
		lTransition_54036.orderedInterruption = true;
		lTransition_54036.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54036.conditions.Length - 1; i >= 0; i--) { lTransition_54036.RemoveCondition(lTransition_54036.conditions[i]); }
		lTransition_54036.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 209f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_54038 = MotionControllerMotion.EditorFindTransition(lState_53076, lState_53068, 0);
		if (lTransition_54038 == null) { lTransition_54038 = lState_53076.AddTransition(lState_53068); }
		lTransition_54038.isExit = false;
		lTransition_54038.hasExitTime = true;
		lTransition_54038.hasFixedDuration = true;
		lTransition_54038.exitTime = 0f;
		lTransition_54038.duration = 0.07265625f;
		lTransition_54038.offset = 0f;
		lTransition_54038.mute = false;
		lTransition_54038.solo = false;
		lTransition_54038.canTransitionToSelf = true;
		lTransition_54038.orderedInterruption = true;
		lTransition_54038.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_54038.conditions.Length - 1; i >= 0; i--) { lTransition_54038.RemoveCondition(lTransition_54038.conditions[i]); }
		lTransition_54038.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 208f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_N8980 = MotionControllerMotion.EditorFindTransition(lState_N8222, lState_N8394, 0);
		if (lTransition_N8980 == null) { lTransition_N8980 = lState_N8222.AddTransition(lState_N8394); }
		lTransition_N8980.isExit = false;
		lTransition_N8980.hasExitTime = true;
		lTransition_N8980.hasFixedDuration = true;
		lTransition_N8980.exitTime = 0.7692308f;
		lTransition_N8980.duration = 0.25f;
		lTransition_N8980.offset = 0f;
		lTransition_N8980.mute = false;
		lTransition_N8980.solo = false;
		lTransition_N8980.canTransitionToSelf = true;
		lTransition_N8980.orderedInterruption = true;
		lTransition_N8980.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_N8980.conditions.Length - 1; i >= 0; i--) { lTransition_N8980.RemoveCondition(lTransition_N8980.conditions[i]); }
		lTransition_N8980.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272561f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_N9196 = MotionControllerMotion.EditorFindTransition(lState_N8394, lState_53078, 0);
		if (lTransition_N9196 == null) { lTransition_N9196 = lState_N8394.AddTransition(lState_53078); }
		lTransition_N9196.isExit = false;
		lTransition_N9196.hasExitTime = true;
		lTransition_N9196.hasFixedDuration = true;
		lTransition_N9196.exitTime = 0.5161291f;
		lTransition_N9196.duration = 0.25f;
		lTransition_N9196.offset = 0f;
		lTransition_N9196.mute = false;
		lTransition_N9196.solo = false;
		lTransition_N9196.canTransitionToSelf = true;
		lTransition_N9196.orderedInterruption = true;
		lTransition_N9196.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_N9196.conditions.Length - 1; i >= 0; i--) { lTransition_N9196.RemoveCondition(lTransition_N9196.conditions[i]); }
		lTransition_N9196.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 209f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_N9346 = MotionControllerMotion.EditorFindTransition(lState_N8394, lState_53068, 0);
		if (lTransition_N9346 == null) { lTransition_N9346 = lState_N8394.AddTransition(lState_53068); }
		lTransition_N9346.isExit = false;
		lTransition_N9346.hasExitTime = true;
		lTransition_N9346.hasFixedDuration = true;
		lTransition_N9346.exitTime = 0.5161291f;
		lTransition_N9346.duration = 0.25f;
		lTransition_N9346.offset = 0f;
		lTransition_N9346.mute = false;
		lTransition_N9346.solo = false;
		lTransition_N9346.canTransitionToSelf = true;
		lTransition_N9346.orderedInterruption = true;
		lTransition_N9346.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_N9346.conditions.Length - 1; i >= 0; i--) { lTransition_N9346.RemoveCondition(lTransition_N9346.conditions[i]); }
		lTransition_N9346.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 208f, "L" + rLayerIndex + "MotionPhase");


		// Run any post processing after creating the state machine
		OnStateMachineCreated();
	 }

#endif

	 // ************************************ END AUTO GENERATED ************************************
	 #endregion


  }
}
