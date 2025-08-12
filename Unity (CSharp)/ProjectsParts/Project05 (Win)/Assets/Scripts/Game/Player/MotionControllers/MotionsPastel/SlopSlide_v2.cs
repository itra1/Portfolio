using com.ootii.Helpers;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections;
using com.ootii.Actors.AnimationControllers;
using FluffyUnderware.Curvy;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Slop Slide v2")]
  [MotionDescription("Моушен скольжения по склону")]
  public class SlopSlide_v2 : MotionControllerMotion, ISlop
  {

	 public const int PHASE_UNKNOWN = 0;
	 public const int PHASE_START = 700;
	 public const int PHASE_END = 705;
	 private const float SLIDE_ANGLE = 10f;

	 private float _phase = 0;
	 private bool _isSlopEnable;
	 private float jumpTimeDeactivation = 0;
	 private Vector3 _gravityValue;
	 private bool _isForceGrounding;
	 private float _isForceGroundingDistance;
	 private float _currentSpeed;
	 private bool _isWaitForceGrounding;
	 private float mTimeStart = 0;
	 private float _maxSpeed;
	 public float MaxSpeed { get => _maxSpeed; set => _maxSpeed = value; }

	 private FluffyUnderware.Curvy.CurvySpline m_Spline;
	 protected Vector3 mLaunchVelocity = Vector3.zero;
	 private System.Action m_SlowDownComplete;

    private VisualEffect _SliceVfxLeft;
	 private VisualEffect _SliceVfxRight;

	 private Vector3 _tangent;
	 public float _RotationSpeed = 180f;
	 public float RotationSpeed
	 {
		get { return _RotationSpeed; }
		set { _RotationSpeed = value; }
	 }
	 private float m_StartSlowDown;

	 public SlopSlide_v2() : base()
	 {
		_Priority = 50;
		_phase = 0;
		mIsStartable = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Slide-SM"; }
#endif
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public SlopSlide_v2(MotionController rController) : base(rController)
	 {
		_Priority = 50;
		_phase = 0;
		mIsStartable = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Slide-SM"; }
#endif
	 }
	 public void SetSpline(FluffyUnderware.Curvy.CurvySpline spline)
	 {
		m_Spline = spline;
		GetTanget();
	 }

	 private void GetTanget()
	 {
		float nearestTF = m_Spline.GetNearestPointTF(mActorController._Transform.position, Space.World);
		GetInterpolatedSourcePosition(nearestTF, out _tangent);
	 }

	 public void SetSliceVfx(VisualEffect sliceVfx)
	 {
		_SliceVfxLeft = MonoBehaviour.Instantiate(sliceVfx.gameObject).GetComponent<VisualEffect>();
		_SliceVfxRight = MonoBehaviour.Instantiate(sliceVfx.gameObject).GetComponent<VisualEffect>();
		_SliceVfxLeft.gameObject.SetActive(true);
		_SliceVfxRight.gameObject.SetActive(true);
	 }

	 protected void GetInterpolatedSourcePosition(float tf, out Vector3 tangent)
	 {
		Vector3 interpolatedPosition;
		CurvySpline spline = m_Spline;
		Transform splineTransform = spline.transform;

		float localF;
		CurvySplineSegment currentSegment = spline.TFToSegment(tf, out localF);
		if (ReferenceEquals(currentSegment, null) == false)
		{
		  currentSegment.InterpolateAndGetTangent(localF, out interpolatedPosition, out tangent);
		}

		else
		{
		  interpolatedPosition = Vector3.zero;
		  tangent = Vector3.zero;
		}

		interpolatedPosition = splineTransform.TransformPoint(interpolatedPosition);
		tangent = splineTransform.TransformDirection(tangent);
	 }

	 public override bool TestInterruption(MotionControllerMotion rMotion)
	 {
		return true;
	 }
	 public override bool TestActivate()
	 {
		if (!mIsStartable) { return false; }
		if (mMotionLayer.ActiveMotion != null &&mMotionLayer.ActiveMotion._Category == EnumMotionCategories.DEATH)
		{
		  return false;
		}

		if (!mMotionController.IsGrounded) { return false; }

		if (Time.time - jumpTimeDeactivation < 0.5f) { return false; }

		if (_phase == 1)
		  return true;

		if (mActorController.State.GroundSurfaceAngle < SLIDE_ANGLE) { return false; }

		return true;
	 }
	 public override bool TestUpdate()
	 {
		if (mIsActivatedFrame) { return true; }

		//if (!mMotionController.IsGrounded) { return false; }

		//if (mMotionController._InputSource.IsJustPressed("Jump"))
		//{
		//  jumpTimeDeactivation = Time.time;
		//  return false;
		//}

		// Defalut to true
		return true;
	 }
	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		_isSlopEnable = mActorController.IsSlidingEnabled;
		mActorController.IsSlidingEnabled = false;
		_gravityValue = mActorController.Gravity;
		mActorController.Gravity = _gravityValue;
		_isWaitForceGrounding = true;
		_isForceGrounding = mActorController.ForceGrounding;
		//_isForceGroundingDistance = mActorController.ForceGroundingDistance;
		//mActorController.ForceGroundingDistance = 0.1f;

		if (_SliceVfxLeft != null)
		  _SliceVfxLeft.SendEvent("OnPlay");
		if (_SliceVfxRight != null)
		  _SliceVfxRight.SendEvent("OnPlay");

		_currentSpeed = mActorController.Velocity.magnitude;

		mTimeStart = Time.time;
		_phase = 1;
		mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START, true);
		//m_SliceVfx.SendEvent("OnPlay");
		return base.Activate(rPrevMotion);
	 }
	 public override void Deactivate()
	 {
		base.Deactivate();
		mActorController.IsSlidingEnabled = _isSlopEnable;
		mActorController.Gravity = _gravityValue;
		mActorController.ForceGrounding = _isForceGrounding;
		if (_SliceVfxLeft != null)
		  _SliceVfxLeft.SendEvent("OnStop");
		if (_SliceVfxRight != null)
		  _SliceVfxRight.SendEvent("OnStop");
		//mActorController.ForceGroundingDistance = _isForceGroundingDistance;
		//m_SliceVfx.SendEvent("OnStop");
	 }
	 public override void UpdateRootMotion(float rDeltaTime, int rUpdateIndex, ref Vector3 rVelocityDelta, ref Quaternion rRotationDelta)
	 {
		rVelocityDelta = Vector3.zero;
		rRotationDelta = Quaternion.identity;
	 }

	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		bool lRotate = true;
		int lStateID = mMotionLayer._AnimatorStateID;
		float lStateTime = mMotionLayer._AnimatorStateNormalizedTime;


		if (_SliceVfxLeft != null)
		  _SliceVfxLeft.transform.position = PlayerBehaviour.Instance.FullBodyBipedIK.references.leftFoot.position;
		if (_SliceVfxRight != null)
		  _SliceVfxLeft.transform.position = PlayerBehaviour.Instance.FullBodyBipedIK.references.rightFoot.position;

		// If we hit a lower slope than the slide allows, transition to the idle
		// if (mActorController.State.GroundSurfaceAngle < SLIDE_ANGLE) {
		//     lRotate = false;
		//     mMotionController.SetAnimatorMotionPhase (mMotionLayer._AnimatorLayerIndex, PHASE_END, true);
		// }

		// When we reach the final state, stop
		if (mMotionLayer._AnimatorStateID == STATE_IdlePose)
		{
		  lRotate = false;
		  Deactivate();
		}

		//if(lStateID == STATE_Slide && lStateTime > 2)
		//{
		if (_isWaitForceGrounding)
		{
		  _isWaitForceGrounding = false;
		  //mActorController.ForceGrounding = false;
		}
		//}


		//m_SliceVfx.SetVector3("LeftLegPosition", m_LeftFoot.position);
		//m_SliceVfx.SetVector3("RightLegPosition", m_RightFoot.position);

		// If we're meant to, rotate towards the direction of the slide
		if (lRotate && _RotationSpeed > 0f)
		{
		  // Grab the direction of force along the ground plane
		  // Vector3 lDirection = Vector3.down;
		  // Vector3 lGroundNormal = mActorController.State.GroundSurfaceNormal;

		  // Vector3.OrthoNormalize (ref lGroundNormal, ref lDirection);

		  // // Convert the direction to a horizontal forward
		  // Vector3 lUp = mActorController._Transform.up;

		  // lDirection = lDirection - lUp;

		  float pointAngle = NumberHelper.GetHorizontalAngle(_tangent, mMotionController._Transform.position);


		  GetTanget();


		  float lAngle = NumberHelper.GetHorizontalAngle(mMotionController._Transform.forward, _tangent);
		  if (mMotionController._InputSource.MovementX != 0)
		  {
			 _tangent = Quaternion.AngleAxis(mMotionController._InputSource.MovementX * 40, Vector3.up) * _tangent;
			 lAngle += mMotionController._InputSource.MovementX * 40;
		  }

		  if (Mathf.Abs(lAngle) > 0.01f)
		  {
			 float lAngularSpeed = lAngle * _RotationSpeed * Time.deltaTime;
			 mAngularVelocity.y = lAngularSpeed;
		  }
		  //mMovement = lDirection * SPEED * (mActorController.State.GroundSurfaceAngle * SPEED_SPEED_KOEF);

		  if (_phase == 1)
		  {

			 if (_currentSpeed < MaxSpeed - .3f)
			 {
				_currentSpeed += MaxSpeed * Time.deltaTime;
			 }else if (_currentSpeed > MaxSpeed + .3f)
			 {
				_currentSpeed -= MaxSpeed * Time.deltaTime;
			 }
			 else
			 {
				_currentSpeed = MaxSpeed;
			 }
			 mLaunchVelocity = _tangent * _currentSpeed;
		  }

		  if (_phase == 2)
		  {
			 _currentSpeed -= MaxSpeed * Time.deltaTime;
			 mLaunchVelocity = _tangent.normalized * _currentSpeed;

			 if (_currentSpeed <= 0)
			 {
				m_SlowDownComplete?.Invoke();
			 }

			 //mMovement = lDirection.normalized * Mathf.Lerp(SPEED, 0, (Time.time - m_StartSlowDown) * 0.3f);
			 //if ((Time.time - m_StartSlowDown) * 0.3f >= 1)
			 //m_SlowDownComplete?.Invoke();

		  }

		  //if (m_phase != 2)
		  //{
		  //if(mLaunchVelocity < lDirection.normalized)
		  //}

		  //// УСтановка скорости
		  //if (m_phase == 0)
		  //{
		  //mMovement = lDirection.normalized * Mathf.Lerp(0, SPEED, (Time.time - mTimeStart) * 0.8f);

		  //if ((Time.time - mTimeStart) * 0.8f >= 1)
		  //m_phase = 1;
		  //}

		  //if (m_phase == 1)
		  //mMovement = lDirection.normalized * SPEED;

		  //if (m_phase == 2)
		  //{
		  //mMovement = lDirection.normalized * Mathf.Lerp(SPEED, 0, (Time.time - m_StartSlowDown) * 0.3f);
		  //if ((Time.time - m_StartSlowDown) * 0.3f >= 1)
		  //m_SlowDownComplete?.Invoke();

		  //}
		}
		//moveVector = mMovement;
		//mVelocity = mMovement;
		mVelocity = mLaunchVelocity;


	 }
	 public void Slowdown(System.Action slowDownComplete)
	 {
		m_SlowDownComplete = slowDownComplete;
		m_StartSlowDown = Time.time;
		_phase = 2;
	 }


#if UNITY_EDITOR

	 /// <summary>
	 /// Allow the constraint to render it's own GUI
	 /// </summary>
	 /// <returns>Reports if the object's value was changed</returns>
	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;

		float lNewRotationVelocity = EditorGUILayout.FloatField(new GUIContent("Rotation Speed", "Determines how quickly the avatar rotates to face the downward slope. Set the value to 0 to not rotate."), _RotationSpeed, GUILayout.MinWidth(30));
		if (lNewRotationVelocity != _RotationSpeed)
		{
		  lIsDirty = true;
		  RotationSpeed = lNewRotationVelocity;
		}

		if (EditorHelper.FloatField("Speed", "", MaxSpeed, mMotionController))
		{
		  lIsDirty = true;
		  MaxSpeed = EditorHelper.FieldFloatValue;
		}

		return lIsDirty;
	 }

#endif

	 #region Auto-Generated
	 // ************************************ START AUTO GENERATED ************************************

	 /// <summary>
	 /// These declarations go inside the class so you can test for which state
	 /// and transitions are active. Testing hash values is much faster than strings.
	 /// </summary>
	 public static int TRANS_EntryState_Slide = -1;
	 public static int TRANS_AnyState_Slide = -1;
	 public static int STATE_Slide = -1;
	 public static int TRANS_Slide_IdlePose = -1;
	 public static int STATE_IdlePose = -1;

	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsInMotionState
	 {
		get
		{
		  int lStateID = mMotionLayer._AnimatorStateID;

		  if (lStateID == STATE_Slide) { return true; }
		  if (lStateID == STATE_IdlePose) { return true; }
		  return false;
		}
	 }


	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsMotionState(int rStateID)
	 {
		if (rStateID == STATE_Slide) { return true; }
		if (rStateID == STATE_IdlePose) { return true; }
		return false;
	 }

	 /// <summary>
	 /// Preprocess any animator data so the motion can use it later
	 /// </summary>
	 public override void LoadAnimatorData()
	 {
		/// <summary>
		/// These assignments go inside the 'LoadAnimatorData' function so that we can
		/// extract and assign the hash values for this run. These are typically used for debugging.
		/// </summary>
		TRANS_EntryState_Slide = mMotionController.AddAnimatorName("Entry -> Base Layer.Slide-SM.Slide");
		TRANS_AnyState_Slide = mMotionController.AddAnimatorName("AnyState -> Base Layer.Slide-SM.Slide");
		STATE_Slide = mMotionController.AddAnimatorName("Base Layer.Slide-SM.Slide");
		TRANS_Slide_IdlePose = mMotionController.AddAnimatorName("Base Layer.Slide-SM.Slide -> Base Layer.Slide-SM.IdlePose");
		STATE_IdlePose = mMotionController.AddAnimatorName("Base Layer.Slide-SM.IdlePose");
	 }

#if UNITY_EDITOR

	 private AnimationClip mSlide = null;
	 private AnimationClip mIdlePose = null;

	 /// <summary>
	 /// Creates the animator substate machine for this motion.
	 /// </summary>
	 protected override void CreateStateMachine()
	 {
		// Grab the root sm for the layer
		UnityEditor.Animations.AnimatorStateMachine lRootStateMachine = _EditorAnimatorController.layers[mMotionLayer.AnimatorLayerIndex].stateMachine;

		// If we find the sm with our name, remove it
		for (int i = 0; i < lRootStateMachine.stateMachines.Length; i++)
		{
		  // Look for a sm with the matching name
		  if (lRootStateMachine.stateMachines[i].stateMachine.name == _EditorAnimatorSMName)
		  {
			 // Allow the user to stop before we remove the sm
			 if (!UnityEditor.EditorUtility.DisplayDialog("Motion Controller", _EditorAnimatorSMName + " already exists. Delete and recreate it?", "Yes", "No"))
			 {
				return;
			 }

			 // Remove the sm
			 lRootStateMachine.RemoveStateMachine(lRootStateMachine.stateMachines[i].stateMachine);
		  }
		}

		UnityEditor.Animations.AnimatorStateMachine lMotionStateMachine = lRootStateMachine.AddStateMachine(_EditorAnimatorSMName);

		// Attach the behaviour if needed
		if (_EditorAttachBehaviour)
		{
		  MotionControllerBehaviour lBehaviour = lMotionStateMachine.AddStateMachineBehaviour(typeof(MotionControllerBehaviour)) as MotionControllerBehaviour;
		  lBehaviour._MotionKey = (_Key.Length > 0 ? _Key : this.GetType().FullName);
		}

		UnityEditor.Animations.AnimatorState lSlide = lMotionStateMachine.AddState("Slide", new Vector3(300, 12, 0));
		lSlide.motion = mSlide;
		lSlide.speed = 1f;

		UnityEditor.Animations.AnimatorState lIdlePose = lMotionStateMachine.AddState("IdlePose", new Vector3(540, 12, 0));
		lIdlePose.motion = mIdlePose;
		lIdlePose.speed = 1f;

		UnityEditor.Animations.AnimatorStateTransition lAnyStateTransition = null;

		// Create the transition from the any state. Note that 'AnyState' transitions have to be added to the root
		lAnyStateTransition = lRootStateMachine.AddAnyStateTransition(lSlide);
		lAnyStateTransition.hasExitTime = false;
		lAnyStateTransition.hasFixedDuration = false;
		lAnyStateTransition.exitTime = 0.9f;
		lAnyStateTransition.duration = 0.2984754f;
		lAnyStateTransition.offset = 0f;
		lAnyStateTransition.mute = false;
		lAnyStateTransition.solo = false;
		lAnyStateTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 700f, "L0MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lStateTransition = null;

		lStateTransition = lSlide.AddTransition(lIdlePose);
		lStateTransition.hasExitTime = false;
		lStateTransition.hasFixedDuration = true;
		lStateTransition.exitTime = 0.75f;
		lStateTransition.duration = 0.25f;
		lStateTransition.offset = 0f;
		lStateTransition.mute = false;
		lStateTransition.solo = false;
		lStateTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 705f, "L0MotionPhase");

	 }

	 /// <summary>
	 /// Used to show the settings that allow us to generate the animator setup.
	 /// </summary>
	 public override void OnSettingsGUI()
	 {
		UnityEditor.EditorGUILayout.IntField(new GUIContent("Phase ID", "Phase ID used to transition to the state."), PHASE_START);
		mSlide = CreateAnimationField("Slide", "Assets/ootii/Assets/MotionController/Content/Animations/Humanoid/Idling/ootii_Slide.fbx/Slide.anim", "Slide", mSlide);
		mIdlePose = CreateAnimationField("IdlePose", "Assets/Raw Mocap Data/Animations/Idle/Idle_IdleToIdlesR.fbx/IdlePose.anim", "IdlePose", mIdlePose);

		// Add the remaining functionality
		base.OnSettingsGUI();
	 }

#endif

	 // ************************************ END AUTO GENERATED ************************************
	 #endregion

  }
}