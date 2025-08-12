using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using JetBrains.Annotations;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.Helpers
{
  public class ActivateNavMesh : FsmStateAction
  {
    public FsmOwnerDefault _gameObject;
    public FsmBool _activate;


    public override void OnEnter()
    {
      base.OnEnter();

      Fsm.GetOwnerDefaultTarget(_gameObject).GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = _activate.Value;

    }
  }
}