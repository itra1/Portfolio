using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Enemy.Golem
{
  [ActionCategory("Enemy")]
  public class CheckPlayerVisible : FsmStateAction
  {
	 public FsmOwnerDefault _gameObject;
	 public FsmEvent OnVisible;
	 public FsmEvent OnLost;
	 public FsmFloat _distanceVisible = 10;
	 public float _visibleAngleHorisontal = 45;
	 public float _visibleAngleVertical = 90;
	 public float _timeVisibleToReaction = 1;
	 public FsmVector3 _positionVisible;

	 private GameObject _go;
	 private float _timeVisible;
	 private bool _isVisible;

	 public override void Awake()
	 {
		_go = Fsm.GetOwnerDefaultTarget(_gameObject);

	 }

	 public override void OnEnter()
	 {
		base.OnEnter();
		_timeVisible = -1;
	 }

	 public override void OnUpdate()
	 {
		if (Player.PlayerBehaviour.Instance == null) return;

		base.OnUpdate();

		bool isHipBone = IsVisible(Player.PlayerBehaviour.Instance.HipBone);
		bool isHeadBone = IsVisible(Player.PlayerBehaviour.Instance.HeadBone);

		if(_timeVisible >= 0)
		{
		  if(!isHipBone && !isHeadBone)
		  {
			 if(_isVisible)
			 {
				Fsm.Event(OnLost);
				_isVisible = false;
			 }
			 _timeVisible = -1;
		  }
		}
		else
		{
		  if (isHipBone || isHeadBone)
		  {
			 _positionVisible = Player.PlayerBehaviour.Instance.transform.position;
			 if (_timeVisible < 0)
			 {
				_timeVisible = Time.timeSinceLevelLoad;
			 }
		  }
		}

		if(!_isVisible && (isHipBone || isHeadBone) && _timeVisible + _timeVisibleToReaction < Time.timeSinceLevelLoad)
		{
		  _isVisible = true;
		  Fsm.Event(OnVisible);
		}

	 }

	 private bool IsVisible(Transform target)
	 {
		Vector3 visibleVector = target.position - _go.transform.position;
		float distance = visibleVector.magnitude;

		if (distance > _distanceVisible.Value)
		  return false;

		float dotRes = Vector3.Dot(_go.transform.forward, visibleVector.normalized);
		if (dotRes < 0)
		  return false;

		Vector3 HorisontalForvard = _go.transform.forward - Vector3.Project(_go.transform.forward, _go.transform.up);
		Vector3 HorisontalVisible = visibleVector - Vector3.Project(visibleVector, _go.transform.up);

		float horisontalAngle = Vector3.Angle(HorisontalForvard, HorisontalVisible);
		if (horisontalAngle > _visibleAngleHorisontal / 2)
		  return false;

		Vector3 tempVForvard = _go.transform.forward * distance;
		tempVForvard.y = visibleVector.y;

		float verticalAngle = Vector3.Angle(HorisontalForvard, tempVForvard);
		if (verticalAngle > _visibleAngleVertical / 2)
		  return false;

		RaycastHit _hit;
		if(Utilites.Geometry.RaycastExt.SafeRaycast(_go.transform.position, visibleVector.normalized, out _hit, _distanceVisible.Value, -1, _go.transform)){

		  Player.PlayerBehaviour pb = _hit.collider.GetComponent<Player.PlayerBehaviour>();
		  if (pb != null)
			 return true;

		  pb = _hit.collider.GetComponentInParent<Player.PlayerBehaviour>();
		  if (pb != null)
			 return true;
		}

		return false;

	 }


  }
}