using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Timing;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory(ActionCategory.Character)]
  [HutongGames.PlayMaker.Tooltip("Актор контроллер. Полет в заданную координату")]
  public class FlyingToPosition : ActorDriver
  {

    public FsmEvent onMoveComplete;

    public FsmVector3 _targetPoint;
    public FsmVector3 _offset;

    public FsmFloat _StopDistance = new FsmFloat(0.2f);
    public float StopDistance
    {
      get { return _StopDistance.Value; }
      set { _StopDistance = value; }
    }


    protected Vector3 mAgentDestination = Vector3.zero;
    protected Vector3 mTargetVector = Vector3.zero;

    protected float mTargetDistance = 0f;

    public override void OnEnter()
    {
      base.OnEnter();
      _actor.IsGravityEnabled = false;
      _actor.ForceGrounding = false;
      _actor.FixGroundPenetration = false;
    }

    public override void OnUpdate()
    {
      base.OnUpdate();
      Vector3 lMovement = Vector3.zero;
      Quaternion lRotation = Quaternion.identity;


      mTargetVector = _targetPoint.Value - _go.transform.position;
      mTargetDistance = mTargetVector.magnitude;

      if (mTargetDistance < _StopDistance.Value)
      {
        //ClearTarget();
        //_hasArrived = true;

        OnArrived();
      }


      CalculateMove(_targetPoint.Value + _offset.Value, ref lMovement, ref lRotation);

      _actor.Move(lMovement);
      _actor.Rotate(lRotation);

      if (((_targetPoint.Value + _offset.Value) - _go.transform.position).magnitude <= 0.1f)
      {
      }
    }

    protected virtual void OnArrived()
    {
      EmitComplete();
    }

    protected virtual void EmitComplete()
    {
      Fsm.Event(onMoveComplete);
    }

    protected virtual void CalculateMove(Vector3 rWaypoint, ref Vector3 rMove, ref Quaternion rRotate)
    {
      float lDeltaTime = TimeManager.SmoothedDeltaTime;

      // Direction we need to travel in
      Vector3 lDirection = rWaypoint -_go.transform.position;
      lDirection.Normalize();

      // Determine our rotation
      Vector3 lVerticalDirection = Vector3.Project(lDirection, _go.transform.up);
      Vector3 lLateralDirection = lDirection - lVerticalDirection;

      float lYawAngle = Vector3Ext.SignedAngle(_go.transform.forward, lLateralDirection);

      if (rotationSpeed.Value == 0f)
      {
        rRotate = Quaternion.AngleAxis(lYawAngle, _go.transform.up);
      }
      else
      {
        rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), rotationSpeed.Value * lDeltaTime), _go.transform.up);
      }


      // Set the final velocity based on the future rotation
      Quaternion lFutureRotation = _go.transform.rotation * rRotate;
      rMove = (lFutureRotation.Forward() + lVerticalDirection) * (movementSpeed.Value * lDeltaTime);

    }
  }
}