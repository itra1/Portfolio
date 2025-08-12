using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine.AI;
using HutongGames.PlayMaker.Actions;
using com.ootii.Graphics;
using com.ootii.Geometry;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

#if UNITY_EDITOR

using EHandles = UnityEditor.Handles;

#endif

namespace it.Game.PlayMaker.ActorController
{
  public class GetRandomPositionInRadius : FsmStateAction
  {
    public FsmGameObject _objectCenter;
    public FsmVector3 radiusCenter;

    private Vector3 _center;

    public FsmVector3 targetMove;

    public FsmFloat _radius = new FsmFloat(3);

    public FsmBool _checkNavMesh;
    public FsmFloat heightRayTest = new FsmFloat(20);
    public FsmFloat rayDistance = new FsmFloat(200);

    [UIHint(UIHint.Layer)]
    [Tooltip("Pick only from these layers.")]
    public FsmInt[] layerMask;

    private NavMeshAgent _agent;
    NavMeshPath path;
    Vector3 newTargetPosition;

    public override void OnEnter()
    {
      base.OnEnter();

      if (_objectCenter.Value != null)
      {
        _center = _objectCenter.Value.transform.position;
      }else if (radiusCenter.Value != Vector3.zero)
      {
        _center = radiusCenter.Value;
      }else
        _center = Owner.transform.position;

      ///

      if (_objectCenter.Value != null)
        _center = _objectCenter.Value.transform.position;
      else
        _center = radiusCenter.Value;

      if (_checkNavMesh.Value && _agent == null)
      {
        _agent = Owner.GetComponent<NavMeshAgent>();
      }

      newTargetPosition = Owner.transform.position;

      GetNewPosition();

      if (_checkNavMesh.Value)
      {

        for (int i = 0; i < 20; i++)
        {

          path = new NavMeshPath();
          _agent.CalculatePath(newTargetPosition, path);

          if (path.status == NavMeshPathStatus.PathComplete)
          {
            newTargetPosition = path.corners[path.corners.Length - 1];
            break;
          }

          GetNewPosition();
        }
      }

      targetMove.Value = newTargetPosition;
      Finish();

    }

    private void GetNewPosition()
    {
      for(int i = 0; i < 20; i++)
      {
        Vector3 pos = _center + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * Random.Range(0, _radius.Value);

        RaycastHit _hit;
        if (RaycastExt.SafeRaycast(pos + Vector3.up * heightRayTest.Value, Vector3.down, out _hit, rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(layerMask, false)))
        {
          newTargetPosition = _hit.point;
        }else
          continue;

        if ((newTargetPosition - Owner.transform.position).magnitude < _radius.Value / 4)
          break;

      }

    }

    public override void OnDrawActionGizmos()
    {
      base.OnDrawActionGizmosSelected();
#if UNITY_EDITOR
      EHandles.DrawWireDisc(_center, Vector3.up, _radius.Value);

#endif


    }


  }
}