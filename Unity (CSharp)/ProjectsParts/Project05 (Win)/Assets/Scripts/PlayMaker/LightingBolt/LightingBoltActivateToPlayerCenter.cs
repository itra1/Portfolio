using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using it.Game.Player;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("LightingBolt")]
  public class LightingBoltActivateToPlayerCenter : FsmStateAction
  {
	 public FsmOwnerDefault _gameObject;
	 public FsmGameObject camera;

	 private Transform _player;

	 private DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript _lighting;

	 public override void OnEnter()
	 {
		base.OnEnter();
		_lighting = Fsm.GetOwnerDefaultTarget(_gameObject).GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPrefabScript>();
		_lighting.Camera = camera.Value.GetComponent<CameraBehaviour>().Camera;
		_player = PlayerBehaviour.Instance.HipBone;
		_lighting.Destination = _player.gameObject;
		_lighting.enabled = true;
	 }

	 public override void OnUpdate()
	 {
		base.OnUpdate();
		_lighting.Destination = _player.gameObject;
	 }

  }
}