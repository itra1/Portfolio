using UnityEngine;
using com.ootii.Actors;
using com.ootii.Actors.Navigation;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Messages;
using com.ootii.Actors.AnimationControllers;
using RaycastExt = Utilites.Geometry.RaycastExt;

#if UNITY_EDITOR
using UnityEditor;
using com.ootii.Graphics;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Flying Path Pastel")]
  [MotionDescription("WindEffect")]
  public class FlyingPath : MotionControllerMotion
  {
	 // Enum values for the motion
	 public const int PHASE_UNKNOWN = 0;

	 /// <summary>
	 /// Основа для блока
	 /// </summary>
	 public const int PHASE_BLOCK = 303;
	 /// <summary>
	 /// Старт
	 /// </summary>
	 public const int PHASE_IDLE = 30300;
	 private bool isFlying = false;

	 private float _radiusMove;

	 private FluffyUnderware.Curvy.Controllers.SplineController _splineController;

	 private Vector3 _offset;

	 private float _speedOffset = 1;

	 private float _minYaw = 0;
	 private float _maxYaw = 0;

	 public FlyingPath()
	  : base()
	 {
		_Category = EnumMotionCategories.IDLE;

		_Priority = 100;
		_OverrideLayers = true;
		mIsStartable = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "FlyingPath"; }
#endif
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public FlyingPath(MotionController rController)
		  : base(rController)
	 {
		_Category = EnumMotionCategories.IDLE;

		_Priority = 100;
		_OverrideLayers = true;
		mIsStartable = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "FlyingPath"; }
#endif
	 }

	 public void Start(FluffyUnderware.Curvy.Controllers.SplineController splineController, float radiusMove, float spedOffset)
	 {
		if (isFlying)
		  return;

		_speedOffset = spedOffset;

		_splineController = splineController;
		isFlying = true;
		_radiusMove = radiusMove;
	 }
	 public void Stop()
	 {
		isFlying = false;
	 }

	 public override bool TestActivate()
	 {
		return isFlying;
	 }

	 public override bool TestUpdate()
	 {
		return isFlying;
	 }
	 public override bool TestInterruption(MotionControllerMotion rMotion)
	 {
		return !isFlying;
	 }

	 public override void Deactivate()
	 {

		mActorController.IsGravityEnabled = true;
		mActorController.ForceGrounding = true;
		mActorController.IsCollsionEnabled = true;
		mActorController.FixGroundPenetration = true;
		mActorController.SetGround(null);

		var cameraMotor = CameraBehaviour.Instance.CameraController.GetMotor<Game.Gamera.Motors.OrbitFollowMotor>();

		cameraMotor.MinYaw = cameraMotor.MinYaw;
		cameraMotor.MaxYaw = cameraMotor.MaxYaw;

		// Finish the deactivation process
		base.Deactivate();
	 }

	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		// Disable actor controller processing for a short time
		mActorController.IsGravityEnabled = false;
		mActorController.ForceGrounding = false;
		mActorController.FixGroundPenetration = false;
		mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_IDLE, true);

		var cameraMotor = CameraBehaviour.Instance.CameraController.GetMotor<Game.Gamera.Motors.OrbitFollowMotor>();
		_minYaw = cameraMotor.MinYaw;
		_maxYaw = cameraMotor.MaxYaw;

		cameraMotor.MinYaw = -60;
		cameraMotor.MaxYaw = 60;
		//if (_playerBehaviour == null)
		//  _playerBehaviour = mMotionController.GetComponent<Game.Player.PlayerBehaviour>();

		return base.Activate(rPrevMotion);
	 }
	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		_offset += new Vector3(mMotionController.State.InputX, mMotionController.State.InputY, 0) * _speedOffset * rDeltaTime;

		if (_offset.magnitude > _radiusMove)
		{
		  _offset = _offset.normalized * _radiusMove;
		}


		float lToCameraAngle = Vector3Ext.HorizontalAngleTo(mMotionController._Transform.forward, _splineController.transform.forward, mMotionController._Transform.up);

		Quaternion lRotation = Quaternion.AngleAxis(lToCameraAngle, Vector3.up);

		mActorController.Yaw = mActorController.Yaw * lRotation;

		//mActorController.Tilt = Quaternion.identity * Quaternion.Euler(90,0,0);

		mActorController._Transform.rotation = /*mActorController.Tilt * */mActorController.Yaw;
		mActorController._Transform.position = _splineController.transform.position + (mActorController._Transform.rotation * _offset);

	 }
  }
}