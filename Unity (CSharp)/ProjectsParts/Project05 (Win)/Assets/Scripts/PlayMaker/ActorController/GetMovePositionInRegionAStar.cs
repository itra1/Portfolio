using UnityEngine;
using System.Collections;
using Pathfinding;
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using com.ootii.Geometry;
using Pathfinding.Util;

namespace it.Game.PlayMaker.ActorController
{
  public class GetMovePositionInRegionAStar : FsmStateAction
  {
	 public FsmGameObject _region;
	 private Collider _regionCollider;

	 public FsmVector3 _targetPosition;

	 public FsmFloat minDistance = new FsmFloat(2);
	 public FsmFloat maxDistance = new FsmFloat(5);

	 //[UIHint(UIHint.Layer)]
	 //[Tooltip("Pick only from these layers.")]
	 //public FsmInt[] layerMask;

	 private Seeker _seeker;

	 protected PathInterpolator interpolator = new PathInterpolator();

	 public override void Reset()
	 {
		_region = null;
		_regionCollider = null;
	 }

	 public override void Awake()
	 {
		_regionCollider = _region.Value.GetComponent<Collider>();
	 }

	 public override void OnEnter()
	 {
		if (_seeker == null)
		  _seeker = Owner.GetComponent<Seeker>();

		FindNewPosition();
	 }

	 private void FindNewPosition()
	 {
		for (int i = 0; i < 20; i++)
		{
		  Vector3 point = Owner.transform.position
			 + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * Random.Range(minDistance.Value, maxDistance.Value);
		  Vector3 dir = Owner.transform.position - point;
		  Vector3 dirNorm = dir.normalized;

		  //if (RaycastExt.SafeRaycast(point, dirNorm, dir.magnitude, ActionHelpers.LayerArrayToLayerMask(layerMask, false)))
		  //{
		  //  continue;
		  //}

		  RaycastHit _hit;

		  if (RaycastExt.SafeRaycast(point, dirNorm, out _hit, dir.magnitude))
		  {
			 if (_hit.collider.Equals(_regionCollider))
				continue;
		  }

		  _seeker.StartPath(Owner.transform.position, point, OnPathComplete);
		  return;
		};
	 }

	 protected void OnPathComplete(Pathfinding.Path newPath)
	 {
		Pathfinding.ABPath p = newPath as Pathfinding.ABPath;

		if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

		p.Claim(this);

		if (p.error)
		{
		  p.Release(this);
		  FindNewPosition();
		  return;
		}

		Path path = p;

		if (path.vectorPath.Count == 1) path.vectorPath.Add(path.vectorPath[0]);
		interpolator.SetPath(path.vectorPath);
		_targetPosition.Value = interpolator.endPoint;

		Finish();

	 }

  }
}