using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using com.ootii.Actors;
using com.ootii.Timing;
using com.ootii.Geometry;
using com.ootii.Timing;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  public class VerticalMoveToHeight : MoveVectical
  {
    public FsmEvent onComplete;
    public FsmEvent onFailed;
    public FsmBool worldHieght = false;
    public FsmFloat height = 3f;
    public FsmFloat distanceStop = new FsmFloat(0.2f);
    private float _targetHeight;

    private float _beforeY;

    public override void OnEnter()
    {
      base.OnEnter();
      if (!worldHieght.Value)
        _targetHeight = _go.transform.position.y + height.Value;
      else
        _targetHeight = height.Value;

      isVerticalMove.Value = height.Value > 0;

		if (CheckMoveReady(height.Value))
		{
        Fsm.Event(onFailed);
      }
      _beforeY = _go.transform.position.y;

    }

	 public override void OnUpdate()
    {
      if (Mathf.Abs(_targetHeight - _beforeY) < Mathf.Abs(_targetHeight - _go.transform.position.y))
      {
        OnArrived();
        return;
      }
      base.OnUpdate();

      _beforeY = _go.transform.position.y;

    }

    private void OnArrived()
	 {
      Fsm.Event(onComplete);
	 }
  }
}