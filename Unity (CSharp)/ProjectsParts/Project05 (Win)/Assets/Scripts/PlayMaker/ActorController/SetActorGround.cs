using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using com.ootii.Actors;
using com.ootii.Timing;
using com.ootii.Geometry;
using com.ootii.Timing;
namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  [HutongGames.PlayMaker.Tooltip("Установка параметров гроунда")]
  public class SetActorGround : FsmStateAction
  {
	 public FsmOwnerDefault gameObject;
	 protected com.ootii.Actors.ActorController _actor;

	 public FsmBool IsGravityEnabled;
	 public FsmBool ForceGrounding;
	 public FsmBool FixGroundPenetration;

	 public override void OnEnter()
	 {
		base.OnEnter();
		GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
		_actor = go.GetComponent<com.ootii.Actors.ActorController>();

		_actor.IsGravityEnabled = IsGravityEnabled.Value;
		_actor.ForceGrounding = ForceGrounding.Value;
		_actor.FixGroundPenetration = FixGroundPenetration.Value;

		Finish();
	 }
  }
}