using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Geometry;

namespace it.Game.NPC.Enemyes.Helpers
{
  public class RoundPlayerCheck : MonoBehaviourBase, IPlayerCheck
  {
	 

	 [SerializeField]
	 private bool _checkRepeat = true;
	 public bool CheckRepeat { get => _checkRepeat; set => _checkRepeat = value; }

	 [SerializeField]
	 private float _timeRepeat = 1f;
	 private float _nextCheck = 0;

	 [SerializeField]
	 private float _radius = 3f;

	 private bool _playerVisible;

	 public bool IsPlayerVisible
	 {
		get
		{
		  return _playerVisible;
		}
	 }

	 private void Update()
	 {
		if (!CheckRepeat || _nextCheck > Time.time)
		  return;

		_nextCheck = Time.time + _timeRepeat;
		_playerVisible = CheckPlayer();
	 }

	 private bool CheckPlayer()
	 {

		RaycastHit[] raycastHits;
		int hits = RaycastExt.SafeSphereCastAll(transform.position + transform.up, transform.forward, _radius, out raycastHits, 0, Game.ProjectSettings.PlayerLayerMask, transform);

		if (hits <= 0) return false;

		Transform charactorTransform = raycastHits[0].transform;

		RaycastHit raycastHitForvard;
		RaycastExt.SafeRaycast(transform.position + transform.up, (charactorTransform.position - (transform.position + transform.up)).normalized, out raycastHitForvard, _radius, -1, transform);

		if (raycastHitForvard.transform != charactorTransform) return false;

		return true;
	 }


	 private void OnDrawGizmosSelected()
	 {
		Gizmos.DrawWireSphere(transform.position, _radius);
	 }

  }
}