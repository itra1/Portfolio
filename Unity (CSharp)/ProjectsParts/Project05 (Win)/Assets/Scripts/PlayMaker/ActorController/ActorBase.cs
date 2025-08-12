using UnityEngine;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  public class ActorBase : FsmStateAction
  {
	 public FsmOwnerDefault _gameObject;
	 protected GameObject _go;
	 protected com.ootii.Actors.ActorController _actor;

	 public override void Awake()
	 {
		_go = Fsm.GetOwnerDefaultTarget(_gameObject);
		_actor = _go.GetComponent<com.ootii.Actors.ActorController>();
	 }

  }
}