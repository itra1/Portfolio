using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionUiGamePlay : ExEvent.EventBehaviour {

	public Animation anim;

	public Text questText;
	public Text questCount;
	public GameObject questPanel;
	public GameObject questIcon;
	public GameObject questIconCompl;
	public GameObject questIconFail;
	public Color waitColor;
	public Color blackColor;
	public Color redBgColor;
	public Color redTextColor;
	List<Questions.Question> listComplited = new List<Questions.Question>();
	[HideInInspector]
	bool readyShow = true;
	public AudioClip questionDoneClip;

	Questions.Question activeQuest;
	float questTimeWait;
	bool activeDialog;
	
	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.QuestionComplete))]
	public void QuestComplited(ExEvent.GameEvents.QuestionComplete quest) {
		AddQuestShow(quest.question);
	}

	void Update() {
		if (activeDialog && questTimeWait <= Time.time) {
			activeDialog = false;
			anim.Play("hide");
		}
	}

	public void AddQuestShow(Questions.Question quest) {
		if (!gameObject.activeInHierarchy) return;
		if (listComplited == null) listComplited = new List<Questions.Question>();

		if (activeDialog && activeQuest.type == quest.type) {
			questTimeWait = Time.time + 2;
			//AudioEffects.PlayEffects(questionDoneClip);
			if (Mathf.Round(quest.value) >= quest.needvalue) {
				questCount.text = quest.needvalue + "/" + quest.needvalue;
				questIconCompl.SetActive(true);
				questIconFail.SetActive(false);
				questIcon.SetActive(false);
			} else {
				questCount.text = Mathf.Round(quest.value) + "/" + quest.needvalue;
				questIconCompl.SetActive(false);
				questIconFail.SetActive(false);
				questIcon.SetActive(true);
			}
			return;
		} else {
			if (listComplited.Exists(x => x.type == quest.type))
				return;
		}

		listComplited.Add(quest);
		if (readyShow)
			questComplited();
	}

	public void questComplited() {

		if (listComplited.Count <= 0) return;

		foreach (Questions.Question ont1 in listComplited) {
			readyShow = false;

			Questions.Question ont = Questions.QuestionManager.GetQuestValue(ont1.type);
			questText.text = LanguageManager.GetTranslate("quest_" + ont.type.ToString()) + (!ont.cumulative ? " " + LanguageManager.GetTranslate("quest_inOneRun") : "");

			activeQuest = ont;
			questTimeWait = Time.time + 2;
			activeDialog = true;

			if (ont.faild) {
				questCount.text = Mathf.Round(ont.value) + "/" + ont.needvalue;
				questText.color = redTextColor;
				questPanel.GetComponent<Image>().color = redBgColor;
				questIconCompl.SetActive(false);
				questIconFail.SetActive(true);
				questIcon.SetActive(false);
			} else if (Mathf.Round(ont.value) >= ont.needvalue) {
				questCount.text = ont.needvalue + "/" + ont.needvalue;
				questText.color = blackColor;
				questPanel.GetComponent<Image>().color = waitColor;
				questIconCompl.SetActive(true);
				questIconFail.SetActive(false);
				questIcon.SetActive(false);
				AudioManager.PlayEffect(questionDoneClip, AudioMixerTypes.runnerEffect);
			} else {
				questCount.text = Mathf.Round(ont.value) + "/" + ont.needvalue;
				questText.color = blackColor;
				questPanel.GetComponent<Image>().color = waitColor;
				questIconCompl.SetActive(false);
				questIconFail.SetActive(false);
				questIcon.SetActive(true);
				//AudioEffects.PlayEffects(questionDoneClip);
			}

			float scale = (questText.preferredWidth + 120) + (questCount.preferredWidth + 20);
			questPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(scale, questPanel.GetComponent<RectTransform>().sizeDelta.y);

			anim.Play("show");

			listComplited.RemoveAt(0);
			return;
		}
	}

	void EventAnimQuest() {
		readyShow = true;
		questComplited();
	}


}
