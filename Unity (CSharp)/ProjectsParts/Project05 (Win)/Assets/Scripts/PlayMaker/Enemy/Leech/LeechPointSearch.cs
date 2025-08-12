using System.Collections.Generic;
using HutongGames.PlayMaker;
using Utilites.Geometry;
using UnityEngine;
using it.Game.Environment.Level5.Leech;

namespace it.Game.PlayMaker.Enemy.Leech
{
  [ActionCategory("Enemy")]
  [HutongGames.PlayMaker.Tooltip("Поиск пути пиявок")]
  public class LeechPointSearch : FsmStateAction
  {
	 public FsmEvent OnComplete;
	 [RequiredField] public FsmOwnerDefault _owner;
	 [RequiredField] public FsmGameObject Tagret;
	 [UIHint(UIHint.Variable)]
	 [RequiredField] public FsmArray WayPoints;
	 [ObjectType(typeof(RoadContacType))]
	 public FsmEnum _wayType;
	 public float _timeRepeat = 0.5f;
	 public bool EveryUpdate;

	 /// Результ
	 public FsmGameObject TargetWayPoint;
	 public FsmGameObject SourceWayPoint; // Исключительно входящий парметр
	 public FsmGameObject NextWayPoint;

	 private GameObject _go;
	 private float _lastUpdate;

	 public override void OnEnter()
	 {
		if(_go== null)
		  _go = Fsm.GetOwnerDefaultTarget(_owner);

		SearchPath(Tagret.Value.transform);

		if (!EveryUpdate)
		  Fsm.Event(OnComplete);
	 }

	 public override void OnUpdate()
	 {
		if (!EveryUpdate) return;

		if (_lastUpdate + _timeRepeat > Time.timeSinceLevelLoad) return;

		SearchPath(Tagret.Value.transform);

		Fsm.Event(OnComplete);
	 }

	 private RoadPoint GetNearestPoint(Transform target)
	 {

		RoadPoint nearestPoint = null;
		float distance = float.MaxValue;

		for (int i = 0; i < WayPoints.Length; i++)
		{
		  float dist = ((WayPoints.Values[i] as GameObject).transform.position - target.position).sqrMagnitude;
		  if (dist < distance)
		  {
			 distance = dist;
			 nearestPoint = (WayPoints.Values[i] as GameObject).GetComponent<RoadPoint>();
		  }
		}
		return nearestPoint;
	 }

	 private void SearchPath(Transform target)
	 {
		// Найдем ближайшую точку маршрута

		RoadPoint TargetWayPoint = GetNearestPoint(target);
		RoadPoint startRoadPoint = GetNearestPoint(_go.transform);
		this.TargetWayPoint.Value = TargetWayPoint.gameObject;

		LeechPath path = new LeechPath();
		path.Distance = 0;
		path.PointList = new List<RoadPoint>();

		LeechPath optimalPath = new LeechPath();
		optimalPath.Distance = float.MaxValue;
		optimalPath.PointList = new List<RoadPoint>();
		
		// Путь
		FindPathAStar(startRoadPoint, TargetWayPoint, path, ref optimalPath);

		// Ищем подходящие точки маршрута
		FindNearestNextWayPoints(optimalPath);

		SelectWayType();
	 }

	 private void FindPathAStar(RoadPoint source, RoadPoint target, LeechPath path, ref LeechPath optimalPath)
	 {
		if(path.PointList.Count == 0)
		  path.Distance += (source.transform.position - _go.transform.position).sqrMagnitude;
		else
		  path.Distance += (source.transform.position - path.PointList[path.PointList.Count-1].transform.position).sqrMagnitude;

		path.PointList.Add(source);
		if (source == target)
		{
		  if (path.Distance < optimalPath.Distance)
			 optimalPath = path;
		  return;
		}

		for (int i = 0; i < source.ContactPoints.Count; i++)
		{
		  if (path.PointList.Contains(source.ContactPoints[i].Point)) continue;

		  LeechPath newPath = new LeechPath();
		  newPath.Distance = path.Distance;
		  newPath.PointList = new List<RoadPoint>(path.PointList);

		  FindPathAStar(source.ContactPoints[i].Point, target, newPath, ref optimalPath);
		}
	 }

	 private void FindNearestNextWayPoints(LeechPath fullPath)
	 {
		if(SourceWayPoint.Value == null)
		{
		  NextWayPoint.Value = fullPath.PointList[0].gameObject;
		  return;
		}

		RoadPoint sPoint = SourceWayPoint.Value.GetComponent<RoadPoint>();

		for (int i = 0; i < fullPath.PointList.Count;i++)
		{
		  if(sPoint == fullPath.PointList[i] && fullPath.PointList.Count >= i + 2)
		  {
			 NextWayPoint.Value = fullPath.PointList[i + 1].gameObject;
			 return;
		  }
		}
		NextWayPoint.Value = fullPath.PointList[0].gameObject;
	 }

	 private void SelectWayType()
	 {
		if (SourceWayPoint.Value == null)
		{
		  _wayType.Value = RoadContacType.AStar;
		}
		else
		{
		  RoadPoint start = SourceWayPoint.Value.GetComponent<RoadPoint>();
		  RoadPoint next = NextWayPoint.Value.GetComponent<RoadPoint>();
		  for (int i = 0; i < start.ContactPoints.Count; i++)
		  {
			 if(start.ContactPoints[i].Point == next)
			 {
				RoadContacType contactTypes = start.ContactPoints[i].Contact;
				int count =System.Enum.GetNames(typeof(RoadContacType)).Length;
				RoadContacType[] values = (RoadContacType[])System.Enum.GetValues(typeof(RoadContacType));
				while (true)
				{
				  RoadContacType rct = values[Random.Range(0, values.Length)];

				  if (rct == RoadContacType.up) continue;
				  if (rct == RoadContacType.wall) continue;

				  if ((start.ContactPoints[i].Contact & rct) != 0)
				  {
					 _wayType.Value = rct;
					 return;
				  }
				}

			 }
		  }
		  _wayType.Value = RoadContacType.AStar;

		}
		  
	 }
  }
}