using System;
using UnityEngine;
using com.ootii.Actors.Navigation;
using com.ootii.Messages;
using com.ootii.Actors.AnimationControllers;
using com.ootii.Helpers;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Double Jump")]
  [MotionDescription("Повторный прыжок в воздуха")]
  public class DoubleJump : JumpPastel
  {
	 public const int PHASE_START_DOUBLE = 272560;
	 public const int PHASE_COMPLETE_DOUBLE = 272561;

	 private bool _isGroundContact = false;

	 public DoubleJump()
	 : base()
	 {
		_Priority = 16;
		_ActionAlias = "Jump";
		mIsStartable = true;
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public DoubleJump(MotionController rController)
		  : base(rController)
	 {
		_Priority = 16;
		_ActionAlias = "Jump";
		mIsStartable = true;
	 }

	 public override bool TestActivate()
	 {
		if (!mIsStartable)
		{
		  return false;
		}

		if (mActorController.IsGrounded)
		{
		  _isGroundContact = true;
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
	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		Debug.Log("Double jump activation");
		mIsActive = true;
		mIsAnimatorActive = false;
		mIsInSubStateMachine = false;
		mIsActivatedFrame = true;

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

		Vector3 vel = mActorController.AccumulatedVelocity;
		vel.y = 0;
		mActorController.AccumulatedVelocity = vel;
		
		mLaunchVelocity = mActorController.State.Velocity;
		mLaunchVelocity.y = 0;
		//mActorController.AccumulatedVelocity = Vector3.zero;
		//mVelocity = mLaunchVelocity;
		mVelocity = Vector3.zero;
		Debug.Log(mActorController.State.Velocity);
		mActorController.AccumulatedVelocity = Vector3.zero;
		mActorController.AddImpulse(mActorController._Transform.up * _Impulse);
		//mActorController.SetVelocity(Vector3.up * _Impulse);

		// Grab the current velocities
		//mLaunchForward = mActorController._Transform.forward;
		//mLaunchVelocity = mLaunchForward * ForwardSpeed * mMotionController.State.InputForward.magnitude;

		// Control whether we're walking or running
		mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START_DOUBLE, true);

		return true;
	 }

	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		base.Update(rDeltaTime, rUpdateIndex);

	 }

	 public override void Deactivate()
	 {
		base.Deactivate();


	 }

  }
}