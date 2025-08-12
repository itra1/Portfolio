using HutongGames.PlayMaker;

namespace it.Game.PlayMaker
{
  [ActionCategory("Pegasus")]
  public class PegasusDeactivator : PegasusBase
  {
	 public FsmEvent _completeMove;

	 public override void OnEnter()
	 {
		base.OnEnter();
		_pegasus.Deactivate();
		Finish();
	 }

  }
}