using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityStandardAssets.Cameras {
  public class FrontLockCam: PivotBasedCameraRig {

    [SerializeField] private float m_MoveSpeed = 1f;

    protected override void FollowTarget(float deltaTime) {
      if (m_Target == null)
        return;
      // Move the rig towards target position.
      transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime * m_MoveSpeed);
    }

  }

}