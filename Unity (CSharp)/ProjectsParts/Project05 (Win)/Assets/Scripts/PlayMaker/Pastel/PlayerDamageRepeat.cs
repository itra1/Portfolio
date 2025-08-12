using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Pastel")]
  public class PlayerDamageRepeat : FsmStateAction
  {
	 public FsmOwnerDefault _owner;
	 public FsmFloat period = 5;
	 public FsmFloat damage = 10;
	 public FsmBool damageEnter = false;
	 public FsmEvent OnDamage;

	 private float _lastDamage;

	 public override void OnEnter()
	 {
		base.OnEnter();

		if (damageEnter.Value)
		{
		  Damage();
		}
		_lastDamage = Time.time;

	 }

	 public override void OnUpdate()
	 {
		base.OnUpdate();

		if(_lastDamage + period.Value < Time.time)
		{
		  Damage();
		  _lastDamage = Time.time;
		}

	 }

	 private void Damage()
	 {
		Game.Managers.GameManager.Instance.UserManager.Health.Damage(Fsm.GetOwnerDefaultTarget(_owner).transform,damage.Value, true);
		Fsm.Event(OnDamage);
	 }
  }
}