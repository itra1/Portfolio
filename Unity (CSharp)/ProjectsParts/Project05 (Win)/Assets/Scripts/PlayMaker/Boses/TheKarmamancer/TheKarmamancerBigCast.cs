using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using com.ootii.Geometry;
using DG.Tweening;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Boses.TheKarmamancer
{
  /// <summary>
  /// Большой каст
  /// </summary>
  [ActionCategory("Enemyes")]
  [Tooltip("Вымолняем большой каст")]
  public class TheKarmamancerBigCast : FsmStateAction
  {
	 [Tooltip("Указание на игрока")]
	 public FsmGameObject target;

	 public FsmGameObject bullet;
	 public FsmGameObject position;

	 [Tooltip("окончание действия")]
	 public FsmEvent OnFinishEvent;

	 private int _repearRayCount = 5;
	 private GameObject _object;
	 private Animator _animator;
	 private it.Game.NPC.Enemyes.Boses.Karmamancer.BigCastBullet _bigCastBullet;

	 private bool _onEnter;

	 private int _stateId;
	 private int _attackId;

	 private float _attackTime;

	 public override void OnPreprocess()
	 {
		Fsm.HandleLateUpdate = true;
	 }

	 public override void OnEnter()
	 {
		if(_animator == null)
		  _animator = Owner.GetComponent<Animator>();
		_attackTime = -1;

		_stateId = Animator.StringToHash("State");
		_attackId = Animator.StringToHash("Attack");

		_animator.SetInteger(_stateId, 4);
		_onEnter = true;
		_bigCastBullet = bullet.Value.GetComponent<it.Game.NPC.Enemyes.Boses.Karmamancer.BigCastBullet>();
	 }

	 public override bool Event(FsmEvent fsmEvent)
	 {

		if (fsmEvent.Name == "OnAttackReady")
		{
		  Debug.Log("OnAttackReady");
		}
		if (fsmEvent.Name == "OnAttack")
		{
		  _attackTime = Time.time;
		  _bigCastBullet.gameObject.SetActive(true);
		  _bigCastBullet.transform.position = position.Value.transform.position;
		  _bigCastBullet.Cast();
		  //Complete();
		  //Cast();

		  //DOVirtual.DelayedCall(5, () =>
		  //{
			 //Complete();

		  //});
		}

		return true;
	 }


	 public override void OnUpdate()
	 {
		if (_attackTime > 0 && _attackTime < Time.time - 5)
		{
		  _attackTime = -1;
		  Complete();
		}
	 }

	 private void Complete()
	 {
		Debug.Log("Complete");
		Fsm.Event(OnFinishEvent);
	 }

	 public override void OnLateUpdate()
	 {
		if (_onEnter)
		{
		  _onEnter = false;
		  _animator.SetInteger(_stateId, -1);

		}
	 }

	 public override void OnExit()
	 {
		_animator.SetInteger(_stateId, 0);
	 }

	 //public override void OnUpdate()
	 //{

		//if (_nextTimeState < Time.time)
		//{
		//  _state++;

		//  if (_state == 1)
		//  {
		//	 _animator.SetTrigger(_attackId);
		//	 _nextTimeState = Time.time + 2;
		//  }
		//  if (_state == 2)
		//  {
		//	 Cast();
		//	 _nextTimeState = Time.time + 1;
		//  }
		//  if (_state == 3)
		//  {
		//	 Fsm.Event(OnComplete);
		//  }
		//}

	 //}


	 //private void Cast()
	 //{
		//float _heightIncrement = 1.7f / _repearRayCount;

		//Vector3 StartRay = Owner.transform.position + Vector3.up;
		//bool isTargetHit = false;

		//for (int i = 0; i < _repearRayCount; i++)
		//{
		//  if (isTargetHit)
		//	 continue;
		//  RaycastHit _hit;
		//  Vector3 targetRay = target.Value.transform.position + Vector3.up * _heightIncrement * i;
		//  if (RaycastExt.SafeRaycast(StartRay, targetRay - StartRay, out _hit,
		//	 (targetRay - StartRay).magnitude, -1, Owner.transform))
		//  {
		//	 if (CheckCollision(target.Value.transform, _hit.transform))
		//	 {
		//		isTargetHit = true;
		//	 }
		//  }
		//}

		//if (isTargetHit)
		//  Game.Managers.GameManager.Instance.UserManager.Health.Damage(1000, true);
	 //}

	 //private bool CheckCollision(Transform checker, Transform collision)
	 //{
		//bool isChecker = collision.transform.GetComponent<Transform>().Equals(checker);

		//if (!isChecker)
		//{
		//  Transform[] trs = collision.transform.GetComponentsInParent<Transform>();

		//  for (int i = 0; i < trs.Length; i++)
		//  {
		//	 if (!isChecker && trs[i].Equals(checker))
		//		isChecker = true;
		//  }
		//}
		//return isChecker;

	 //}

  }
}