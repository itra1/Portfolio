using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Geometry;
using it.Game.NPC.Enemyes.Helpers;

namespace it.Game.NPC.Enemyes
{
  public abstract class Enemy : NPC
  {
	 protected bool _isPlayerVisible;
	 protected Transform _lockTransform;

	 private it.Game.NPC.Enemyes.Helpers.IPlayerCheck _playerCheck;


	 protected IPlayerCheck PlayerCheck { 
		get {
		  if (_playerCheck == null)
			 _playerCheck = GetComponent<IPlayerCheck>();
		  return _playerCheck;
		}
		set => _playerCheck = value; }



	 protected bool IsPlayerVisible()
	 {
		return false;

		//RaycastHit[] raycastHits;
		////int hits = RaycastExt.SafeSphereCastAll(transform.position + transform.up, transform.forward, _radiusPlayerCheck, out raycastHits, _radiusPlayerCheck, LayerMask.NameToLayer("Player"), transform);
		//int hits = RaycastExt.SafeSphereCastAll(transform.position + transform.up, transform.forward, _radiusPlayerCheck, out raycastHits, 0, Game.ProjectSettings.PlayerLayerMask, transform);

		//if (hits <= 0)
		//  return false;

		//RaycastHit raycastHitForvard;
		//RaycastExt.SafeRaycast(transform.position + transform.up, (raycastHits[0].transform.position - (transform.position + transform.up)).normalized, out raycastHitForvard, _radiusPlayerCheck, -1, transform);

		//if (raycastHitForvard.transform != raycastHits[0].transform)
		//  return false;


		//Vector3 direction = raycastHits[0].transform.position - transform.position;

		//float angle = Vector3Ext.HorizontalAngleTo(transform.forward, direction);

		//Debug.Log(angle);

		//return Mathf.Abs(angle) < 20;
	 }


  }

}