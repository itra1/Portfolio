using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.Challenges.KingGameOfUr
{
  public class Piramide : MonoBehaviourBase, Game.Items.IInteraction
  {

	 [SerializeField]
	 private KingGameOfUr _Manager;
	 [SerializeField]
	 private Transform m_RotateBlock;
	 private int m_TargetValue = 3;
	 private float m_TargetAngle = 3;
	 private bool m_isRotate = false;

	 private float m_TimeRotationStop;

	 public bool IsInteractReady => _Manager.State <= 1;

	 public void SetValue(int val, bool force = false)
	 {
		m_TargetValue = val;
		m_TargetAngle = GetAngle(m_TargetValue);
		m_TimeRotationStop = force ? Time.time : Time.time + Random.Range(1f, 2f);
		m_isRotate = true;
	 }

	 [ContextMenu("SetOne")]
	 public void SetOne()
	 {
		SetValue(1);
	 }
	 [ContextMenu("SetTwo")]
	 public void SetTwo()
	 {
		SetValue(2);
	 }
	 [ContextMenu("SetTree")]
	 public void SetTree()
	 {
		SetValue(3);
	 }
	 [ContextMenu("SetFour")]
	 public void SetFour()
	 {
		SetValue(4);
	 }

	 private float GetAngle(int num)
	 {
		switch (num)
		{
		  case 1:
			 {
				return 270;
			 }
		  case 2:
			 {
				return 180;
			 }
		  case 3:
			 {
				return 0;
			 }
		  case 4:
		  default:
			 {
				return 90;
			 }
		}
	 }

	 public void Clear()
	 {
		SetValue(3, true);
	 }

	 //private void Update()
	 //{
	 //if (!m_isRotate)
	 //  return;

	 //Vector3 angleBefore = m_RotateBlock.rotation.eulerAngles + transform.rotation.eulerAngles + new Vector3(0, 90, 0);

	 //m_RotateBlock.rotation = Quaternion.Euler(0, 720 * Time.deltaTime, 0) * m_RotateBlock.rotation;

	 //Vector3 afterBefore = m_RotateBlock.rotation.eulerAngles + transform.rotation.eulerAngles + new Vector3(0, 90, 0);
	 //if (m_TimeRotationStop < Time.time)
	 //{
	 //  if (m_TargetAngle == 0)
	 //  {
	 //	 if (angleBefore.y > 180 && afterBefore.y < 180)
	 //	 {
	 //		m_RotateBlock.rotation = Quaternion.Euler(0, m_TargetAngle, 0);
	 //		Debug.Log(m_TargetAngle);
	 //		m_isRotate = false;
	 //	 }
	 //  }
	 //  else
	 //  {
	 //	 if (angleBefore.y <= m_TargetAngle && afterBefore.y >= m_TargetAngle)
	 //	 {
	 //		m_RotateBlock.rotation = Quaternion.Euler(0, m_TargetAngle, 0);
	 //		Debug.Log(m_TargetAngle);
	 //		m_isRotate = false;
	 //	 }
	 //  }
	 //}
	 //}

	 private void Update()
	 {
		if (!m_isRotate)
		  return;

		Vector3 angleBefore = m_RotateBlock.localEulerAngles;

		m_RotateBlock.rotation = Quaternion.Euler(0, 720 * Time.deltaTime, 0) * m_RotateBlock.rotation;

		Vector3 afterBefore = m_RotateBlock.localEulerAngles;
		if (m_TimeRotationStop < Time.time)
		{
		  if (m_TargetAngle == 0)
		  {
			 if (angleBefore.y > 180 && afterBefore.y < 180)
			 {
				m_RotateBlock.localEulerAngles = new Vector3(0, m_TargetAngle, 0);
				m_isRotate = false;
			 }
		  }
		  else
		  {
			 if (angleBefore.y <= m_TargetAngle && afterBefore.y >= m_TargetAngle)
			 {
				m_RotateBlock.localEulerAngles = new Vector3(0, m_TargetAngle, 0);
				m_isRotate = false;
			 }
		  }
		}
	 }

	 public void StartInteract()
	 {
		switch (_Manager.State)
		{
		  case 0:
			 _Manager.StartInteract();
			 break;
		}

	 }

	 public void StopInteract()
	 {
		_Manager.StopInteract();
	 }
  }
}