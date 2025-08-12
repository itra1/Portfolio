using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using it.Game.Managers;

namespace it.Game.PlayMaker
{
  [ActionCategory("Dialog")]
  public class DialogPlay : FsmStateAction
  {
	 [RequiredField]
	 public FsmString uuid;
	 public FsmEvent onStart;
	 public FsmEvent onNextEvent;
	 public FsmEvent onComplete;
	 public FsmInt frameIndex;

	 public override void OnEnter()
	 {
		GameManager.Instance.DialogsManager.ShowDialog(uuid.Value, () =>
		 {
			Fsm.Event(onStart);
		 }, (index) =>
		 {
			frameIndex.Value = index;
			Fsm.Event(onNextEvent);
		 }, () =>
		 {
			Fsm.Event(onComplete);
		 });
	 }

  }
}