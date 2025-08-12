using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using it.Game.Managers;

namespace it.Game.PlayMaker
{
  [ActionCategory(ActionCategory.Vector3)]
  public class Vector3Distance : FsmStateAction
  {

	 public FsmVector3 vecto1;
	 public FsmVector3 vecto2;

	 public FsmFloat storeResult;

	 public bool everyFrame;

	 public override void Reset()
	 {
		vecto1 = null;
		vecto2 = null;
		storeResult = null;
		everyFrame = true;
	 }

	 public override void OnEnter()
	 {
		DoGetDistance();

		if (!everyFrame)
		{
		  Finish();
		}
	 }
	 public override void OnUpdate()
	 {
		DoGetDistance();
	 }

	 void DoGetDistance()
	 {
		storeResult.Value = Vector3.Distance(vecto1.Value, vecto2.Value);
	 }

  }
}