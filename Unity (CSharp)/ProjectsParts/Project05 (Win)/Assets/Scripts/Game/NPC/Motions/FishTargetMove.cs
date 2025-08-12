using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.ootii.Actors;

namespace it.Game.NPC.Animals
{
  public class FishTargetMove : Fish
  {

	 /// <summary>
	 /// Случайный выбор позиции
	 /// </summary>
	 [SerializeField]
	 private bool _Randomize = false;

	 /// <summary>
	 /// Дистанция до точки для смены
	 /// </summary>
	 [SerializeField]
	 private float _ArrivalDistance = 0.5f;

	 /// <summary>
	 /// Контроллер управляющий движением, движение за счет контроллер
	 /// </summary>
	 protected MoveToDriveWishVertical mDriver = null;

	 /// <summary>
	 /// Запрет изменения высоты
	 /// </summary>
	 [SerializeField]
	 private bool _IgnoreY = false;

	 /// <summary>
	 /// Текущий индекс точки движения
	 /// </summary>
	 private int mCurrentIndex = -1;

	 /// <summary>
	 /// Список для движения позиций
	 /// </summary>
	 [SerializeField]
	 protected List<Transform> _Waypoints = new List<Transform>();
	 protected override void Awake()
	 {
		base.Awake();
	 }

	 private void Start()
	 {

		mDriver = gameObject.GetComponent<MoveToDriveWishVertical>();

		if (mDriver != null && _Waypoints.Count > 0)
		{
		  if (_Randomize)
		  {
			 mCurrentIndex = UnityEngine.Random.Range(0, _Waypoints.Count - 1);
		  }
		  else
		  {
			 mCurrentIndex = 0;
		  }

		  mDriver.Target = _Waypoints[mCurrentIndex];
		}
	 }

	 protected override void Update()
	 {
		base.Update();

		if (mDriver == null) { return; }
		if (_Waypoints.Count == 0) { return; }

		Vector3 lPosition = gameObject.transform.position;
		Vector3 lWaypoint = _Waypoints[mCurrentIndex].position;

		if (_IgnoreY)
		{
		  lPosition.y = 0f;
		  lWaypoint.y = 0f;
		}

		float lDistance = Vector3.Distance(lPosition, lWaypoint);
		if (lDistance < _ArrivalDistance)
		{
		  if (_Randomize)
		  {
			 mCurrentIndex = UnityEngine.Random.Range(0, _Waypoints.Count - 1);
		  }
		  else
		  {
			 mCurrentIndex++;
		  }

		  if (mCurrentIndex >= _Waypoints.Count) { mCurrentIndex = 0; }

		  mDriver.Target = _Waypoints[mCurrentIndex];
		}
	 }

  }
}