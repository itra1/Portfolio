using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using it.Game.Managers;

namespace it.Game.PlayMaker.Helpers
{

  [ActionCategory("Game")]
  public class FullColorScreen : FsmStateAction
  {
	 public FsmColor fullColor;
	 public FsmFloat duration;
	 public FsmFloat middle;
	 public FsmEvent OnStart;
	 public FsmEvent OnMiddle;
	 public FsmEvent OnEnd;
	 public override void OnEnter()
	 {
		base.OnEnter();

		UiManager.Instance.FillAndRepeatColor(fullColor.Value, duration.Value,
		  () => { Fsm.Event(OnStart); }, 
		  () => { Fsm.Event(OnMiddle); }, 
		  () => { Fsm.Event(OnEnd); }, middle.Value);

		Finish();
	 }
  }
}