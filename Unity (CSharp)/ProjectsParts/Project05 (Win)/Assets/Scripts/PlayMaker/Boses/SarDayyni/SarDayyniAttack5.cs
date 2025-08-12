using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using DG.Tweening;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Boses.SarDayyni
{
  [ActionCategory("Enemyes")]
  [Tooltip("Выпускает атаку из спиралей по позиции игрока")]
  public class SarDayyniAttack5 : SarDayyniAttack
  {
	 public FsmGameObject bullet;
	 public FsmGameObject spawnPosition;

	 public override void OnEnter()
	 {
		GameObject inst = MonoBehaviour.Instantiate(bullet.Value, spawnPosition.Value.transform.position, Quaternion.identity);

		MonoBehaviour.Destroy(inst, 1);

		Fsm.Event(OnComplete);
	 }
  }
}