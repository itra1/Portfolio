using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using DG.Tweening;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Boses.SarDayyni
{
  [ActionCategory("Enemyes")]
  [Tooltip("Вторая атака поднимает две руки вверх- столб золотого огня выжигает весь центральный круг помещения до потолка убивая персонажа сразу.")]
  public class SarDayyniAttack2 : SarDayyniAttack
  {
	 public FsmGameObject _castPostition;
	 public FsmGameObject bullet;

	 public override void OnEnter()
	 {
		base.OnEnter();
		_animator.SetInteger("State", 300);

		DOVirtual.DelayedCall(1, () =>
		{
		  bullet.Value.SetActive(true);
		});
		DOVirtual.DelayedCall(11, () =>
		{
		  _animator.SetInteger("State", 0);
		  bullet.Value.SetActive(false);
		});
		DOVirtual.DelayedCall(12, () =>
		{
		  bullet.Value.SetActive(false);
		  Fsm.Event(OnComplete);
		});

	 }

  }
}