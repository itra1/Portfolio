using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using JetBrains.Annotations;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.Helpers
{
  public class GetPositionRadiusNM : FsmStateAction
  {
    public FsmOwnerDefault gameObject;
    public FsmGameObject target;
    public FsmFloat radius;
    public FsmVector3 _targetPosition;

    private UnityEngine.AI.NavMeshAgent _agent;

    public override void OnEnter()
    {
      base.OnEnter();
      if (_agent == null)
        _agent = Owner.GetComponent<UnityEngine.AI.NavMeshAgent>();

      FindNewPosition();

      Finish();
    }

    private void FindNewPosition()
    {
      GameObject owr = Fsm.GetOwnerDefaultTarget(gameObject);

      for (int i = 0; i < 20; i++)
      {
        Vector3 point = ((owr.transform.position - target.Value.transform.position).normalized * radius.Value) + target.Value.transform.position;

        Vector3 dir = Owner.transform.position - point;
        Vector3 dirNorm = dir.normalized;

        UnityEngine.AI.NavMeshPath _path = new UnityEngine.AI.NavMeshPath();
        _agent.CalculatePath(point, _path);
        if (_path.status != UnityEngine.AI.NavMeshPathStatus.PathComplete)
          continue;


        //if ((_path.corners[_path.corners.Length - 1] - point).magnitude > 0.1f)
        //  continue;


        _targetPosition.Value = _path.corners[_path.corners.Length - 1];
        break;

      };
    }
  }
}