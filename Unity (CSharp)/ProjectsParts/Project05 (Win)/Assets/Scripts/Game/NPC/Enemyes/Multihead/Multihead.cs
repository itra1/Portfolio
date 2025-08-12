using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using com.ootii.Actors;
using it.Game.Player;
using RootMotion.FinalIK;

namespace it.Game.NPC.Enemyes
{
  public class Multihead : Enemy
  {
	 [SerializeField]	 private Collider _leftHand;
	 [SerializeField]	 private Collider _rightHand;
	 [SerializeField]	 private AimIK _aimIK;
	 [SerializeField]	 private Transform _aimTarget;

	 private Transform _particle;
	 private ActorController _actor;
	 private List<WaterLilyData> _WaterLilyList = new List<WaterLilyData>();

	 protected override void OnEnable()
	 {
		base.OnEnable();

		_actor = GetComponent<ActorController>();
	 }

	 public void PlayerContact()
	 {
		GetComponentInParent<Environment.Level2.MultiheadView>().PlayerContact();
	 }

	 public void RestoreWaterLily()
	 {
		for(int i = 0; i < _WaterLilyList.Count; i++)
		{
		  _WaterLilyList[i]._object.transform.position = _WaterLilyList[i].position;
		  Collider col = _WaterLilyList[i]._object.GetComponentInChildren<Collider>(true);
		  if(col != null)
		  {
			 col.enabled = true;
		  }
		}
	 }

	 public void DamagePlayer()
	 {

		Game.Managers.GameManager.Instance.UserManager.Health.Damage(this,1000, true);
	 }


	 public void FindParticle()
	 {
		_particle = transform.parent.Find("MultiheadParticles");
	 }

	 protected override void Update()
	 {
		base.Update();

		//if(PlayerBehaviour.Instance != null)
		//{
		//  _aimIK.solver.IKPositionWeight = 1;
		//  _aimTarget.position = PlayerBehaviour.Instance.transform.position + Vector3.up;
		//}
		//else
		//{
		//  _aimIK.solver.IKPositionWeight = 0;
		//}

		CheckDamageBox();

		for (int i = 0; i < _actor.BodyShapes.Count; i++)
		{
		  List<BodyShapeHit> _hitList = _actor.BodyShapes[i].CollisionOverlap(Vector3.zero, Quaternion.identity, -1);
		  for(int h = 0;h < _hitList.Count; h++)
		  {
			 if(_hitList[h].HitCollider != null)
			 {
				if(_hitList[h].HitCollider.transform.parent != null && _hitList[h].HitCollider.transform.parent.CompareTag("MultiheadDown"))
				{
				  _WaterLilyList.Add(new WaterLilyData()
				  {
					 _object = _hitList[h].HitCollider.transform.parent.gameObject,
					 position = _hitList[h].HitCollider.transform.parent.position
				  });

				  _hitList[h].HitCollider.enabled = false;
				  _hitList[h].HitCollider.transform.parent.DOMoveY(_hitList[h].HitCollider.transform.parent.position.y - 5, 1);
				}
			 }
		  }
		}

		if (_particle != null)
		{
		  Vector3 position = transform.position;
		  position.y = _particle.transform.position.y;
		  _particle.transform.position = position;
		}

	 }

	 private void CheckDamageBox()
	 {
		List<BodyShapeHit> hits = _actor.GetCollision();

		for (int i = 0; i < hits.Count; i++)
		{
		  var box = hits[i].HitCollider.transform.GetComponentInParent<it.Game.Environment.Level5.MultiheadArena.BoxChamfer>();
		  if (box != null)
			 box.Damage();
  		}
	 }

  }

  public struct WaterLilyData
  {
	 public GameObject _object;
	 public Vector3 position;
  }
}