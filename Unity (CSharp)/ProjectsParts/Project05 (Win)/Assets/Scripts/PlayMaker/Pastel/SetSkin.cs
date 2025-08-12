using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Actors.AnimationControllers;
using UnityEngine.AI;
using it.Game.Player;

namespace it.Game.PlayMaker
{
  [ActionCategory("Pastel")]
  public class SetSkin : FsmStateAction
  {
	 public FsmGameObject _actorSource;
	 public int dress;
	 public override void OnEnter()
	 {
		_actorSource.Value.GetComponent<PlayerDress>().SetDress(dress);
	 }

  }
}