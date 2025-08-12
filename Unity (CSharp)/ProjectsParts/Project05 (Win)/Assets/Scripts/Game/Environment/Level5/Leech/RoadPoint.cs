using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Environment.Level5.Leech
{
  public class RoadPoint : MonoBehaviour
  {
	 [SerializeField] private List<ContacntRoadPoints> _contactPoints;
	 [SerializeField] private float _distanceComplete = 1;
	 [SerializeField] private bool _isFirst;

	 public List<ContacntRoadPoints> ContactPoints { get => _contactPoints; set => _contactPoints = value; }
	 public float DistanceComplete { get => _distanceComplete; set => _distanceComplete = value; }
	 public bool IsFirst { get => _isFirst; set => _isFirst = value; }

#if UNITY_EDITOR

	 private void OnDrawGizmos()
	 {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, _distanceComplete);
		
		if (_contactPoints.Count > 0)
		{
		  for(int p = 0; p < _contactPoints.Count; p++)
		  {
			 if(_contactPoints[p].Point != null)
			 {
				_contactPoints[p].Point.Add(_contactPoints[p], this);

				if (_contactPoints[p].Path != null) continue;

				if((_contactPoints[p].Contact & RoadContacType.AStar) != 0)
				  Gizmos.color = Color.white;
				if ((_contactPoints[p].Contact & RoadContacType.up) != 0)
				  Gizmos.color = Color.yellow;
				if ((_contactPoints[p].Contact & RoadContacType.jump) != 0)
				  Gizmos.color = Color.red;
				if ((_contactPoints[p].Contact & RoadContacType.wall) != 0)
				  Gizmos.color = Color.cyan;
				if (_contactPoints[p].Contact != 0)
				  Gizmos.DrawLine(transform.position, _contactPoints[p].Point.transform.position);

			 }
		  }
		}

	 }

	 [ContextMenu("ClearNoCorrectLinks")]
	 private void ClearNoCorrectLinks()
	 {
		for(int i = 0; i < _contactPoints.Count; i++)
		{
		  if (_contactPoints[i].Point == null)
		  {
			 _contactPoints.Remove(_contactPoints[i]);
			 continue;
		  }

		  if(_contactPoints[i].Contact == 0)
		  {
			 ContacntRoadPoints targetRoad = _contactPoints[i].Point.ContactPoints.Find(x => x.Point == this);

			 if(targetRoad.Contact == 0)
			 {
				_contactPoints[i].Point.ContactPoints.Remove(targetRoad);
				_contactPoints.Remove(_contactPoints[i]);
				ClearNoCorrectLinks();
				return;
			 }
		  }

		}
	 }

	 private void Add(ContacntRoadPoints road, RoadPoint point)
	 {
		for(int i = 0; i < _contactPoints.Count; i++)
		{
		  if (_contactPoints[i].Point == point) return;
		}

		ContacntRoadPoints newItem = new ContacntRoadPoints();
		newItem.Point = point;
		newItem.Contact = RoadContacType.none;
		_contactPoints.Add(newItem);
	 }

#endif

  }

  [System.Serializable]
  public struct ContacntRoadPoints
  {
	 public RoadPoint Point;
	 public RoadContacType Contact;
	 public FluffyUnderware.Curvy.CurvySpline Path;
  }

  [System.Flags]
  public enum RoadContacType
  {
	 none = 0,
	 AStar = 1,
	 jump = 2,
	 up = 4,
	 wall = 8
  }
}