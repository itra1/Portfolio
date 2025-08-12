using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using com.ootii.Actors;

namespace it.Game.Animals.Motions

{
  public class FishRandomMove : MonoBehaviourBase
  {

	 /// <summary>
	 /// Контроллер управляющий движением, движение за счет контроллер
	 /// </summary>
	 protected MoveToDriveWishVertical mDriver = null;

	 /// <summary>
	 /// Дистанция до точки для смены
	 /// </summary>
	 [SerializeField]
	 private float _ArrivalDistance = 0.5f;

	 private Vector3 _targetPoint;

	 /// <summary>
	 /// Запрет изменения высоты
	 /// </summary>
	 [SerializeField]
	 private bool _IgnoreY = false;

	 private void Start()
	 {
		mDriver = gameObject.GetComponent<MoveToDriveWishVertical>();

		_targetPoint = GetRandomPoint();

		mDriver.TargetPosition = _targetPoint;
	 }

	 protected void Update()
	 {

		if (mDriver == null) { return; }

		Vector3 lPosition = gameObject.transform.position;

		if (_IgnoreY)
		{
		  lPosition.y = 0f;
		}

		float lDistance = Vector3.Distance(lPosition, _targetPoint);
		if (lDistance < _ArrivalDistance)
		{

		  _targetPoint = GetRandomPoint();

		  mDriver.TargetPosition = _targetPoint;
		}
	 }

	 private void OnDrawGizmosSelected()
	 {
		Gizmos.DrawWireSphere(_targetPoint, 0.1f);
	 }

	 private Vector3 GetRandomPoint()
	 {
		bool exists = false;
		int iteration = 20;

		Vector3 target = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));

		while (!exists && iteration > 0)
		{
		  iteration--;
		  Vector3 direction = new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		  float distance = Random.Range(0.5f, 5f);

		  target = transform.position + direction.normalized * distance;

		  RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance);

		  foreach(RaycastHit hit in hits)
		  {
			 if (hit.collider.GetComponent<FishRandomMove>() != null || hit.collider.GetComponentInParent<FishRandomMove>())
				continue;

			 exists = true;

		  }

		}
		return target;
	 }
  }


}