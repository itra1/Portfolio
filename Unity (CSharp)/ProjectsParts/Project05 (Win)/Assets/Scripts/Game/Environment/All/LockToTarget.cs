using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.All
{
  /// <summary>
  /// Вращение в направление цели
  /// </summary>
  public class LockToTarget : MonoBehaviourBase
  {
	 private Transform _target;
	 public Transform Target
	 {
		get => _target; set => _target = value;
	 }

	 private void LateUpdate()
	 {
		if (_target == null)
		  return;

		transform.LookAt(_target);

	 }

  }
}