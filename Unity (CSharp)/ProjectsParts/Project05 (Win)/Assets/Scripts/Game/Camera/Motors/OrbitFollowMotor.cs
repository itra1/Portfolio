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
  [BaseName("3rd Person Follow Pastel")]
  [BaseDescription("Motor that allows the rig to orbit the anchor and anchor offset. This rig drags behind the anchor as if attached by a rope.")]
  public class OrbitFollowMotor : YawPitchMotor
  {
	 /// <summary>
	 /// Actual distance from the anchor's position
	 /// </summary>
	 public override float Distance
	 {
		get { return mDistance; }
		set
		{
		  mDistance = value;
		  _MaxDistance = value;
		}
	 }

	 private float _maxDistanceCamera = 5;
	 public float MaxDistanceCamera { get => _maxDistanceCamera; set => _maxDistanceCamera = value; }

	 private float _minDistanceCamera = 3;
	 public float MinDistanceCamera { get => _minDistanceCamera; set => _minDistanceCamera = value; }

	 public string DistanceChangeAlias { get => _distanceChangeAlias; set => _distanceChangeAlias = value; }

	 private string _distanceChangeAlias = "Camera Zoom";

	 private bool _changeDistance = true;
	 public bool ChangeDistance { get => _changeDistance; set => _changeDistance = value; }
	 public bool _distanceResetOnRelease = true;
	 public bool DistanceResetOnRelease
	 {
		get { return _distanceResetOnRelease; }
		set { _distanceResetOnRelease = value; }
	 }

	 public float ZoomSpeed { get => _zoomSpeed; set => _zoomSpeed = value; }

	 private float _zoomSpeed = 0.5f;

	 /// <summary>
	 /// Called when the motor is deserialized
	 /// </summary>
	 public override void Awake()
	 {
		base.Awake();

		if (Application.isPlaying && Anchor == null)
		{
		  mRigTransform.Position = RigController._Transform.position;
		  mRigTransform.Rotation = RigController._Transform.rotation;
		}
	 }

	 /// <summary>
	 /// Updates the motor over time. This is called by the controller
	 /// every update cycle so movement can be updated. 
	 /// </summary>
	 /// <param name="rDeltaTime">Time since the last frame (or fixed update call)</param>
	 /// <param name="rUpdateIndex">Index of the update to help manage dynamic/fixed updates. [0: Invalid update, >=1: Valid update]</param>
	 /// <param name="rTiltAngle">Amount of tilting the camera needs to do to match the anchor</param>
	 public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
	 {

		Transform lAnchorTransform = Anchor;
		if (lAnchorTransform == null) { return mRigTransform; }

		if (RigController == null) { return mRigTransform; }
		Transform lCameraTransform = RigController._Transform;

		// Determine how much the anchor's yaw changed (we want to stay relative to the anchor direction)
		float lAnchorRootYawDelta = Vector3Ext.HorizontalAngleTo(mAnchorLastRotation.Forward(), lAnchorTransform.forward, lAnchorTransform.up);
		float lAnchorYaw = (Mathf.Abs(rTiltAngle) >= 2f ? lAnchorRootYawDelta : 0f);

		if (!RigController.RotateAnchorOffset)
		{
		  lAnchorRootYawDelta = 0f;
		  lAnchorYaw = 0f;
		}

		// Grab any euler changes this frame
		mFrameEuler = GetFrameEuler(true, true); // RigController.RotateAnchorOffset);

		// Get our predicted based on the frame and anchor changes. We want this to get the right-vector
		Quaternion lNewCameraRotation = Quaternion.AngleAxis(mFrameEuler.y + lAnchorYaw, (RigController.RotateAnchorOffset ? RigController.Tilt.Up() : Vector3.up)) * lCameraTransform.rotation;

		// Now grab the full (vertical + horizontal) position of our final focus.
		//Vector3 lNewFocusPosition = lAnchorTransform.position + (lAnchorTransform.up * lOffset.y);
		//lNewFocusPosition = lNewFocusPosition + (lNewCameraRotation.Right() * lOffset.x);
		Vector3 lNewFocusPosition = GetFocusPosition(lNewCameraRotation);

		// Default the value
		Vector3 lNewCameraPosition = lCameraTransform.position;
		Vector3 lToFocusPosition = lCameraTransform.forward;

		UpdateZoom(rDeltaTime);

		// If we're tilting, act like a fixed
		if (RigController.FrameForceToFollowAnchor || Mathf.Abs(rTiltAngle) >= 2f)
		{
		  // Get the local position and the right vector of the camera relative to the last frame
		  Matrix4x4 lOldFocusMatrix = Matrix4x4.TRS(mFocusLastPosition, (RigController.RotateAnchorOffset ? mAnchorLastRotation : Quaternion.identity), Vector3.one);
		  Matrix4x4 lNewFocusMatrix = Matrix4x4.TRS(lNewFocusPosition, (RigController.RotateAnchorOffset ? lAnchorTransform.rotation : Quaternion.identity), Vector3.one);

		  // The matrix will add our anchor delta. But, we also added it when we use the LocalYaw
		  // We'll remove it so we don't double up.
		  if (mTargetYaw < float.MaxValue)
		  {
			 mFrameEuler.y = mFrameEuler.y - lAnchorRootYawDelta;
		  }

		  // If nothing has changed, we won't update. This is important due to the fact
		  // that the inverse of the matix causes a small floating point error to move us.
		  if (mFrameEuler.sqrMagnitude != 0f || lOldFocusMatrix != lNewFocusMatrix)
		  {
			 Vector3 lLocalPosition = lOldFocusMatrix.inverse.MultiplyPoint(lCameraTransform.position);
			 Vector3 lLocalCameraRight = lOldFocusMatrix.inverse.MultiplyVector(lCameraTransform.right);

			 // Rotate the old local position based on the frame's rotation changes
			 Quaternion lLocalRotation = Quaternion.AngleAxis(mFrameEuler.y, Vector3.up) * Quaternion.AngleAxis(mFrameEuler.x, lLocalCameraRight);
			 lLocalPosition = lLocalRotation * lLocalPosition;

			 // Grab the new position based on the updated matrix
			 lNewCameraPosition = lNewFocusMatrix.MultiplyPoint(lLocalPosition);
		  }
		}
		// If we have a target forward, act like a fixed
		else if (mTargetForward.sqrMagnitude > 0f)
		{
		  lNewCameraRotation = lNewCameraRotation * Quaternion.AngleAxis(mFrameEuler.x, Vector3.right);
		  lNewCameraPosition = lNewFocusPosition - (lNewCameraRotation.Forward() * Distance);
		}
		else
		{
		  lNewCameraRotation = lNewCameraRotation * Quaternion.AngleAxis(mFrameEuler.x, Vector3.right);
		  Vector3 lOldCameraPosition = mFocusLastPosition - (lNewCameraRotation.Forward() * Distance);

		  // Grab the new focus position using the drag rotation we get
		  lToFocusPosition = lNewFocusPosition - lOldCameraPosition;
		  if (lToFocusPosition.sqrMagnitude < 0.0001f) { lToFocusPosition = lCameraTransform.forward; }

		  lNewCameraRotation = Quaternion.LookRotation(lToFocusPosition.normalized, (RigController.RotateAnchorOffset ? lAnchorTransform.up : Vector3.up));
		  //lNewFocusPosition = lAnchorTransform.position + (lAnchorTransform.up * lOffset.y);
		  //lNewFocusPosition = lNewFocusPosition + (lNewCameraRotation.Right() * lOffset.x);
		  lNewFocusPosition = GetFocusPosition(lNewCameraRotation);

		  // If something is between the new focus position and the anchor, the new focus position is the anchor
		  //Color lHitColor = Color.black;
		  //RaycastHit lFocusHit;
		  Vector3 lAnchorPosition = Vector3.zero;

		  if (RigController.RotateAnchorOffset)
		  {
			 lAnchorPosition = lAnchorTransform.position + (lAnchorTransform.rotation * AnchorOffset) + (lAnchorTransform.up * _Offset.y);
		  }
		  else
		  {
			 lAnchorPosition = lAnchorTransform.position + AnchorOffset + (Vector3.up * _Offset.y);
		  }

		  lToFocusPosition = lNewFocusPosition - lAnchorPosition;

		  // Get the drag direction and pull out the horizontal component
		  lToFocusPosition = lNewFocusPosition - lOldCameraPosition;
		  if (lToFocusPosition.sqrMagnitude < 0.0001f) { lToFocusPosition = lCameraTransform.forward; }

		  Vector3 lToFocusPositionVertical = Vector3.Project(lToFocusPosition, (RigController.RotateAnchorOffset ? RigController.Tilt.Up() : Vector3.up));
		  Vector3 lToFocusPositionHorizontal = lToFocusPosition - lToFocusPositionVertical;

		  // Get the pitch rotation regardless of our tilt
		  lNewCameraRotation = lCameraTransform.rotation * Quaternion.AngleAxis(mFrameEuler.x, Vector3.right);
		  Vector3 lRotationEuler = (RigController.RotateAnchorOffset ? Quaternion.Inverse(RigController.Tilt) * lNewCameraRotation : lNewCameraRotation).eulerAngles;

		  // Add the pitch to our look-at rotation
		  Quaternion lNewAnchorRotation = Quaternion.LookRotation(lToFocusPositionHorizontal, (RigController.RotateAnchorOffset ? RigController.Tilt.Up() : Vector3.up)) * Quaternion.Euler(lRotationEuler.x, 0f, 0f);

		  // Using the rotation, grab the new camera position
		  lNewCameraPosition = lNewFocusPosition - (lNewAnchorRotation.Forward() * Distance);
		}

		// Determine our rotation
		lToFocusPosition = lNewFocusPosition - lNewCameraPosition;
		if (lToFocusPosition.sqrMagnitude < 0.0001f) { lToFocusPosition = lCameraTransform.forward; }

		lNewCameraRotation = Quaternion.LookRotation(lToFocusPosition.normalized, (RigController.RotateAnchorOffset ? lAnchorTransform.up : Vector3.up));

		// We have to do a final check if we have exceeded rotation limits
		Quaternion lNewLocalCameraRotation = (RigController.RotateAnchorOffset ? Quaternion.Inverse(Anchor.transform.rotation) : Quaternion.identity) * lNewCameraRotation;
		Vector3 lNewLocalEuler = lNewLocalCameraRotation.eulerAngles;

		if (lNewLocalEuler.y > 180f) { lNewLocalEuler.y = lNewLocalEuler.y - 360f; }
		else if (lNewLocalEuler.y < -180f) { lNewLocalEuler.y = lNewLocalEuler.y + 360f; }

		if (lNewLocalEuler.x > 180f) { lNewLocalEuler.x = lNewLocalEuler.x - 360f; }
		else if (lNewLocalEuler.x < -180f) { lNewLocalEuler.x = lNewLocalEuler.x + 360f; }

		float lYaw = (_MinYaw > -180f || _MaxYaw < 180f ? Mathf.Clamp(lNewLocalEuler.y, _MinYaw, _MaxYaw) : lNewLocalEuler.y);
		float lPitch = Mathf.Clamp(lNewLocalEuler.x, _MinPitch, _MaxPitch);
		if (lYaw != lNewLocalEuler.y || lPitch != lNewLocalEuler.x)
		{
		  lNewCameraRotation = (RigController.RotateAnchorOffset ? Anchor.transform.rotation : Quaternion.identity) * Quaternion.Euler(lPitch, lYaw, 0f);
		  lNewCameraPosition = lNewFocusPosition - (lNewCameraRotation.Forward() * Distance);
		}

		// Return the results
		mRigTransform.Position = lNewCameraPosition;
		mRigTransform.Rotation = lNewCameraRotation;
		return mRigTransform;
	 }

	 private void UpdateZoom(float deltaTime)
	 {
		if (ChangeDistance && DistanceChangeAlias.Length > 0)
		{
		  if (_distanceResetOnRelease && RigController.InputSource.IsJustReleased(DistanceChangeAlias))
		  {
			 Distance = MaxDistanceCamera;
		  }
		  else
		  {
			 float lZoomMax = MaxDistanceCamera;

			 float lFrameFieldOfView = -RigController.InputSource.GetValue(DistanceChangeAlias) * ZoomSpeed;
			 Distance = Mathf.Clamp(Distance + lFrameFieldOfView, (false ? MaxDistanceCamera : MinDistanceCamera), lZoomMax);
		  }
		}
		else if (Distance != MaxDistanceCamera)
		{
		  Distance = MaxDistanceCamera;
		}

		//if (ChangeDistance && Mathf.Abs(Distance - _Camera.fieldOfView) > 0.001f)
		//{
		//  _Camera.fieldOfView = Mathf.SmoothDampAngle(_Camera.fieldOfView, mTargetFOV, ref mZoomVelocity, _ZoomSmoothing);
		//}
		//else
		//{
		//  mZoomVelocity = 0f;
		//}
	 }

	 // **************************************************************************************************
	 // Following properties and function only valid while editing
	 // **************************************************************************************************

#if UNITY_EDITOR

	 /// <summary>
	 /// Allow the motion to render it's own GUI
	 /// </summary>
	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;

		if (base.OnInspectorGUI())
		{
		  lIsDirty = true;
		}

		GUILayout.Space(5f);

		float lLabelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 70f;

		if (EditorHelper.FloatField("Distance", "Distance from the anchor that we orbit", MaxDistance, RigController))
		{
		  lIsDirty = true;
		  MaxDistance = EditorHelper.FieldFloatValue;

		  if (Application.isPlaying) { mDistance = MaxDistance; }
		}


		string lDistanceChangeAlias = EditorGUILayout.TextField(new GUIContent("Distance change", ""), DistanceChangeAlias, GUILayout.MinWidth(30));
		if (lDistanceChangeAlias != DistanceChangeAlias)
		{
		  lIsDirty = true;
		  DistanceChangeAlias = lDistanceChangeAlias;
		}

		if (EditorHelper.FloatField("Min distance camera", "", MinDistanceCamera, RigController))
		{
		  lIsDirty = true;
		  MinDistanceCamera = EditorHelper.FieldFloatValue;

		}

		if (EditorHelper.FloatField("Max distance camera", "", MaxDistanceCamera, RigController))
		{
		  lIsDirty = true;
		  MaxDistanceCamera = EditorHelper.FieldFloatValue;

		}

		if (EditorHelper.FloatField("Zoom speed", "", ZoomSpeed, RigController))
		{
		  lIsDirty = true;
		  ZoomSpeed = EditorHelper.FieldFloatValue;
		}

		EditorGUIUtility.labelWidth = lLabelWidth;

		return lIsDirty;
	 }

#endif
  }
}