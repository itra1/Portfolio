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
  [MotionName("Running Jump v2")]
  [MotionDescription("Jump when the actor is running forward.")]
  public class RunningJump_v2 : MotionControllerMotion, IRunningJump
  {

	 public const int PHASE_UNKNOWN = 0;
	 public const int PHASE_START = 272500;
	 public const int PHASE_START_LEFT = 272501;
	 public const int PHASE_START_RIGHT = 272502;
	 public const int PHASE_UP = 272503;
	 public const int PHASE_DOWN = 272504;

	 public const int PHASE_TOP = 272520;
	 public const int PHASE_FALL = 272530;
	 public const int PHASE_LAND_IDLE = 272540;
	 public const int PHASE_LAND_RUN = 272545;

	 public bool _ConvertToHipBase = false;
	 public bool ConvertToHipBase
	 {
		get { return _ConvertToHipBase; }
		set { _ConvertToHipBase = value; }
	 }
	 protected Transform mHipBone = null;

	 protected IWalkRunMotion mWalkRunMotion = null; 
	 
	 public float _RequiredOverheadDistance = 0f;
	 public float RequiredOverheadDistance
	 {
		get { return _RequiredOverheadDistance; }
		set { _RequiredOverheadDistance = value; }
	 }

	 public float _Impulse = 250f;
	 public float Impulse
	 {
		get { return _Impulse; }
		set { _Impulse = value; }
	 }

	 public float _MaxJumpTime = 5f;
	 public float MaxJumpTime
	 {
		get { return _MaxJumpTime; }
		set { _MaxJumpTime = value; }
	 }

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
    public float _RotationToCameraSpeed = 360f;
    public float RotationToCameraSpeed
    {
      get { return _RotationToCameraSpeed; }
      set { _RotationToCameraSpeed = value; }
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

    public float _ForwardImpulse = 7f;
    public float ForwardImpulse
    {
      get { return _ForwardImpulse; }
      set { _ForwardImpulse = value; }
    }
    public float _MinFallHeight = 2f;
    public float MinFallHeight
    {
      get { return _MinFallHeight; }
      set { _MinFallHeight = value; }
    }
    private float _forwardSpeed = 4;
    public float ForwardSpeed { get => _forwardSpeed; set => _forwardSpeed = value; }

    protected float mLastHipDistance = 0f;
	 protected Vector3 mLaunchForward = Vector3.zero;
	 protected bool mIsImpulseApplied = false;
	 protected Vector3 mLaunchVelocity = Vector3.zero;
    private bool _isLeft;
    protected bool mRotateWithCamera = false;
    protected bool mLinkRotation = false;
    protected MotionControllerMotion mFall = null;
    protected bool mIsExitTriggered = false;

    public RunningJump_v2()
		  : base()
	 {
		_Priority = 16;
		_ActionAlias = "Jump";
		mIsStartable = true;
		//mIsGroundedExpected = false;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "RunningJumpPastel-SM"; }
#endif
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public RunningJump_v2(MotionController rController)
		  : base(rController)
	 {
		_Priority = 16;
		_ActionAlias = "Jump";
		mIsStartable = true;
		//mIsGroundedExpected = false;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "RunningJumpPastel-SM"; }
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
		  if (RaycastExt.SafeSphereCast(mActorController.transform.position + mActorController.transform.up * mActorController.Height, mActorController.transform.up, mActorController.BodyShapes[0].Radius / 2, 0.2f, mActorController.CollisionLayers, mActorController._Transform))
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

	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		// Flag the motion as active
		mIsActivatedFrame = true;
		mIsStartable = false;

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
      PlayAnim();
      //mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START_SHORT, true);
		mActorController.AddImpulse(mActorController._Transform.up * _Impulse);
		mActorController.AddImpulse(mActorController._Transform.forward * (_Impulse / 3));

		// Register this motion with the camera
		if (_RotateWithCamera && mMotionController.CameraRig is BaseCameraRig)
		{
		  ((BaseCameraRig)mMotionController.CameraRig).OnPostLateUpdate -= OnCameraUpdated;
		  ((BaseCameraRig)mMotionController.CameraRig).OnPostLateUpdate += OnCameraUpdated;
		}

		// Report that we're good to enter the jump
		return base.Activate(rPrevMotion);
    }

    private void PlayAnim()
    {

      if (_isLeft)
      {
        mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START_LEFT, true);
      }
      else
      {
        mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START_RIGHT, true);
      }
    }

    public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		Debug.Log("Vel jump start " + mActorController.Velocity);
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
      //if (lTransitionID == TRANS_EntryState_JumpRise)
      //{
      //  if (!mIsImpulseApplied && lTransitionTime > 0.5f)
      //  {
      //	 mIsImpulseApplied = true;
      //	 mActorController.AddImpulse(mActorController._Transform.up * _Impulse);
      //  }
      //}
      // This is the start of the jump. The animator will automatically move on after the node
      // has finished. However, it could move to the "JumpRisePose" or "JumpRiseToTop"
      //else if (lStateID == STATE_JumpRise)
      //{
      //  // We really shouldn't get here, but if the transition time is 
      //  // too short, we'll end up skipping past the previous impluse. This
      //  // happens with something like the idle pose
      //  if (!mIsImpulseApplied)
      //  {
      //	 mIsImpulseApplied = true;
      //	 mActorController.AddImpulse(mActorController._Transform.up * _Impulse);
      //  }
      //  // If our velocity is trailing off, move to the top position
      //  else if (lVelocity.y < 1.5f)
      //  {
      //	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RISE_TO_TOP);
      //  }
      //}
      // This is the holding position for a super high jump. The pose gives us time
      // before the top occurs.
      //else if (lStateID == STATE_JumpRisePose)
      //{
      //  // If our velocity is trailing off, move to the top position
      //  if (lVelocity.y < 2.5f)
      //  {
      //	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RISE_TO_TOP);
      //  }

      //  //mMovement = (mActorController._Transform.rotation * new Vector3(0f, lHipDistanceDelta, 0f));
      //  mMovement = mActorController._Transform.up * lHipDistanceDelta;
      //}
      // At this point, we're close to the peak of the jump and we need to transition
      // into the top position.
      //else if (lStateID == STATE_JumpRiseToTop)
      //{
      //  Debug.Log(lVelocity.y);
      //  // If we slow down, start moving to the fall position
      //  //if (lVelocity.y < -1.5f) // || lState.GroundDistance < 0.45f)
      //  if (mActorController.AccumulatedVelocity.y < -1.5f) // || lState.GroundDistance < 0.45f)
      //  {
      //	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_TOP_TO_FALL);
      //  }

      //  //mMovement = (mActorController._Transform.rotation * new Vector3(0f, lHipDistanceDelta, 0f));
      //  mMovement = mActorController._Transform.up * lHipDistanceDelta;
      //}
      // We should be at the peak of the jump. We don't expect to wait here
      // long, but this gives us a "pose" to hold onto if needed
      //else if (lStateID == STATE_JumpTopPose)
      //{
      //  // We may have moved over something during the jump. If so, 
      //  // we can move straight to the recover
      //  if (mActorController.State.GroundSurfaceDistance < 0.5f)
      //  {
      //	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_IDLE);
      //  }
      //  // If we've reached the fall speed, transition
      //  else if (lVelocity.y < -0.25f)
      //  {
      //	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_TOP_TO_FALL);
      //	 StartFallHeight = mActorController._Transform.position.y;
      //  }
      //  // Otherwise, ensure we're in the right phase
      //  else
      //  {
      //	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_TOP);
      //  }

      //  mMovement = mActorController._Transform.up * lHipDistanceDelta;
      //}
      // Here we come out of the top pose and start moving into the fall pose
      //else if (lStateID == STATE_JumpTopToFall)
      //{
      //  // If we got ontop of something, we may need to recover.
      //  if (mActorController.State.GroundSurfaceDistance < 0.15f)
      //  {
      //	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_IDLE);
      //  }
      //  // Look for the ground and prepare to transition
      //  else if (mActorController.State.GroundSurfaceDistance < 0.35f)
      //  {
      //	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_LAND);
      //  }

      //  mMovement = mActorController._Transform.up * lHipDistanceDelta;
      //}
      // We could be falling for a while. This animation allows us to 
      // hold in the falling state until we hit the ground.



      //if (lStateID == STATE_JumpFallPose)
      //{
      //  //mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, Jump_old.PHASE_FALL);

      //  // Look for the ground. In this case, we want a 
      //  // value that is slightly greater than our collider radius
      //  if (mActorController.State.GroundSurfaceDistance < 0.35f)
      //  {
      //	 if (isFaling)
      //	 {
      //		isFaling = false;
      //		float fallHeight = StartFallHeight - mActorController._Transform.position.y;

      //		if (fallHeight > 6)
      //		{
      //		  Game.Managers.GameManager.Instance.UserManager.Health.Damage(mMotionController, 1000);
      //		  return;
      //		}
      //	 }


      //	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_LAND);
      //  }

      //  mMovement = mActorController._Transform.up * lHipDistanceDelta;
      //}
      //// This is the first state in the jump where we hit the ground
      //else if (lStateID == STATE_JumpLand)
      //{
      //  if (mActorController.State.IsGrounded)
      //  {
      //	 // If there is no controller input, we can stop
      //	 if (lState.InputMagnitudeTrend.Value < 0.03f)
      //	 {
      //		mLaunchVelocity = Vector3.zero;
      //		mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_IDLE);
      //	 }
      //	 // If the player is messing with the input, we need to think about
      //	 // what to transition to
      //	 else
      //	 {
      //		if (mWalkRunMotion != null && mWalkRunMotion.IsEnabled && mWalkRunMotion.IsRunActive)
      //		{
      //		  if (Mathf.Abs(lState.InputFromAvatarAngle) > 140) { mLaunchVelocity = Vector3.zero; }
      //		  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_MOVE);
      //		}
      //		else
      //		{
      //		  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RECOVER_TO_IDLE);
      //		}
      //	 }
      //  }
      //  else
      //  {
      //	 mMovement = mActorController._Transform.up * lHipDistanceDelta;
      //  }
      //}


      if (lStateID == STATE_RunJumpLeftStart
        || lStateID == STATE_RunJumpRightStart)
      {
        if (!mIsImpulseApplied)
        {
          mIsImpulseApplied = true;
        }

        // As we come to the end, we have a couple of options
        if (/*!mIsExitTriggered &&*/ lStateTime > 1f)
        {
          // If we're a long way from the ground, transition to a fall
          if (mFall != null && mFall.IsEnabled && mActorController.State.GroundSurfaceDistance > _MinFallHeight)
          {
            Debug.Log("Fall");
            mIsExitTriggered = true;
            mMotionController.ActivateMotion(mFall);
          }
          // If we're still getting input, keep running
          else if (lState.InputMagnitudeTrend.Value >= 0.1f) // && Mathf.Abs(lState.InputFromAvatarAngle) < 10f)
          {
            //Deactivate();
            if (/*mActorController.State.Velocity.y <= 0 &&*/ mActorController.State.GroundSurfaceDistance <= 0.4f)
            {
              mIsExitTriggered = true;
              //mWalkRunMotion.StartInRun = mWalkRunMotion.IsRunActive;
              //mWalkRunMotion.StartInWalk = !mWalkRunMotion.StartInRun;
              //mMotionController.ActivateMotion(mWalkRunMotion as MotionControllerMotion);
              Debug.Log("PHASE_LAND_RUN");
              mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_LAND_RUN, true);
            }
            //
          }
          // Come to a quick stop
          else if (mActorController.State.GroundSurfaceDistance <= 0.15f)
          {
            mIsExitTriggered = true;
            Deactivate();
            Debug.Log("PHASE_LAND_IDLE");
            mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_LAND_IDLE, true);
          }
        }
        // While in the jump, adjust the displacement
        else
        {
          mMovement = mActorController._Transform.up * lHipDistanceDelta;
        }
      }
      else if (!mIsImpulseApplied && lStateID == STATE_RunForward)
      {
        PlayAnim();
      }

      if (lStateID == STATE_RunJumpLeftStart
        || lStateID == STATE_RunJumpRightStart)
      {

      }
      else if (!mIsImpulseApplied && lStateID == STATE_RunJump_RunForward)
      {
        PlayAnim();
      }
      // Once we get into the run forward, we can transition to the true run
      else if (mIsImpulseApplied && (lStateID == STATE_RunJumpLeftStop
            || lStateID == STATE_RunJumpLeftStop0
            || lStateID == STATE_RunJumpRightStop
            || lStateID == STATE_RunJumpRightStop0))
      {
        lAllowSlide = true;
        //if (TestActivateJump(false))
        //{
        //  ActivateJump(!_isLeft);
        //  return;
        //}

        if (mIsImpulseApplied)
        {
          if (lStateTime > 0.4f && mActorController.State.GroundSurfaceDistance <= 0.15f)
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
      }



      //// Called when the avatar starts to come out of the impact
      //if  (lStateID == STATE_Idle)
      //{
      //  mIsStartable = true;

      //  // If we're moving forward, transition into the run/walk
      //  if (lState.InputMagnitudeTrend.Value >= 0.1f && Mathf.Abs(lState.InputFromAvatarAngle) < 20f)
      //  {
      //	 // Allow us to keep moving so we blend into the run
      //	 lAllowSlide = true;

      //	 // It may be time to move into the walk/run
      //	 if (lStateTime > 0.3f)
      //	 {
      //		if (mWalkRunMotion != null && mWalkRunMotion.IsEnabled)
      //		{
      //		  mWalkRunMotion.StartInRun = mWalkRunMotion.IsRunActive;
      //		  mWalkRunMotion.StartInWalk = !mWalkRunMotion.StartInRun;
      //		  mMotionController.ActivateMotion(mWalkRunMotion as MotionControllerMotion);
      //		}

      //	 }
      //  }
      //}
      //// Called when the avatar starts to come out of the impact
      //else if (lStateID == STATE_RunForward)
      //{
      //  // Allow the animation to control movement again
      //  mLaunchVelocity = Vector3.zero;

      //  if (mWalkRunMotion != null && mWalkRunMotion.IsEnabled)
      //  {
      //	 // Allow us to keep moving so we blend into the run
      //	 lAllowSlide = true;

      //	 if (lStateTime > 0.2f)
      //	 {
      //		mWalkRunMotion.StartInRun = mWalkRunMotion.IsRunActive;
      //		mWalkRunMotion.StartInWalk = !mWalkRunMotion.StartInRun;
      //		mMotionController.ActivateMotion(mWalkRunMotion as MotionControllerMotion);
      //	 }
      //  }
      //  else
      //  {
      //	 Deactivate();
      //  }
      //}
      //// If there's no movement, we're done
      //else if (lStateID == STATE_IdlePose && mAge > 0.05f)
      //{
      //  Deactivate();
      //}

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
		Debug.Log("Vel jump end " + mVelocity);
	 }

	 protected bool IsInLandedState
	 {
		get
		{
		  int lStateID = mMotionLayer._AnimatorStateID;
		  //if (lStateID == STATE_JumpRecoverIdle) { return true; }
		  if (lStateID == STATE_RunForward) { return true; }
		  if (lStateID == STATE_IdlePose) { return true; }

		  return false;
		}
	 }

	 public override void UpdateRootMotion(float rDeltaTime, int rUpdateIndex, ref Vector3 rVelocityDelta, ref Quaternion rRotationDelta)
	 {
		rRotationDelta = Quaternion.identity;

		if (!IsInLandedState)
		{
		  rVelocityDelta = Vector3.zero;
		}
	 }

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
      if ((lStateID != STATE_RunForward || rAllowSlide) &&
          (lStateID != STATE_Idle || rAllowSlide) &&
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
      float lNewForvardImpulse = EditorGUILayout.FloatField(new GUIContent("Impulse Forvard", "Forvard"), ForwardImpulse);
      if (lNewForvardImpulse != ForwardImpulse)
      {
        lIsDirty = true;
        ForwardImpulse = lNewForvardImpulse;
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
    public int STATE_IdlePose = -1;
    public int STATE_RunJump_RunForward = -1;
    public int STATE_RunningJump = -1;
    public int STATE_LandToIdle = -1;
    public int STATE_RunJumpEnd = -1;
    public int STATE_RunJumpIdle = -1;
    public int STATE_RunJumpLeftStart = -1;
    public int STATE_RunJumpLeftStop = -1;
    public int STATE_RunJumpRightStart = -1;
    public int STATE_RunJumpRightStop = -1;
    public int STATE_RunJumpLeftStop0 = -1;
    public int STATE_RunJumpRightStop0 = -1;
    public int STATE_RunJumpLeftUp = -1;
    public int STATE_RunJumpLeftDown = -1;
    public int STATE_RunJumpRightUp = -1;
    public int STATE_RunJumpRightDown = -1;
    public int STATE_RunForward = -1;
    public int STATE_Idle = -1;
    public int TRANS_AnyState_RunningJump = -1;
    public int TRANS_EntryState_RunningJump = -1;
    public int TRANS_AnyState_RunJumpLeftStart = -1;
    public int TRANS_EntryState_RunJumpLeftStart = -1;
    public int TRANS_AnyState_RunJumpRightStart = -1;
    public int TRANS_EntryState_RunJumpRightStart = -1;
    public int TRANS_RunningJump_RunJump_RunForward = -1;
    public int TRANS_RunningJump_LandToIdle = -1;
    public int TRANS_RunningJump_RunJumpIdle = -1;
    public int TRANS_LandToIdle_IdlePose = -1;
    public int TRANS_RunJumpIdle_RunJump_RunForward = -1;
    public int TRANS_RunJumpIdle_LandToIdle = -1;
    public int TRANS_RunJumpLeftStart_RunJumpLeftUp = -1;
    public int TRANS_RunJumpLeftStop_Idle = -1;
    public int TRANS_RunJumpRightStart_RunJumpRightUp = -1;
    public int TRANS_RunJumpRightStop_Idle = -1;
    public int TRANS_RunJumpLeftStop0_RunForward = -1;
    public int TRANS_RunJumpRightStop0_RunForward = -1;
    public int TRANS_RunJumpLeftUp_RunJumpLeftDown = -1;
    public int TRANS_RunJumpLeftDown_RunJumpLeftStop0 = -1;
    public int TRANS_RunJumpLeftDown_RunJumpLeftStop = -1;
    public int TRANS_RunJumpRightUp_RunJumpRightDown = -1;
    public int TRANS_RunJumpRightDown_RunJumpRightStop0 = -1;
    public int TRANS_RunJumpRightDown_RunJumpRightStop = -1;

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
          if (lStateID == STATE_IdlePose) { return true; }
          if (lStateID == STATE_RunJump_RunForward) { return true; }
          if (lStateID == STATE_RunningJump) { return true; }
          if (lStateID == STATE_LandToIdle) { return true; }
          if (lStateID == STATE_RunJumpEnd) { return true; }
          if (lStateID == STATE_RunJumpIdle) { return true; }
          if (lStateID == STATE_RunJumpLeftStart) { return true; }
          if (lStateID == STATE_RunJumpLeftStop) { return true; }
          if (lStateID == STATE_RunJumpRightStart) { return true; }
          if (lStateID == STATE_RunJumpRightStop) { return true; }
          if (lStateID == STATE_RunJumpLeftStop0) { return true; }
          if (lStateID == STATE_RunJumpRightStop0) { return true; }
          if (lStateID == STATE_RunJumpLeftUp) { return true; }
          if (lStateID == STATE_RunJumpLeftDown) { return true; }
          if (lStateID == STATE_RunJumpRightUp) { return true; }
          if (lStateID == STATE_RunJumpRightDown) { return true; }
          if (lStateID == STATE_RunForward) { return true; }
          if (lStateID == STATE_Idle) { return true; }
        }

        if (lTransitionID == TRANS_AnyState_RunningJump) { return true; }
        if (lTransitionID == TRANS_EntryState_RunningJump) { return true; }
        if (lTransitionID == TRANS_AnyState_RunJumpLeftStart) { return true; }
        if (lTransitionID == TRANS_EntryState_RunJumpLeftStart) { return true; }
        if (lTransitionID == TRANS_AnyState_RunJumpRightStart) { return true; }
        if (lTransitionID == TRANS_EntryState_RunJumpRightStart) { return true; }
        if (lTransitionID == TRANS_RunningJump_RunJump_RunForward) { return true; }
        if (lTransitionID == TRANS_RunningJump_LandToIdle) { return true; }
        if (lTransitionID == TRANS_RunningJump_RunJumpIdle) { return true; }
        if (lTransitionID == TRANS_LandToIdle_IdlePose) { return true; }
        if (lTransitionID == TRANS_RunJumpIdle_RunJump_RunForward) { return true; }
        if (lTransitionID == TRANS_RunJumpIdle_LandToIdle) { return true; }
        if (lTransitionID == TRANS_RunJumpLeftStart_RunJumpLeftUp) { return true; }
        if (lTransitionID == TRANS_RunJumpLeftStop_Idle) { return true; }
        if (lTransitionID == TRANS_RunJumpRightStart_RunJumpRightUp) { return true; }
        if (lTransitionID == TRANS_RunJumpRightStop_Idle) { return true; }
        if (lTransitionID == TRANS_RunJumpLeftStop0_RunForward) { return true; }
        if (lTransitionID == TRANS_RunJumpRightStop0_RunForward) { return true; }
        if (lTransitionID == TRANS_RunJumpLeftUp_RunJumpLeftDown) { return true; }
        if (lTransitionID == TRANS_RunJumpLeftDown_RunJumpLeftStop0) { return true; }
        if (lTransitionID == TRANS_RunJumpLeftDown_RunJumpLeftStop) { return true; }
        if (lTransitionID == TRANS_RunJumpRightUp_RunJumpRightDown) { return true; }
        if (lTransitionID == TRANS_RunJumpRightDown_RunJumpRightStop0) { return true; }
        if (lTransitionID == TRANS_RunJumpRightDown_RunJumpRightStop) { return true; }
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
      if (rStateID == STATE_IdlePose) { return true; }
      if (rStateID == STATE_RunJump_RunForward) { return true; }
      if (rStateID == STATE_RunningJump) { return true; }
      if (rStateID == STATE_LandToIdle) { return true; }
      if (rStateID == STATE_RunJumpEnd) { return true; }
      if (rStateID == STATE_RunJumpIdle) { return true; }
      if (rStateID == STATE_RunJumpLeftStart) { return true; }
      if (rStateID == STATE_RunJumpLeftStop) { return true; }
      if (rStateID == STATE_RunJumpRightStart) { return true; }
      if (rStateID == STATE_RunJumpRightStop) { return true; }
      if (rStateID == STATE_RunJumpLeftStop0) { return true; }
      if (rStateID == STATE_RunJumpRightStop0) { return true; }
      if (rStateID == STATE_RunJumpLeftUp) { return true; }
      if (rStateID == STATE_RunJumpLeftDown) { return true; }
      if (rStateID == STATE_RunJumpRightUp) { return true; }
      if (rStateID == STATE_RunJumpRightDown) { return true; }
      if (rStateID == STATE_RunForward) { return true; }
      if (rStateID == STATE_Idle) { return true; }
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
        if (rStateID == STATE_IdlePose) { return true; }
        if (rStateID == STATE_RunJump_RunForward) { return true; }
        if (rStateID == STATE_RunningJump) { return true; }
        if (rStateID == STATE_LandToIdle) { return true; }
        if (rStateID == STATE_RunJumpEnd) { return true; }
        if (rStateID == STATE_RunJumpIdle) { return true; }
        if (rStateID == STATE_RunJumpLeftStart) { return true; }
        if (rStateID == STATE_RunJumpLeftStop) { return true; }
        if (rStateID == STATE_RunJumpRightStart) { return true; }
        if (rStateID == STATE_RunJumpRightStop) { return true; }
        if (rStateID == STATE_RunJumpLeftStop0) { return true; }
        if (rStateID == STATE_RunJumpRightStop0) { return true; }
        if (rStateID == STATE_RunJumpLeftUp) { return true; }
        if (rStateID == STATE_RunJumpLeftDown) { return true; }
        if (rStateID == STATE_RunJumpRightUp) { return true; }
        if (rStateID == STATE_RunJumpRightDown) { return true; }
        if (rStateID == STATE_RunForward) { return true; }
        if (rStateID == STATE_Idle) { return true; }
      }

      if (rTransitionID == TRANS_AnyState_RunningJump) { return true; }
      if (rTransitionID == TRANS_EntryState_RunningJump) { return true; }
      if (rTransitionID == TRANS_AnyState_RunJumpLeftStart) { return true; }
      if (rTransitionID == TRANS_EntryState_RunJumpLeftStart) { return true; }
      if (rTransitionID == TRANS_AnyState_RunJumpRightStart) { return true; }
      if (rTransitionID == TRANS_EntryState_RunJumpRightStart) { return true; }
      if (rTransitionID == TRANS_RunningJump_RunJump_RunForward) { return true; }
      if (rTransitionID == TRANS_RunningJump_LandToIdle) { return true; }
      if (rTransitionID == TRANS_RunningJump_RunJumpIdle) { return true; }
      if (rTransitionID == TRANS_LandToIdle_IdlePose) { return true; }
      if (rTransitionID == TRANS_RunJumpIdle_RunJump_RunForward) { return true; }
      if (rTransitionID == TRANS_RunJumpIdle_LandToIdle) { return true; }
      if (rTransitionID == TRANS_RunJumpLeftStart_RunJumpLeftUp) { return true; }
      if (rTransitionID == TRANS_RunJumpLeftStop_Idle) { return true; }
      if (rTransitionID == TRANS_RunJumpRightStart_RunJumpRightUp) { return true; }
      if (rTransitionID == TRANS_RunJumpRightStop_Idle) { return true; }
      if (rTransitionID == TRANS_RunJumpLeftStop0_RunForward) { return true; }
      if (rTransitionID == TRANS_RunJumpRightStop0_RunForward) { return true; }
      if (rTransitionID == TRANS_RunJumpLeftUp_RunJumpLeftDown) { return true; }
      if (rTransitionID == TRANS_RunJumpLeftDown_RunJumpLeftStop0) { return true; }
      if (rTransitionID == TRANS_RunJumpLeftDown_RunJumpLeftStop) { return true; }
      if (rTransitionID == TRANS_RunJumpRightUp_RunJumpRightDown) { return true; }
      if (rTransitionID == TRANS_RunJumpRightDown_RunJumpRightStop0) { return true; }
      if (rTransitionID == TRANS_RunJumpRightDown_RunJumpRightStop) { return true; }
      return false;
    }

    /// <summary>
    /// Preprocess any animator data so the motion can use it later
    /// </summary>
    public override void LoadAnimatorData()
    {
      string lLayer = mMotionController.Animator.GetLayerName(mMotionLayer._AnimatorLayerIndex);
      TRANS_AnyState_RunningJump = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".RunningJumpPastel-SM.RunningJump");
      TRANS_EntryState_RunningJump = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".RunningJumpPastel-SM.RunningJump");
      TRANS_AnyState_RunJumpLeftStart = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".RunningJumpPastel-SM.RunJumpLeftStart");
      TRANS_EntryState_RunJumpLeftStart = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".RunningJumpPastel-SM.RunJumpLeftStart");
      TRANS_AnyState_RunJumpRightStart = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".RunningJumpPastel-SM.RunJumpRightStart");
      TRANS_EntryState_RunJumpRightStart = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".RunningJumpPastel-SM.RunJumpRightStart");
      STATE_Start = mMotionController.AddAnimatorName("" + lLayer + ".Start");
      STATE_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.IdlePose");
      STATE_RunJump_RunForward = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJump_RunForward");
      STATE_RunningJump = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunningJump");
      TRANS_RunningJump_RunJump_RunForward = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunningJump -> " + lLayer + ".RunningJumpPastel-SM.RunJump_RunForward");
      TRANS_RunningJump_LandToIdle = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunningJump -> " + lLayer + ".RunningJumpPastel-SM.LandToIdle");
      TRANS_RunningJump_RunJumpIdle = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunningJump -> " + lLayer + ".RunningJumpPastel-SM.RunJumpIdle");
      STATE_LandToIdle = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.LandToIdle");
      TRANS_LandToIdle_IdlePose = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.LandToIdle -> " + lLayer + ".RunningJumpPastel-SM.IdlePose");
      STATE_RunJumpEnd = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpEnd");
      STATE_RunJumpIdle = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpIdle");
      TRANS_RunJumpIdle_RunJump_RunForward = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpIdle -> " + lLayer + ".RunningJumpPastel-SM.RunJump_RunForward");
      TRANS_RunJumpIdle_LandToIdle = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpIdle -> " + lLayer + ".RunningJumpPastel-SM.LandToIdle");
      STATE_RunJumpLeftStart = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpLeftStart");
      TRANS_RunJumpLeftStart_RunJumpLeftUp = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpLeftStart -> " + lLayer + ".RunningJumpPastel-SM.RunJumpLeftUp");
      STATE_RunJumpLeftStop = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpLeftStop");
      TRANS_RunJumpLeftStop_Idle = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpLeftStop -> " + lLayer + ".RunningJumpPastel-SM.Idle");
      STATE_RunJumpRightStart = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpRightStart");
      TRANS_RunJumpRightStart_RunJumpRightUp = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpRightStart -> " + lLayer + ".RunningJumpPastel-SM.RunJumpRightUp");
      STATE_RunJumpRightStop = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpRightStop");
      TRANS_RunJumpRightStop_Idle = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpRightStop -> " + lLayer + ".RunningJumpPastel-SM.Idle");
      STATE_RunJumpLeftStop0 = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpLeftStop 0");
      TRANS_RunJumpLeftStop0_RunForward = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpLeftStop 0 -> " + lLayer + ".RunningJumpPastel-SM.RunForward");
      STATE_RunJumpRightStop0 = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpRightStop 0");
      TRANS_RunJumpRightStop0_RunForward = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpRightStop 0 -> " + lLayer + ".RunningJumpPastel-SM.RunForward");
      STATE_RunJumpLeftUp = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpLeftUp");
      TRANS_RunJumpLeftUp_RunJumpLeftDown = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpLeftUp -> " + lLayer + ".RunningJumpPastel-SM.RunJumpLeftDown");
      STATE_RunJumpLeftDown = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpLeftDown");
      TRANS_RunJumpLeftDown_RunJumpLeftStop0 = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpLeftDown -> " + lLayer + ".RunningJumpPastel-SM.RunJumpLeftStop 0");
      TRANS_RunJumpLeftDown_RunJumpLeftStop = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpLeftDown -> " + lLayer + ".RunningJumpPastel-SM.RunJumpLeftStop");
      STATE_RunJumpRightUp = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpRightUp");
      TRANS_RunJumpRightUp_RunJumpRightDown = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpRightUp -> " + lLayer + ".RunningJumpPastel-SM.RunJumpRightDown");
      STATE_RunJumpRightDown = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpRightDown");
      TRANS_RunJumpRightDown_RunJumpRightStop0 = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpRightDown -> " + lLayer + ".RunningJumpPastel-SM.RunJumpRightStop 0");
      TRANS_RunJumpRightDown_RunJumpRightStop = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunJumpRightDown -> " + lLayer + ".RunningJumpPastel-SM.RunJumpRightStop");
      STATE_RunForward = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.RunForward");
      STATE_Idle = mMotionController.AddAnimatorName("" + lLayer + ".RunningJumpPastel-SM.Idle");
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

      UnityEditor.Animations.AnimatorStateMachine lSSM_53576 = MotionControllerMotion.EditorFindSSM(lLayerStateMachine, "RunningJumpPastel-SM");
      if (lSSM_53576 == null) { lSSM_53576 = lLayerStateMachine.AddStateMachine("RunningJumpPastel-SM", new Vector3(390, -150, 0)); }

      UnityEditor.Animations.AnimatorState lState_53628 = MotionControllerMotion.EditorFindState(lSSM_53576, "IdlePose");
      if (lState_53628 == null) { lState_53628 = lSSM_53576.AddState("IdlePose", new Vector3(500, 1500, 0)); }
      lState_53628.speed = 1f;
      lState_53628.mirror = false;
      lState_53628.tag = "";
      lState_53628.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/IdleWalkRun.fbx", "Idle");

      UnityEditor.Animations.AnimatorState lState_53630 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJump_RunForward");
      if (lState_53630 == null) { lState_53630 = lSSM_53576.AddState("RunJump_RunForward", new Vector3(-520, 1060, 0)); }
      lState_53630.speed = 1f;
      lState_53630.mirror = false;
      lState_53630.tag = "";
      lState_53630.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/RunningJump.fbx", "RunJumpEnd");

      UnityEditor.Animations.AnimatorState lState_53632 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunningJump");
      if (lState_53632 == null) { lState_53632 = lSSM_53576.AddState("RunningJump", new Vector3(-790, 940, 0)); }
      lState_53632.speed = 1f;
      lState_53632.mirror = false;
      lState_53632.tag = "";
      lState_53632.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/RunningJump.fbx", "RunJumpStart");

      UnityEditor.Animations.AnimatorState lState_53634 = MotionControllerMotion.EditorFindState(lSSM_53576, "LandToIdle");
      if (lState_53634 == null) { lState_53634 = lSSM_53576.AddState("LandToIdle", new Vector3(-150, 1420, 0)); }
      lState_53634.speed = 2f;
      lState_53634.mirror = false;
      lState_53634.tag = "";
      lState_53634.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations_old/Export/Generic/Land/LandToIdle.fbx", "LandToIdle");

      UnityEditor.Animations.AnimatorState lState_53636 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpEnd");
      if (lState_53636 == null) { lState_53636 = lSSM_53576.AddState("RunJumpEnd", new Vector3(500, 1210, 0)); }
      lState_53636.speed = 1f;
      lState_53636.mirror = false;
      lState_53636.tag = "";
      lState_53636.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/RunningJump.fbx", "RunJumpEnd");

      UnityEditor.Animations.AnimatorState lState_53638 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpIdle");
      if (lState_53638 == null) { lState_53638 = lSSM_53576.AddState("RunJumpIdle", new Vector3(-800, 1090, 0)); }
      lState_53638.speed = 0.1f;
      lState_53638.mirror = false;
      lState_53638.tag = "";
      lState_53638.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/RunningJump.fbx", "RunJumpIdle");

      UnityEditor.Animations.AnimatorState lState_53640 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpLeftStart");
      if (lState_53640 == null) { lState_53640 = lSSM_53576.AddState("RunJumpLeftStart", new Vector3(1710, -340, 0)); }
      lState_53640.speed = 1f;
      lState_53640.mirror = false;
      lState_53640.tag = "";
      lState_53640.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunLeftStart");

      UnityEditor.Animations.AnimatorState lState_53642 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpLeftStop");
      if (lState_53642 == null) { lState_53642 = lSSM_53576.AddState("RunJumpLeftStop", new Vector3(1820, 230, 0)); }
      lState_53642.speed = 1f;
      lState_53642.mirror = false;
      lState_53642.tag = "";
      lState_53642.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunLeftStop");

      UnityEditor.Animations.AnimatorState lState_53644 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpRightStart");
      if (lState_53644 == null) { lState_53644 = lSSM_53576.AddState("RunJumpRightStart", new Vector3(810, -340, 0)); }
      lState_53644.speed = 1f;
      lState_53644.mirror = false;
      lState_53644.tag = "";
      lState_53644.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunRightStart");

      UnityEditor.Animations.AnimatorState lState_53646 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpRightStop");
      if (lState_53646 == null) { lState_53646 = lSSM_53576.AddState("RunJumpRightStop", new Vector3(680, 240, 0)); }
      lState_53646.speed = 1f;
      lState_53646.mirror = false;
      lState_53646.tag = "";
      lState_53646.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunRightStop");

      UnityEditor.Animations.AnimatorState lState_53648 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpLeftStop 0");
      if (lState_53648 == null) { lState_53648 = lSSM_53576.AddState("RunJumpLeftStop 0", new Vector3(1580, 230, 0)); }
      lState_53648.speed = 1f;
      lState_53648.mirror = false;
      lState_53648.tag = "";
      lState_53648.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunLeftStop");

      UnityEditor.Animations.AnimatorState lState_53650 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpRightStop 0");
      if (lState_53650 == null) { lState_53650 = lSSM_53576.AddState("RunJumpRightStop 0", new Vector3(920, 240, 0)); }
      lState_53650.speed = 1f;
      lState_53650.mirror = false;
      lState_53650.tag = "";
      lState_53650.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunRightStop");

      UnityEditor.Animations.AnimatorState lState_53652 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpLeftUp");
      if (lState_53652 == null) { lState_53652 = lSSM_53576.AddState("RunJumpLeftUp", new Vector3(1710, -160, 0)); }
      lState_53652.speed = 1f;
      lState_53652.mirror = false;
      lState_53652.tag = "";
      lState_53652.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunLeftUp");

      UnityEditor.Animations.AnimatorState lState_53654 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpLeftDown");
      if (lState_53654 == null) { lState_53654 = lSSM_53576.AddState("RunJumpLeftDown", new Vector3(1710, 0, 0)); }
      lState_53654.speed = 1f;
      lState_53654.mirror = false;
      lState_53654.tag = "";
      lState_53654.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunLeftDown");

      UnityEditor.Animations.AnimatorState lState_53656 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpRightUp");
      if (lState_53656 == null) { lState_53656 = lSSM_53576.AddState("RunJumpRightUp", new Vector3(810, -160, 0)); }
      lState_53656.speed = 1f;
      lState_53656.mirror = false;
      lState_53656.tag = "";
      lState_53656.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunRightUp");

      UnityEditor.Animations.AnimatorState lState_53658 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunJumpRightDown");
      if (lState_53658 == null) { lState_53658 = lSSM_53576.AddState("RunJumpRightDown", new Vector3(810, 0, 0)); }
      lState_53658.speed = 1f;
      lState_53658.mirror = false;
      lState_53658.tag = "";
      lState_53658.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/JumpFly.fbx", "JumpRunRightDown");

      UnityEditor.Animations.AnimatorState lState_53660 = MotionControllerMotion.EditorFindState(lSSM_53576, "RunForward");
      if (lState_53660 == null) { lState_53660 = lSSM_53576.AddState("RunForward", new Vector3(810, 740, 0)); }
      lState_53660.speed = 1f;
      lState_53660.mirror = false;
      lState_53660.tag = "";
      lState_53660.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/Run3New.fbx", "Run");

      UnityEditor.Animations.AnimatorState lState_53662 = MotionControllerMotion.EditorFindState(lSSM_53576, "Idle");
      if (lState_53662 == null) { lState_53662 = lSSM_53576.AddState("Idle", new Vector3(1710, 700, 0)); }
      lState_53662.speed = 1f;
      lState_53662.mirror = false;
      lState_53662.tag = "";
      lState_53662.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/Animations/Result/IdleWalkRun.fbx", "Idle");

      UnityEditor.Animations.AnimatorStateTransition lAnyTransition_53854 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_53632, 0);
      if (lAnyTransition_53854 == null) { lAnyTransition_53854 = lLayerStateMachine.AddAnyStateTransition(lState_53632); }
      lAnyTransition_53854.isExit = false;
      lAnyTransition_53854.hasExitTime = false;
      lAnyTransition_53854.hasFixedDuration = true;
      lAnyTransition_53854.exitTime = 0.7499999f;
      lAnyTransition_53854.duration = 0.1595023f;
      lAnyTransition_53854.offset = 0f;
      lAnyTransition_53854.mute = false;
      lAnyTransition_53854.solo = false;
      lAnyTransition_53854.canTransitionToSelf = false;
      lAnyTransition_53854.orderedInterruption = true;
      lAnyTransition_53854.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lAnyTransition_53854.conditions.Length - 1; i >= 0; i--) { lAnyTransition_53854.RemoveCondition(lAnyTransition_53854.conditions[i]); }
      lAnyTransition_53854.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272500f, "L" + rLayerIndex + "MotionPhase");

      UnityEditor.Animations.AnimatorStateTransition lAnyTransition_53860 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_53640, 0);
      if (lAnyTransition_53860 == null) { lAnyTransition_53860 = lLayerStateMachine.AddAnyStateTransition(lState_53640); }
      lAnyTransition_53860.isExit = false;
      lAnyTransition_53860.hasExitTime = false;
      lAnyTransition_53860.hasFixedDuration = true;
      lAnyTransition_53860.exitTime = 0.7499999f;
      lAnyTransition_53860.duration = 0.1063478f;
      lAnyTransition_53860.offset = 0f;
      lAnyTransition_53860.mute = false;
      lAnyTransition_53860.solo = false;
      lAnyTransition_53860.canTransitionToSelf = false;
      lAnyTransition_53860.orderedInterruption = true;
      lAnyTransition_53860.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lAnyTransition_53860.conditions.Length - 1; i >= 0; i--) { lAnyTransition_53860.RemoveCondition(lAnyTransition_53860.conditions[i]); }
      lAnyTransition_53860.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272501f, "L" + rLayerIndex + "MotionPhase");

      UnityEditor.Animations.AnimatorStateTransition lAnyTransition_53862 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_53644, 0);
      if (lAnyTransition_53862 == null) { lAnyTransition_53862 = lLayerStateMachine.AddAnyStateTransition(lState_53644); }
      lAnyTransition_53862.isExit = false;
      lAnyTransition_53862.hasExitTime = false;
      lAnyTransition_53862.hasFixedDuration = true;
      lAnyTransition_53862.exitTime = 0.75f;
      lAnyTransition_53862.duration = 0.25f;
      lAnyTransition_53862.offset = 0f;
      lAnyTransition_53862.mute = false;
      lAnyTransition_53862.solo = false;
      lAnyTransition_53862.canTransitionToSelf = false;
      lAnyTransition_53862.orderedInterruption = true;
      lAnyTransition_53862.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lAnyTransition_53862.conditions.Length - 1; i >= 0; i--) { lAnyTransition_53862.RemoveCondition(lAnyTransition_53862.conditions[i]); }
      lAnyTransition_53862.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272502f, "L" + rLayerIndex + "MotionPhase");

      UnityEditor.Animations.AnimatorStateTransition lTransition_54588 = MotionControllerMotion.EditorFindTransition(lState_53632, lState_53630, 0);
      if (lTransition_54588 == null) { lTransition_54588 = lState_53632.AddTransition(lState_53630); }
      lTransition_54588.isExit = false;
      lTransition_54588.hasExitTime = false;
      lTransition_54588.hasFixedDuration = true;
      lTransition_54588.exitTime = 0.8318413f;
      lTransition_54588.duration = 0.09999999f;
      lTransition_54588.offset = 0f;
      lTransition_54588.mute = false;
      lTransition_54588.solo = false;
      lTransition_54588.canTransitionToSelf = true;
      lTransition_54588.orderedInterruption = true;
      lTransition_54588.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54588.conditions.Length - 1; i >= 0; i--) { lTransition_54588.RemoveCondition(lTransition_54588.conditions[i]); }
      lTransition_54588.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272545f, "L" + rLayerIndex + "MotionPhase");

      UnityEditor.Animations.AnimatorStateTransition lTransition_54590 = MotionControllerMotion.EditorFindTransition(lState_53632, lState_53634, 0);
      if (lTransition_54590 == null) { lTransition_54590 = lState_53632.AddTransition(lState_53634); }
      lTransition_54590.isExit = false;
      lTransition_54590.hasExitTime = false;
      lTransition_54590.hasFixedDuration = true;
      lTransition_54590.exitTime = 0.8040466f;
      lTransition_54590.duration = 0.1708505f;
      lTransition_54590.offset = 0f;
      lTransition_54590.mute = false;
      lTransition_54590.solo = false;
      lTransition_54590.canTransitionToSelf = true;
      lTransition_54590.orderedInterruption = true;
      lTransition_54590.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54590.conditions.Length - 1; i >= 0; i--) { lTransition_54590.RemoveCondition(lTransition_54590.conditions[i]); }
      lTransition_54590.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272540f, "L" + rLayerIndex + "MotionPhase");

      UnityEditor.Animations.AnimatorStateTransition lTransition_54592 = MotionControllerMotion.EditorFindTransition(lState_53632, lState_53638, 0);
      if (lTransition_54592 == null) { lTransition_54592 = lState_53632.AddTransition(lState_53638); }
      lTransition_54592.isExit = false;
      lTransition_54592.hasExitTime = true;
      lTransition_54592.hasFixedDuration = true;
      lTransition_54592.exitTime = 0.9158372f;
      lTransition_54592.duration = 0.04366505f;
      lTransition_54592.offset = 0.01120519f;
      lTransition_54592.mute = false;
      lTransition_54592.solo = false;
      lTransition_54592.canTransitionToSelf = true;
      lTransition_54592.orderedInterruption = true;
      lTransition_54592.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54592.conditions.Length - 1; i >= 0; i--) { lTransition_54592.RemoveCondition(lTransition_54592.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54596 = MotionControllerMotion.EditorFindTransition(lState_53634, lState_53628, 0);
      if (lTransition_54596 == null) { lTransition_54596 = lState_53634.AddTransition(lState_53628); }
      lTransition_54596.isExit = false;
      lTransition_54596.hasExitTime = true;
      lTransition_54596.hasFixedDuration = true;
      lTransition_54596.exitTime = 0.01424012f;
      lTransition_54596.duration = 0.09816757f;
      lTransition_54596.offset = 0f;
      lTransition_54596.mute = false;
      lTransition_54596.solo = false;
      lTransition_54596.canTransitionToSelf = true;
      lTransition_54596.orderedInterruption = true;
      lTransition_54596.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54596.conditions.Length - 1; i >= 0; i--) { lTransition_54596.RemoveCondition(lTransition_54596.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54600 = MotionControllerMotion.EditorFindTransition(lState_53638, lState_53630, 0);
      if (lTransition_54600 == null) { lTransition_54600 = lState_53638.AddTransition(lState_53630); }
      lTransition_54600.isExit = false;
      lTransition_54600.hasExitTime = false;
      lTransition_54600.hasFixedDuration = true;
      lTransition_54600.exitTime = 0.04810545f;
      lTransition_54600.duration = 0.03901095f;
      lTransition_54600.offset = 0f;
      lTransition_54600.mute = false;
      lTransition_54600.solo = false;
      lTransition_54600.canTransitionToSelf = true;
      lTransition_54600.orderedInterruption = true;
      lTransition_54600.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54600.conditions.Length - 1; i >= 0; i--) { lTransition_54600.RemoveCondition(lTransition_54600.conditions[i]); }
      lTransition_54600.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272545f, "L" + rLayerIndex + "MotionPhase");

      UnityEditor.Animations.AnimatorStateTransition lTransition_54602 = MotionControllerMotion.EditorFindTransition(lState_53638, lState_53634, 0);
      if (lTransition_54602 == null) { lTransition_54602 = lState_53638.AddTransition(lState_53634); }
      lTransition_54602.isExit = false;
      lTransition_54602.hasExitTime = false;
      lTransition_54602.hasFixedDuration = true;
      lTransition_54602.exitTime = 0.3487694f;
      lTransition_54602.duration = 0.03939033f;
      lTransition_54602.offset = 0f;
      lTransition_54602.mute = false;
      lTransition_54602.solo = false;
      lTransition_54602.canTransitionToSelf = true;
      lTransition_54602.orderedInterruption = true;
      lTransition_54602.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54602.conditions.Length - 1; i >= 0; i--) { lTransition_54602.RemoveCondition(lTransition_54602.conditions[i]); }
      lTransition_54602.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 272540f, "L" + rLayerIndex + "MotionPhase");

      UnityEditor.Animations.AnimatorStateTransition lTransition_54606 = MotionControllerMotion.EditorFindTransition(lState_53640, lState_53652, 0);
      if (lTransition_54606 == null) { lTransition_54606 = lState_53640.AddTransition(lState_53652); }
      lTransition_54606.isExit = false;
      lTransition_54606.hasExitTime = true;
      lTransition_54606.hasFixedDuration = true;
      lTransition_54606.exitTime = 0.9131885f;
      lTransition_54606.duration = 0.02542034f;
      lTransition_54606.offset = 0f;
      lTransition_54606.mute = false;
      lTransition_54606.solo = false;
      lTransition_54606.canTransitionToSelf = true;
      lTransition_54606.orderedInterruption = true;
      lTransition_54606.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54606.conditions.Length - 1; i >= 0; i--) { lTransition_54606.RemoveCondition(lTransition_54606.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54610 = MotionControllerMotion.EditorFindTransition(lState_53642, lState_53662, 0);
      if (lTransition_54610 == null) { lTransition_54610 = lState_53642.AddTransition(lState_53662); }
      lTransition_54610.isExit = false;
      lTransition_54610.hasExitTime = true;
      lTransition_54610.hasFixedDuration = true;
      lTransition_54610.exitTime = 0.3750001f;
      lTransition_54610.duration = 0.25f;
      lTransition_54610.offset = 0f;
      lTransition_54610.mute = false;
      lTransition_54610.solo = false;
      lTransition_54610.canTransitionToSelf = true;
      lTransition_54610.orderedInterruption = true;
      lTransition_54610.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54610.conditions.Length - 1; i >= 0; i--) { lTransition_54610.RemoveCondition(lTransition_54610.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54614 = MotionControllerMotion.EditorFindTransition(lState_53644, lState_53656, 0);
      if (lTransition_54614 == null) { lTransition_54614 = lState_53644.AddTransition(lState_53656); }
      lTransition_54614.isExit = false;
      lTransition_54614.hasExitTime = true;
      lTransition_54614.hasFixedDuration = true;
      lTransition_54614.exitTime = 0.9423746f;
      lTransition_54614.duration = 0.01888344f;
      lTransition_54614.offset = 0f;
      lTransition_54614.mute = false;
      lTransition_54614.solo = false;
      lTransition_54614.canTransitionToSelf = true;
      lTransition_54614.orderedInterruption = true;
      lTransition_54614.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54614.conditions.Length - 1; i >= 0; i--) { lTransition_54614.RemoveCondition(lTransition_54614.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54618 = MotionControllerMotion.EditorFindTransition(lState_53646, lState_53662, 0);
      if (lTransition_54618 == null) { lTransition_54618 = lState_53646.AddTransition(lState_53662); }
      lTransition_54618.isExit = false;
      lTransition_54618.hasExitTime = true;
      lTransition_54618.hasFixedDuration = true;
      lTransition_54618.exitTime = 0f;
      lTransition_54618.duration = 0.1280787f;
      lTransition_54618.offset = 0f;
      lTransition_54618.mute = false;
      lTransition_54618.solo = false;
      lTransition_54618.canTransitionToSelf = true;
      lTransition_54618.orderedInterruption = true;
      lTransition_54618.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54618.conditions.Length - 1; i >= 0; i--) { lTransition_54618.RemoveCondition(lTransition_54618.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54622 = MotionControllerMotion.EditorFindTransition(lState_53648, lState_53660, 0);
      if (lTransition_54622 == null) { lTransition_54622 = lState_53648.AddTransition(lState_53660); }
      lTransition_54622.isExit = false;
      lTransition_54622.hasExitTime = true;
      lTransition_54622.hasFixedDuration = true;
      lTransition_54622.exitTime = 0.375f;
      lTransition_54622.duration = 0.25f;
      lTransition_54622.offset = 0.4638001f;
      lTransition_54622.mute = false;
      lTransition_54622.solo = false;
      lTransition_54622.canTransitionToSelf = true;
      lTransition_54622.orderedInterruption = true;
      lTransition_54622.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54622.conditions.Length - 1; i >= 0; i--) { lTransition_54622.RemoveCondition(lTransition_54622.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54624 = MotionControllerMotion.EditorFindTransition(lState_53650, lState_53660, 0);
      if (lTransition_54624 == null) { lTransition_54624 = lState_53650.AddTransition(lState_53660); }
      lTransition_54624.isExit = false;
      lTransition_54624.hasExitTime = true;
      lTransition_54624.hasFixedDuration = true;
      lTransition_54624.exitTime = 3.267798E-09f;
      lTransition_54624.duration = 0.174159f;
      lTransition_54624.offset = 0f;
      lTransition_54624.mute = false;
      lTransition_54624.solo = false;
      lTransition_54624.canTransitionToSelf = true;
      lTransition_54624.orderedInterruption = true;
      lTransition_54624.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54624.conditions.Length - 1; i >= 0; i--) { lTransition_54624.RemoveCondition(lTransition_54624.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54626 = MotionControllerMotion.EditorFindTransition(lState_53652, lState_53654, 0);
      if (lTransition_54626 == null) { lTransition_54626 = lState_53652.AddTransition(lState_53654); }
      lTransition_54626.isExit = false;
      lTransition_54626.hasExitTime = true;
      lTransition_54626.hasFixedDuration = true;
      lTransition_54626.exitTime = 0.6986235f;
      lTransition_54626.duration = 0.02106269f;
      lTransition_54626.offset = 0f;
      lTransition_54626.mute = false;
      lTransition_54626.solo = false;
      lTransition_54626.canTransitionToSelf = true;
      lTransition_54626.orderedInterruption = true;
      lTransition_54626.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54626.conditions.Length - 1; i >= 0; i--) { lTransition_54626.RemoveCondition(lTransition_54626.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54630 = MotionControllerMotion.EditorFindTransition(lState_53654, lState_53648, 0);
      if (lTransition_54630 == null) { lTransition_54630 = lState_53654.AddTransition(lState_53648); }
      lTransition_54630.isExit = false;
      lTransition_54630.hasExitTime = true;
      lTransition_54630.hasFixedDuration = true;
      lTransition_54630.exitTime = 0.5222301f;
      lTransition_54630.duration = 0.09250762f;
      lTransition_54630.offset = 0f;
      lTransition_54630.mute = false;
      lTransition_54630.solo = false;
      lTransition_54630.canTransitionToSelf = true;
      lTransition_54630.orderedInterruption = true;
      lTransition_54630.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54630.conditions.Length - 1; i >= 0; i--) { lTransition_54630.RemoveCondition(lTransition_54630.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54632 = MotionControllerMotion.EditorFindTransition(lState_53654, lState_53642, 0);
      if (lTransition_54632 == null) { lTransition_54632 = lState_53654.AddTransition(lState_53642); }
      lTransition_54632.isExit = false;
      lTransition_54632.hasExitTime = true;
      lTransition_54632.hasFixedDuration = true;
      lTransition_54632.exitTime = 0.6139733f;
      lTransition_54632.duration = 0.074159f;
      lTransition_54632.offset = 0f;
      lTransition_54632.mute = false;
      lTransition_54632.solo = false;
      lTransition_54632.canTransitionToSelf = true;
      lTransition_54632.orderedInterruption = true;
      lTransition_54632.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54632.conditions.Length - 1; i >= 0; i--) { lTransition_54632.RemoveCondition(lTransition_54632.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54636 = MotionControllerMotion.EditorFindTransition(lState_53656, lState_53658, 0);
      if (lTransition_54636 == null) { lTransition_54636 = lState_53656.AddTransition(lState_53658); }
      lTransition_54636.isExit = false;
      lTransition_54636.hasExitTime = true;
      lTransition_54636.hasFixedDuration = true;
      lTransition_54636.exitTime = 0.5577971f;
      lTransition_54636.duration = 0.03340977f;
      lTransition_54636.offset = 0.2145376f;
      lTransition_54636.mute = false;
      lTransition_54636.solo = false;
      lTransition_54636.canTransitionToSelf = true;
      lTransition_54636.orderedInterruption = true;
      lTransition_54636.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54636.conditions.Length - 1; i >= 0; i--) { lTransition_54636.RemoveCondition(lTransition_54636.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54640 = MotionControllerMotion.EditorFindTransition(lState_53658, lState_53650, 0);
      if (lTransition_54640 == null) { lTransition_54640 = lState_53658.AddTransition(lState_53650); }
      lTransition_54640.isExit = false;
      lTransition_54640.hasExitTime = true;
      lTransition_54640.hasFixedDuration = true;
      lTransition_54640.exitTime = 0.01976004f;
      lTransition_54640.duration = 0.1847095f;
      lTransition_54640.offset = 0.02140673f;
      lTransition_54640.mute = false;
      lTransition_54640.solo = false;
      lTransition_54640.canTransitionToSelf = true;
      lTransition_54640.orderedInterruption = true;
      lTransition_54640.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54640.conditions.Length - 1; i >= 0; i--) { lTransition_54640.RemoveCondition(lTransition_54640.conditions[i]); }

      UnityEditor.Animations.AnimatorStateTransition lTransition_54642 = MotionControllerMotion.EditorFindTransition(lState_53658, lState_53646, 0);
      if (lTransition_54642 == null) { lTransition_54642 = lState_53658.AddTransition(lState_53646); }
      lTransition_54642.isExit = false;
      lTransition_54642.hasExitTime = true;
      lTransition_54642.hasFixedDuration = true;
      lTransition_54642.exitTime = 0f;
      lTransition_54642.duration = 0.25f;
      lTransition_54642.offset = 0f;
      lTransition_54642.mute = false;
      lTransition_54642.solo = false;
      lTransition_54642.canTransitionToSelf = true;
      lTransition_54642.orderedInterruption = true;
      lTransition_54642.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
      for (int i = lTransition_54642.conditions.Length - 1; i >= 0; i--) { lTransition_54642.RemoveCondition(lTransition_54642.conditions[i]); }


      // Run any post processing after creating the state machine
      OnStateMachineCreated();
    }

#endif

    // ************************************ END AUTO GENERATED ************************************
    #endregion


  }
}