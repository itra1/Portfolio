using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("LightingBolt")]
  public class LightingBoltDeactivate : FsmStateAction
  {
	 public FsmOwnerDefault _gameObject;

	 private DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript _lighting;


	 public override void OnEnter()
	 {
		base.OnEnter();
		_lighting = Fsm.GetOwnerDefaultTarget(_gameObject).GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript>();
		_lighting.enabled = false;

		Finish();
	 }

  }
}