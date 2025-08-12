using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.FinalIK;

namespace it.Game.Player.Interactions
{
  public class InteractionTargetPoserMotion : SolverManager
  {
	 private bool initiated;
	 private Transform[] _poseBones;
	 [SerializeField]
	 public Transform sourceBoneRoot;
	 private Transform _boneRoot;
	 private Transform[] poseChildren;

	 [Range(0f, 1f)] public float localRotationWeight = 1f;

	 [Range(0f, 1f)] public float localPositionWeight = 1f;

	 /// <summary>
	 /// The master weight.
	 /// </summary>
	 [Range(0f, 1f)] public float weight = 1f;

	 /// <summary>
	 /// For manual update of the poser.
	 /// </summary>
	 public void UpdateManual()
	 {
		UpdatePoser();
	 }
	 protected override void UpdateSolver()
	 {
		if (!initiated) InitiateSolver();
		if (!initiated) return;

		UpdatePoser();
	 }

	 protected override void InitiateSolver()
	 {
		if (initiated) return;
		InitiatePoser();
		initiated = true;
	 }

	 protected override void FixTransforms()
	 {
		if (!initiated) return;
		FixPoserTransforms();
	 }
	 public void AutoMapping()
	 {
		if (sourceBoneRoot == null) poseChildren = new Transform[0];
		else poseChildren = (Transform[])sourceBoneRoot.GetComponentsInChildren<Transform>();
		_boneRoot = sourceBoneRoot;
	 }
	 private void UpdatePoser()
	 {
		if (weight <= 0f) return;

		if (_boneRoot != sourceBoneRoot) AutoMapping();

		if (_boneRoot == null)
		  return;

		float rW = localRotationWeight * weight;
		float pW = localPositionWeight * weight;

		for (int i = 0; i < poseChildren.Length; i++)
		{
		  if (poseChildren[i] != _boneRoot)
		  {
			 poseChildren[i].localRotation = Quaternion.Lerp(poseChildren[i].localRotation, _poseBones[i].localRotation, rW);
			 poseChildren[i].localPosition = Vector3.Lerp(poseChildren[i].localPosition, _poseBones[i].localPosition, pW);
		  }
		}
	 }
	 protected void InitiatePoser()
	 {
		_poseBones = (Transform[])GetComponentsInChildren<Transform>();

	 }

	 protected void FixPoserTransforms()
	 {

	 }

  }
}