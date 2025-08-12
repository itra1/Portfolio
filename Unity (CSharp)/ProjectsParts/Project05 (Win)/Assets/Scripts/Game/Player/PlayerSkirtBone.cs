using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

namespace it.Game.Player
{
  public class PlayerSkirtBone : MonoBehaviourBase
  {
	 public FullBodyBipedIK ik;

	 public DataBone[] _dataBones;
	 public DataMiddleBone[] _dataMiddleBone;
	 public BoneLine[] _bones;


	 private void Start()
	 {
		ik.solver.OnPostUpdate = () =>
	  {
		 for (int i = 0; i < _dataBones.Length; i++)
		 {
			if (_dataBones[i].targetBone != null && _dataBones[i].sourceBone != null && _dataBones[i].active)
			{
			  _dataBones[i].targetBone.rotation = _dataBones[i].sourceBone.rotation * Quaternion.Euler(_dataBones[i].offset);
			}
		 }
		 //for (int i = 0; i < _dataMiddleBone.Length; i++)
		 //{
		 //if (_dataMiddleBone[i].targetBone != null && _dataMiddleBone[i].sourceBone1 != null && _dataMiddleBone[i].sourceBone2 != null && _dataMiddleBone[i].active)
		 //{
		 //  _dataMiddleBone[i].targetBone.rotation =Quaternion.Slerp(_dataMiddleBone[i].sourceBone1.rotation, _dataMiddleBone[i].sourceBone2.rotation,.5f);
		 //  _dataMiddleBone[i].targetBone.rotation *= Quaternion.Euler(_dataMiddleBone[i].offset);
		 //}
		 //}
		 for (int i = 0; i < _dataMiddleBone.Length; i++)
		 {
			if (_dataMiddleBone[i].targetBone != null && _dataMiddleBone[i].sourceBone1 != null && _dataMiddleBone[i].sourceBone2 != null && _dataMiddleBone[i].active)
			{
			  _dataMiddleBone[i].target.position = Vector3.Lerp(_dataMiddleBone[i].sourceBone1.position, _dataMiddleBone[i].sourceBone2.position, .5f);
			  _dataMiddleBone[i].target.rotation = Quaternion.LookRotation((transform.position - _dataMiddleBone[i].targetBone.position), _dataMiddleBone[i].target.up);
			 // _dataMiddleBone[i].targetBone.LookAt(pos + new Vector3(_dataMiddleBone[i].target.right.x * _dataMiddleBone[i].forvartOffset.x,
			 // _dataMiddleBone[i].target.up.y * _dataMiddleBone[i].forvartOffset.y,
			 // _dataMiddleBone[i].target.forward.z * _dataMiddleBone[i].forvartOffset.z)
			 //, _dataMiddleBone[i].targetBone.up);
			 // _dataMiddleBone[i].targetBone.rotation *= Quaternion.Euler(_dataMiddleBone[i].rotateOffset);
			  _dataMiddleBone[i].targetBone.LookAt(_dataMiddleBone[i].target.position + new Vector3(_dataMiddleBone[i].target.right.x * _dataMiddleBone[i].forvartOffset.x,
				 _dataMiddleBone[i].target.up.y * _dataMiddleBone[i].forvartOffset.y,
				 _dataMiddleBone[i].target.forward.z * _dataMiddleBone[i].forvartOffset.z)
				, _dataMiddleBone[i].target.up);
			  _dataMiddleBone[i].targetBone.rotation *= Quaternion.Euler(_dataMiddleBone[i].rotateOffset);
			}
		 }


	  };
	 }

	 private void OnDrawGizmosSelected()
	 {

		for (int i = 0; i < _bones.Length; i++)
		{
		  if (_bones[i].sourceBone == null)
			 continue;

		  if (_bones[i].childrenBones == null || _bones[i].childrenBones.Length == 0)
			 _bones[i].childrenBones = _bones[i].sourceBone.GetComponentsInChildren<Transform>();

		  if (_bones[i].childrenBones.Length > 0)
		  {
			 for (int ii = 0; ii < _bones[i].childrenBones.Length; ii++)
			 {
				if (ii == _bones[i].childrenBones.Length - 1)
				  continue;

				Gizmos.DrawLine(_bones[i].childrenBones[ii].position, _bones[i].childrenBones[ii + 1].position);
			 }
		  }

		}
		Gizmos.color = Color.red;
		for (int i = 0; i < _dataBones.Length; i++)
		{
		  if (_dataBones[i].targetBone != null && _dataBones[i].sourceBone != null && _dataBones[i].active)
		  {
			 Gizmos.DrawWireSphere(_dataBones[i].targetBone.position, 0.01f);
		  }
		}

		Gizmos.color = Color.green;
		for (int i = 0; i < _dataMiddleBone.Length; i++)
		{
		  if (_dataMiddleBone[i].targetBone != null && _dataMiddleBone[i].active)
		  {
			 Vector3 pos = Vector3.Lerp(_dataMiddleBone[i].sourceBone1.position, _dataMiddleBone[i].sourceBone2.position, .5f);
			 Gizmos.DrawLine(_dataMiddleBone[i].targetBone.position, pos 
				+ new Vector3(_dataMiddleBone[i].target.right.x * _dataMiddleBone[i].forvartOffset.x,
				 _dataMiddleBone[i].target.up.y * _dataMiddleBone[i].forvartOffset.y,
				 _dataMiddleBone[i].target.forward.z * _dataMiddleBone[i].forvartOffset.z));
			 Gizmos.DrawWireSphere(_dataMiddleBone[i].targetBone.position, 0.01f);
		  }
		}
	 }

	 //private void LateUpdate()
	 //{

	 //  _bone.rotation = ik.references.leftThigh.rotation * Quaternion.Euler(_rotate);
	 //}

	 [System.Serializable]
	 public struct DataBone
	 {
		public Transform sourceBone;
		public Transform targetBone;
		public bool active;
		public Vector3 offset;
	 }
	 [System.Serializable]
	 public struct DataMiddleBone
	 {
		public Transform sourceBone1;
		public Transform sourceBone2;
		public Transform targetBone;
		public Transform target;
		public bool active;
		public Vector3 rotateOffset;
		public Vector3 forvartOffset;
	 }
	 [System.Serializable]
	 public struct BoneLine
	 {
		public Transform sourceBone;
		[HideInInspector]
		public Transform[] childrenBones;
	 }

  }
}