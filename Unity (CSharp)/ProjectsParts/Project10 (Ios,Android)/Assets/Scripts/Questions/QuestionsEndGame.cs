using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuestionsEndGame : PanelUi {
  
	public Action OnGate;
	
  public GameObject[] questionOne;

  List<int> questionsList;

	Questions.LevelList listQuestion;

  public AudioClip getKeyClip;
  public AudioClip keyIdleClip;

  public AudioClip openClip;
  public AudioClip closeClip;

  [HideInInspector]
  public string beforeTo;

  //bool showGateScene;

  protected override void OnEnable() {
		base.OnEnable();
    isSpeed = false;
    AudioManager.PlayEffect(openClip, AudioMixerTypes.runnerUi);
    AudioManager.HidePercent50Sound();
    listQuestion = Questions.QuestionManager.Instance.actionList;
    questionsList = new List<int>();

  }

	protected override void OnDisable() {
		base.OnDisable();
		if (OnClose != null) OnClose();
	}

	IEnumerator OpenPanel() {

    yield return new WaitForSeconds(0.3f);

    for(int i = 0; i < questionOne.Length; i++) {
      questionOne[i].SetActive(true);
      QuestionOneEndGame oneQuestInList = questionOne[i].GetComponent<QuestionOneEndGame>();
      oneQuestInList.titleText.text = LanguageManager.GetTranslate("quest_" + Questions.QuestionManager.Instance.actionList.items[i].type.ToString()) + (!Questions.QuestionManager.Instance.actionList.items[i].cumulative ? " " + LanguageManager.GetTranslate("quest_inOneRun") : "");  //listQuestion.items[i].descr;

      oneQuestInList.countText.text = Mathf.Round(listQuestion.items[i].value) + "/" + Questions.QuestionManager.Instance.actionList.items[i].needvalue;
      ChangeWight(oneQuestInList.titleText, oneQuestInList.countText);
      oneQuestInList.lineText.GetComponent<RectTransform>().sizeDelta = new Vector2(oneQuestInList.titleText.preferredWidth + 10, oneQuestInList.lineText.GetComponent<RectTransform>().sizeDelta.y);

      if(Questions.QuestionManager.Instance.actionList.items[i].needvalue <= Questions.QuestionManager.Instance.actionList.items[i].value && !RunnerController.Instance.questionInRaceList.Exists(x => x == i))
        questionOne[i].GetComponent<Animator>().SetBool("complitedNoAnim", true);

      yield return new WaitForSeconds(0.3f);
    }
  }


  IEnumerator WainShowCompl() {
    yield return new WaitForSeconds(2f);

    for(int i = 0; i < questionOne.Length; i++) {
      if(!questionsList.Exists(x => x == i) && RunnerController.Instance.questionInRaceList.Exists(x => x == i)) {
        questionsList.Add(i);
        questionOne[i].GetComponent<Animator>().SetTrigger("complitedAnim");
        yield return new WaitForSeconds(0.3f);
      }
    }
    yield return new WaitForSeconds(1f);
    CheckComplList();
  }

  public void OpenItem(int num) {

		if (RunnerController.Instance.questionInRaceList.Count < num+1) return;

    if(Questions.QuestionManager.Instance.actionList.items == null
    || Questions.QuestionManager.Instance.actionList.items.Length <= 0
    || Questions.QuestionManager.Instance.actionList.items.Length < num + 1
    ) {
      questionOne[num].SetActive(false);
      return;
    }

    questionOne[num].SetActive(true);
    QuestionOneEndGame oneQuestInList = questionOne[num].GetComponent<QuestionOneEndGame>();
    oneQuestInList.titleText.text = LanguageManager.GetTranslate("quest_" + Questions.QuestionManager.Instance.actionList.items[num].type.ToString()) + (!Questions.QuestionManager.Instance.actionList.items[num].cumulative ? " " + LanguageManager.GetTranslate("quest_inOneRun") : "");  //listQuestion.items[i].descr;

    oneQuestInList.countText.text = Mathf.Round(listQuestion.items[num].value) + "/" + Questions.QuestionManager.Instance.actionList.items[num].needvalue;
    ChangeWight(oneQuestInList.titleText, oneQuestInList.countText);
    oneQuestInList.lineText.GetComponent<RectTransform>().sizeDelta = new Vector2(oneQuestInList.titleText.preferredWidth + 10, oneQuestInList.lineText.GetComponent<RectTransform>().sizeDelta.y);

    //questionOne[i].GetComponent<QuestionOneEndGame>().SetStatusQuest(runner.questionInRaceList.Exists(x => x == i), quest.actionList.items[i]);

    if(Questions.QuestionManager.Instance.actionList.items[num].faild)
      questionOne[num].GetComponent<Animator>().SetBool("failed", true);

    if(Questions.QuestionManager.Instance.actionList.items[num].needvalue <= Questions.QuestionManager.Instance.actionList.items[num].value && !RunnerController.Instance.questionInRaceList.Exists(x => x == num)) {
      questionOne[num].GetComponent<Animator>().SetTrigger("complitedNoAnim");
    }

    questionOne[num].GetComponent<Animator>().SetBool("speedy", isSpeed);
  }

  public void ComplitedItem(int num) {
    if(!questionsList.Exists(x => x == num) && RunnerController.Instance.questionInRaceList.Exists(x => x == num)) {
      questionsList.Add(num);
      questionOne[num].GetComponent<Animator>().SetTrigger("complitedAnim");
    }
  }

  public void EventAnim() {
    ClosePanelNow();
  }

  public void ClosePanelNow() {

    //runner.closePanel(panelTypes.questionEndGame);
    gameObject.SetActive(false);
  }

  public void CheckComplList() {
    if(Questions.QuestionManager.Instance.CheckList()) {
			if (OnGate != null) OnGate();
			// Если список готов, включаем анимаию ключа
			/*if(RunnerController.instance.keysInRace > 0) {
          int key = PlayerManager.instance.keys + RunnerController.instance.keysInRace;
          PlayerPrefs.SetInt("keys" , key);
          RunnerController.instance.keysInRace = 0;
      }*/

			//ShowGateScene();
    } else {
      ClosePanel();
    }
  }


  void ShowGateScene() {
    AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerEffect0, 0.5f);
    AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerMusic0, 0.5f);
    GameManager.ShowGateScene(CloseGateScene);
  }

  void CloseGateScene() {
    AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerEffect50, 0.5f);
    AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerMusic50, 0.5f);
    ClosePanelNow();
  }

  public void ClosePanel() {
    GetComponent<Animator>().SetTrigger("close");
    StartCoroutine(HideAudio());
    AudioManager.PlayEffect(closeClip, AudioMixerTypes.runnerUi);
  }

  public void ButtonKey() {
    UiController.ClickButtonAudio();
    ClosePanel();
  }

  public void GetKeyIdlePlay() {
    GetComponent<AudioSource>().clip = keyIdleClip;
    GetComponent<AudioSource>().loop = true;
    GetComponent<AudioSource>().Play();
  }

  void ChangeWight(Text txt, Text txt2) {

    while(!Checkscale(txt, txt2)) {
      txt.fontSize -= 1;
      txt2.fontSize -= 1;
    }

  }

  bool Checkscale(Text txt, Text txt2) {
    float textWidth = txt.preferredWidth;
    float textWidth2 = txt2.preferredWidth;
    float parentWidth = txt.rectTransform.parent.GetComponent<RectTransform>().rect.width;

    if(textWidth + textWidth2 + 120 > parentWidth)
      return false;
    else
      return true;
  }

  bool isSpeed;

  /// <summary>
  /// Требование ускорить процесс
  /// </summary>
  /// <returns></returns>
  public void Speedy() {
    if(isSpeed) return;
    isSpeed = true;

    GetComponent<Animator>().SetTrigger("speedy");

    if(!questionOne[0].activeInHierarchy)
      OpenItem(0);
    else {
      questionOne[0].GetComponent<Animator>().SetBool("speedy", isSpeed);
    }

    if(!questionOne[1].activeInHierarchy)
      OpenItem(1);
    else {
      questionOne[1].GetComponent<Animator>().SetBool("speedy", isSpeed);
    }

    if(!questionOne[2].activeInHierarchy)
      OpenItem(2);
    else {
      questionOne[2].GetComponent<Animator>().SetBool("speedy", isSpeed);
    }
    ComplitedItem(0);
    ComplitedItem(1);
    ComplitedItem(2);
  }

  IEnumerator HideAudio() {

    float needValume = 0;
    float thisValume = GetComponent<AudioSource>().volume;

    while(thisValume > needValume) {
      yield return new WaitForSeconds(0.1f);
      thisValume -= 0.1f;
      GetComponent<AudioSource>().volume = thisValume;
    }

  }

	public override void BackButton() {
		ClosePanel();
	}
}
