using UnityEngine;
using System.Collections;

namespace it.Game.Environment.Level2
{
  public class DagotBarrier : Environment
  {

	 [SerializeField]
	 private it.Game.NPC.Enemyes.Enemy _dagot;

	 /// <summary>
	 /// Контакт с игроком
	 /// </summary>
	 public void PlayerContact()
	 {
		_dagot.GetComponent<PlayMakerFSM>().SendEvent("OnContact");
	 }

  }
}