using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingDialog : UiDialog {

	public GameObject baseButton;

	private void Start() {
    if(baseButton != null)
      baseButton.SetActive(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Game");
	}

  protected override void OnEnable() {
    base.OnEnable();
    ChangeParametrs();
    GameTime.timeScale = 0;
  }

  protected override void OnDisable() {
    base.OnDisable();
    GameTime.timeScale = 1;
  }

  public void CloseButton() {
    UiController.Instance.ClickSound();
    Hide();
	}
  
  public GameObject musicButtonActive;
  public GameObject musicButtonDeactive;

  public GameObject effectsButtonActive;
  public GameObject effectsButtonDeactive;

  public void MusicClick() {
    UiController.Instance.ClickSound();
    ExEvent.GameEvents.OnChangeMusic.Call();
  }

  public void SoundClick()
  {
    UiController.Instance.ClickSound();
    ExEvent.GameEvents.OnChangeEffects.Call();
  }

  /// <summary>
  /// Интерфейская кнопка поддержки
  /// </summary>
  public void SupportButton()
  {
    UiController.Instance.ClickSound();
    var mailMessage = new UT.MailMessage()
      .AddTo("clickgamesru@gmail.com")
      .SetSubject("From game " + Application.productName)
      .SetBody("")
      .SetBodyHtml(true);

    UT.Mail.Compose(mailMessage, "");
    
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeEffects))]
  private void OnChangeEffects(ExEvent.GameEvents.OnChangeEffects eventData) {
    ChangeParametrs();
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeMusic))]
  private void OnChangeMusic(ExEvent.GameEvents.OnChangeMusic eventData) {
    ChangeParametrs();
  }

  private void ChangeParametrs() {
    musicButtonActive.SetActive(AudioManager.Instance.Music);
    musicButtonDeactive.SetActive(!AudioManager.Instance.Music);
    effectsButtonActive.SetActive(AudioManager.Instance.Effects);
    effectsButtonDeactive.SetActive(!AudioManager.Instance.Effects);
  }

}
