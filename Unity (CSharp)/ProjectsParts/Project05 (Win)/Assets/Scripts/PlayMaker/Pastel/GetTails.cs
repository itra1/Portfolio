using HutongGames.PlayMaker;

using it.Game.Player;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Pastel")]
  public class GetTails : FsmStateAction
  {
	 public FsmGameObject _player;
	 public FsmGameObject _tails;

	 public override void OnEnter()
	 {
		//_tails.Value = _player.Value.GetComponentInChildren<PlayerTails>().gameObject;
		Finish();
	 }
  }
}