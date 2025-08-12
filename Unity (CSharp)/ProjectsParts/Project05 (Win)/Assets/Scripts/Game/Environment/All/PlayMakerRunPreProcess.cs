using UnityEngine;
using System.Collections;
using it.Game.Player.Interactions;
using it.Game.Handles;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace it.Game.Environment
{

  public class PlayMakerRunPreProcess : PlayMakerRun
  {
	 private bool preProcess = false;

	 public void PreProcess()
	 {
		if (preProcess)
		  return;
		preProcess = true;
		_fsm.SendEvent("PreProcess");
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
		  if (State == 0)
			 preProcess = false;
		}


	 }

  }
}