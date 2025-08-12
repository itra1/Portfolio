using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ExEvent;

public class QuestionOneGui : EventBehaviour {
	
	public Text titleText;
	public Text countText;

	public GameObject iconStar;
	public GameObject iconConpl;

	public GameObject backComplited;
	public GameObject backNoComplited;

	public GameObject lineText;
	
	[Range(0,2)]
	public int questNum;

	void OnEnable() {

		if(questNum+1 > Questions.QuestionManager.Instance.actionList.items.Length) {
			gameObject.SetActive(false);
			return;
		}

		ChangeLanuage(null);
		lineText.GetComponent<RectTransform>().sizeDelta = new Vector2(titleText.preferredWidth + 10, lineText.GetComponent<RectTransform>().sizeDelta.y);

		bool isComplited = (Questions.QuestionManager.Instance.actionList.items[questNum].needvalue <= Questions.QuestionManager.Instance.actionList.items[questNum].value);

		iconConpl.SetActive(isComplited);
		iconStar.SetActive(!isComplited);
		backComplited.SetActive(isComplited);
		backNoComplited.SetActive(!isComplited);
		lineText.SetActive(isComplited);

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.LanguageEvents.LanuageChange))]
	public void ChangeLanuage(ExEvent.LanguageEvents.LanuageChange e) {
		Questions.LevelList listQuestion = Questions.QuestionManager.Instance.actionList;
		titleText.text = listQuestion.items[questNum].titleTranslate;
		countText.text = Questions.QuestionManager.Instance.actionList.items[questNum].countGui;
	}


}
