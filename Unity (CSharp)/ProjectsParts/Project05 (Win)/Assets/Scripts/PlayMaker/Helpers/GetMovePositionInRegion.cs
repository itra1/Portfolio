using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using JetBrains.Annotations;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.Helpers
{
  public class GetMovePositionInRegion : FsmStateAction
  {
    public FsmGameObject _region;

    public FsmVector3 _targetPosition;

    public FsmFloat minDistance = new FsmFloat(2);
    public FsmFloat maxDistance = new FsmFloat(5);

    [UIHint(UIHint.Layer)]
    [Tooltip("Pick only from these layers.")]
    public FsmInt[] layerMask;
    private UnityEngine.AI.NavMeshAgent _agent;


    public override void OnEnter()
    {
      base.OnEnter();
      if ( _agent == null)
      {
        _agent = Owner.GetComponent<UnityEngine.AI.NavMeshAgent>();
      }
      Collider collider = _region.Value.GetComponent<Collider>();

      FindNewPosition();

      // collider.transform.position

      Finish();
    }

    private void FindNewPosition()
    {

      for (int i = 0; i < 20; i++)
      {
        Vector3 point = Owner.transform.position
          + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * Random.Range(minDistance.Value, maxDistance.Value);
        Vector3 dir = Owner.transform.position - point;
        Vector3 dirNorm = dir.normalized;


        //correct = RaycastExt.SafeCircularCast(point, dirNorm, transform.up, out _hit, dir.magnitude, 30, _layerRegion);
        if (RaycastExt.SafeRaycast(point, dirNorm, dir.magnitude, ActionHelpers.LayerArrayToLayerMask(layerMask, false)))
        {
          continue;
        }

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