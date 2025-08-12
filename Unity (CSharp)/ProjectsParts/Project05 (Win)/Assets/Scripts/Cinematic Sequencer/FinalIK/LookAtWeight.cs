using UnityEngine;
using System.Collections;
using com.ootii.Actors;
using RootMotion.FinalIK;

namespace Slate.ActionClips.ActorControllers
{
  [Category("Final IK")]
  public class LookAtWeight : ActorActionClip
  {
	 [SerializeField]
	 [HideInInspector]
	 private float _length = 1;

	 public float ikWeight;
	 public EaseType interpolation = EaseType.QuadraticInOut;
	 public Transform _target;

	 private float _weightStart;
	 private LookAtIK _lookIK;

	 public override float length
	 {
		get { return _length; }
		set { _length = value; }
	 }
	 protected override void OnEnter()
	 {
		_lookIK = actor.GetComponentInChildren<LookAtIK>();
		_lookIK.solver.target = _target;
		_weightStart = _lookIK.solver.IKPositionWeight;
	 }

	 protected override void OnUpdate(float deltaTime)
	 {
		_lookIK.solver.IKPositionWeight = Easing.Ease(interpolation, _weightStart, ikWeight, deltaTime / length);
	 }
  }
}