using UnityEngine;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker
{
  [ActionCategory("Pegasus")]
  public class PegasusActivator : PegasusBase
  {
	 public FsmBool _fromFameraPosition = true;
	 public FsmEvent _completeMove;

	 public override void OnEnter()
	 {
		base.OnEnter();
		_pegasus.FromCameraPosition = _fromFameraPosition.Value;
		_pegasus.Activate(() =>
		{
		  Fsm.Event(_completeMove);

		});
		Finish();
	 }
  }
}