using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.All.RoadBlock
{
  /// <summary>
  /// Блок дороги
  /// </summary>
  public class RoadBlock : Environment
  {
	 /*
	  * Статусы:
	  * 0 - стандартно
	  * 1 - открыто
	  */

	 [SerializeField]
	 private Transform _block;

	 [SerializeField]
	 private float _heightOffset = -20;

	 [SerializeField]
	 private ParticleSystem _particleSystem;

	 [SerializeField]
	 private float _delayDown = 0;

	 /// <summary>
	 /// Открытие дороги
	 /// </summary>
	 [ContextMenu("Open gate")]
	 public void OpenRoad()
	 {
		State = 1;
		ConfirmState(false);
		Save();
	 }

	 protected override void ConfirmState(bool force = false)
	 {
		base.ConfirmState(force);
		if (State == 1)
		{
		  if (force)
			 _block.transform.localPosition = new Vector3(_block.transform.localPosition.x, _heightOffset, _block.transform.localPosition.z);
		  else
		  {
			 DOTween.To(() => _block.transform.localPosition,
				(x) => _block.transform.localPosition = x,
				new Vector3(_block.transform.localPosition.x, _heightOffset, _block.transform.localPosition.z), 1f).SetDelay(_delayDown);

			 if (_particleSystem != null)
				_particleSystem.Play();

		  }
		  GetComponent<BoxCollider>().enabled = false;
		}
		else
		{
		  _block.transform.localPosition = Vector3.zero;
		  GetComponent<BoxCollider>().enabled = true;
		}
		  //GetComponent<Animator>().SetTrigger(force ? "ForceOpen" : "Open");
	 }

  }
}