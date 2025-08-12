using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIK : MonoBehaviour {

	public Animator anim;
	private Vector3 _leftFootPos;
	private Vector3 _rightFootPos;
	private Quaternion _leftFootRoot;
	private Quaternion _rightFootRoot;
	private float _leftFootWeight;
	private float _rightFootWeight;

	private Transform _leftFoot;
	private Transform _rightFoot;

	public float offsetY;

	public float lookIKWeight;
	public float eyesWeight;
	public float headWeight;
	public float bodyWeight;
	public float clampWeigth;
	public Transform targetPos;

	private void Start() {

		_leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
		_leftFootRoot = _leftFoot.rotation;
		_rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
		_rightFootRoot = _rightFoot.rotation;

	}

	private void Update() {

		RaycastHit leftHit;
		Vector3 lpos = _leftFoot.position;
		if (Physics.Raycast(lpos + Vector3.up * 5f, Vector3.down * 15, out leftHit, 8)) {
			_leftFootPos = Vector3.Lerp(lpos, leftHit.point + Vector3.up * offsetY, Time.deltaTime * 10f);
			_leftFootRoot = Quaternion.FromToRotation(transform.up, leftHit.normal) * transform.rotation;
			Debug.DrawLine(lpos,_leftFootPos);
		}

		RaycastHit rightHit;
		Vector3 rpos = _leftFoot.position;
		if (Physics.Raycast(rpos + Vector3.up * 5f, Vector3.down*15, out rightHit, 8)) {
			_rightFootPos = Vector3.Lerp(rpos, rightHit.point + Vector3.up * offsetY, Time.deltaTime * 10f);
			_rightFootRoot = Quaternion.FromToRotation(transform.up, rightHit.normal) * transform.rotation;
			Debug.DrawLine(rpos, _rightFootPos);
		}

	}

	private void OnAnimatorIK(int layerIndex) {

		anim.SetLookAtWeight(lookIKWeight,bodyWeight,headWeight,eyesWeight,clampWeigth);
		anim.SetLookAtPosition(targetPos.position);

		//_leftFootWeight = anim.GetFloat("LeftFoot");
		_leftFootWeight = 0.5f;

		anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _leftFootWeight);
		anim.SetIKPosition(AvatarIKGoal.LeftFoot, _leftFootPos);

		anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _leftFootWeight);
		anim.SetIKRotation(AvatarIKGoal.LeftFoot, _leftFootRoot);

		//_rightFootWeight = anim.GetFloat("RightFoot");
		_rightFootWeight = 0.5f;

		anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, _rightFootWeight);
		anim.SetIKPosition(AvatarIKGoal.RightFoot, _rightFootPos);

		anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, _rightFootWeight);
		anim.SetIKRotation(AvatarIKGoal.RightFoot, _rightFootRoot);

	}
	
}
