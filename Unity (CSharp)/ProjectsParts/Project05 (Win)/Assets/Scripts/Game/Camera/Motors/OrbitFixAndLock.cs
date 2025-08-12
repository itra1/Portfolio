using UnityEngine;
using com.ootii.Base;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Cameras;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Gamera.Motors
{
  public class OrbitFixAndLock : YawPitchMotor
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
	 [System.NonSerialized]
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

	 Vector3 delta;

	 public override void Activate(CameraMotor rOldMotor)
	 {
		base.Activate(rOldMotor);
		CalcDelta();
	 }

	 public void CalcDelta()
	 {
		delta = RigController.transform.position - RigController.Anchor.position;
	 }

	 public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
	 {
		if (Target != null)
		{
		  mRigTransform.Position = RigController.Anchor.position + delta;
		  mRigTransform.Rotation = Quaternion.LookRotation(Target.position - RigController.transform.position);
		}

		return mRigTransform;
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