using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.User;

/// <summary>
/// Гуи настроек
/// </summary>
public class Options : UiDialog {

  public delegate void Button();

  public Button OnShare;
  public Button OnExit;

  public GameObject optionsPanel;
  public GameObject supportPanel;
  public GameObject difficultyPanel;

  enum Pages {
    options,
    support,
    difficulty
  }

  private void OnEnable() {
    AudioManager.SoundChange += SoundChange;
    InitAudio();
    SwitchPages(Pages.options);
  }

  private void OnDisable() {
    AudioManager.SoundChange -= SoundChange;
  }

  void SwitchPages(Pages newPage) {
    optionsPanel.SetActive(newPage == Pages.options);
    supportPanel.SetActive(newPage == Pages.support);
    difficultyPanel.SetActive(newPage == Pages.difficulty);
  }

  public void OptionsButton() {
    SwitchPages(Pages.options);
  }

  public void SupportButton() {
    SwitchPages(Pages.support);
  }

  public void DifficultyButton() {
    SwitchPages(Pages.difficulty);
  }

  public void ShareButton() {
    if(OnShare != null) OnShare();
  }

  public void BackButton() {
    gameObject.SetActive(false);
  }

  public void ExitButton() {
    if(OnExit != null) OnExit();
  }

  public void DifficultyLevelButton(int level) {
    UserManager.Instance.difficultyLevel.Value = level;
  }
  

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

	public void LanuageEnglish() {
    //LanguageManager.Instance.ChangeLanguage("en");
    I2.Loc.LocalizationManager.CurrentLanguage = "Russian";
	}
	public void LanuageRussian() {
    I2.Loc.LocalizationManager.CurrentLanguage = "English";
    //LanguageManager.Instance.ChangeLanguage("ru");
  }

	public void ResetButton() {
		UserManager.Instance.ResetDefaultData();
	}

}
