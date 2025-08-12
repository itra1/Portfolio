using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.ActorController
{
  [HutongGames.PlayMaker.Tooltip("Актор контроллер. Базовый класс")]
  public class ActorDriver : FsmStateAction
  {
	 [DisplayOrder(0)]
	 public FsmOwnerDefault gameObject;
	 protected com.ootii.Actors.ActorController _actor;
	 protected GameObject _go;



	 [Tooltip("Скорость движения")]
	 public FsmFloat movementSpeed = 5f;

	 [Tooltip("Скорость вращения")]
	 public FsmFloat rotationSpeed = 240f;


	 public FsmFloat _jumpForce = 10f;
	 public virtual float JumpForce
	 {
		get { return _jumpForce.Value; }
		set { _jumpForce = value; }
	 }

	 public override void Awake()
	 {
		base.Awake();

		//_go = Fsm.GetOwnerDefaultTarget(gameObject);
		//_actor = _go.GetComponent<com.ootii.Actors.ActorController>();

	 }
	 public override void OnEnter()
	 {
		base.OnEnter();
		Inicialization();
	 }

	 public virtual void Inicialization()
	 {
		if (_go == null)
		{
		  _go = Fsm.GetOwnerDefaultTarget(gameObject);
		  _actor = _go.GetComponent<com.ootii.Actors.ActorController>();
		}
	 }


	 protected virtual void CalcRotation(Vector3 target, float speedRotation, ref Quaternion rRotate)
	 {
		Vector3 lDirection = target - _go.transform.position;
		lDirection.Normalize();

		Vector3 lVerticalDirection = Vector3.Project(lDirection, _go.transform.up);
		Vector3 lLateralDirection = lDirection - lVerticalDirection;

		float lYawAngle = Vector3Ext.SignedAngle(_go.transform.forward, lLateralDirection);

		if (speedRotation == 0f)
		{
		  rRotate = Quaternion.AngleAxis(lYawAngle, _go.transform.up);
		}
		else
		{
		  rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), speedRotation * Time.deltaTime), _go.transform.up);
		}

	 }

	 protected virtual void CalcMove(Vector3 target, float speed, ref Vector3 rMove, ref Quaternion rRotate)
	 {
		float lMoveSpeed = speed;
		Quaternion lFutureRotation = _go.transform.rotation * rRotate;
		rMove = lFutureRotation.Forward() * (lMoveSpeed * Time.deltaTime);
	 }


  }
}