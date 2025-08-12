using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Inputs
{
  [ActionCategory("Easy Inputs")]
  public class EnableGameInput : FsmStateAction
  {
	 public FsmBool activate;

	 public override void OnEnter()
	 {
		it.Game.Managers.GameManager.Instance.GameInputSource.IsEnabled = activate.Value;
		Finish();
	 }
  }
}