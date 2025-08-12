using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Camera
{
  [ActionCategory("Camera controller")]
  [HutongGames.PlayMaker.Tooltip("Камера вокруг объекта")]
  public class CameraToRoundObject : CameraControllerBase
  {
	 private it.Game.Gamera.Motors.OrbitFollowMotor _motion;
	 public override void OnEnter()
	 {
		base.OnEnter();
		_motion = _controller.GetMotor<it.Game.Gamera.Motors.OrbitFollowMotor>("Game");
		Confirm(_motion);

	 }
  }
}