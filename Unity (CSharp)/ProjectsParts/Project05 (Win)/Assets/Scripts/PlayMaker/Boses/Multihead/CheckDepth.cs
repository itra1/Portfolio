using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using com.ootii.Geometry;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Boses.MultiHead
{
  public class CheckDepth : FsmStateAction
  {
	 public FsmFloat maxSurfaceTest;

    [ObjectType(typeof(Operation))]
    public FsmEnum operation;
    public FsmFloat value;

    [UIHint(UIHint.Layer)]
    [Tooltip("Pick only from these layers.")]
    public FsmInt[] waterLayerMask;

    public FsmEvent OnSuccess;
    public FsmEvent OnFail;

    public enum Operation
    {
      equal,
      less,
      great,
      lessOrEquals,
      greatOrQuals
    }

	 public override void OnEnter()
    {
      float depth = GetDepth();

      switch ((Operation)operation.Value)
      {
        case Operation.greatOrQuals:
          if (depth >= value.Value)
            Fsm.Event(OnSuccess);
          else
            Fsm.Event(OnFail);
          break;
        case Operation.lessOrEquals:
          if (depth >= value.Value)
            Fsm.Event(OnSuccess);
          else
            Fsm.Event(OnFail);
          break;
        case Operation.great:
          if (depth >= value.Value)
            Fsm.Event(OnSuccess);
          else
            Fsm.Event(OnFail);
          break;
        case Operation.less:
          if (depth >= value.Value)
            Fsm.Event(OnSuccess);
          else
            Fsm.Event(OnFail);
          break;
        case Operation.equal:
        default:
          if (depth >= value.Value)
            Fsm.Event(OnSuccess);
          else
            Fsm.Event(OnFail);
          break;

      }
      Finish();
    }

	 private float GetDepth()
    {
      float lDepth = -1f;

      RaycastHit lHitInfo;
      Vector3 lStart = Owner.transform.position + (Vector3.up * maxSurfaceTest.Value);

      if (RaycastExt.SafeRaycast(lStart, Vector3.down, out lHitInfo, maxSurfaceTest.Value, ActionHelpers.LayerArrayToLayerMask(waterLayerMask, false), Owner.transform, null, false))
      {
        Transform surface = lHitInfo.collider.gameObject.transform;
        if (surface != null)
        {
          lDepth = surface.position.y - Owner.transform.position.y;
        }
      }

      return lDepth;
    }

  }

}