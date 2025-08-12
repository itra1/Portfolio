using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordTranslateUi : MonoBehaviour {

	public Text wordText;
	public TextUiScale translateText;

	public Animation anim;

	private bool isAnimate;
	public GameObject graph;

	public Queue<GameCompany.Word> queueSelectWords = new Queue<GameCompany.Word>();
	private GameCompany.Word activeWord;

	private void OnEnable() {
		graph.SetActive(false);
		isAnimate = false;
		activeWord = null;
	}
	
	public void ShowData(GameCompany.Word word) {
		
		if (PlayerManager.Instance.company.actualCompany == PlayerManager.Instance.translateLanuage) return;

		if (queueSelectWords.Contains(word) || (activeWord != null && activeWord == word)) return;

		queueSelectWords.Enqueue(word);
		Show();
	}

	void Show() {

		if (PlayerManager.Instance.translateLanuage == "none") return;

		if (isAnimate || queueSelectWords.Count == 0) return;

		activeWord = queueSelectWords.Dequeue();
		
		string wordString = activeWord.word;
		string translateString = "";
		try {
			translateString = activeWord.translations.Find(x => x.lang == PlayerManager.Instance.translateLanuage).value;
		}catch {}

		if (System.String.IsNullOrEmpty(translateString)) {
			activeWord = null;
			Show();
			return;
		}

		graph.SetActive(true);

		wordText.text = wordString.ToUpper();
		translateText.SetText(translateString, PlayerManager.Instance.translateLanuage);
		anim.Play("show");
		isAnimate = true;
	}

	public void ShowComplete() {
		isAnimate = false;
		anim.Stop();
		activeWord = null;
		graph.SetActive(false);
		Show();
	}

}
