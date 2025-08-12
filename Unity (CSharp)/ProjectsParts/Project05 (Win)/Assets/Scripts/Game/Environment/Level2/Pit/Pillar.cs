using UnityEngine;
using System.Collections;
using it.Game.Managers;
using DG.Tweening;

namespace it.Game.Environment.Level2.Pit
{
  public class Pillar : MonoBehaviourBase
  {
	 [SerializeField]
	 private Transform _playerTriggerEnter;
	 [SerializeField]
	 private ParticleSystem _particles;

	 private void Start()
	 {
		ActivateLighting(false);
	 }

	 public void ActivateLighting(bool isActivate)
	 {
		if (isActivate)
		{
		  DOVirtual.DelayedCall(0.3f, () => { ActivateLighting(false); }, false);
		  _particles.Play();
		}
		_playerTriggerEnter.gameObject.SetActive(isActivate);
	 }

	 public void PlayerEnter()
	 {
		GameManager.Instance.UserManager.Health.Damage(this);
	 }
  }
}