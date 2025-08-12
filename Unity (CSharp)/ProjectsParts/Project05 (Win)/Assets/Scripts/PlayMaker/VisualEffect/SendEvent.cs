using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;

using HutongGames.PlayMaker;
namespace it.Game.PlayMaker.VFX
{
  [ActionCategory("VFX")]
  public class SendEvent : FsmStateAction
  {
	 [RequiredField]
	 public FsmGameObject targetObject;
	 public FsmString eventName;

	 public override void OnEnter()
	 {
		targetObject.Value.GetComponent<UnityEngine.VFX.VisualEffect>().SendEvent(eventName.Value);
		Finish();
	 }

  }
}