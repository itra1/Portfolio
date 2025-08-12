using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapQuestion : MonoBehaviour {

  bool thisClose;
  public MapGamePlay gamePlay;
  public MapController map;
  public QuestionOne[] quest;

  public AudioClip keyGetClip;
  public AudioClip keyIdleClip;

  public AudioClip openClip;
  public AudioClip closeClip;
  bool audioReady;

  void OnEnable() {
    thisClose = false;
    audioReady = true;
    GetComponent<Animator>().SetBool("open", true);
    //map.blockSmash++;
    ChangeLanuage();
  }

  void OnDisable() {
    map.StartAnimOpenGate();
    //map.blockSmash--;
  }

  public void ChangeCoinsCount() {
    gamePlay.SetCoinsCountText(new ExEvent.RunEvents.CoinsChange(UserManager.coins));
  }

  #region Добавление ключа

  public void addKey() {
    ShowGateScene();
  }

  public void addKeyAnimEvent() {

    Questions.QuestionManager.Instance.InitQuestionsGroup();
    gamePlay.ChangeKey(1);
  }

  public void ButtonKey() {
    GetComponent<Animator>().SetTrigger("getKey");
    UiController.ClickButtonAudio();
    GetComponent<AudioSource>().Stop();
  }

  #endregion

  public void AudioEffect() {
    if(audioReady) {
      audioReady = false;
      if(thisClose) {
        AudioManager.PlayEffect(closeClip, AudioMixerTypes.mapEffect);
      } else {
        AudioManager.PlayEffect(openClip, AudioMixerTypes.mapEffect);
      }
    }

  }

  #region Close panel

  public void animEvent() {
    if(thisClose) {
      gameObject.SetActive(false);
    }
  }

  public void buttomClose() {
    thisClose = true;
    audioReady = true;
    UiController.ClickButtonAudio();
    GetComponent<Animator>().SetBool("open", false);
  }

  public void CloseOneuestions(int num) {
    if(!thisClose) return;
    quest[num].GetComponent<Animator>().SetBool("showAnim", false);
    quest[num].GetComponent<Animator>().SetBool("hide", true);
  }

  public void CloseOneuestionsNow(int num) {
    quest[num].GetComponent<Animator>().SetBool("showAnim", false);
    quest[num].GetComponent<Animator>().SetBool("hide", true);
  }


  public void GetKeyIdlePlay() {
    GetComponent<AudioSource>().clip = keyIdleClip;
    GetComponent<AudioSource>().loop = true;
    GetComponent<AudioSource>().Play();
  }


  void ShowGateScene() {
    addKeyAnimEvent();
    AudioManager.SetSoundMixer(AudioSnapshotTypes.mapEffect0, 0.5f);
    GameManager.ShowGateScene(CloseGateScene);
  }

  void CloseGateScene() {
    AudioManager.SetSoundMixer(AudioSnapshotTypes.mapEffectDef, 0.5f);
    gameObject.SetActive(false);
  }

  #endregion

  #region Lanuage

  public Text titleText;
  public Text infoText;

  void ChangeLanuage() {
    titleText.text = LanguageManager.GetTranslate("mqst_Question");
    infoText.text = LanguageManager.GetTranslate("mqst_QuestonInfo");
  }

  #endregion
}
