using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;

using HutongGames.PlayMaker;

using UnityEngine;

namespace it.Game.NPC.Animals.Raven
{
  public class RavenFlyOut : FsmStateAction
  {
	 public FsmGameObject _gameObject;
	 public FsmGameObject _player;

	 private Animation _animation;

	 public override void OnEnter()
	 {
		_animation = _gameObject.Value.GetComponent<Animation>();
	 }

  }
}