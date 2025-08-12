using UnityEngine;
using System.Collections;
using it.Game.Managers;
using it.Game.Player;
using com.ootii.Cameras;

namespace it.Game.Environment.All
{
  [System.Serializable]
  public class NextFrameEvent : UnityEngine.Events.UnityEvent<int>
  {

  }
  /// <summary>
  /// Запуск диалога
  /// </summary>
  public class DialogPlay : Environment
  {
	 [SerializeField]
	 private string uuidDaolog;
	 [SerializeField]
	 private bool disableControl = true;
	 [SerializeField]
	 private bool disableMouse = true;


	 public UnityEngine.Events.UnityEvent onStart;
	 public NextFrameEvent onNextFrame;
	 public UnityEngine.Events.UnityEvent onComplete;

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
	 }

	 public void Interaction()
	 {
		if (State != 0)
		  return;

		State = 1;
		Save();

		var activeMotor = CameraBehaviour.Instance.CameraController.ActiveMotor;

		Managers.GameManager.Instance.DialogsManager.ShowDialog(uuidDaolog,
		  () => {

			 if (disableControl == true)
				GameManager.Instance.GameInputSource.IsEnabled = false;
			 if (disableMouse)
			 {
				var motor = CameraBehaviour.Instance.CameraController.GetMotor<BasicMotor>();
				CameraBehaviour.Instance.CameraController.ActivateMotor(motor);
			 }
			 
			 onStart?.Invoke(); },
		  (index) => { onNextFrame?.Invoke(index); },
		  () => {

			 if (disableControl == true)
				GameManager.Instance.GameInputSource.IsEnabled = true;
			 if (disableMouse)
			 {
				CameraBehaviour.Instance.CameraController.ActivateMotor(activeMotor);

			 }

			 onComplete?.Invoke(); });

	 }


  }
}