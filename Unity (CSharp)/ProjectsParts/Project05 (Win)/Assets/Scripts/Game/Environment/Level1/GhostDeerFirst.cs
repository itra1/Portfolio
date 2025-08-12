using UnityEngine;
using System.Collections;
using it.Game.Player;
using it.Game.Environment.Handlers;
using DG.Tweening;

namespace it.Game.Environment.Level1
{

  /// <summary>
  /// Первый показ оленя
  /// </summary>
  public class GhostDeerFirst : PlayMakerRun
  {
	 /*
	  * Состояния
	  * 
	  * 0 - ждем первого появления игрока
	  * 
	  */
	 [SerializeField]
	 private PegasusController _pegasus;

	 [SerializeField]
	 private string dialogUuid;

	 public void NoteReadComplete()
	 {
		_pegasus.Activate(() =>
		{
		  _fsm.SendEvent("NoteReadComplete");
		  DOVirtual.DelayedCall(5, () =>
		  {
			 _pegasus.Deactivate();
			 Managers.GameManager.Instance.DialogsManager.ShowDialog(dialogUuid, null, null, null);
		  });
		});
		State = 2;
		Save();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (State == 1)
		{
		  _fsm.SendEvent("Step2");
		}

	 }
  }
}