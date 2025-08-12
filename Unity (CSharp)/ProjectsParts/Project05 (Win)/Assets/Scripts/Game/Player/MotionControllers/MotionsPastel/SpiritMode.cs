using System.Collections.Generic;
using UnityEngine;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Utilities.Debug;
using com.ootii.Actors.AnimationControllers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Spirit mode")]
  [MotionDescription("Spirit mode")]
  public class SpiritMode : MotionControllerMotion
  {
	 public const int PHASE_START = 30100;
    public float _RotationSpeed = 360f;
    public SpiritMode() : base()
	 {
		_Form = -1;
		_Priority = 25;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Flying-SM"; }
#endif
	 }
	 public SpiritMode(MotionController rController) : base(rController)
	 {
		_Form = -1;
		_Priority = 25;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Flying-SM"; }
#endif

	 }
	 public override void Awake()
	 {
		base.Awake();
	 }
	 public override bool TestActivate()
	 {
		return Configs.IsSpirit;
	 }

	 public override bool TestUpdate()
	 {
		return Configs.IsSpirit;
	 }
	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		mActorController.IsGravityEnabled = false;
		mActorController.ForceGrounding = false;
		//mActorController.FixGroundPenetration = false;
		mActorController.State.Velocity = mActorController.AccumulatedVelocity;
		mActorController.IsCollsionEnabled = false;

		var cGame = CameraBehaviour.Instance.CameraController.GetMotor<com.ootii.Cameras.OrbitFollowMotor>();

		if(cGame != null)
		  cGame.MinPitch = -50;

		//for(int i = 0; i < mActorController.BodyShapes.Count; i++)
		//{
		//  for (int x = 0; x < mActorController.BodyShapes[i].Colliders.Length; x++)
		//	 mActorController.BodyShapes[i].Colliders[x].enabled = false;
		//}

		mMotionController.CameraRig.LockMode = false;
		mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START, true);

		return base.Activate(rPrevMotion);
	 }
	 public override void Deactivate()
	 {

		var cGame = CameraBehaviour.Instance.CameraController.GetMotor<com.ootii.Cameras.OrbitFollowMotor>();

		if (cGame != null)
		  cGame.MinPitch = 0;

		//for (int i = 0; i < mActorController.BodyShapes.Count; i++)
		//{
		//  for (int x = 0; x < mActorController.BodyShapes[i].Colliders.Length; x++)
		//	 mActorController.BodyShapes[i].Colliders[x].enabled = true;
		//}

		mActorController.IsGravityEnabled = true;
		mActorController.IsCollsionEnabled = true;
		mActorController.ForceGrounding = true;
		base.Deactivate();
	 }

	 public override bool TestInterruption(MotionControllerMotion rMotion)
	 {
		return false;
	 }
	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		mMovement = Vector3.zero;
		mRotation = Quaternion.identity;
		GetRotationVelocityWithView(rDeltaTime, ref mAngularVelocity);
		SmoothInput();

    }
	 protected void SmoothInput()
	 {
		MotionState lState = mMotionController.State;

		// Convert the input to radial so we deal with keyboard and gamepad input the same.
		float lInputMax = 1;

		mActorController.Move(mMotionController.CameraTransform.forward * lState.InputY *Time.deltaTime*5);
		mActorController.Move(mMotionController.CameraTransform.right * lState.InputX * Time.deltaTime * 5);



		//float lInputX = Mathf.Clamp(lState.InputX, -lInputMax, lInputMax);
		//float lInputY = Mathf.Clamp(lState.InputY, -lInputMax, lInputMax);
		//float lInputMagnitude = Mathf.Clamp(lState.InputMagnitudeTrend.Value, 0f, lInputMax);
		//InputManagerHelper.ConvertToRadialInput(ref lInputX, ref lInputY, ref lInputMagnitude);

		//// Smooth the input
		//mInputX.Add(lInputX);
		//mInputY.Add(lInputY);
		//mInputMagnitude.Add(lInputMagnitude);

		//// Modify the input values to add some lag
		//mMotionController.State.InputX = mInputX.Average;
		//mMotionController.State.InputY = mInputY.Average;
		//mMotionController.State.InputMagnitudeTrend.Replace(mInputMagnitude.Average);
	 }
	 private void GetRotationVelocityWithView(float rDeltaTime, ref Vector3 rRotationalVelocity)
    {
      if (mMotionController._CameraTransform == null) { return; }

      float lRotationVelocity = 0f;
      float lSmoothedDeltaTime = Time.deltaTime;

      // Determine the global direction the character should face
      float lAngle = NumberHelper.GetHorizontalAngle(mActorController._Transform.forward, mMotionController._CameraTransform.forward, mActorController._Transform.up);

      // We want to work our way to the goal smoothly
      if (lAngle > 0f)
      {
        // Rotate instantly
        if (_RotationSpeed == 0f)
        {
          lRotationVelocity = lAngle / lSmoothedDeltaTime;
        }
        else
        {
          // Use the MC's rotation velocity
          if (_RotationSpeed < 0f)
          {
            lRotationVelocity = mMotionController._RotationSpeed;
          }
          // Rotate over time
          else
          {
            lRotationVelocity = _RotationSpeed;
          }

          // If we're rotating too much, limit
          if (lRotationVelocity * lSmoothedDeltaTime > lAngle)
          {
            lRotationVelocity = lAngle / lSmoothedDeltaTime;
          }
        }
      }
      else if (lAngle < 0f)
      {
        // Rotate instantly
        if (_RotationSpeed == 0f)
        {
          lRotationVelocity = lAngle / lSmoothedDeltaTime;
        }
        // Rotate over time
        else
        {
          // Use the MC's rotation velocity
          if (_RotationSpeed < 0f)
          {
            lRotationVelocity = -mMotionController._RotationSpeed;
          }
          // Rotate over time
          else
          {
            lRotationVelocity = -_RotationSpeed;
          }

          // If we're rotating too much, limit
          if (lRotationVelocity * lSmoothedDeltaTime < lAngle)
          {
            lRotationVelocity = lAngle / lSmoothedDeltaTime;
          }
        }
      }

      rRotationalVelocity = Vector3.up * lRotationVelocity;
    }
  }
}