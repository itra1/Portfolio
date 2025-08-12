using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using com.ootii.Geometry;

#if UNITY_EDITOR

using EHandles = UnityEditor.Handles;

#endif

namespace it.Game.PlayMaker.ActorController
{
  public class PlayerCheckInRadius : FsmStateAction
  {
    public FsmOwnerDefault _gameObject;
    
    public FsmGameObject _target;
    public FsmFloat _radius;
    public FsmVector3 _offset = new FsmVector3(Vector3.up);

    public FsmEvent onCheckEvent;
    public FsmEvent offCheckEvent;

    public FsmBool _forvardVisible;

    public FsmBool _checkHeight;

    public FsmFloat _heightDelta = new FsmFloat(3);

    private GameObject go;


    public override void OnEnter()
    {
      base.OnEnter();

      go = Fsm.GetOwnerDefaultTarget(_gameObject);

    }

    public override void OnUpdate()
    {
      base.OnUpdate();

      if (_target.Value == null)
        return;


      if(_checkHeight.Value && Mathf.Abs(_target.Value.transform.position.y - go.transform.position.y) > _heightDelta.Value)
      {
        Fsm.Event(offCheckEvent);
        return;
      }

      bool isRound = 
      (_target.Value.transform.position - go.transform.position).magnitude < _radius.Value;

      if (!isRound)
      {
        Fsm.Event(offCheckEvent);
        return;
      }

      if (!_forvardVisible.Value)
      {
        Fsm.Event(onCheckEvent);
        return;
      }

      RaycastHit hit;

      if (!RaycastExt.SafeRaycast(go.transform.position + _offset.Value,
        _target.Value.transform.position - go.transform.position,
        out hit, _radius.Value))
        return;

      if (hit.collider.gameObject.Equals(_target.Value.gameObject))
      {
        Fsm.Event(onCheckEvent);
        return;
      }



    }

  }
}