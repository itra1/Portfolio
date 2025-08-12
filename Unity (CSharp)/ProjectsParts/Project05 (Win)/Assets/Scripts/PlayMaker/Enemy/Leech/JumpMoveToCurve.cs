using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Timing;
using com.ootii.Geometry;
using it.Game.Environment.Level5.Leech;
using DG.Tweening;

namespace it.Game.PlayMaker.ActorController
{

  [ActionCategory("Enemy")]
  [HutongGames.PlayMaker.Tooltip("Актор контроллер. Движение вдоль кривой")]
  public class JumpMoveToCurve : ActorDriver
  {
	 public FsmEvent OnComplete;

	 [RequiredField] public FsmGameObject Source;
	 [RequiredField] public FsmGameObject Tagret;

	 private Animator _animator;
	 private FluffyUnderware.Curvy.CurvySpline _curve;
	 private FluffyUnderware.Curvy.Controllers.SplineController _splineController;
	 private bool _forvardCurve;
	 private bool _initPath;
	 private bool _isMove;

	 public override void Awake()
	 {
		base.Awake();
		Fsm.HandleLateUpdate = true;
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();

		if (_animator == null)
		  _animator = _go.GetComponent<Animator>();

		_isMove = false;

		FindCurve();
		CreateController();

		_animator.SetInteger("Form", 1);
		_animator.SetFloat("Forvard", 0);

		DOVirtual.DelayedCall(0.5f, () =>
		{
		  _splineController.Play();
		  _animator.SetInteger("Form", 0);
		  _animator.SetFloat("Forvard", 0);
		  _isMove = true;
		});

		_actor.IsGravityEnabled = false;
		_actor.ForceGrounding = false;
	 }

	 public override void OnExit()
	 {
		base.OnExit();
		_actor.IsGravityEnabled = true;
		_actor.ForceGrounding = true;
	 }

	 public override void OnLateUpdate()
	 {
		base.OnLateUpdate();

		if(_isMove)
		  Move();
	 }

	 private void Move()
	 {
		Vector3 lDirection = Tagret.Value.transform.position - _go.transform.position;
		Quaternion lRotation = Quaternion.identity;
		Vector3 lVerticalDirection = Vector3.Project(lDirection, _go.transform.up);
		Vector3 lLateralDirection = lDirection - lVerticalDirection;

		float lYawAngle = Vector3Ext.SignedAngle(_go.transform.forward, lLateralDirection);

		lRotation = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), rotationSpeed.Value * Time.deltaTime), _go.transform.up);

		Vector3 lMovement = (_splineController.transform.position - _actor.Transform.position).normalized * movementSpeed.Value * Time.deltaTime;
		//Quaternion lRotation = Quaternion.identity;
		//CalcMove(_splineController.transform.position, movementSpeed.Value, ref lMovement, ref lRotation);

		_actor.Move(lMovement);
		_actor.Rotate(lRotation);

	 }

	 private void FindCurve()
	 {
		RoadPoint sourceRoadPoint = Source.Value.GetComponent<RoadPoint>();
		RoadPoint targetRoadPoint = Tagret.Value.GetComponent<RoadPoint>();

		bool exists = false;

		for(int i = 0; i < sourceRoadPoint.ContactPoints.Count; i++)
		{
		  if (sourceRoadPoint.ContactPoints[i].Point == targetRoadPoint
			 && (sourceRoadPoint.ContactPoints[i].Contact & RoadContacType.jump) != 0)
		  {
			 _curve = sourceRoadPoint.ContactPoints[i].Path;
			 _forvardCurve = true;
			 exists = true;
		  }

		}

		if (!exists)
		{

		  for (int i = 0; i < targetRoadPoint.ContactPoints.Count; i++)
		  {
			 if (targetRoadPoint.ContactPoints[i].Point == sourceRoadPoint
				&& (targetRoadPoint.ContactPoints[i].Contact & RoadContacType.jump) != 0)
			 {
				_forvardCurve = false;
				_curve = targetRoadPoint.ContactPoints[i].Path;
			 }

		  }
		}

	 }

	 private void CreateController()
	 {
		GameObject splipeMover = new GameObject();
		splipeMover.SetActive(false);
		_splineController = splipeMover.AddComponent<FluffyUnderware.Curvy.Controllers.SplineController>();
		_splineController.UpdateIn = FluffyUnderware.Curvy.CurvyUpdateMethod.Update;
		_splineController.Spline = _curve;
		_splineController.PositionMode = FluffyUnderware.Curvy.CurvyPositionMode.Relative;
		_splineController.Position = 0;
		_splineController.MoveMode = FluffyUnderware.Curvy.Controllers.CurvyController.MoveModeEnum.AbsolutePrecise;
		_splineController.Speed = movementSpeed.Value;
		_splineController.MovementDirection = _forvardCurve
		  ? FluffyUnderware.Curvy.Controllers.MovementDirection.Forward
		  : FluffyUnderware.Curvy.Controllers.MovementDirection.Backward;
		_splineController.Clamping = FluffyUnderware.Curvy.CurvyClamping.Clamp;
		_splineController.PlayAutomatically = false;
		_splineController.ConnectionBehavior = FluffyUnderware.Curvy.Controllers.SplineControllerConnectionBehavior.CurrentSpline;
		_splineController.OrientationMode = FluffyUnderware.Curvy.OrientationModeEnum.Orientation;
		_splineController.OrientationAxis = FluffyUnderware.Curvy.OrientationAxisEnum.Up;
		_splineController.OnInitialized.AddListener((arg1) =>
		{
		  _initPath = true;
		  //_splineController.Play();
		});
		_splineController.OnEndReached.AddListener((arg1) =>
		{

		  MonoBehaviour.Destroy(_splineController.gameObject);

		  Fsm.Event(OnComplete);
		});
		splipeMover.SetActive(true);
	 }


  }
}