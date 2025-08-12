using HutongGames.PlayMaker;
namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Pastel")]
  public class EnableGameControl : FsmStateAction
  {
	 public FsmBool _enabled;
	 public override void OnEnter()
	 {
		it.Game.Managers.GameManager.Instance.GameInputSource.enabled = _enabled.Value;
	 }
  }
}