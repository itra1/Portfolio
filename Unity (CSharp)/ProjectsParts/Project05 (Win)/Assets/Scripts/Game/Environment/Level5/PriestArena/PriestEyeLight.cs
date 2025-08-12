using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.Level5.PriestArena
{
  public class PriestEyeLight : MonoBehaviourBase
  {
	 [SerializeField]
	 private ProgressControlV3D _laser;
	 [SerializeField]
	 private Transform _centerArena;

	 private void Awake()
	 {
		gameObject.SetActive(false);
	 }

	 private void OnEnable()
	 {
		LookRotation();
		DOVirtual.DelayedCall(0.2f, () =>
		{
		  Shoot();
		});
	 }

	 public void Shoot()
	 {
		_laser.always = true;
		Vector3 localRotate = transform.localEulerAngles;
		localRotate.x = -24;
		transform.DOLocalRotate(localRotate, 3).SetDelay(2).OnComplete(() =>
		{
		  DOVirtual.DelayedCall(1, () =>
		  {
			 _laser.always = false;
			 DOVirtual.DelayedCall(3f, () =>
			 {
				gameObject.SetActive(false);
			 });
		  });
		});
	 }

	 public void Stop()
	 {
		_laser.always = false;
	 }

	 [ContextMenu("LookRotation")]
	 private void LookRotation()
	 {
		Vector3 center = _centerArena.position;
		center.y = transform.position.y;
		transform.LookAt(center);
	 }

  }
}