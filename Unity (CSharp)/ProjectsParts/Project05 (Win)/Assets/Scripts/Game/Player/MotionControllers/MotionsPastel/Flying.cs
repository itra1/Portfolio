using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Actors.Navigation;
using com.ootii.Cameras;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Messages;
using com.ootii.Actors;
using com.ootii.Actors.AnimationControllers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Flying")]
  [MotionDescription("Полет")]
  public class Flying : MotionControllerMotion
  {
	 // Enum values for the motion
	 public const int PHASE_UNKNOWN = 0;

	 /// <summary>
	 /// Основа для блока
	 /// </summary>
	 public const int PHASE_BLOCK = 301;
	 public const int PHASE_START = 30100;
	 public const int PHASE_LEFT_ROTATION = 30101;
	 public const int PHASE_RIGHT_ROTATION = 30102;
	 public const int PHASE_STOP = 30103;

	 private float _RotationSpeed = 90f;
	 public float RotationSpeed
	 {
		get { return _RotationSpeed; }
		set { _RotationSpeed = value; }
	 }

	 /// <summary>
	 /// Активация вращения
	 /// </summary>
	 public string _RotateActionAlias = "";
	 public string RotateActionAlias
	 {
		get { return _RotateActionAlias; }
		set { _RotateActionAlias = value; }
	 }

	 private float _ForwardSpeed = 1f;
	 public float ForwardSpeed
	 {
		get { return _ForwardSpeed; }
		set { _ForwardSpeed = value; }
	 }

	 private float _SideSpeed = 0.2f;
	 public float SideSpeed
	 {
		get { return _SideSpeed; }
		set { _SideSpeed = value; }
	 }

	 private float _Gravity = 1;
	 public float Gravity
	 {
		get { return _Gravity; }
		set { _Gravity = value; }
	 }

	 /// <summary>
	 /// Клавиша для ускорения
	 /// </summary>
	 public string _ActionForceAlias = "";
	 public string ActionForceAlias
	 {
		get { return _ActionForceAlias; }
		set { _ActionForceAlias = value; }
	 }

	 public float _AccelerationCoefficient = 1.5f;
	 public float AccelerationCoefficient
	 {
		get { return _AccelerationCoefficient; }
		set { _AccelerationCoefficient = value; }
	 }

	 private float SpeedAcceelration
	 {
		get
		{
		  if (mMotionController._InputSource == null) { return 1; }
		  return mMotionController._InputSource.IsPressed(ActionForceAlias) ? AccelerationCoefficient : 1;
		}
	 }

	 private BodyShape _bodyShape;

	 private float _timeStateActivation = 0;
	 //private com.ootii.Actors.BoneControllers.BoneControllerBone _rootBone;
	 private Game.Player.PlayerBehaviour _playerBehaviour;

	 public Flying() : base()
	 {
		_Form = -1;
		_Priority = 25;
		_ActionAlias = "Jump";

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Flying-SM"; }
#endif
	 }

	 public Flying(MotionController rController) : base(rController)
	 {
		_Form = -1;
		_Priority = 25;
		_ActionAlias = "Jump";

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Flying-SM"; }
#endif

	 }

	 /// <summary>
	 /// Вызывается после инициализации всех объектов
	 /// </summary>
	 public override void Awake()
	 {
		base.Awake();
	 }

	 private void EnableBodyFly(bool isEnable)
	 {
		_bodyShape.IsEnabledOnGround = isEnable;
		_bodyShape.IsEnabledOnSlope = isEnable;
		_bodyShape.IsEnabledAboveGround = isEnable;
		_bodyShape.UseUnityColliders = isEnable;
	 }

	 /// <summary>
	 /// Тест на активацию
	 /// </summary>
	 /// <returns></returns>
	 public override bool TestActivate()
	 {
		if (_bodyShape == null)
		{
		  _bodyShape = mActorController.GetBodyShape("Fly Body");
		  if(_bodyShape != null)
		  {
			 EnableBodyFly(false);
		  }
		}

		// Если мы не можем начать, это легко
		if (!mIsStartable)
		{
		  return false;
		}

		// Проверьте, соответствует ли условие формы нашей текущей форме по умолчанию
		if (_Form >= 0 && mMotionController.CurrentForm != _Form)
		{
		  return false;
		}

		// Если касаемся поверхности, не активируем
		if (mActorController.IsGrounded)
		{
		  return false;
		}

		// Убедитесь, что у нас есть вход для тестирования
		if (mMotionController._InputSource == null)
		{
		  return false;
		}

		// Только если нажал пробел
		if (mMotionController._InputSource.IsJustPressed(_ActionAlias))
		{

		  if (RaycastExt.SafeRaycast(mMotionController._Transform.position + mMotionController._Transform.forward * 2, Vector3.down, 2f, -1, mActorController._Transform))
		  {
			 return false;
		  }

		  if (mActorController.State.GroundSurfaceDistance < 0.25f)
			 return false;


		  return true;
		}

		return false;

	 }

	 /// <summary>
	 /// Проверка на продолжение. Если нет, то действие отключается
	 /// </summary>
	 /// <returns></returns>
	 public override bool TestUpdate()
	 {

		if (mActorController.State.IsColliding)
		  return false;

		if (mIsActivatedFrame)
		{		}
		// Если стоит, запрет активации
		if (mActorController.IsGrounded)
		{
		  return false;
		}
		if (mMotionController._InputSource.IsJustPressed(_ActionAlias))
		{
		  return false;
		}

		if (!IsInMotionState)
		{
		  // Tell this motion to get out
		  return false;
		}

		return true;
	 }

	 /// <summary>
	 /// Вызывается при активации
	 /// </summary>
	 /// <param name="rPrevMotion"></param>
	 /// <returns></returns>
	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		//_rootBone = mMotionController.GetComponent< com.ootii.Actors.BoneControllers.BoneController >().Root;
		_playerBehaviour = mMotionController.GetComponent<Game.Player.PlayerBehaviour>();
		mActorController.IsGravityEnabled = false;
		mActorController.ForceGrounding = false;
		//mActorController.FixGroundPenetration = false;
		EnableBodyFly(true);
		mActorController.State.Velocity = mActorController.AccumulatedVelocity;

		mMotionController.CameraRig.LockMode = false;
		mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START, true);

		return base.Activate(rPrevMotion);
	 }

	 /// <summary>
	 /// Вызывается для остановки действия
	 /// </summary>
	 public override void Deactivate()
	 {
		mActorController.IsGravityEnabled = true;
		mActorController.ForceGrounding = true;

		EnableBodyFly(false);
		base.Deactivate();
	 }

	 /// <summary>
	 /// Позволяет движению изменять скорости корневого движения до их применения.
	 /// 
	 /// NOTE:
	 /// Будьте осторожны при удалении вращений, так как некоторые переходы захотят вращений, 
	 /// даже если состояние, из которого они переходят, не происходит
	 /// </summary>
	 /// <param name="rDeltaTime">Время с момента последнего кадра (или фиксированного вызова обновления)</param>
	 /// <param name="rUpdateIndex">Индекс обновления для управления динамическими/фиксированными обновлениями. [0: недопустимое обновление, >=1: допустимое обновление]</param>
	 /// <param name="rVelocityDelta">Корень-линейная скорость движения относительно поступательного движения актера</param>
	 /// <param name="rRotationDelta">Скорость вращения корневого движения</param>
	 /// <returns></returns>
	 public override void UpdateRootMotion(float rDeltaTime, int rUpdateIndex, ref Vector3 rVelocityDelta, ref Quaternion rRotationDelta)
	 {
		rRotationDelta = Quaternion.identity;
	 }

	 /// <summary>
	 /// Обновляет движение с течением времени. Это вызывается контроллером каждый цикл обновления, 
	 /// так что анимации и этапы могут быть обновлены.
	 /// </summary>
	 /// <param name="rDeltaTime">Время с момента последнего кадра (или фиксированного вызова обновления)</param>
	 /// <param name="rUpdateIndex">Индекс обновления для управления динамическими/фиксированными обновлениями. [0: недопустимое обновление, >=1: допустимое обновление]</param>
	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		mMovement = Vector3.zero;
		mVelocity = Vector3.zero;
		mRotation = Quaternion.identity;

		SmoothInput();

		MotionState lState = mMotionController.State;

		int lStateID = mMotionLayer._AnimatorStateID;
		float lStateTime = mMotionLayer._AnimatorStateNormalizedTime;

		int lTransitionID = mMotionLayer._AnimatorTransitionID;
		float lTransitionTime = mMotionLayer._AnimatorTransitionNormalizedTime;

		//mMovement = mActorController._Transform.forward * _ForwardSpeed * SpeedAcceelration * rDeltaTime;

		if (lStateID != STATE_Flying && (lTransitionID == TRANS_FlyingRotateLeft_Flying || lTransitionID == TRANS_FlyingRotateRight_Flying))
		{
		  //mMotionController.CameraRig.LockMode = false;
		  mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_START, true);
		}

		if (lStateID == STATE_Flying)
		{
		  //_rootBone.SetWorldRotation(Quaternion.Euler(0,0,90), 1);
		  RotateToInput(mMotionController.State.InputFromAvatarAngle, rDeltaTime, ref mRotation);
		  //RaycastHit lMidHitInfo;
		  //// 22 секунды - время анимации приземления
		  //if (RaycastExt.SafeRaycast(mMotionController._Transform.position + mMotionController._Transform.forward*2, Vector3.down, out lMidHitInfo, Mathf.Abs(_Gravity),-1, mActorController._Transform))
		  //{
			 //mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_STOP, true);
			 //return;
		  //}


		  if (mActorController.State.GroundSurfaceDistance < 0.25f)
		  {
			 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_STOP, true);
			 return;
		  }

		  if (mMotionController._InputSource.IsPressed(_RotateActionAlias))
		  {
			 if (mMotionController.State.InputX < -0.2f)
			 {
				//mMotionController.CameraRig.LockMode = true;
				mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_LEFT_ROTATION, true);
			 }
			 if (mMotionController.State.InputX > 0.2f)
			 {
				//mMotionController.CameraRig.LockMode = true;
				mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_RIGHT_ROTATION, true);
			 }
		  }
		  else
		  {
			 if (mMotionController.State.InputX < -0.2f)
			 {
				//_rootBone.SetWorldRotation(Quaternion.Euler(20, 0, 0) * _rootBone.Skeleton.RootTransform.rotation, 1);
			 }
			 if (mMotionController.State.InputX > 0.2f)
			 {
				//_rootBone.SetWorldRotation(Quaternion.Euler(-20, 0, 0) * _rootBone.Skeleton.RootTransform.rotation, 1);
			 }
		  }

		  //_timeStateActivation;

		}
		else if (lStateID == STATE_FlyingStop)
		{
		  mActorController.ForceGrounding = true;
		  mActorController.IsGravityEnabled = true;

		}
		else if (lStateID == STATE_FlyingRotateLeft)
		{
		  //mMovement += mActorController._Transform.right * -_SideSpeed * rDeltaTime;
		}
		else if (lStateID == STATE_FlyingRotateRight)
		{
		  //mMovement += -mActorController._Transform.right * _SideSpeed * rDeltaTime;
		}
		//mMovement += Vector3.up * -Gravity * rDeltaTime;

		//mMovement += _playerBehaviour.Wind * rDeltaTime;
		//mVelocity = mActorController._Transform.up * 3;
		//mVelocity += mActorController._Transform.forward * 5;

		mVelocity = CalculateVelocity(rDeltaTime);

		if (lStateID == STATE_FlyingStop)
		  mVelocity = Vector3.zero;

	 }

	 private Vector3 CalculateVelocity(float deltaTime)
	 {
		Vector3 vel = mActorController._Transform.forward * _ForwardSpeed * SpeedAcceelration;
		vel += Vector3.up * -Gravity;
		vel += _playerBehaviour.Wind;


		if(Mathf.Abs(mActorController.State.Velocity.magnitude - vel.magnitude)<= 0.1f)
		  return vel;

		Vector3 actualVelocity = mActorController.State.Velocity;

		float Ydelta = vel.y- mActorController.State.Velocity.y;
		Vector3 currentHoriz = actualVelocity;
		currentHoriz.y = 0;
		Vector3 targetHoriz = vel;
		targetHoriz.y = 0;
		float Hdelta = targetHoriz.magnitude - currentHoriz.magnitude;

		if(Hdelta != 0)
		{

		  float chenageH = Mathf.Sign(Hdelta) * 3 * deltaTime;
		  if (chenageH > Mathf.Abs(Hdelta))
		  {
			// currentHoriz = currentHoriz.normalized * targetHoriz.magnitude;
			 actualVelocity = mActorController._Transform.forward * targetHoriz.magnitude;
		  }
		  else
		  {
			 //float magn = currentHoriz.magnitude;
			 //currentHoriz = currentHoriz.normalized * (currentHoriz.magnitude - chenageH);
			 actualVelocity = mActorController._Transform.forward * (currentHoriz.magnitude + chenageH);
		  }
		}
		actualVelocity.y = mActorController.State.Velocity.y;
		if (Ydelta != 0)
		{
		  float chenageY = Mathf.Sign(Ydelta) * 3 * deltaTime;
		  if (chenageY > Mathf.Abs(Ydelta))
			 actualVelocity.y = vel.y;
		  actualVelocity.y += chenageY;
		}


		return actualVelocity;
	 }

	 /// <summary>
	 /// We use these classes to help smooth the input values so that
	 /// movement doesn't drop from 1 to 0 immediately.
	 /// </summary>
	 protected FloatValue mInputX = new FloatValue(0f, 10);
	 protected FloatValue mInputY = new FloatValue(0f, 10);
	 protected FloatValue mInputMagnitude = new FloatValue(0f, 15);

	 protected void SmoothInput()
	 {
		MotionState lState = mMotionController.State;

		// Convert the input to radial so we deal with keyboard and gamepad input the same.
		float lInputMax = 1;

		float lInputX = Mathf.Clamp(lState.InputX, -lInputMax, lInputMax);
		float lInputY = Mathf.Clamp(lState.InputY, -lInputMax, lInputMax);
		float lInputMagnitude = Mathf.Clamp(lState.InputMagnitudeTrend.Value, 0f, lInputMax);
		InputManagerHelper.ConvertToRadialInput(ref lInputX, ref lInputY, ref lInputMagnitude);

		// Smooth the input
		mInputX.Add(lInputX);
		mInputY.Add(lInputY);
		mInputMagnitude.Add(lInputMagnitude);

		// Modify the input values to add some lag
		mMotionController.State.InputX = mInputX.Average;
		mMotionController.State.InputY = mInputY.Average;
		mMotionController.State.InputMagnitudeTrend.Replace(mInputMagnitude.Average);
	 }
	 protected void RotateToInput(float rInputFromAvatarAngle, float rDeltaTime, ref Quaternion rRotation)
	 {
		if (rInputFromAvatarAngle != 0f)
		{
		  if (_RotationSpeed > 0f && Mathf.Abs(rInputFromAvatarAngle) > _RotationSpeed * rDeltaTime)
		  {
			 rInputFromAvatarAngle = Mathf.Sign(rInputFromAvatarAngle) * _RotationSpeed * rDeltaTime;
		  }

		  rRotation = Quaternion.Euler(0f, rInputFromAvatarAngle, 0f);
		}
	 }

	 /// <summary>
	 /// Вызывается контроллером при получении сообщения
	 /// </summary>
	 public override void OnMessageReceived(IMessage rMessage)
	 {
		if (rMessage == null) { return; }
	 }


	 /// <summary>
	 /// Когда мы хотим повернуть в зависимости от направления камеры, нам нужно настроить 
	 /// поворот актера после обработки камеры. В противном случае, мы можем получить небольшие 
	 /// заикания во время вращения камеры.
	 /// 
	 /// Это единственный способ держать их полностью в синхронизации. Это также означает, 
	 /// что мы не можем запустить ни один из наших AC-процессоров, поскольку AC уже запущен. 
	 /// Таким образом, мы делаем минимальную работу здесь
	 /// </summary>
	 /// <param name="rDeltaTime"></param>
	 /// <param name="rUpdateIndex"></param>
	 /// <param name="rCamera"></param>
	 private void OnCameraUpdated(float rDeltaTime, int rUpdateIndex, BaseCameraRig rCamera)
	 {

	 }

#if UNITY_EDITOR
	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;

		string lNewActionAlias = EditorGUILayout.TextField(new GUIContent("Action Alias", "Action alias that triggers a climb."), ActionAlias, GUILayout.MinWidth(30));
		if (lNewActionAlias != ActionAlias)
		{
		  lIsDirty = true;
		  ActionAlias = lNewActionAlias;
		}

		string lNewRotateActionAlias = EditorGUILayout.TextField(new GUIContent("Rotate Action Alias", "Action alias that rotate side"), RotateActionAlias, GUILayout.MinWidth(30));
		if (lNewRotateActionAlias != RotateActionAlias)
		{
		  lIsDirty = true;
		  RotateActionAlias = lNewRotateActionAlias;
		}

		float lNewForwardSpeed = EditorGUILayout.FloatField(new GUIContent("Forward Speed", "Maximum Forward Speed"), ForwardSpeed, GUILayout.MinWidth(30));
		if (lNewForwardSpeed != ForwardSpeed)
		{
		  lIsDirty = true;
		  ForwardSpeed = lNewForwardSpeed;
		}

		float lNewRotationSpeed = EditorGUILayout.FloatField(new GUIContent("Rotation Speed", "Maximum Rotation Speed"), RotationSpeed, GUILayout.MinWidth(30));
		if (lNewRotationSpeed != RotationSpeed)
		{
		  lIsDirty = true;
		  RotationSpeed = lNewRotationSpeed;
		}

		float lNewGravity = EditorGUILayout.FloatField(new GUIContent("Gravity", "Maximum Gravity"), Gravity, GUILayout.MinWidth(30));
		if (lNewGravity != Gravity)
		{
		  lIsDirty = true;
		  Gravity = lNewGravity;
		}

		float lNewSideSpeed = EditorGUILayout.FloatField(new GUIContent("Side Speed", "Side Speed"), SideSpeed, GUILayout.MinWidth(30));
		if (lNewSideSpeed != SideSpeed)
		{
		  lIsDirty = true;
		  SideSpeed = lNewSideSpeed;
		}

		string lNewActionForceAlias = EditorGUILayout.TextField(new GUIContent("Action Force Alias", "Action alias that triggers a climb."), ActionForceAlias, GUILayout.MinWidth(30));
		if (lNewActionForceAlias != ActionForceAlias)
		{
		  lIsDirty = true;
		  ActionForceAlias = lNewActionForceAlias;
		}

		float lNewAccelerationCoefficient = EditorGUILayout.FloatField(new GUIContent("Acceleration Coefficient", "Коеффициент ускорения"), AccelerationCoefficient, GUILayout.MinWidth(30));
		if (lNewAccelerationCoefficient != AccelerationCoefficient)
		{
		  lIsDirty = true;
		  AccelerationCoefficient = lNewAccelerationCoefficient;
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
	 public int STATE_Start = -1;
	 public int STATE_FlyingRotateLeft = -1;
	 public int STATE_FlyingRotateRight = -1;
	 public int STATE_FlyingStop = -1;
	 public int STATE_Flying = -1;
	 public int TRANS_AnyState_Flying = -1;
	 public int TRANS_EntryState_Flying = -1;
	 public int TRANS_FlyingRotateLeft_Flying = -1;
	 public int TRANS_FlyingRotateRight_Flying = -1;
	 public int TRANS_Flying_FlyingRotateLeft = -1;
	 public int TRANS_Flying_FlyingRotateRight = -1;
	 public int TRANS_Flying_FlyingStop = -1;

	 /// <summary>
	 /// Determines if we're using auto-generated code
	 /// </summary>
	 public override bool HasAutoGeneratedCode
	 {
		get { return true; }
	 }

	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsInMotionState
	 {
		get
		{
		  int lStateID = mMotionLayer._AnimatorStateID;
		  int lTransitionID = mMotionLayer._AnimatorTransitionID;

		  if (lTransitionID == 0)
		  {
			 if (lStateID == STATE_Start) { return true; }
			 if (lStateID == STATE_FlyingRotateLeft) { return true; }
			 if (lStateID == STATE_FlyingRotateRight) { return true; }
			 if (lStateID == STATE_FlyingStop) { return true; }
			 if (lStateID == STATE_Flying) { return true; }
		  }

		  if (lTransitionID == TRANS_AnyState_Flying) { return true; }
		  if (lTransitionID == TRANS_EntryState_Flying) { return true; }
		  if (lTransitionID == TRANS_FlyingRotateLeft_Flying) { return true; }
		  if (lTransitionID == TRANS_FlyingRotateRight_Flying) { return true; }
		  if (lTransitionID == TRANS_Flying_FlyingRotateLeft) { return true; }
		  if (lTransitionID == TRANS_Flying_FlyingRotateRight) { return true; }
		  if (lTransitionID == TRANS_Flying_FlyingStop) { return true; }
		  return false;
		}
	 }

	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsMotionState(int rStateID)
	 {
		if (rStateID == STATE_Start) { return true; }
		if (rStateID == STATE_FlyingRotateLeft) { return true; }
		if (rStateID == STATE_FlyingRotateRight) { return true; }
		if (rStateID == STATE_FlyingStop) { return true; }
		if (rStateID == STATE_Flying) { return true; }
		return false;
	 }

	 /// <summary>
	 /// Used to determine if the actor is in one of the states for this motion
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsMotionState(int rStateID, int rTransitionID)
	 {
		if (rTransitionID == 0)
		{
		  if (rStateID == STATE_Start) { return true; }
		  if (rStateID == STATE_FlyingRotateLeft) { return true; }
		  if (rStateID == STATE_FlyingRotateRight) { return true; }
		  if (rStateID == STATE_FlyingStop) { return true; }
		  if (rStateID == STATE_Flying) { return true; }
		}

		if (rTransitionID == TRANS_AnyState_Flying) { return true; }
		if (rTransitionID == TRANS_EntryState_Flying) { return true; }
		if (rTransitionID == TRANS_FlyingRotateLeft_Flying) { return true; }
		if (rTransitionID == TRANS_FlyingRotateRight_Flying) { return true; }
		if (rTransitionID == TRANS_Flying_FlyingRotateLeft) { return true; }
		if (rTransitionID == TRANS_Flying_FlyingRotateRight) { return true; }
		if (rTransitionID == TRANS_Flying_FlyingStop) { return true; }
		return false;
	 }

	 /// <summary>
	 /// Preprocess any animator data so the motion can use it later
	 /// </summary>
	 public override void LoadAnimatorData()
	 {
		string lLayer = mMotionController.Animator.GetLayerName(mMotionLayer._AnimatorLayerIndex);
		TRANS_AnyState_Flying = mMotionController.AddAnimatorName("AnyState -> " + lLayer + ".Flying-SM.Flying");
		TRANS_EntryState_Flying = mMotionController.AddAnimatorName("Entry -> " + lLayer + ".Flying-SM.Flying");
		STATE_Start = mMotionController.AddAnimatorName("" + lLayer + ".Start");
		STATE_FlyingRotateLeft = mMotionController.AddAnimatorName("" + lLayer + ".Flying-SM.FlyingRotateLeft");
		TRANS_FlyingRotateLeft_Flying = mMotionController.AddAnimatorName("" + lLayer + ".Flying-SM.FlyingRotateLeft -> " + lLayer + ".Flying-SM.Flying");
		STATE_FlyingRotateRight = mMotionController.AddAnimatorName("" + lLayer + ".Flying-SM.FlyingRotateRight");
		TRANS_FlyingRotateRight_Flying = mMotionController.AddAnimatorName("" + lLayer + ".Flying-SM.FlyingRotateRight -> " + lLayer + ".Flying-SM.Flying");
		STATE_FlyingStop = mMotionController.AddAnimatorName("" + lLayer + ".Flying-SM.FlyingStop");
		STATE_Flying = mMotionController.AddAnimatorName("" + lLayer + ".Flying-SM.Flying");
		TRANS_Flying_FlyingRotateLeft = mMotionController.AddAnimatorName("" + lLayer + ".Flying-SM.Flying -> " + lLayer + ".Flying-SM.FlyingRotateLeft");
		TRANS_Flying_FlyingRotateRight = mMotionController.AddAnimatorName("" + lLayer + ".Flying-SM.Flying -> " + lLayer + ".Flying-SM.FlyingRotateRight");
		TRANS_Flying_FlyingStop = mMotionController.AddAnimatorName("" + lLayer + ".Flying-SM.Flying -> " + lLayer + ".Flying-SM.FlyingStop");
	 }

#if UNITY_EDITOR

	 /// <summary>
	 /// New way to create sub-state machines without destroying what exists first.
	 /// </summary>
	 protected override void CreateStateMachine()
	 {
		int rLayerIndex = mMotionLayer._AnimatorLayerIndex;
		MotionController rMotionController = mMotionController;

		UnityEditor.Animations.AnimatorController lController = null;

		Animator lAnimator = rMotionController.Animator;
		if (lAnimator == null) { lAnimator = rMotionController.gameObject.GetComponent<Animator>(); }
		if (lAnimator != null) { lController = lAnimator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController; }
		if (lController == null) { return; }

		while (lController.layers.Length <= rLayerIndex)
		{
		  UnityEditor.Animations.AnimatorControllerLayer lNewLayer = new UnityEditor.Animations.AnimatorControllerLayer();
		  lNewLayer.name = "Layer " + (lController.layers.Length + 1);
		  lNewLayer.stateMachine = new UnityEditor.Animations.AnimatorStateMachine();
		  lController.AddLayer(lNewLayer);
		}

		UnityEditor.Animations.AnimatorControllerLayer lLayer = lController.layers[rLayerIndex];

		UnityEditor.Animations.AnimatorStateMachine lLayerStateMachine = lLayer.stateMachine;

		UnityEditor.Animations.AnimatorStateMachine lSSM_185724 = MotionControllerMotion.EditorFindSSM(lLayerStateMachine, "Flying-SM");
		if (lSSM_185724 == null) { lSSM_185724 = lLayerStateMachine.AddStateMachine("Flying-SM", new Vector3(390, 50, 0)); }

		UnityEditor.Animations.AnimatorState lState_186370 = MotionControllerMotion.EditorFindState(lSSM_185724, "FlyingRotateLeft");
		if (lState_186370 == null) { lState_186370 = lSSM_185724.AddState("FlyingRotateLeft", new Vector3(50, -40, 0)); }
		lState_186370.speed = 1f;
		lState_186370.mirror = false;
		lState_186370.tag = "";
		lState_186370.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/NineAnimations/Flying.fbx", "FlyingRotateLeft");

		UnityEditor.Animations.AnimatorState lState_186372 = MotionControllerMotion.EditorFindState(lSSM_185724, "FlyingRotateRight");
		if (lState_186372 == null) { lState_186372 = lSSM_185724.AddState("FlyingRotateRight", new Vector3(0, 210, 0)); }
		lState_186372.speed = 1f;
		lState_186372.mirror = false;
		lState_186372.tag = "";
		lState_186372.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/NineAnimations/Flying.fbx", "FlyingRotateRight");

		UnityEditor.Animations.AnimatorState lState_186374 = MotionControllerMotion.EditorFindState(lSSM_185724, "FlyingStop");
		if (lState_186374 == null) { lState_186374 = lSSM_185724.AddState("FlyingStop", new Vector3(200, 90, 0)); }
		lState_186374.speed = 2f;
		lState_186374.mirror = false;
		lState_186374.tag = "";
		lState_186374.motion = MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/NineAnimations/Flying.fbx", "FlyingStop");

		UnityEditor.Animations.AnimatorState lState_186376 = MotionControllerMotion.EditorFindState(lSSM_185724, "Flying");
		if (lState_186376 == null) { lState_186376 = lSSM_185724.AddState("Flying", new Vector3(-70, 90, 0)); }
		lState_186376.speed = 1f;
		lState_186376.mirror = false;
		lState_186376.tag = "";

		UnityEditor.Animations.BlendTree lM_189186 = MotionControllerMotion.EditorCreateBlendTree("Blend Tree", lController, rLayerIndex);
		lM_189186.blendType = UnityEditor.Animations.BlendTreeType.Simple1D;
		lM_189186.blendParameter = "InputX";
		lM_189186.blendParameterY = "InputX";
#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3)
		lM_189186.useAutomaticThresholds = true;
#endif
		lM_189186.AddChild(MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/NineAnimations/NineTails.fbx", "Gliding"), -1f);
		lM_189186.AddChild(MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/NineAnimations/NineTails.fbx", "Gliding"), 0f);
		lM_189186.AddChild(MotionControllerMotion.EditorFindAnimationClip("Assets/Content/Models/Pastel/NineAnimations/NineTails.fbx", "Gliding"), 1f);
		lState_186376.motion = lM_189186;

		UnityEditor.Animations.AnimatorStateTransition lAnyTransition_185798 = MotionControllerMotion.EditorFindAnyStateTransition(lLayerStateMachine, lState_186376, 0);
		if (lAnyTransition_185798 == null) { lAnyTransition_185798 = lLayerStateMachine.AddAnyStateTransition(lState_186376); }
		lAnyTransition_185798.isExit = false;
		lAnyTransition_185798.hasExitTime = false;
		lAnyTransition_185798.hasFixedDuration = true;
		lAnyTransition_185798.exitTime = 0.75f;
		lAnyTransition_185798.duration = 0.25f;
		lAnyTransition_185798.offset = 0f;
		lAnyTransition_185798.mute = false;
		lAnyTransition_185798.solo = false;
		lAnyTransition_185798.canTransitionToSelf = false;
		lAnyTransition_185798.orderedInterruption = true;
		lAnyTransition_185798.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lAnyTransition_185798.conditions.Length - 1; i >= 0; i--) { lAnyTransition_185798.RemoveCondition(lAnyTransition_185798.conditions[i]); }
		lAnyTransition_185798.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 30100f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_189232 = MotionControllerMotion.EditorFindTransition(lState_186370, lState_186376, 0);
		if (lTransition_189232 == null) { lTransition_189232 = lState_186370.AddTransition(lState_186376); }
		lTransition_189232.isExit = false;
		lTransition_189232.hasExitTime = true;
		lTransition_189232.hasFixedDuration = true;
		lTransition_189232.exitTime = 0.9135472f;
		lTransition_189232.duration = 0.0810014f;
		lTransition_189232.offset = 0f;
		lTransition_189232.mute = false;
		lTransition_189232.solo = false;
		lTransition_189232.canTransitionToSelf = true;
		lTransition_189232.orderedInterruption = true;
		lTransition_189232.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_189232.conditions.Length - 1; i >= 0; i--) { lTransition_189232.RemoveCondition(lTransition_189232.conditions[i]); }

		UnityEditor.Animations.AnimatorStateTransition lTransition_189484 = MotionControllerMotion.EditorFindTransition(lState_186372, lState_186376, 0);
		if (lTransition_189484 == null) { lTransition_189484 = lState_186372.AddTransition(lState_186376); }
		lTransition_189484.isExit = false;
		lTransition_189484.hasExitTime = true;
		lTransition_189484.hasFixedDuration = true;
		lTransition_189484.exitTime = 0.8990617f;
		lTransition_189484.duration = 0.0749656f;
		lTransition_189484.offset = 0f;
		lTransition_189484.mute = false;
		lTransition_189484.solo = false;
		lTransition_189484.canTransitionToSelf = true;
		lTransition_189484.orderedInterruption = true;
		lTransition_189484.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_189484.conditions.Length - 1; i >= 0; i--) { lTransition_189484.RemoveCondition(lTransition_189484.conditions[i]); }

		UnityEditor.Animations.AnimatorStateTransition lTransition_189180 = MotionControllerMotion.EditorFindTransition(lState_186376, lState_186370, 0);
		if (lTransition_189180 == null) { lTransition_189180 = lState_186376.AddTransition(lState_186370); }
		lTransition_189180.isExit = false;
		lTransition_189180.hasExitTime = false;
		lTransition_189180.hasFixedDuration = true;
		lTransition_189180.exitTime = 0.7897944f;
		lTransition_189180.duration = 0.06454048f;
		lTransition_189180.offset = 0f;
		lTransition_189180.mute = false;
		lTransition_189180.solo = false;
		lTransition_189180.canTransitionToSelf = true;
		lTransition_189180.orderedInterruption = true;
		lTransition_189180.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_189180.conditions.Length - 1; i >= 0; i--) { lTransition_189180.RemoveCondition(lTransition_189180.conditions[i]); }
		lTransition_189180.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 30101f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_189182 = MotionControllerMotion.EditorFindTransition(lState_186376, lState_186372, 0);
		if (lTransition_189182 == null) { lTransition_189182 = lState_186376.AddTransition(lState_186372); }
		lTransition_189182.isExit = false;
		lTransition_189182.hasExitTime = false;
		lTransition_189182.hasFixedDuration = true;
		lTransition_189182.exitTime = 0.8755555f;
		lTransition_189182.duration = 0.04307273f;
		lTransition_189182.offset = 0f;
		lTransition_189182.mute = false;
		lTransition_189182.solo = false;
		lTransition_189182.canTransitionToSelf = true;
		lTransition_189182.orderedInterruption = true;
		lTransition_189182.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_189182.conditions.Length - 1; i >= 0; i--) { lTransition_189182.RemoveCondition(lTransition_189182.conditions[i]); }
		lTransition_189182.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 30102f, "L" + rLayerIndex + "MotionPhase");

		UnityEditor.Animations.AnimatorStateTransition lTransition_189184 = MotionControllerMotion.EditorFindTransition(lState_186376, lState_186374, 0);
		if (lTransition_189184 == null) { lTransition_189184 = lState_186376.AddTransition(lState_186374); }
		lTransition_189184.isExit = false;
		lTransition_189184.hasExitTime = false;
		lTransition_189184.hasFixedDuration = true;
		lTransition_189184.exitTime = 0.80619f;
		lTransition_189184.duration = 0.2245874f;
		lTransition_189184.offset = 0.3061132f;
		lTransition_189184.mute = false;
		lTransition_189184.solo = false;
		lTransition_189184.canTransitionToSelf = true;
		lTransition_189184.orderedInterruption = true;
		lTransition_189184.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)0;
		for (int i = lTransition_189184.conditions.Length - 1; i >= 0; i--) { lTransition_189184.RemoveCondition(lTransition_189184.conditions[i]); }
		lTransition_189184.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 30103f, "L" + rLayerIndex + "MotionPhase");


		// Run any post processing after creating the state machine
		OnStateMachineCreated();
	 }

#endif

	 // ************************************ END AUTO GENERATED ************************************
	 #endregion


  }
}