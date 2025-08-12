using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld: WorldAbstract {

  public LettersController lettersController;
  public GameCrossword gameCrossword;

  private void Start() {

#if UNITY_IOS
    if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX) {
    lettersController.transform.localScale = Vector3.one * 0.8f;
    }
#endif

    if ((float)Camera.main.pixelWidth / (float)Camera.main.pixelHeight <= 0.56f)
      gameCrossword.transform.localScale = Vector3.one * 0.8f;

  }

  public void OnHide() {

  }

  public ParticleSystem salut;
  public void PlaySalut() {
    //salut.Play();
  }

  public void PauseSalut() {
    //salut.Pause();
  }

  public void StopSalut() {
    //salut.Stop();
  }

  public void Init() {
    GraphicReady();

    lettersController.SetData();
    gameCrossword.InitData();

  }

  public void GraphicReady() {

    //islandObject.SetActive(!(PlayerManager.Instance.company.isBonusLevel || Tutorial.Instance.isTutorial));
    //if (PlayerManager.Instance.company.isBonusLevel || Tutorial.Instance.isTutorial) {
    //	spriteRenderer.sprite = bonusBack;
    //} else {
    //	spriteRenderer.sprite = defBack;
    //}
  }

  public void OnShow() {
    lettersController.ShowLetter();
    gameCrossword.ShowLetter();

    if (PlayerManager.Instance.company.GetActualSaveLevel().words.Count == 0) {

      switch (PlayerManager.Instance.company.actualLevelNum) {
        case 1:
          FirebaseManager.Instance.LogEvent("start_level", "level", "1.1");
          break;
        case 5:
          FirebaseManager.Instance.LogEvent("start_level", "level", "1.5");
          break;
        case 10:
          FirebaseManager.Instance.LogEvent("start_level", "level", "1.10");
          break;
        case 15:
          FirebaseManager.Instance.LogEvent("start_level", "level", "1.15");
          break;
        case 20:
          FirebaseManager.Instance.LogEvent("start_level", "level", "1.20");
          break;
      }

    }

  }

}
