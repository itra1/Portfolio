using UnityEngine;
using com.ootii.Actors.AnimationControllers;

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Platform Move")]
  [MotionDescription("Platform Move")]
  public class PlatformMove : MotionControllerMotion
  {

	 private Game.Player.PlayerBehaviour _playerBehaviour;
	 private Transform _platform;
	 private Vector3 _platformPosition;

	 public PlatformMove() : base()
	 {
		_Form = -1;
		_Priority = 16;

	 }

	 public PlatformMove(MotionController rController) : base(rController)
	 {
		_Form = -1;
		_Priority = 999;

	 }
	 public override void Initialize()
	 {
		_playerBehaviour = mMotionController.GetComponent<PlayerBehaviour>();
	 }

	 public override bool TestActivate()
	 {
		if (!IsEnabled)
		  return false;

		if (!mActorController.IsGrounded)
		  return false;

		if ( _playerBehaviour.Platform != null)
		  return true;

		return false;
	 }

	 public override bool TestUpdate()
	 {
		if (_playerBehaviour.Platform == null)
		  return false;

		return true;
	 }

	 public override void Deactivate()
	 {
		mMovement = Vector3.zero;
		//mActorController.IsGravityEnabled = true;
	 }

	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		_platform = _playerBehaviour.Platform;
		_platformPosition = _platform.position;
		//mActorController.IsGravityEnabled = false;


		return base.Activate(rPrevMotion);
	 }

	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		Debug.Log("UpdatePlatform");
		mMovement = _platform.position - _platformPosition;
		_platformPosition = _platform.position;

	 }

  }
}