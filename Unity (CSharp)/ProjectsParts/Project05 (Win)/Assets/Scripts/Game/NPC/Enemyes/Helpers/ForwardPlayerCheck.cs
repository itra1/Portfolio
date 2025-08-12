using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Geometry;

namespace it.Game.NPC.Enemyes.Helpers
{
  public class ForwardPlayerCheck : MonoBehaviourBase, IPlayerCheck
  {
	 [SerializeField]
	 private float _radius = 3f;
	 private float _forwardAngle = 20f;

	 public bool IsPlayerVisible
	 {
		get
		{
		  RaycastHit[] raycastHits;
		  int hits = RaycastExt.SafeSphereCastAll(transform.position + transform.up, transform.forward, _radius, out raycastHits, 0, Game.ProjectSettings.PlayerLayerMask, transform);

		  if (hits <= 0) return false;

		  RaycastHit raycastHitForvard;
		  RaycastExt.SafeRaycast(transform.position + transform.up, (raycastHits[0].transform.position - (transform.position + transform.up)).normalized, out raycastHitForvard, _radius, -1, transform);

		  if (raycastHitForvard.transform != raycastHits[0].transform)	 return false;

		  Vector3 direction = raycastHits[0].transform.position - transform.position;

		  return Mathf.Abs(Vector3Ext.HorizontalAngleTo(transform.forward, direction)) < _forwardAngle;
		}

	 }
  }
}