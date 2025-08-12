using System;
using UnityEngine;
using com.ootii.Base;
using com.ootii.Helpers;

namespace com.ootii.Cameras
{
  [BaseName("Transform From Target")]
  [BaseDescription("Repeat position from target")]
  public class CopyTransformFromTarget : CameraMotor
  {
	 /// <summary>
	 /// INTERNAL ONLY: Index into the Camera Rig Controller's stored gameobjects
	 /// </summary>
	 public int _TargetIndex = -1;
	 public virtual int TargetIndex
	 {
		get { return _TargetIndex; }

		set
		{
		  _TargetIndex = value;

		  if (_TargetIndex >= 0)
		  {
			 if (RigController != null && _TargetIndex < RigController.StoredTransforms.Count)
			 {
				_Target = RigController.StoredTransforms[_TargetIndex];
			 }
		  }
		}
	 }

	 [NonSerialized]
	 public Transform _Target = null;
	 public virtual Transform Target
	 {
		get { return _Target; }

		set
		{
		  _Target = value;

#if UNITY_EDITOR

		  if (!Application.isPlaying)
		  {
			 if (RigController != null)
			 {
				if (_Target == null)
				{
				  if (_TargetIndex >= 0 && _TargetIndex < RigController.StoredTransforms.Count)
				  {
					 RigController.StoredTransforms[_TargetIndex] = null;
				  }
				}
				else
				{
				  if (_TargetIndex == -1)
				  {
					 _TargetIndex = RigController.StoredTransforms.Count;
					 RigController.StoredTransforms.Add(null);
				  }

				  RigController.StoredTransforms[_TargetIndex] = _Target;
				}
			 }
		  }

#endif

		}
	 }

	 public override void Activate(CameraMotor rOldMotor)
	 {
		base.Activate(rOldMotor);

		if (Target != null)
		{
		  RigController.transform.position = Target.position;
		  RigController.transform.rotation = Target.rotation;
		}
	 }

	 public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
	 {
		if (Target != null)
		{
		  RigController.transform.position = Target.position;
		  RigController.transform.rotation = Target.rotation;
		}

		return mRigTransform;
	 }

	 public override void PostRigLateUpdate()
	 {
		base.PostRigLateUpdate();
		if (Target != null)
		{
		  RigController.transform.position = Target.position;
		  RigController.transform.rotation = Target.rotation;
		}
	 }

#if UNITY_EDITOR

	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;

		GUILayout.Space(5f);

		if (EditorHelper.ObjectField<Transform>("Target", "Base position the camera will look at.", _Target, RigController))
		{
		  lIsDirty = true;
		  Target = EditorHelper.FieldObjectValue as Transform;
		}

		return lIsDirty;
	 }

#endif

  }
}
