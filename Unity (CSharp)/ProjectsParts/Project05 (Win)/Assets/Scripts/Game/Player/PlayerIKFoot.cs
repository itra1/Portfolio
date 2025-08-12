using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Player
{
	public class PlayerIKFoot : MonoBehaviour
	{
		private Animator m_anim;
		private Vector3 m_RightFootPosition, m_LeftFootPosition;
		private Vector3 m_LeftFootIkPosition, m_RightFootIkPosition;
		private Quaternion m_LeftFootIkRotation, m_RightFootIkRosition;
		private float m_LastPelvisPositionY;
		private float m_LastRightFootPositionY, m_LastLeftFootPositionY;

		public bool m_EnableFeetIk = true;
		[Range(0, 2)] [SerializeField] private float m_HeightFromGroundRaycast = 1.14f;
		[Range(0, 2)] [SerializeField] private float m_RaycastDownDistance = 1.5f;
		[SerializeField] private LayerMask m_environmentLayer;
		[SerializeField] private float m_PelvisOffset = 0f;
		[Range(0, 1)] [SerializeField] private float m_PelvisUpAndDownSpeed = 0.28f;
		[Range(0, 1)] [SerializeField] private float m_FeetToIkPositionSpeed = 0.5f;

		public string m_LeftFootAnimVariableName = "LeftFootCurve";
		public string m_RightFootAnimVariableName = "RightFootCurve";

		public bool m_UserProIkReature = false;
		public bool m_ShowSolverDebug = true;

		private void Awake()
		{
			m_anim = GetComponent<Animator>();
		}

		private void FixedUpdate()
		{

			if (!m_EnableFeetIk)
				return;
			if (m_anim == null)
				return;

			AdjustFeetTarget(ref m_RightFootPosition, HumanBodyBones.RightFoot);
			AdjustFeetTarget(ref m_LeftFootPosition, HumanBodyBones.LeftFoot);

			FeetPositionSolver(m_RightFootPosition, ref m_RightFootIkPosition, ref m_RightFootIkRosition);
			FeetPositionSolver(m_LeftFootPosition, ref m_LeftFootIkPosition, ref m_LeftFootIkRotation);
		}

		private void OnAnimationIk(int layerIndex)
		{

			if (!m_EnableFeetIk)
				return;
			if (m_anim == null)
				return;

			MovePelvisHeight();

			m_anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

			if (m_UserProIkReature)
			{
				m_anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, m_anim.GetFloat(m_RightFootAnimVariableName));
			}
			MoveFeetToIkPoint(AvatarIKGoal.RightFoot, m_RightFootIkPosition, m_RightFootIkRosition, ref m_LastRightFootPositionY);

			m_anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

			if (m_UserProIkReature)
			{
				m_anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, m_anim.GetFloat(m_LeftFootAnimVariableName));
			}
			MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, m_LeftFootIkPosition, m_LeftFootIkRotation, ref m_LastLeftFootPositionY);
		}

		private void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPositionY)
		{
			Vector3 targetIkPosition = m_anim.GetIKPosition(foot);
			if (positionIkHolder != Vector3.zero)
			{
				targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
				positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

				float yVariable = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, m_FeetToIkPositionSpeed);
				targetIkPosition.y += yVariable;

				lastFootPositionY = yVariable;
				targetIkPosition = transform.TransformPoint(targetIkPosition);
				m_anim.SetIKRotation(foot, rotationIkHolder);
			}
			m_anim.SetIKPosition(foot, targetIkPosition);
		}

		private void MovePelvisHeight()
		{
			if (m_RightFootIkPosition == Vector3.zero || m_LeftFootIkPosition == Vector3.zero || m_LastPelvisPositionY == 0)
			{
				m_LastPelvisPositionY = m_anim.bodyPosition.y;
				return;
			}
			float lOffsetPosition = m_LeftFootIkPosition.y - transform.position.y;
			float rOffsetPosition = m_RightFootIkPosition.y - transform.position.y;

			float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

			Vector3 newPelvisPosition = m_anim.bodyPosition + Vector3.up * totalOffset;

			newPelvisPosition.y = Mathf.Lerp(m_LastPelvisPositionY, newPelvisPosition.y, m_PelvisUpAndDownSpeed);
			m_anim.bodyPosition = newPelvisPosition;
			m_LastPelvisPositionY = m_anim.bodyPosition.y;
		}

		private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotations)
		{
			RaycastHit feetOutHit;

			if (m_ShowSolverDebug)
				Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (m_RaycastDownDistance + m_HeightFromGroundRaycast), Color.yellow);

			if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, m_RaycastDownDistance + m_HeightFromGroundRaycast, m_environmentLayer))
			{
				feetIkPositions = fromSkyPosition;
				feetIkPositions.y = feetOutHit.point.y + m_PelvisOffset;
				feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

				return;
			}
			feetIkPositions = Vector3.zero;

		}

		private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
		{
            feetPositions = m_anim.GetBoneTransform(foot).position;
            feetPositions.y = transform.position.y + m_HeightFromGroundRaycast;
		}

	}

}