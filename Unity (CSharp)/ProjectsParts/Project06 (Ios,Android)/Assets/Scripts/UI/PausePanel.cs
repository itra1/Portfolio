using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Панель интерфейса паузы
/// </summary>
public class PausePanel : UiDialog {
	private bool isActive;

	public Action OnClose;
	public Action OnDeactive;
	public Action OnRestart;
	public Action OnExit;
	public Action OnComplited;
	public Action OnFailed;

	private void OnEnable() {
		AudioManager.SoundChange += SoundChange;
		confirmDialog.SetActive(false);
		InitAudio();
		Helpers.Invoke(this, Activete, 0.1f);
	}

	void Activete() {
		isActive = true;
	}

	private void OnDisable() {
		isActive = false;
		AudioManager.SoundChange -= SoundChange;
		if (OnDeactive != null) OnDeactive();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.Pause))]
	private void KeyDown(ExEvent.BattleEvents.Pause pause) {
		if (!pause.isPause) {
			UIController.ClickPlay();
			Close();
		}
	}

	public void Close() {
		gameObject.SetActive(false);
	}

	public void CloseButton() {
		UIController.ClickPlay();
		if (OnClose != null) OnClose();
		Close();
	}

	#region Перезапуск

	public void RestartButton() {
		UIController.ClickPlay();
		ConfirmDialogShow("Начать заново?",
			() => {
				UIController.ClickPlay();
				if (OnRestart != null) OnRestart();
			}, () => {
				UIController.ClickPlay();
			});
	}

	public void ExitButton() {
		UIController.ClickPlay();
		ConfirmDialogShow("Уже сдаешься?",
			() => {
				UIController.ClickPlay();
				if (OnExit != null) OnExit();
			}, () => {
				UIController.ClickPlay();
			});
	}

	#endregion

	#region Music

	public Image soundImage;
	public Image musikImage;

	public void InitAudio() {
		SoundChange();
	}

	void SoundChange() {
		SetColorSoundImage(AudioManager.Instance.IsEffects);
		SetColorMusikImage(AudioManager.Instance.IsMusic);
	}

	public void SetColorSoundImage(bool flag) {
		soundImage.color = (flag ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1f));
	}
	public void SetColorMusikImage(bool flag) {
		musikImage.color = (flag ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1f));
	}

	public void MusicButton() {
		AudioManager.Instance.ToggleMusic();
	}

	public void EffectsButton() {
		AudioManager.Instance.ToggleEffect();
	}

	#endregion

	#region Confirm dialog

	public GameObject confirmDialog;
	public Text confirmText;

	public delegate void ButtonClick();
	public ButtonClick ConfirmOkClick;
	public ButtonClick ConfirmCancelClick;

	public void ConfirmDialogShow(string confirmTextValue, ButtonClick clickOk, ButtonClick clickCancel) {
		confirmDialog.SetActive(true);
		confirmText.text = confirmTextValue;
		ConfirmOkClick = clickOk;
		ConfirmCancelClick = clickCancel;
	}

	public void ConfirmOkButton() {
		if (ConfirmOkClick != null) ConfirmOkClick();
		confirmDialog.SetActive(false);
	}

	public void ConfirmCancelButton() {
		if (ConfirmCancelClick != null) ConfirmCancelClick();
		confirmDialog.SetActive(false);
	}

	#endregion

	#region Читерские кнопки

	public void ComplitedButton() {
		if (OnComplited != null) OnComplited();
	}

	public void FailedButton() {
		if (OnFailed != null) OnFailed();
	}

	#endregion

}
