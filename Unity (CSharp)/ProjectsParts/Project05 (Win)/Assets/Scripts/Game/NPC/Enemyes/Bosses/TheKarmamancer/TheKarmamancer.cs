using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.VFX;

namespace it.Game.NPC.Enemyes.Boses.Karmamancer
{
  public class TheKarmamancer : Enemy
  {
	 /*
	  * Состояния:
	  * 
	  * 1 - простой бой
	  * 2 - большой каст
	  * 3 - откидывание
	  * 4 - смерть
	  * 
	  */

	 int damageCount = 3;
	 private bool _isAttack;
	 public bool IsAttack { get => _isAttack; set => _isAttack = value; }

	 private bool _isBigCast = false;

	 [SerializeField]
	 private VisualEffect _groundEffects;

	 public bool IsBigCast { get => _isBigCast; set => _isBigCast = value; }
	 public ParticleSystem _handParticles;
	 public ParticleSystem.EmissionModule _handParticlesEmiter;
	 public ParticleSystem _klucaParticles;
	 public ParticleSystem.EmissionModule _klucaParticlesEmiter;

	 [SerializeField]
	 private PlayMakerFSM _fsmBehaviour;
	 public PlayMakerFSM FsmBehaviour { get => _fsmBehaviour; set => _fsmBehaviour = value; }

	 protected override void OnEnable()
	 {
		base.OnEnable();
		_handParticlesEmiter = _handParticles.emission;
		_klucaParticlesEmiter = _klucaParticles.emission;
		damageCount = 3;
		//ActiveAllBehaviourTree(false);
		//ActiveBehaviourTree("Behavior");
		isHandDamageReady = false;
		isKlukaDamageReady = false;
		_handParticlesEmiter.enabled = false;
		_klucaParticlesEmiter.enabled = false;
		SetState(1);
	 }

	 public override int State
	 {
		get => base.State;
		set
		{
		  base.State = value;
		  //var variable = GetBehaviourTree("Behavior").GetVariable("State");
		  //variable.SetValue(base.State);
		  //GetBehaviourTree("Behavior").SetVariable("State", variable);
		}
	 }


	 /// <summary>
	 /// ДИстанционная атака
	 /// 
	 /// вызывается из PlayerMaker
	 /// </summary>
	 public void CastDistantionAttack()
	 {
		Debug.Log("Cast");
		StartCoroutine(Cast());
	 }

	 IEnumerator Cast()
	 {
		var fsm = GetFsm("Distantion Attack");
		yield return new WaitForSeconds(1);
		for (int i = 0; i < 3; i++)
		{
		  yield return new WaitForSeconds(2f);
		  fsm.SendEvent("Attack");
		}
		yield return new WaitForSeconds(1);
		fsm.SendEvent("AllMiss");
	 }

	 [ContextMenu("BigCast")]
	 public void BigCast()
	 {
		SetState(2);
	 }

	 public override void SetState(int state)
	 {
		base.SetState(state);

		//var tree = GetBehaviourTree("Behavior");
		_fsmBehaviour.FsmVariables.GetFsmInt("State").Value = State;
		_fsmBehaviour.SendEvent("StateChange");
	 }

	 public void BigCastComplete()
	 {
		//ActiveBehaviourTree("Behavior");
	 }

	 /// <summary>
	 /// Событие вызывается из анимации
	 /// </summary>
	 public void AnimEventBigCast()
	 {
		Debug.Log("AnimEventBigCast");
	 }

	 [ContextMenu("Damage")]
	 public void Damage()
	 {
		Debug.Log("Damage");
		damageCount--;

		if (damageCount == 0)
		{
		  Debug.Log("dead");
		  gameObject.GetComponentInParent<it.Game.Environment.Level4.KarmamancerBossArena>().DestroyBoss();
		  //SetState(4);
		}
		else
		{
		  //SetState(3);
		  FsmBehaviour.SendEvent("OnDamage");
		}
	 }


	 /// <summary>
	 /// Событие тригера
	 /// </summary>
	 public void HandPlayerContact()
	 {
		if (!isHandDamageReady)
		  return;
		Game.Managers.GameManager.Instance.UserManager.Health.Damage(this, 1000, true);
	 }
	 /// <summary>
	 /// Событие тригера
	 /// </summary>
	 public void KlukaPlayerContact()
	 {
		if (!isKlukaDamageReady)
		  return;
		Game.Managers.GameManager.Instance.UserManager.Health.Damage(this, 1000, true);
	 }

	 public void AttackReady(bool isActive)
	 {
		isHandDamageReady = isActive;
		isKlukaDamageReady = isActive;
	 }

	 bool isHandDamageReady;
	 bool isKlukaDamageReady;

	 public void AnimationEvent(string eventName)
	 {
		if (eventName.Equals("HandAttackStart"))
		{
		  isHandDamageReady = true;
		  //_handLine.emitting = true;
		  _handParticlesEmiter.enabled = true;
		}
		if (eventName.Equals("HandAttackEnd"))
		{
		  isHandDamageReady = false;
		  //_handLine.emitting = false;
		  _handParticlesEmiter.enabled = false;
		}
		if (eventName.Equals("KlikaAttackStart"))
		{
		  isKlukaDamageReady = true;
		  //_klukaLine.emitting = true;
		  _klucaParticlesEmiter.enabled = true;
		}
		if (eventName.Equals("KlikaAttackEnd"))
		{
		  isKlukaDamageReady = false;
		  //_klukaLine.emitting = false;
		  _klucaParticlesEmiter.enabled = false;
		}



	 }

  }
}