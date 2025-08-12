using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RootMotion.FinalIK;

namespace it.Game.PlayMaker.FinalIK
{

  public class SetLookAtWeight : FsmStateAction
  {
    public FsmOwnerDefault gameObject;
    public FsmFloat _value;

    public FsmEvent setComplate;

    private GameObject go;
    private LookAtIK _lookAt;
    public FsmBool _everyUpdate;

    public override void Awake()
    {
      base.Awake();
    }

    public override void OnEnter()
    {
      base.OnEnter();

      go = Fsm.GetOwnerDefaultTarget(gameObject);

      _lookAt = go.GetComponent<LookAtIK>();
      if (_lookAt == null)
        _lookAt = go.GetComponentInChildren<LookAtIK>();

      _lookAt.solver.IKPositionWeight = _value.Value;
      Fsm.Event(setComplate);
    }

    public override void OnUpdate()
    {
      base.OnUpdate();

      if (_everyUpdate.Value)
      {
        _lookAt.solver.IKPositionWeight = _value.Value;
        Fsm.Event(setComplate);
      }

    }

  }
}