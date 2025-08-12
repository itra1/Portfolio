using UnityEngine;
using System.Collections;
using com.ootii.Geometry;
using HutongGames.PlayMaker;
using com.ootii.Actors;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  public class AnimatorDriver : ActorDriver
  {
    protected Quaternion mRootMotionRotation = Quaternion.identity;
    protected Vector3 mRootMotionMovement = Vector3.zero;
    protected Animator _animator;

    public override void OnEnter()
    {
      base.OnEnter();
      _animator = _go.GetComponent<Animator>();
    }

    protected virtual void OnAnimatorMove()
    {
      if (Time.deltaTime == 0f) { return; }

      // Clear any root motion values
      if (_animator == null)
      {
        mRootMotionMovement = Vector3.zero;
        mRootMotionRotation = Quaternion.identity;
      }
      // Store the root motion as a velocity per second. We also
      // want to keep it relative to the avatar's forward vector (for now).
      // Use Time.deltaTime to create an accurate velocity (as opposed to Time.fixedDeltaTime).
      else
      {
        // Convert the movement to relative the current rotation
        mRootMotionMovement = Quaternion.Inverse(_go.transform.rotation) * (_animator.deltaPosition);

        // Store the rotation as a velocity per second.
        mRootMotionRotation = _animator.deltaRotation;
      }
    }
  }
}