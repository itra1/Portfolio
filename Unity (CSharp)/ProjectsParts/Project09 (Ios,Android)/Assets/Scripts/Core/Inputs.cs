using UnityEngine;
using CnControls;
using System;
using System.Collections;

/// <summary>
/// Обработка входных данных
/// 
/// <para>Используется на случай использования дополнительны контроллерв или просто дублирования входящих сигналов управления</para>
/// </summary>
public class Inputs : MonoBehaviour {

  public delegate void OnPointerDelegate(Vector3 pointerPosition);
  public static event OnPointerDelegate OnPointerRightScreenUp;         // Отпускание тапа по экрану в правой части
  public static event OnPointerDelegate OnPointerRightScreenDown;       // Зажатие тапа по экрану в правой части
  public static event OnPointerDelegate OnPointerRightScreenDrag;       // Драг тапа после зажатия в правой части
  public delegate void OnButtons(float value = 1);
  public static event Action<Vector2> OnVectorMove;
  //public static event OnButtons OnPressButtonHorizontal;                // Использование горизонтального курсора
  protected string horizontalAxis = "Horizontal";                       // Идентификатор входного сигнала горизонтального курсора
  protected float _horizontalAxisValue;                                 // Значение входного сигнала горизонтального курсора
  protected bool isHorizontal;                                          // Флаг использования горизонтального курсора
  //public static event OnButtons OnPressButtonVertical;                  // Использование вертикального курсора
  protected string verticalAxis = "Vertical";                           // Идентификатор входного сигнала вертикального курсора
  protected float _verticalAxisValue;                                   // Значение входного сигнала вертикального курсора
  protected bool isVertical;                                            // Флаг использования вертикального курсора
  public void Start() {
    ScreenTap.OnPointUp += OnPointerUp;
    ScreenTap.OnPointDown += OnPointerDown;
    ScreenTap.OnPointDrag += OnPointerDrag;
  }
  public void OnDestroy() {
    ScreenTap.OnPointUp -= OnPointerUp;
    ScreenTap.OnPointDown -= OnPointerDown;
    ScreenTap.OnPointDrag -= OnPointerDrag;
  }
  public void Update() {
    // Обработка горизонтальной оси курсора
    CheckHorizontalInput();
    // Обработка вертикальной оси курсора
    CheckVerticalInput();
    // Обработка работы джостика
    CheckJoystick();

    if(moveVector != lastMoveVector)
      OnMoveVector(moveVector);

  }
  /// <summary>
  /// Зажатие клавиши мыши
  /// </summary>
  /// <param name="pointerPosition"></param>
  protected void OnPointerUp(Vector3 pointerPosition) {
    if (OnPointerRightScreenUp != null) OnPointerRightScreenUp(pointerPosition);
  }
  /// <summary>
  /// Отпускание клавиши мыши
  /// </summary>
  /// <param name="pointerPosition"></param>
  protected void OnPointerDown(Vector3 pointerPosition) {
    if (OnPointerRightScreenDown != null) OnPointerRightScreenDown(pointerPosition);
  }
  /// <summary>
  /// Событие смещение пальца
  /// </summary>
  /// <param name="pointerPosition">позиция пальца</param>
  protected void OnPointerDrag(Vector3 pointerPosition) {
    if (OnPointerRightScreenDrag != null) OnPointerRightScreenDrag(pointerPosition);
  }
  /// <summary>
  /// Проверка зажатия горизонтальных стрелок или использования горизинтальных осей джостика
  /// </summary>
  protected void CheckHorizontalInput() {
    PressHorizontalArrow(Input.GetAxis(horizontalAxis));

    //_horizontalAxisValue = Input.GetAxis(horizontalAxis);

    //if (_horizontalAxisValue != 0) {
    //  isHorizontal = true;
    //  PressHorizontalArrow(Mathf.Sign(_horizontalAxisValue));
    //} else if (isHorizontal) {
    //  isHorizontal = false;
    //  PressHorizontalArrow(0);
    //}
  }
  /// <summary>
  /// Проверка зажатия вертикальных стрелок или использование вертикальных осей джостика
  /// </summary>
  protected void CheckVerticalInput() {
    PressVerticalArrow(Input.GetAxis(verticalAxis));
    //_verticalAxisValue = Input.GetAxis(verticalAxis);

    //if(_verticalAxisValue != 0) {
    //  isVertical = true;
    //  PressVerticalArrow(Mathf.Sign(_verticalAxisValue));
    //} else if (isVertical) {
    //  isVertical = false;
    //  PressVerticalArrow(0);
    //}
  }
  /// <summary>
  /// обработка входящих данных с джостика
  /// </summary>
  protected void CheckJoystick() {
    //var inputVector = new Vector3(CnInputManager.GetAxis("Horizontal"), CnInputManager.GetAxis("Vertical"));

    //if (Mathf.Abs(inputVector.x) > 0.5f) {
    //  isHorizontal = true;
    //  PressHorizontalArrow(Mathf.Sign(inputVector.x));
    //} else if (isHorizontal) {
    //  isHorizontal = false;
    //  PressHorizontalArrow(0);
    //}

    PressHorizontalArrow(CnInputManager.GetAxis("Horizontal"));
    PressVerticalArrow(CnInputManager.GetAxis("Vertical"));

    //if (Mathf.Abs(inputVector.y) > 0.5f) {
    //  isVertical = true;
    //  PressVerticalArrow(Mathf.Sign(inputVector.y));
    //} else if (isVertical) {
    //  isVertical = false;
    //  PressVerticalArrow(0);
    //}
    
  }
  /// <summary>
  /// Использование горизонтальных стрелок
  /// </summary>
  /// <param name="value">Значение</param>
  protected void PressHorizontalArrow(float value) {
    moveVector.x = value;
  }
  /// <summary>
  /// Использование вертикальных стрелок
  /// </summary>
  /// <param name="value">Значение</param>
  protected void PressVerticalArrow(float value) {
    moveVector.y = value;
  }

  Vector2 lastMoveVector;
  Vector2 moveVector;
  protected void OnMoveVector(Vector2 moveVector) {
    lastMoveVector = moveVector;
    if(OnVectorMove != null) OnVectorMove(moveVector);
  }
}
