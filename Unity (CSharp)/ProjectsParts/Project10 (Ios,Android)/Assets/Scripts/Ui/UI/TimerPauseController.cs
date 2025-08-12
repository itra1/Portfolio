using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Таймер
/// </summary>
public class TimerPauseController : PanelUiBase {
	
	public Action OnStart;

  public AudioClip numClip;
  public AudioClip startClip;
  public Text startText;
  void OnEnable() {
    startText.text = LanguageManager.GetTranslate("timer_Start");
  }
  /// <summary>
  /// Запуск таймера
  /// </summary>
  public void GoTimer() {
    GetComponent<Animator>().SetTrigger("goTimer");
  }
  /// <summary>
  /// Отметка номера
  /// </summary>
  public void Nomer() {
    AudioManager.PlayEffect(numClip, AudioMixerTypes.runnerEffect, 0.70f);
  }
  /// <summary>
  /// Старт таймера
  /// </summary>
  public void GoStart() {
    AudioManager.PlayEffect(startClip, AudioMixerTypes.runnerEffect, 0.70f);
		if (OnStart != null)
			OnStart();
  }
  /// <summary>
  /// Закрытие анимации
  /// </summary>
  public void CloseTimer() {
		//if (OnClose != null)
		//	OnClose();
		gameObject.SetActive(false);
  }
}
