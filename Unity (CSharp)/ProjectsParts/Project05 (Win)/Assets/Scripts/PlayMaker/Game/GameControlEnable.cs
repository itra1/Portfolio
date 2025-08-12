using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using it.Game.Managers;

namespace it.Game.PlayMaker.Helpers
{

  public class GameControlEnable : FsmStateAction
  {
	 public FsmBool isEnable;

	 public override void OnEnter()
	 {
		base.OnEnter();

		GameManager.Instance.GameInputSource.enabled = isEnable.Value;
		Finish();

	 }

  }
}