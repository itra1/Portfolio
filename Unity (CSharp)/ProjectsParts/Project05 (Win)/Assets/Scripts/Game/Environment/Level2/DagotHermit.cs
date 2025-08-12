using UnityEngine;
using System.Collections;
using it.Game.NPC.Enemyes;

namespace it.Game.Environment.Level2
{
  /// <summary>
  /// Отшельник
  /// </summary>
  public class DagotHermit : Environment
  {

	 /*
	  * Состояния
	  * 0 - старт
	  * 1 - увидел игрока
	  * 2 - диалог с игроком
	  * 3 - диалог закончен
	  */
	 [SerializeField]
	 private it.Game.NPC.Enemyes.Enemy _dagot;

	 public void PlayerVisible()
	 {
		if (State >= 1)
		  return;

		State = 1;
		_dagot.GetComponent<PlayMakerFSM>().SendEvent("OnVisible");
	 }

	 public void PlayerOutVisible()
	 {
		if (State != 1)
		  return;
		_dagot.GetComponent<PlayMakerFSM>().SendEvent("OnStart");
		State = 0;
	 }

	 public void PlayerContact()
	 {
		if (State != 1)
		  return;
		_dagot.GetComponent<PlayMakerFSM>().SendEvent("OnDialog");
		State = 2;
		Save();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		switch (State)
		{
		  case 0:
			 break;
		  case 2:
			 _dagot.GetComponent<PlayMakerFSM>().SendEvent("OnComplete");
			 break;
		}

	 }

  }
}