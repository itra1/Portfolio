using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using DG.Tweening;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Boses.SarDayyni
{
  [ActionCategory("Enemyes")]
  [Tooltip("Третья атака вытягивает обе руки вперед после чего прицельно стреляет в персонажа спиралевидным сгустком - линией несколько раз(от 2 до 4) с интервалом 3 секунды. Каждый луч наносит ¼ урона")]
  public class SarDayyniAttack3 : SarDayyniAttack
  {
	 public FsmGameObject bullet;
	 public FsmGameObject spawnPosition;

	 public override void OnEnter()
	 {
		base.OnEnter();
		StartCoroutine(SpawnItems());

		_animator.SetInteger("State", 400);
	 }

	 private IEnumerator SpawnItems()
	 {
		int count = Random.Range(2, 5);

		yield return new WaitForSeconds(1.5f);

		for (int i = 0; i < count; i++)
		{
		  SpawnBullet();
		  if(i != count-1)
			 yield return new WaitForSeconds(2);
		}
		yield return new WaitForSeconds(1);
		Complete();
	 }

	 private void Complete()
	 {
		_animator.SetInteger("State", 0);
		DOVirtual.DelayedCall(1f, () =>
		{
		  Fsm.Event(OnComplete);
		});
	 }

	 private void SpawnBullet()
	 {
		GameObject inst = MonoBehaviour.Instantiate(bullet.Value, spawnPosition.Value.transform.position, Quaternion.identity);
		inst.gameObject.SetActive(true);
	 }

  }
}