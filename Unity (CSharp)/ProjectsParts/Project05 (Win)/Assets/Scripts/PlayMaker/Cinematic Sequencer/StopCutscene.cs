using UnityEngine;
using HutongGames.PlayMaker;
using Slate;
using com.ootii.Cameras;

namespace it.Game.PlayMaker
{
  [ActionCategory("Cinematic")]
  public class StopCutscene : FsmStateAction
  {
	 [RequiredField]
	 public FsmGameObject _cutScene;
	 public FsmEvent _onStop;
	 public override void OnEnter()
	 {
		Cutscene cutscene = _cutScene.Value.GetComponent<Cutscene>();
		if (cutscene.isActive)
		  cutscene.Stop();
		Fsm.Event(_onStop);
	 }
  }
}