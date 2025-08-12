using UnityEngine;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.ActorController
{
  public class FlyingToWaypoints : FlyingToPosition
  {
	 [ArrayEditor(VariableType.GameObject)]
	 public FsmArray _wayPoints;
	 private int _index;

	 public FsmBool _loop;

	 public override void OnEnter()
	 {
		base.OnEnter();

		_index = -1;
		OnArrived();

	 }

	 protected override void OnArrived()
	 {
		_index++;

		if (_loop.Value && _index >= _wayPoints.Length)
		  _index = 0;

		if (_index >= _wayPoints.Length)
		  EmitComplete();
		else
		{

		  _targetPoint.Value = ((GameObject)_wayPoints.Values[_index]).transform.position;
		}
	 }
  }
}