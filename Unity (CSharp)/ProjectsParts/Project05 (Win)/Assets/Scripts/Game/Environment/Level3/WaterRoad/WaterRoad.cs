using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment
{
  /// <summary>
  /// Дорога над водой
  /// 
  /// Ведет к небольшому острову с платформой
  /// </summary>
  public class WaterRoad : Environment
  {
	 /*
     * Состояния
     * 0 - под водой
     * 2 - над водой
     * 
     */

	 [SerializeField]
	 private Transform[] _platforms;

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
		  for (int i = 0; i < _platforms.Length; i++)
		  {
			 _platforms[i].localPosition = (State == 2 ? Vector3.zero : _platforms[i].up * -4);
		  }
		}

	 }

	 public void Activate()
	 {
		if (State == 2)
		  return;

		State = 2;
		Save();

		StartCoroutine(PlayUp());
	 }

	 IEnumerator PlayUp()
	 {
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < _platforms.Length; i++)
		{
		  _platforms[i].DOLocalMove(Vector3.zero, 0.3f);
		  yield return new WaitForSeconds(0.2f);
		}
	 }
  }
}