using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RootMotion.FinalIK;

namespace it.Game.PlayMaker.FinalIK
{
  [HutongGames.PlayMaker.Tooltip("Установка целе осмотра")]
  public class SetLookTarget : FsmStateAction
  {
    [HutongGames.PlayMaker.RequiredField]
    public FsmOwnerDefault gameObject;
    public FsmGameObject _target;
    private LookAtIK _lookAt;
    public FsmEvent OnComplete;

    public override void OnEnter()
    {
      base.OnEnter();

      _lookAt = Fsm.GetOwnerDefaultTarget(gameObject).gameObject.GetComponentInChildren<LookAtIK>();

      if (_lookAt == null)
      {
        Fsm.Event(OnComplete);
        return;
      }

      _lookAt.solver.target = _target.Value.transform;

      Fsm.Event(OnComplete);
    }
  }
}