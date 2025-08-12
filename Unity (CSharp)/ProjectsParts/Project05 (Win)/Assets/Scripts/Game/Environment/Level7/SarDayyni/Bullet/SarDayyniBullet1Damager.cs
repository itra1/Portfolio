using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace it.Game.Environment.Level7.SarDayyni
{
  public class SarDayyniBullet1Damager : MonoBehaviourBase
  {
	 [SerializeField]
	 private Renderer _renderer;

	 [SerializeField]
	 private float _damag = 34;

	 private void OnEnable()
	 {
		_renderer.material = Instantiate(_renderer.material);
		_renderer.transform.localScale = Vector3.zero;
		_renderer.transform.DOScale(2, 0.3f);
		_renderer.material.DOFloat(1, "_CutOut", 0.3f);
		DOVirtual.DelayedCall(0.4f, () => {
		  Destroy(gameObject);
		});
	 }

	 public void PlayerDamage()
	 {
		Game.Managers.GameManager.Instance.UserManager.Health.Damage(this, _damag, true);
	 }

  }
}