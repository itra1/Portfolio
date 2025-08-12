using UnityEngine;
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.Helpers
{
  public class CheckTargetOutRegoin : FsmStateAction
  {
    public FsmOwnerDefault gameObject;
    private GameObject _go;
    private Collider _regionCollider;

    public FsmEvent _targetOut;

    public FsmGameObject TargetObject;
    public FsmVector3 TargetPoint;
    public FsmGameObject _region;
    [UIHint(UIHint.Layer)]
    [Tooltip("Pick only from these layers.")]
    public FsmInt[] layerMask;

    public override void Awake()
    {
      _regionCollider = _region.Value.GetComponent<Collider>();
    }

    public override void OnEnter()
    {
      base.OnEnter();
      if(_go == null)
        _go = Fsm.GetOwnerDefaultTarget(gameObject);
    }
    public override void OnUpdate()
    {
      base.OnUpdate();

      Vector3 point;

      if (TargetObject.Value != null)
        point = TargetObject.Value.transform.position;
      else
        point = TargetPoint.Value;

      point = _go.transform.position + _go.transform.forward * 2;

      Vector3 dir = _go.transform.position - point;
      Vector3 dirNorm = dir.normalized;

      RaycastHit _hit;
      if (RaycastExt.SafeRaycast(point, dirNorm, out _hit, dir.magnitude))
      {
        if (_hit.collider.Equals(_regionCollider))
          Fsm.Event(_targetOut);
      }
    }

  }
}