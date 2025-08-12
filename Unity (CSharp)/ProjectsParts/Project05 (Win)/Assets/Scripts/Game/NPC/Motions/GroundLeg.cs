using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

namespace it.Game.NPC.Motions
{
  public class GroundLeg : MonoBehaviourBase
  {
	 private IK ik;
	 [SerializeField]
	 private Transform foot;
	 [SerializeField]
	 private Vector3 footUpAxis;

	 [SerializeField]
	 private Transform _toe;

	 private Quaternion lastFootLocalRotation;

	 Vector3 _preIkFootPosition;

	 void Awake()
	 {
		// Find the ik component
		ik = GetComponent<IK>();

		if (foot != null)
		{
		  if (footUpAxis == Vector3.zero) footUpAxis = Quaternion.Inverse(foot.rotation) * Vector3.up;
		  lastFootLocalRotation = foot.localRotation;
		  ik.GetIKSolver().OnPreUpdate += BeforaIK;
		  ik.GetIKSolver().OnPostUpdate += AfterIK;
		}
	 }

	 private void Start()
	 {
		
	 }

	 private void BeforaIK()
	 {
		if (foot == null) return;


		_preIkFootPosition = foot.position;

		if (foot.position.y < 0.1f)
		  ik.GetIKSolver().IKPositionWeight = 1;
		else
		  ik.GetIKSolver().IKPositionWeight = 0;
		_toe.position = ik.GetIKSolver().IKPosition + transform.up;


	 }
	 private void AfterIK()
	 {
		if (foot == null) return;


		ik.GetIKSolver().IKPosition = new Vector3(_preIkFootPosition.x, 0.1f, _preIkFootPosition.z) ;
		_toe.position = ik.GetIKSolver().IKPosition + transform.up;


	 }

  }

}