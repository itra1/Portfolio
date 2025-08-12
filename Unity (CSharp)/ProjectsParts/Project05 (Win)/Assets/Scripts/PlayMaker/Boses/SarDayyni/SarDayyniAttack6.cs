using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using DG.Tweening;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Boses.SarDayyni
{
  [ActionCategory("Enemyes")]
  [Tooltip("Первая все те же спирали но теперь в 4 стороны от босса")]
  public class SarDayyniAttack6 : SarDayyniAttack
  {
	 public FsmGameObject bullet;
	 [UIHint(UIHint.FsmGameObject)]
	 [ArrayEditor(VariableType.GameObject)]
	 public FsmArray _spawnPositions;

	 public override void OnEnter()
	 {
		for (int i = 0; i < _spawnPositions.Values.Length; i++)
		  SpawnBullet((_spawnPositions.Values[i] as GameObject).transform);

		Fsm.Event(OnComplete);
	 }

	 private void SpawnBullet(Transform source)
	 {

		GameObject inst = MonoBehaviour.Instantiate(bullet.Value, source.position, Quaternion.identity);
		inst.transform.rotation = source.rotation;
		inst.SetActive(true);
	 }

  }
}