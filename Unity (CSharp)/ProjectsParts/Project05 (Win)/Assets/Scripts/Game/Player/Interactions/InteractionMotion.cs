using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using com.ootii.Graphics;
using RaycastExt = Utilites.Geometry.RaycastExt;

namespace it.Game.Player.Interactions
{

  public class InteractionMotion : MonoBehaviourBase
  {
	 public MotionType Motion { get => _motion; set => _motion = value; }
	 public FullBodyBipedIK Ik { get => _ik; set => _ik = value; }

	 private InteractionTarget[] _iterationsTarget;

	 [SerializeField]
	 private MotionType _motion;

	 private FullBodyBipedIK _ik;
	 private float _weight = 0;

	 private bool _isInteract = false;

	 private void Awake()
	 {
	 }

	 public void Initiate(FullBodyBipedIK ik)
	 {
		this.Ik = ik;
		_iterationsTarget = GetComponentsInChildren<InteractionTarget>();
		_weight = 0;

		//for(int i = 0; i < _iterationsTarget.Length; i++)
		//{

		//}
	 }

	 public void FixHeight(int layers )
	 {
		RaycastHit RaycastHit = RaycastExt.EmptyHitInfo;
		for (int i = 0; i < _iterationsTarget.Length; i++)
		{
		  if(RaycastExt.SafeRaycast(_iterationsTarget[i].transform.parent.transform.position + Vector3.up * 0.1f, -Vector3.up, out RaycastHit, 0.21f, layers))
		  {
			 _iterationsTarget[i].transform.parent.transform.position = RaycastHit.point;
#if UNITY_EDITOR
			 GraphicsManager.DrawLine(_iterationsTarget[i].transform.parent.transform.position, _iterationsTarget[i].transform.parent.transform.position + RaycastHit.normal, Color.black, null, 60f);
#endif

			 //_iterationsTarget[i].transform.parent.rotation = Quaternion.identity * Quaternion.Euler(RaycastHit.normal.x,0,RaycastHit.normal.z);
		  }

		}
	 }

	 public void DeInitiate()
	 {
		this.Ik = null;
	 }

	 public void SetWeight(float weight)
	 {
		this._weight = weight;

		if(!_isInteract && this._weight > 0)
		{
		  _isInteract = true;
		}
		if (_isInteract && this._weight <= 0)
		{
		  _isInteract = false;
		  ConfirmWeight();
		}
	 }

	 private void LateUpdate()
	 {
		if (this.Ik == null || this._weight <= 0)
		  return;

		ConfirmWeight();
	 }

	 private void ConfirmWeight()
	 {

		for (int i = 0; i < _iterationsTarget.Length; i++)
		{
		  var effector = Ik.solver.GetEffector(_iterationsTarget[i].effectorType);
		  effector.position = _iterationsTarget[i].transform.position;
		  effector.rotation = _iterationsTarget[i].transform.rotation;
		  effector.positionWeight = _weight;
		  effector.rotationWeight = _weight;

		  InteractionTargetPoserMotion ipm = _iterationsTarget[i].GetComponent<InteractionTargetPoserMotion>();
		  ipm.sourceBoneRoot = effector.bone;
		  ipm.weight = _weight;
		}
	 }

	 public enum MotionType
	 {
		climbing_2_5,
		climbing_1_8,
		climbing_1,
		climbing_0_5,
		ladderStoneLeft,
		ladderStoneRight,
		ladderStandartLeft,
		ladderStandartRight
	 }
  }
}