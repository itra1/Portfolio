using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

namespace it.Game.Player
{
  public class PlayerSkirtIK : MonoBehaviourBase
  {
	 [Tooltip("Reference to the FBBIK componet.")]
	 public FullBodyBipedIK ik;

	 [SerializeField]
	 private bool _read = false;

	 [SerializeField]
	 private DataBone[] _dataBones = new DataBone[0];

	 void OnDrawGizmosSelected()
	 {
		if (ik == null) ik = GetComponent<FullBodyBipedIK>();
		if (ik == null) ik = GetComponentInParent<FullBodyBipedIK>();
		if (ik == null) ik = GetComponentInChildren<FullBodyBipedIK>();

	 }
	 [ContextMenu("Find")]
	 public void Find()
	 {

		for (int i = 0; i < _dataBones.Length; i++)
		{
		  _dataBones[i]._bodyPartNext = _dataBones[i]._bodyPart.GetComponentInChildren<Transform>();
		}
	 }
	 void OnDrawGizmos()
	 {
		Gizmos.color = Color.red;
		for (int i = 0; i < _dataBones.Length; i++)
		{
		  Gizmos.DrawWireSphere(_dataBones[i]._target.position, 0.01f);
		}
	 }
	 //private void OnAnimatorMove()
	 //{
		//for (int i = 0; i < _dataBones.Length; i++)
		//{
		//  //_dataBones[i]._target.position = DiffOffset(ik.transform, _dataBones[i]._bodyPart.position, _dataBones[i]._offset);
		//  _dataBones[i]._target.rotation = Quaternion.LookRotation(_dataBones[i]._bodyPartNext.position - _dataBones[i]._bodyPart.position, transform.forward);
		//  _dataBones[i]._target.position = AltOffset(_dataBones[i]._target, _dataBones[i]._bodyPart.position, _dataBones[i]._offsetQuaternion);
		//}
	 //}
	 //private void OnAnimatorIK(int layerIndex)
	 //{

	 //}
	 private void Update()
	 {
		if (_read)
		{
		  for (int i = 0; i < _dataBones.Length; i++)
		  {
			 if (_dataBones[i]._target == null || _dataBones[i]._bodyPart == null || _dataBones[i]._bodyPartNext == null)
				return;

			 //_dataBones[i]._target.position = DiffOffset(ik.transform, _dataBones[i]._bodyPart.position, _dataBones[i]._offset);
			 _dataBones[i]._target.rotation = Quaternion.LookRotation(_dataBones[i]._bodyPartNext.position - _dataBones[i]._bodyPart.position, transform.up);
			 Vector3 planar = _dataBones[i]._target.position - _dataBones[i]._bodyPart.position;
			 Vector3 planarZ = Vector3.Project(planar, _dataBones[i]._target.forward);
			 Vector3 planarY = Vector3.Project(planar, _dataBones[i]._target.up);
			 Vector3 planarX = Vector3.Project(planar, _dataBones[i]._target.right);
			 float modulZ = Mathf.Sign(Vector3.Dot(_dataBones[i]._target.forward, planarZ));
			 float modulY = Mathf.Sign(Vector3.Dot(_dataBones[i]._target.up, planarY));
			 float modulX = Mathf.Sign(Vector3.Dot(_dataBones[i]._target.right, planarX));


			 _dataBones[i]._offsetQuaternion = new Vector3(planarX.magnitude * modulX, planarY.magnitude * modulY, planarZ.magnitude * modulZ);
		  }
		  return;
		}


		for (int i = 0; i < _dataBones.Length; i++)
		{
		  if (_dataBones[i]._target == null || _dataBones[i]._bodyPart == null || _dataBones[i]._bodyPartNext == null)
			 return;

		  //_dataBones[i]._target.position = DiffOffset(ik.transform, _dataBones[i]._bodyPart.position, _dataBones[i]._offset);
		  _dataBones[i]._target.rotation = Quaternion.LookRotation(_dataBones[i]._bodyPartNext.position - _dataBones[i]._bodyPart.position, transform.up);
		  _dataBones[i]._target.position = AltOffset(_dataBones[i]._target, _dataBones[i]._bodyPart.position, _dataBones[i]._offsetQuaternion);
		}

		//_target.position = DiffOffset(ik.transform, _bodyPart.position, _offset);

		//if (_isLeft)
		//_target.position = DiffOffset(ik.transform, _bodyPart.position, _offset);
		//else
		//_target.position = DiffOffset(ik.transform, ik.references.rightCalf.position, _offset);
		//_target.position = ik.references.rightCalf.position + (ik.transform.forward + _offset);
		//Debug.Log(ik.references.leftCalf.position);
	 }


	 [System.Serializable]
	 public struct DataBone
	 {
		public string name;
		public Transform _target;
		public Transform _bodyPart;
		public Transform _bodyPartNext;
		public Vector3 _offset;
		public Vector3 _offsetQuaternion;
	 }

	 private Vector3 AltOffset(Transform forvard, Vector3 position, Vector3 offset)
	 {
		return position
		  + (forvard.forward * offset.z)
		  + (forvard.up * offset.y)
		  + (forvard.right * offset.x);
		//return position
		//  + (forvard.forward * offset.z)
		//  + (forvard.up * offset.y)
		//  + (forvard.right * offset.x);
		//return position + offset;
		;
	 }

	 private Vector3 DiffOffset(Transform forvard, Vector3 position, Vector3 offset)
	 {
		return position
		  + (forvard.forward * offset.z)
		  + (forvard.up * offset.y)
		  + (forvard.right * offset.x);
		//return position
		//  + (forvard.forward * offset.z)
		//  + (forvard.up * offset.y)
		//  + (forvard.right * offset.x);
		//return position + offset;
		;
	 }
  }
}