using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.NPC.Helpers
{
  public class WayPointHelper : MonoBehaviourBase
  {

	 public bool IsLoop { get => _isLoop; set => _isLoop = value; }
	 [SerializeField]
	 private bool _isLoop;
	 public Transform[] WayPoints { get => _wayPoints; set => _wayPoints = value; }

	 [SerializeField]
	 private Transform[] _wayPoints;


	 private void OnDrawGizmos()
	 {
		Gizmos.color = Color.cyan;

		if (_wayPoints.Length > 0)
		{
		  for (int i = 0; i < _wayPoints.Length; i++)
		  {
			 if (i == _wayPoints.Length - 1)
				continue;
			 Gizmos.DrawLine(_wayPoints[i].position, _wayPoints[i + 1].position);
		  }
		}

	 }

  }
}