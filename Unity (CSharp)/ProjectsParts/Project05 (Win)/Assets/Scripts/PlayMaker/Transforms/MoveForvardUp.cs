using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Transforms
{
  public class MoveForvardUp : FsmStateAction
  {
	 public FsmGameObject _gameObject;
	 public FsmFloat _forwardSpeed = 1;
	 public FsmFloat _upSpeed = 1;

	 public override void OnUpdate()
	 {
		_gameObject.Value.transform.position += ((_forwardSpeed.Value * _gameObject.Value.transform.forward) + (_upSpeed.Value * Vector3.up)) * Time.deltaTime;
	 }
  }
}