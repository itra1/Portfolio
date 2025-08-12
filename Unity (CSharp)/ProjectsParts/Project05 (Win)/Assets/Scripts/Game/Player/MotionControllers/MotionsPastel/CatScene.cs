using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Actors.Navigation;
using com.ootii.Cameras;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Messages;
using com.ootii.Actors.AnimationControllers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Cat Scene")]
  [MotionDescription("Cat scene")]
  public class CatScene : MotionControllerMotion
  {
	 private readonly string _animName = "CatScene";

	 // Enum values for the motion
	 public const int PHASE_UNKNOWN = 0;

	 /// <summary>
	 /// Основа для блока
	 /// </summary>
	 public const int PHASE_BLOCK = 777;
	 /// <summary>
	 /// Бездействие
	 /// </summary>
	 public const int PHASE_DEFAULT = 777000;

	 /// <summary>
	 /// Первый уровень: Стартовая анимация
	 /// </summary>
	 public const int PHASE_L1_START = 777001;

	 /// <summary>
	 /// Первый уровень: Конечная анимация при использвании портала
	 /// </summary>
	 public const int PHASE_L1_SHINISH = 777002;

	 private Game.Player.PlayerBehaviour _playerBehaviour;

	 public CatScene() : base()
	 {
		_Form = -1;
		_Priority = 999;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = _animName; }
#endif
	 }

	 public CatScene(MotionController rController) : base(rController)
	 {
		_Form = -1;
		_Priority = 999;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = _animName; }
#endif


	 }
	 /// <summary>
	 /// Инициализация, вызывает после анициализации и до вызова движений
	 /// </summary>
	 public override void Initialize()
	 {
		_playerBehaviour = mMotionController.GetComponent<PlayerBehaviour>();
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
		// Если мы не можем начать, это легко
		if (!mIsStartable)
		{
		  return false;
		}

		//if (_playerBehaviour.CatScene)
		//  return true;

		return false;

	 }

	 /// <summary>
	 /// Проверка на продолжение. Если нет, то действие отключается
	 /// </summary>
	 /// <returns></returns>
	 public override bool TestUpdate()
	 {

		if (mIsActivatedFrame)
		{
		  // Если стоит, запрет активации
		  if (mActorController.IsGrounded)
		  {
			 return false;
		  }
		}

		//if (!_playerBehaviour.CatScene)
		//  return false;

		return true;
	 }

	 /// <summary>
	 /// Вызывается при активации
	 /// </summary>
	 /// <param name="rPrevMotion"></param>
	 /// <returns></returns>
	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {

		//switch (_playerBehaviour.CatSceneNum)
		//{
		//  case 1:
		//	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_L1_START, true);
		//	 break;
		//  case 2:
		//	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_L1_SHINISH, true);
		//	 break;
		//  case 0:
		//  default:
		//	 mMotionController.SetAnimatorMotionPhase(mMotionLayer._AnimatorLayerIndex, PHASE_DEFAULT, true);
		//	 break;
		//}

		return base.Activate(rPrevMotion);

	 }

	 /// <summary>
	 /// Обновляет движение с течением времени. Это вызывается контроллером каждый цикл обновления, 
	 /// так что анимации и этапы могут быть обновлены.
	 /// </summary>
	 /// <param name="rDeltaTime">Время с момента последнего кадра (или фиксированного вызова обновления)</param>
	 /// <param name="rUpdateIndex">Индекс обновления для управления динамическими/фиксированными обновлениями. [0: недопустимое обновление, >=1: допустимое обновление]</param>
	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
	 }

	 public override void OnMessageReceived(IMessage rMessage)
	 {
		if (rMessage == null) { return; }
		if (mActorController.State.Stance != EnumControllerStance.UNCONSCIOUS) { return; }

		// Check if we should activate
		if (!mIsActive)
		{
		}
		// Continue if we get that message
		else if (rMessage is MotionMessage)
		{
		  MotionMessage lMessage = rMessage as MotionMessage;
		  if (lMessage.ID == MotionMessage.MSG_MOTION_CONTINUE || lMessage.ID == MotionMessage.MSG_MOTION_DEACTIVATE)
		  {
		  }
		}
	 }

#if UNITY_EDITOR
	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;

		return lIsDirty;
	 }
#endif


	 #region Auto-Generated

	 /// <summary>
	 /// Эти объявления идут внутри класса, поэтому вы можете проверить, какие состояния и переходы активны. Тестирование хеш-значений выполняется намного быстрее, чем строки.
	 /// </summary>
	 public static int STATE_l1_Idle = -1;
	 public static int STATE_l1_Finish = -1;

	 public static int TRANS_EntryState_L1_Idle = -1;
	 public static int TRANS_EntryState_L1_Finish = -1;

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

		  if (lStateID == STATE_l1_Idle) { return true; }
		  if (lStateID == STATE_l1_Finish) { return true; }

		  if (lTransitionID == TRANS_EntryState_L1_Idle) { return true; }
		  if (lTransitionID == TRANS_EntryState_L1_Finish) { return true; }

		  return false;
		}
	 }
	 /// <summary>
	 /// Используется для определения, находится ли актер в одном из состояний для этого движения
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsMotionState(int rStateID)
	 {
		if (rStateID == STATE_l1_Idle) { return true; }
		if (rStateID == STATE_l1_Finish) { return true; }

		return false;
	 }

	 /// <summary>
	 /// Используется для определения, находится ли актер в одном из состояний для этого движения
	 /// </summary>
	 /// <returns></returns>
	 public override bool IsMotionState(int rStateID, int rTransitionID)
	 {
		if (rStateID == STATE_l1_Idle) { return true; }
		if (rStateID == STATE_l1_Finish) { return true; }
		if (rTransitionID == TRANS_EntryState_L1_Idle) { return true; }
		if (rTransitionID == TRANS_EntryState_L1_Finish) { return true; }
		return false;
	 }
	 /// <summary>
	 /// Предварительно обработайте любые данные аниматора, чтобы движение могло использовать их позже
	 /// </summary>
	 public override void LoadAnimatorData()
	 {
		STATE_l1_Idle = mMotionController.AddAnimatorName("Base Layer."+ _animName + ".L1_Start");
		STATE_l1_Finish = mMotionController.AddAnimatorName("Base Layer." + _animName + ".L1_Finish");

		TRANS_EntryState_L1_Finish = mMotionController.AddAnimatorName("AnyState ->  Base Layer." + _animName + ".L1_Start");
		TRANS_EntryState_L1_Finish = mMotionController.AddAnimatorName("AnyState -> Base Layer." + _animName + ".L1_Finish");

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