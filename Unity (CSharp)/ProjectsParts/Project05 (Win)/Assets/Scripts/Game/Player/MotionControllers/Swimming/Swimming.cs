using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Actors.Navigation;
using com.ootii.Cameras;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Actors.AnimationControllers;
using com.ootii.Messages;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
	/// <summary>
	/// Плавание
	/// </summary>
	[MotionName("Swimming")]
	[MotionDescription("Плавание")]
	public class Swimming : MotionControllerMotion
	{
		// Enum values for the motion
		public const int PHASE_UNKNOWN = 0;

		/// <summary>
		/// Основа для блока
		/// </summary>
		public const int PHASE_BLOCK = 302;

		public const int PHASE_DIVE = 30201;
		public const int PHASE_ABOVEWATER = 30202;
		public const int PHASE_UNDERWATER = 30203;


		private float _ForwardSpeed = 1f;
		public float ForwardSpeed
		{
			get { return _ForwardSpeed; }
			set { _ForwardSpeed = value; }
		}

		/// <summary>
		/// Determines if we run by default or walk
		/// </summary>
		public bool _DefaultToRun = false;
		public bool DefaultToRun
		{
			get { return _DefaultToRun; }
			set { _DefaultToRun = value; }
		}

		/// <summary>
		/// Degrees per second to rotate the actor in order to face the input direction
		/// </summary>
		public float _RotationSpeed = 60f;
		public float RotationSpeed
		{
			get { return _RotationSpeed; }
			set { _RotationSpeed = value; }
		}

		public Swimming() : base()
		{
			_Form = -1;
			_Priority = 1;

#if UNITY_EDITOR
			if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Swimming-SM"; }
#endif
		}

		public Swimming(MotionController rController) : base(rController)
		{
			_Form = -1;
			_Priority = 1;

#if UNITY_EDITOR
			if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Swimming-SM"; }
#endif

		}

		/// <summary>
		/// Инициализация, вызывает после анициализации и до вызова движений
		/// </summary>
		public override void Initialize()
		{
		}

		/// <summary>
		/// Вызывается после инициализации всех объектов
		/// </summary>
		public override void Awake()
		{
			base.Awake();
		}

		/// <summary>
		/// Тест на активацию
		/// </summary>
		/// <returns></returns>
		public override bool TestActivate()
		{
			return true;
		}

		/// <summary>
		/// Проверка на продолжение. Если нет, то действие отключается
		/// </summary>
		/// <returns></returns>
		public override bool TestUpdate()
		{

			if (mIsActivatedFrame)
			{
				return true;
			}

			return true;
		}

		/// <summary>
		/// Determines if the actor should be running based on input
		/// </summary>
		public virtual bool IsRunActive
		{
			get
			{
				if (mMotionController.TargetNormalizedSpeed > 0f && mMotionController.TargetNormalizedSpeed <= 0.5f) { return false; }
				if (mMotionController._InputSource == null) { return _DefaultToRun; }
				return ((_DefaultToRun && !mMotionController._InputSource.IsPressed(_ActionAlias)) || (!_DefaultToRun && mMotionController._InputSource.IsPressed(_ActionAlias)));
			}
		}

		/// <summary>
		/// Вызывается при активации
		/// </summary>
		/// <param name="rPrevMotion"></param>
		/// <returns></returns>
		public override bool Activate(MotionControllerMotion rPrevMotion)
		{

			mActorController.IsGravityEnabled = false;
			mActorController.ForceGrounding = false;
			mActorController.FixGroundPenetration = false;

			mMotionController.CameraRig.LockMode = false;
			mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_UNDERWATER, true);

			return base.Activate(rPrevMotion);
		}

		/// <summary>
		/// Вызывается для остановки действия
		/// </summary>
		public override void Deactivate()
		{
			mActorController.IsGravityEnabled = true;
			mActorController.ForceGrounding = true;
			mActorController.FixGroundPenetration = true;
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
			mRotation = Quaternion.identity;

			int lStateID = mMotionLayer._AnimatorStateID;
			float lStateTime = mMotionLayer._AnimatorStateNormalizedTime;

			int lTransitionID = mMotionLayer._AnimatorTransitionID;
			float lTransitionTime = mMotionLayer._AnimatorTransitionNormalizedTime;

			//mMovement = mActorController._Transform.forward * _ForwardSpeed;

			if(lStateID != PHASE_DIVE)
			{
				mMovement = mActorController._Transform.forward * _ForwardSpeed * mMotionController.State.InputY;

				// Smooth the input so we don't start and stop immediately in the blend tree.
				SmoothInput();

				// Use the AC to rotate the character towards the input
				RotateToInput(mMotionController.State.InputFromAvatarAngle, rDeltaTime, ref mRotation);

			}

		}

		/// <summary>
		/// We use these classes to help smooth the input values so that
		/// movement doesn't drop from 1 to 0 immediately.
		/// </summary>
		protected FloatValue mInputX = new FloatValue(0f, 10);
		protected FloatValue mInputY = new FloatValue(0f, 10);
		protected FloatValue mInputMagnitude = new FloatValue(0f, 15);

		/// <summary>
		/// We smooth the input so that we don't start and stop immediately in the blend tree. That can create pops.
		/// </summary>
		protected void SmoothInput()
		{
			MotionState lState = mMotionController.State;

			// Convert the input to radial so we deal with keyboard and gamepad input the same.
			float lInputMax = (IsRunActive ? 1f : 0.5f);

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

		/// <summary>
		/// Create a rotation velocity that rotates the character based on input
		/// </summary>
		/// <param name="rInputFromAvatarAngle"></param>
		/// <param name="rDeltaTime"></param>
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
		/// Возвращает текущую скорость движения
		/// </summary>
		protected Vector3 DetermineVelocity(bool rAllowSlide)
		{
			Vector3 lVelocity = Vector3.zero;
			return lVelocity;
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

			float lNewForwardSpeed = EditorGUILayout.FloatField(new GUIContent("Forward Speed", "Maximum Forward Speed"), ForwardSpeed, GUILayout.MinWidth(30));
			if (lNewForwardSpeed != ForwardSpeed)
			{
				lIsDirty = true;
				ForwardSpeed = lNewForwardSpeed;
			}

			return lIsDirty;
		}

#endif

		#region Auto-Generated

		/// <summary>
		/// Эти объявления идут внутри класса, поэтому вы можете проверить, какие состояния и переходы активны. Тестирование хеш-значений выполняется намного быстрее, чем строки.
		/// </summary>
		public static int STATE_AboveWater = -1;
		public static int STATE_UnderWater = -1;
		public static int STATE_SwinDive = -1;

		public static int TRANS_EntryState_AboveWater = -1;
		public static int TRANS_EntryState_UnderWater = -1;
		public static int TRANS_AboveWater_SwinDive = -1;
		public static int TRANS_SwinDive_UnderWater = -1;
		public static int TRANS_UnderWater_AboveWater = -1;

		/// <summary>
		/// Используется для определения, находится ли актер в одном из состояний для этого движения
		/// </summary>
		/// <returns></returns>
		public override bool IsInMotionState
		{
			get
			{
				int lStateID = mMotionLayer._AnimatorStateID;
				int lTransitionID = mMotionLayer._AnimatorTransitionID;

				if (lStateID == STATE_AboveWater) { return true; }
				if (lStateID == STATE_UnderWater) { return true; }
				if (lStateID == STATE_SwinDive) { return true; }
				if (lTransitionID == TRANS_EntryState_AboveWater) { return true; }
				if (lTransitionID == TRANS_EntryState_UnderWater) { return true; }
				if (lTransitionID == TRANS_AboveWater_SwinDive) { return true; }
				if (lTransitionID == TRANS_SwinDive_UnderWater) { return true; }
				if (lTransitionID == TRANS_UnderWater_AboveWater) { return true; }

				return false;
			}
		}
		/// <summary>
		/// Используется для определения, находится ли актер в одном из состояний для этого движения
		/// </summary>
		/// <returns></returns>
		public override bool IsMotionState(int rStateID)
		{

			if (rStateID == STATE_AboveWater) { return true; }
			if (rStateID == STATE_UnderWater) { return true; }
			if (rStateID == STATE_SwinDive) { return true; }
			return false;
		}

		/// <summary>
		/// Используется для определения, находится ли актер в одном из состояний для этого движения
		/// </summary>
		/// <returns></returns>
		public override bool IsMotionState(int rStateID, int rTransitionID)
		{

			if (rStateID == STATE_AboveWater) { return true; }
			if (rStateID == STATE_UnderWater) { return true; }
			if (rStateID == STATE_SwinDive) { return true; }
			if (rTransitionID == TRANS_EntryState_AboveWater) { return true; }
			if (rTransitionID == TRANS_EntryState_UnderWater) { return true; }
			if (rTransitionID == TRANS_AboveWater_SwinDive) { return true; }
			if (rTransitionID == TRANS_SwinDive_UnderWater) { return true; }
			if (rTransitionID == TRANS_UnderWater_AboveWater) { return true; }
			return false;
		}
		/// <summary>
		/// Предварительно обработайте любые данные аниматора, чтобы движение могло использовать их позже
		/// </summary>
		public override void LoadAnimatorData()
		{
			STATE_AboveWater = mMotionController.AddAnimatorName("Base Layer.Swimming-SM.AboveWater");
			STATE_UnderWater = mMotionController.AddAnimatorName("Base Layer.Swimming-SM.UnderWater");
			STATE_SwinDive = mMotionController.AddAnimatorName("Base Layer.Swimming-SM.SwinDive");

			TRANS_EntryState_AboveWater = mMotionController.AddAnimatorName("Entry -> Base Layer.Swimming-SM.AboveWater");
			TRANS_EntryState_UnderWater = mMotionController.AddAnimatorName("Entry -> Base Layer.Swimming-SM.UnderWater");
			TRANS_AboveWater_SwinDive = mMotionController.AddAnimatorName("Base Layer.Swimming-SM.AboveWater -> Base Layer.Swimming-SM.SwinDive");
			TRANS_SwinDive_UnderWater = mMotionController.AddAnimatorName("Base Layer.Swimming-SM.UnderWater -> Base Layer.Swimming-SM.FlyingRotateLeft");
			TRANS_UnderWater_AboveWater = mMotionController.AddAnimatorName("Base Layer.Swimming-SM.UnderWater -> Base Layer.Swimming-SM.AboveWater");
		}

#if UNITY_EDITOR

		/// <summary>
		/// Создает подсистему аниматора для этого движения.
		/// </summary>
		protected override void CreateStateMachine()
		{
		}

		/// <summary>
		/// Используется для отображения настроек, которые позволяют нам создавать настройки аниматора.
		/// </summary>
		public override void OnSettingsGUI()
		{
			// Add the remaining functionality
			base.OnSettingsGUI();
		}

#endif


		#endregion

	}
}