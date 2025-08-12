using UnityEngine;
using HutongGames.PlayMaker;
using Pegasus;

namespace it.Game.PlayMaker
{
  [ActionCategory("Pegasus")]
  public class PegasusPause : PegasusBase
  {
	 public override void OnEnter()
	 {
		base.OnEnter();

		_pegasus.PegasusManager.PauseFlythrough();
		Finish();

	 }
  }
}