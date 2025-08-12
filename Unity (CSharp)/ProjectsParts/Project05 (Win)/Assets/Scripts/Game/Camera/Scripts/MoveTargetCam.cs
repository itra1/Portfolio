using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetCam : MonoBehaviourBase
{
  private Transform m_target;
  private float m_speed = 3;
  private Transform m_Cam;
  private Transform m_Pivot;
  private Vector3 m_PivotEulers;

  private Quaternion m_PivotTargetRot;
  private Quaternion m_TransformTargetRot;

  private bool m_isMove;
  private float m_startMove;
  private float m_timeMove;

  protected void Awake() {
    m_Cam = GetComponentInChildren<Camera>().transform;
    m_Pivot = m_Cam.parent;
  }


  public void Move() {
    transform.position = Vector3.Lerp(transform.position, m_target.position - m_Pivot.localPosition - m_Cam.localPosition, Time.deltaTime * m_speed);
    //transform.position = Vector3.Lerp(transform.position, m_target.position - m_Pivot.localPosition, Time.deltaTime * m_speed);
  }

  [ContextMenu("Move")]
  public void StartMove(Transform target) {
    m_isMove = true;
    m_startMove = Time.time;
    m_target = target;
    m_PivotEulers = m_Pivot.rotation.eulerAngles;

    Debug.Log(m_target.rotation.eulerAngles);

    m_TransformTargetRot = Quaternion.Euler(0f, m_target.rotation.eulerAngles.y, 0f);
    m_PivotTargetRot = Quaternion.Euler(m_target.rotation.x, 0, 0);
  }

  private void Update() {
    if (m_target == null || !m_isMove)
      return;

    Move();
    Rotation();
  }

  private void Rotation() {
    transform.rotation = Quaternion.RotateTowards(transform.rotation, m_TransformTargetRot, 5);
    m_Pivot.rotation = Quaternion.RotateTowards(transform.rotation, m_PivotTargetRot, 5);
  }


}
