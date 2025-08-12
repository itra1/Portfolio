using UnityEngine;
using HutongGames.PlayMaker;
using Slate;
using com.ootii.Cameras;

namespace it.Game.PlayMaker
{
  [ActionCategory("Cinematic")]
  public class PlayCutscene : FsmStateAction
  {
	 [RequiredField]
	 public FsmGameObject _cutScene;
	 public FsmGameObject _camera;
	 public FsmEvent _onStop;
	 public FsmBool _freeCamera = true;
	 protected CameraController _controller;

	 private CameraMotor _startMotor;
	 private CameraMotor _freeMotor;
	 private Vector3 _startPosition;
	 private Quaternion _startRotation;

	 private bool cursor;

	 public override void OnEnter()
	 {

		cursor = Cursor.visible;
		Cursor.visible = false;
		Cutscene cutscene = _cutScene.Value.GetComponent<Cutscene>();

		if (_freeCamera.Value)
		{
		  _controller = _camera.Value.GetComponent<CameraController>();
		  _startPosition = _camera.Value.transform.position;
		  _startRotation = _camera.Value.transform.rotation;

		  _startMotor = _controller.ActiveMotor;
		  _freeMotor = _controller.GetMotor<CameraMotor>("FreeMove");
		  _controller.ActivateMotor(_freeMotor);

		}

		cutscene.Play(() =>
		{
		  Cursor.visible = cursor;
		  cutscene.Stop();
		  if (_freeCamera.Value)
		  {
			 _camera.Value.transform.position = _startPosition;
			 _camera.Value.transform.rotation = _startRotation;
			 _controller.ActivateMotor(_startMotor);
		  }
		  Fsm.Event(_onStop);
		});
	 }
  }
}