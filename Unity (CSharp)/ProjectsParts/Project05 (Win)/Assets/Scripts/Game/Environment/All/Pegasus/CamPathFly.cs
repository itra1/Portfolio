using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.All
{
  public class CamPathFly : Environment
  {

	 [SerializeField]
	 private Pegasus.PegasusManager _pegasusManager;


	 public void Activate()
	 {
		CameraBehaviour.Instance.CameraController.Anchor = null;
		CameraBehaviour.Instance.CameraController.ActivateMotor(5);
		_pegasusManager.m_target = CameraBehaviour.Instance.gameObject;
		_pegasusManager.StartFlythrough(false);
		Game.Managers.GameManager.Instance.GameInputSource.enabled = false;

		StartCoroutine(CoroutinaWait());
	 }

	 private IEnumerator CoroutinaWait()
	 {

		while (_pegasusManager.m_currentState != Pegasus.PegasusConstants.FlythroughState.Stopped)
		  yield return null;

		Deactivare();
	 }

	 private void Deactivare()
	 {
		CameraBehaviour.Instance.CameraController.Anchor = Game.Player.PlayerBehaviour.Instance.transform;
		CameraBehaviour.Instance.CameraController.ActivateMotor(0);
		Game.Managers.GameManager.Instance.GameInputSource.enabled = true;
	 }

  }
}