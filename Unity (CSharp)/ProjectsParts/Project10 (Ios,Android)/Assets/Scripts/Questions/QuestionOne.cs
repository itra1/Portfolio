using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestionOne : MonoBehaviour {

	public MapQuestion mapQuestion;

	[Range(0, 2)]
	public int questNum;
	public Text titleText;
	public Text countText;
	public Text[] coinsText;
	public GameObject lineText;
	public AudioClip byeSound;
	public AudioClip clickSound;
	public AudioClip questWrite;

	bool ready;

	Animator anim;

	void OnEnable() {
		anim = GetComponent<Animator>();
		anim.SetBool("showAnim", false);

		SetStatusQuest();
		ChangeLanuage();
	}

	void OnDisable() {
		anim.SetBool("showAnim", false);
		anim.Rebind();
	}

	public void SetStatusQuest() {

		if(Questions.QuestionManager.Instance.actionList.items == null
		|| Questions.QuestionManager.Instance.actionList.items.Length <= 0
		|| Questions.QuestionManager.Instance.actionList.items.Length < questNum + 1
		) {
			gameObject.SetActive(false);
			return;
		}

		anim.SetBool("ready", false);
		ready = false;

		titleText.text = LanguageManager.GetTranslate("quest_" + Questions.QuestionManager.Instance.actionList.items[questNum].type.ToString()) + (!Questions.QuestionManager.Instance.actionList.items[questNum].cumulative ? " " + LanguageManager.GetTranslate("quest_inOneRun") : "");  //quest.actionList.items[questNum].descr;

		countText.text = Mathf.Round(Questions.QuestionManager.Instance.actionList.items[questNum].value) + "/" + Questions.QuestionManager.Instance.actionList.items[questNum].needvalue;
		ChangeWight(titleText, countText);

		for(int i = 0; i < coinsText.Length; i++)
			coinsText[i].text = Questions.QuestionManager.Instance.actionList.items[questNum].price.ToString();

		lineText.GetComponent<RectTransform>().sizeDelta = new Vector2(titleText.preferredWidth + 10, lineText.GetComponent<RectTransform>().sizeDelta.y);

		if(Questions.QuestionManager.Instance.actionList.items[questNum].needvalue <= Questions.QuestionManager.Instance.actionList.items[questNum].value) {
			anim.SetBool("complited", true);
			anim.SetBool("showAnim", true);
			return;
		} else {
			anim.SetBool("complited", false);
		}

		if(Questions.QuestionManager.Instance.actionList.items[questNum].faild) {
			anim.SetBool("faild", true);
			anim.SetBool("showAnim", true);
		}

		if(Questions.QuestionManager.Instance.actionList.items[questNum].price >= UserManager.coins) {
			anim.SetBool("active", false);
		} else {
			anim.SetBool("active", true);
		}
		anim.SetBool("showAnim", true);

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


	public void resetEvent() {
		ready = false;
		anim.SetBool("ready", false);
		anim.SetBool("bye", false);
	}

	public void ByeThis() {

		int thiCoins = UserManager.coins;

		if(ready) {
			if(Questions.QuestionManager.Instance.actionList.items[questNum].price <= thiCoins) {
				UserManager.coins = thiCoins - Questions.QuestionManager.Instance.actionList.items[questNum].price;

				YAppMetrica.Instance.ReportEvent("Квесты: куплено " + LanguageManager.GetTranslate(Questions.QuestionManager.Instance.actionList.items[questNum].descr));

				GAnalytics.Instance.LogEvent("Квесты", "Куплено", LanguageManager.GetTranslate(Questions.QuestionManager.Instance.actionList.items[questNum].descr), 1);
				
				anim.SetBool("bye", true);
				anim.SetBool("complited", true);
				AudioManager.PlayEffect(byeSound, AudioMixerTypes.mapEffect);
				AudioManager.PlayEffect(questWrite, AudioMixerTypes.mapEffect);
				Questions.QuestionManager.byeElem(questNum);
				countText.text = Questions.QuestionManager.Instance.actionList.items[questNum].needvalue + "/" + Questions.QuestionManager.Instance.actionList.items[questNum].needvalue;

				if(mapQuestion) {
					mapQuestion.ChangeCoinsCount();
				}

				ready = false;

				// Включаем анимациюпо купки ключа
				if(Questions.QuestionManager.Instance.CheckList()) {
					if(mapQuestion) {
						Invoke("MapAddKey", 0.5f);
						//Invoke("ShowGateScene", 0.5f); 
					}
				}

				// Обновляем коллег :)
				QuestionOne[] allQuest = transform.parent.GetComponentsInChildren<QuestionOne>();
				if(allQuest.Length > 0) {
					foreach(QuestionOne one in allQuest)
						one.SetStatusQuest();

				}

				return;
			}
		} else {
			AudioManager.PlayEffect(clickSound, AudioMixerTypes.mapEffect);
		}

		if(!ready) {
			anim.SetBool("ready", true);

			if(Questions.QuestionManager.Instance.actionList.items[questNum].price <= UserManager.coins) {
				ready = true;
				return;
			}

		}

	}

	void MapAddKey() {
		mapQuestion.addKey();
	}


	#region Lanuage

	public Text payCheck;
	public Text noMoney;
	public Text skip;

	void ChangeLanuage() {


		if(Questions.QuestionManager.Instance.actionList.items == null || Questions.QuestionManager.Instance.actionList.items.Length <= 0) {
			gameObject.SetActive(false);
			return;
		}

		if(payCheck != null) payCheck.text = LanguageManager.GetTranslate("mqst_PayCheck");
		if(noMoney != null) noMoney.text = LanguageManager.GetTranslate("mqst_PayNoCoins");
		if(skip != null) {
			skip.text = LanguageManager.GetTranslate("mqst_Skip");
			while(skip.preferredWidth > skip.transform.parent.GetComponent<RectTransform>().rect.width - 20) skip.fontSize--;
		}

	}

	#endregion

}
