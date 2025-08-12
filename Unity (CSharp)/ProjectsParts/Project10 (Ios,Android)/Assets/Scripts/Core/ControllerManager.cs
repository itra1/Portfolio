using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Обработка управления
/// </summary>
public class ControllerManager : Singleton<ControllerManager> {
	
	public static event Action<float> OnHorizontal;                   // Событие горизонтального курсора
	public static event Action<float> OnVertical;                     // Событие горизонтального курсора
	public static event Action<bool> OnPause;                         // Событие зажатия паузы
	public static event Action<bool> OnFire;                          // Событие кнопки атаки
	public static event Action<bool> OnJump;                          // Событие кнопки прыжка
	public static event Action<bool> OnSetJoystick;                   // Изменение статуса работы с джойстиком

	bool isKeyBoard = false;
	bool isJoystick = false;
	
	void Start() {
		StartCoroutine(CheckForControllers());
#if UNITY_EDITOR
		//isKeyBoard = true;
#endif
	}


	static bool leftKeyUi;
	public static void SetLeftKey(bool isKey) {
		leftKeyUi = isKey;
		if(!isKey)
		OnHorizontalButton(0);
	}

	static bool rightKeyUi;
	public static void SetRightKey(bool isKey) {
		rightKeyUi = isKey;
		if (!isKey)
			OnHorizontalButton(0);
	}

	IEnumerator CheckForControllers() {
		while (true) {
			var controllers = Input.GetJoystickNames();
			if (!isJoystick && controllers.Length > 0) {
				isJoystick = true;
				if (OnSetJoystick != null)
					OnSetJoystick(isJoystick);
			} else if (isJoystick && controllers.Length == 0) {
				isJoystick = false;
				if (OnSetJoystick != null)
					OnSetJoystick(isJoystick);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void Update() {
		if (isJoystick || isKeyBoard) {
			OnHorizontalButton(Input.GetAxis("Horizontal"));
			OnVerticalButton(Input.GetAxis("Vertical"));
			OnFireButton(Input.GetButtonDown("Fire1"));
			OnJumpButton(Input.GetButtonDown("Jump"));
			OnPauseButton(Input.GetButtonDown("Pause"));
		} else {

			if (leftKeyUi)
				OnHorizontalButton(-1);
			else if (rightKeyUi)
				OnHorizontalButton(1);
		}
	}

	public static void OnHorizontalButton(float keyValue) {
		if (OnHorizontal != null)
			OnHorizontal(keyValue);
	}

	public static void OnVerticalButton(float keyValue) {
		if (OnVertical != null)
			OnVertical(keyValue);
	}

	public static void OnFireButton(bool isDown) {
		if (isDown && OnFire != null)
			OnFire(isDown);
	}

	public static void OnJumpButton(bool isDown) {
		if (isDown && OnJump != null)
			OnJump(isDown);
	}

	public static void OnPauseButton(bool press) {
		if (OnPause != null)
			OnPause(press);
	}

}
