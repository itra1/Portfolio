using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.Level2.Pit
{
  public class Pit : Environment
  {
	 [SerializeField]
	 private Pillar[] _pillar;
	 [SerializeField]
	 private float _period = 5;
	 [SerializeField]
	 private float _betweenPillar = 2;


	 private bool _isPlayer;
	 private bool _IsActive;

	 public void PlayerVisible() {
		_isPlayer = true;

		if (_IsActive)
		  return;

		StartCoroutine(ActiveCroutine());
	 }

	 public void PlayerUnvisible()
	 {
		_isPlayer = false;
	 }

	 IEnumerator ActiveCroutine()
	 {
		if (!_isPlayer)
		  yield break;

		_IsActive = true;

		for (int i = 0; i < _pillar.Length; i++)
		{
		  _pillar[i].ActivateLighting(true);
		  yield return new WaitForSeconds(_betweenPillar);
		}

		yield return new WaitForSeconds(_period);

		_IsActive = false;

		DOVirtual.DelayedCall(0.1f, () => {
		  StartCoroutine(ActiveCroutine());		
		}, false);
	 }
  }
}