using HutongGames.PlayMaker;
using com.ootii.Cameras;

namespace it.Game.PlayMaker.Camera
{
  [ActionCategory("Camera controller")]
  [HutongGames.PlayMaker.Tooltip("Базовый класс для камер")]
  public abstract class CameraControllerBase : FsmStateAction
  {

	 public FsmGameObject _camera;

	 public FsmBool _translation = false;
	 public FsmFloat _translationTime = 0.5f;
	 public FsmString _transitionMotorName = "CustomTransition";
	 public FsmEvent _onComplete;

	 protected CameraController _controller;
	 public override void OnEnter()
	 {
		base.OnEnter();
		_controller = _camera.Value.GetComponent<CameraController>();

	 }

	 protected void Confirm(CameraMotor targetMotor)
	 {

		if (!_translation.Value)
		  ActiveMotor(targetMotor);
		else
		  Transition(targetMotor);
		EmitEvent();
	 }

	 public void ActiveMotor(CameraMotor targetMotor)
	 {
		_controller.ActivateMotor(targetMotor);
	 }

	 protected virtual void Transition(CameraMotor targetMotor)
	 {

		int activeMotor = _controller.ActiveMotorIndex;
		int indexTarget = _controller.GetMotorIndex(targetMotor.Name);
		TransitionMotor motor = _controller.GetMotor<TransitionMotor>(_transitionMotorName.Value);
		motor.StartMotorIndex = activeMotor;
		motor.EndMotorIndex = indexTarget;
		motor.TransitionTime = _translationTime.Value;
		ActiveMotor(motor);
	 }

	 protected void EmitEvent()
	 {
		Fsm.Event(_onComplete);
	 }


  }

}
