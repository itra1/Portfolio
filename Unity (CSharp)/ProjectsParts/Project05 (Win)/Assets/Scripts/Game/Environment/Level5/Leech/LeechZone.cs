using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace it.Game.Environment.Level5.Leech
{
  /// <summary>
  /// Червячная зона
  /// 
  /// Черви приследуют 2 зоны
  /// </summary>
  public class LeechZone : Environment
  {
	 private RoadPoint[] _roadPoints;
	 private it.Game.NPC.Enemyes.Leech[] Leechs;
	 private GameObject[] RoadPointsObjects;
	 private float _lasteCalc = 0;

	 protected override void Awake()
	 {
		base.Awake();

		_roadPoints = GetComponentsInChildren<RoadPoint>();
		Leechs = GetComponentsInChildren<it.Game.NPC.Enemyes.Leech>();
		RoadPointsObjects = new GameObject[_roadPoints.Length];

		for (int i = 0; i < _roadPoints.Length; i++)
		  RoadPointsObjects[i] = _roadPoints[i].gameObject;

		for(int i = 0; i < Leechs.Length; i++)
		{
		  Leechs[i].GetComponent<PlayMakerFSM>().FsmVariables.GetFsmArray("WayPoints").Values = RoadPointsObjects;
		}

	 }

	 private void FindPath(Transform target, RoadPoint startPoint)
	 {
		// Найдем ближайшую точку маршрута

		RoadPoint nearestPoint = null;
		float distance = float.MaxValue;

		for (int i = 0; i < _roadPoints.Length; i++)
		{
		  float dist = (_roadPoints[i].transform.position - target.position).sqrMagnitude;
		  if (dist < distance)
		  {
			 distance = dist;
			 nearestPoint = _roadPoints[i];
		  }
		}

		LeechPath path = new LeechPath();
		path.Distance = 0;
		path.PointList = new List<RoadPoint>();
		path.PointList.Add(startPoint);

		LeechPath optimalPath = new LeechPath();
		optimalPath.Distance = float.MaxValue;
		optimalPath.PointList = new List<RoadPoint>();
		FindPathAStar(startPoint, nearestPoint, path, ref optimalPath);

	 }
	 private void Update()
	 {
		//if (_lasteCalc + 0.5f > Time.time) return;
		//_lasteCalc = Time.time;

		//RoadPoint startPoint = null;

		//for (int i = 0; i < _roadPoints.Length; i++)
		//{
		//  if (_roadPoints[i].IsFirst)
		//	 startPoint = _roadPoints[i];
		//}

		//FindPath(Player.PlayerBehaviour.Instance.transform, startPoint);

	 }

	 private void FindPathAStar(RoadPoint source, RoadPoint target, LeechPath path, ref LeechPath optimalPath)
	 {
		for (int i = 0; i < source.ContactPoints.Count; i++)
		{
		  if (path.PointList.Contains(source.ContactPoints[i].Point)) continue;

		  LeechPath newPath = new LeechPath();
		  newPath.Distance = path.Distance;
		  newPath.PointList = new List<RoadPoint>(path.PointList);

		  newPath.PointList.Add(source.ContactPoints[i].Point);
		  newPath.Distance += (source.ContactPoints[i].Point.transform.position - source.transform.position).sqrMagnitude;

		  if (source.ContactPoints[i].Point == target)
		  {
			 if (newPath.Distance < optimalPath.Distance)
				optimalPath = newPath;
			 return;
		  }

		  FindPathAStar(source.ContactPoints[i].Point, target, newPath, ref optimalPath);
		}
	 }

  }

  public struct LeechPath
  {
	 public float Distance;
	 public List<RoadPoint> PointList;
  }

}