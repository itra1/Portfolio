using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using DG.Tweening;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.Boses.SarDayyni
{
  [ActionCategory("Enemyes")]
  [Tooltip("Первая поднимает левую руку вверх после чего генерирует 6 орбов которые  дновременно подают в 6 разных участков комнаты нанося урон по площади в 1 - 2 метра(сносят по пол хр)")]
  public class SarDayyniAttack1 : SarDayyniAttack
  {
	 public FsmGameObject _castPostition;
	 public FsmGameObject bullet;

	 public override void OnEnter()
	 {
		base.OnEnter();

		_animator.SetInteger("State", 100);

		DOVirtual.DelayedCall(1.6f, () =>
		{
		  CastBullet();
		});
		DOVirtual.DelayedCall(4.5f, () =>
		{
		  _animator.SetInteger("State", 0);
		});
		DOVirtual.DelayedCall(5.5f, () =>
		{
		  Fsm.Event(OnComplete);
		});
	 }

	 private void CastBullet()
	 {
		GameObject bulletInst = MonoBehaviour.Instantiate(bullet.Value, _castPostition.Value.transform.position, Quaternion.identity);
		bulletInst.gameObject.SetActive(true);
	 }

  }
}