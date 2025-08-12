using UnityEngine;
using HutongGames.PlayMaker;
using Pegasus;

namespace it.Game.PlayMaker
{
  [ActionCategory("Pegasus")]
  public class PegasusResume : PegasusBase
  {
	 public override void OnEnter()
	 {
		base.OnEnter();

		_pegasus.PegasusManager.ResumeFlythrough();
		Finish();

	 }
  }
}