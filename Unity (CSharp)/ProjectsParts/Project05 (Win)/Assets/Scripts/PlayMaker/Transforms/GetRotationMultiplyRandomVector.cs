using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Transforms
{
  public class GetRotationMultiplyRandomVector : FsmStateAction
  {
	 public FsmGameObject _gameObject;
	 public FsmQuaternion _resultQuaternion;
	 public FsmFloat _angles;


	 public override void OnEnter()
	 {
		_resultQuaternion.Value = _gameObject.Value.transform.rotation * Quaternion.Euler(0,Random.Range(-_angles.Value, _angles.Value),0);
		Finish();
	 }
  }
}