using UnityEngine;
using System.Collections;
using com.ootii.Geometry;
using it.Game.Player;
using DG.Tweening;

namespace it.Game.NPC.Enemyes.Boses.Karmamancer
{
  /// <summary>
  /// Большой каст
  /// </summary>
  public class BigCastBullet : MonoBehaviourBase
  {
	 [SerializeField]
	 private Transform _sphere;
	 [SerializeField]
	 private ParticleSystem _particleSystem;
	 private ParticleSystem.EmissionModule _emission;
	 private ParticleSystem.ShapeModule _shape;
	 private ParticleSystem.MinMaxCurve _emissionCurve;

	 private int _repearRayCount = 5;

	 private void Init()
	 {

		_sphere.localScale = Vector3.zero;
		_sphere.gameObject.SetActive(false);
		_emission = _particleSystem.emission;
		_shape = _particleSystem.shape;
		_emissionCurve = _emission.rateOverTime;
		_emission.enabled = false;

		_emissionCurve.constant = 0;
		_shape.radius = 0.0001f;
	 }

	 public void Cast()
	 {
		Init();
		_emission.enabled = true;
		_emissionCurve.constant = 200;
		_shape.radius = 0.0001f;
		_sphere.gameObject.SetActive(true);

		DOTween.To(() => _shape.radius, (x) =>  _shape.radius = x , 50, 4);
		DOTween.To(() => _emissionCurve.constant, (x) => _emissionCurve.constant = x, 1000, 4);

		_sphere.DOScale(new Vector3(100, 100, 100), 4).OnComplete(()=> {
		  _emission.enabled = false;
		  _emissionCurve.constant = 0;
		  _shape.radius = 0.0001f;
		  _sphere.gameObject.SetActive(false);
		});
	 }

	 public void OnPlayerEnter()
	 {
		RayCast();
	 }

	 private void RayCast()
	 {
		float _heightIncrement = 1.7f / _repearRayCount;

		Vector3 StartRay = transform.position + Vector3.up;
		bool isTargetHit = false;

		for (int i = 0; i < _repearRayCount; i++)
		{
		  if (isTargetHit)
			 continue;
		  RaycastHit _hit;
		  Vector3 targetRay = PlayerBehaviour.Instance.transform.position + Vector3.up * _heightIncrement * i;
		  if (RaycastExt.SafeRaycast(StartRay, targetRay - StartRay, out _hit,
			 (targetRay - StartRay).magnitude, -1, transform))
		  {
			 if (CheckCollision(PlayerBehaviour.Instance.transform, _hit.transform))
			 {
				isTargetHit = true;
			 }
		  }
		}

		if (isTargetHit)
		  Game.Managers.GameManager.Instance.UserManager.Health.Damage(this,1000, true);
	 }
	 private bool CheckCollision(Transform checker, Transform collision)
	 {
		bool isChecker = collision.transform.GetComponent<Transform>().Equals(checker);

		if (!isChecker)
		{
		  Transform[] trs = collision.transform.GetComponentsInParent<Transform>();

		  for (int i = 0; i < trs.Length; i++)
		  {
			 if (!isChecker && trs[i].Equals(checker))
				isChecker = true;
		  }
		}
		return isChecker;

	 }

  }
}