using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.All
{
  /// <summary>
  /// Удержание расстояния до объекта
  /// </summary>
  public class FixedDistanceToTarget : MonoBehaviourBase
  {
	 private Transform _target;
	 public Transform Target { get => _target; set => _target = value; }

	 /// <summary>
	 /// Растояние которое требуется придерживаться
	 /// </summary>
	 [SerializeField]
	 private float _distance = 950;

	 public float Distance { get => _distance; set => _distance = value; }

	 private void LateUpdate()
	 {
		if (_target == null)
		  return;

		if (_distance > 0)
		{
		  Vector3 direction = transform.position - Target.position;
		  transform.position = Target.position + direction.normalized * _distance;

		}

	 }

  }
}