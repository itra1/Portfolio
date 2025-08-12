using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RootMotion.FinalIK;

namespace it.Game.PlayMaker.FinalIK
{
  public class SetLookAtTargetPlayer : FsmStateAction
  {

    private LookAtIK _lookAt;

    public override void Awake()
    {
      base.Awake();
      _lookAt = Owner.GetComponent<LookAtIK>();
    }

    public override void OnEnter()
    {
      base.OnEnter();
      if(it.Game.Player.PlayerBehaviour.Instance != null)
      _lookAt.solver.target = it.Game.Player.PlayerBehaviour.Instance.transform;


    }

  }
}