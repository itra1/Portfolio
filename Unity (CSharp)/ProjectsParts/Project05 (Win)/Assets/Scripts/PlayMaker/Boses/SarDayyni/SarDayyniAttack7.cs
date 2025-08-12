using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using DG.Tweening;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.Boses.SarDayyni
{
  [ActionCategory("Enemyes")]
  [Tooltip("Вторая повторяет атаку охотника с взрывами на земле только теперь это золотые столбы света и они появляются сразу во множестве мест(как в ситуации с глазами в комнате жрицы)")]
  public class SarDayyniAttack7 : SarDayyniAttack
  {
	 public FsmGameObject bullet;
	 public FsmInt count = 8;

	 public FsmFloat maxHeight;
	 public FsmFloat minHeight;
	 public FsmFloat maxRadius;
	 public FsmFloat midRadius;
	 public FsmFloat minRadius;

	 public override void OnEnter()
	 {
		base.OnEnter();
		for (int i = 0; i < count.Value; i++)
		{
		  SpawnBullet();
		}

		DOVirtual.DelayedCall(3, () =>
		{
		  Fsm.Event(OnComplete);
		});
	 }

	 private void SpawnBullet()
	 {
		Vector3 target = _go.transform.position;

		bool isCurrectPoint = false;
		do
		{
		  target = GetPosition();

		  RaycastHit _hit;
		  if (RaycastExt.SafeRaycast(target, Vector3.down, out _hit, (maxHeight.Value - minHeight.Value)*3 ))
		  {
			 target = _hit.point;
			 isCurrectPoint = InHeight(target);
		  }

		} while (!isCurrectPoint);

		GameObject bulletInst = MonoBehaviour.Instantiate(bullet.Value, target, Quaternion.identity);
		bulletInst.SetActive(true);
	 }

	 private Vector3 GetPosition()
	 {
		Vector2 direct = Random.insideUnitCircle;
		float dist = Random.Range(minRadius.Value, maxRadius.Value);

		Vector3 target = _go.transform.position;
		target.y = maxHeight.Value + 1f;
		target += new Vector3(direct.x, 0, direct.y) * dist;
		return target;
	 }


	 public bool InHeight(Vector3 target)
	 {
		return ((target.y < maxHeight.Value + 0.1f || target.y > maxHeight.Value - 0.1f) || (target.y < maxHeight.Value + 0.1f || target.y > maxHeight.Value - 0.1f));
	 }

  }
}