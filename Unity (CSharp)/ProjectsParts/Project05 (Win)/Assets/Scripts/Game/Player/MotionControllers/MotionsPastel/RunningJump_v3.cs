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
  [MotionName("Running Jump v3")]
  public class RunningJump_v3 : JumpPastel, IRunningJump
  {
	 public const int PHASE_START_LEFT = 272551;
	 public const int PHASE_START_RIGHT = 272552;

	 private bool _isLeftFoot;
	 public RunningJump_v3()
	 : base()
	 {
		_Priority = 20;
		_IsControlEnabled = false;
		_ConvertToHipBase = false;

		_Impulse = 0f;
		mIsStartable = true;
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public RunningJump_v3(MotionController rController)
		  : base(rController)
	 {
		_Priority = 20;
		_IsControlEnabled = false;
		_ConvertToHipBase = false;

		_Impulse = 0f;
		mIsStartable = true;
	 }

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

		return TestActivateJump();
	 }

	 private bool TestActivateJump()
	 {
		if (mMotionController.State.InputForward.magnitude < 0.2f /*|| Mathf.Abs(mMotionController.State.InputFromAvatarAngle) > 10f*/)
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

	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		ActiveJump((mMotionController.Animator.GetFloat(Game.Player.PlayerBehaviour.ANIM_CRV_LEFT_FOOT) > 0.55f));
		// Report that we're good to enter the jump
		return true;
	 }

	 private void ActiveJump(bool left)
	 {
		_isLeftFoot = left;
		// Flag the motion as active
		mIsActive = true;
		mIsAnimatorActive = false;
		mIsInSubStateMachine = false;
		mIsActivatedFrame = true;
		mIsStartable = false;
		_isStartFall = false;
		_toIdle = false;
		_toRun = false;
		_noExistJump = true;

		// Store the current active Walk Motion (if it is)
		//if (rPrevMotion is IWalkRunMotion)
		//{
		//  mWalkRunMotion = rPrevMotion as IWalkRunMotion;
		//}

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

		Vector3 forvardImpuls = mLaunchVelocity;
		forvardImpuls.y = 0;

		// Initialize the jump
		//mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START, true);
		PlayAnim();
		mActorController.AddImpulse(mActorController._Transform.up * _Impulse);

		if(forvardImpuls.magnitude <= 0.2f)
		  mActorController.AddImpulse(mActorController._Transform.forward * _Impulse/3);

		//mActorController.AddImpulse(mActorController._Transform.forward * (forvardImpuls.magnitude * mActorController.Mass));

	 }

	 private void PlayAnim()
	 {

		if (_isLeftFoot)
		{
		  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START_LEFT, true);
		}
		else
		{
		  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START_RIGHT, true);
		}
	 }

	 protected override bool TestReactivate()
	 {
		if (TestActivateJump())
		{
		  ActiveJump(!_isLeftFoot);
		  return true;
		}
		return false;
	 }

	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		int lStateID = mMotionLayer._AnimatorStateID;
		if (mMotionController._InputSource != null  && mMotionController._InputSource.IsJustPressed(_ActionAlias))
		{
		  _isJumpTime = Time.time;
		}

		if(_noExistJump && !(lStateID == STATE_JumpRunLeftStart || lStateID == STATE_JumpRunRightStart))
		{
		  PlayAnim();
		}

		base.Update(rDeltaTime, rUpdateIndex);
	 }


  }
}