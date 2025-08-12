using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExEvent;
using TMPro;
using UnityEngine.UI;

public class WordUseUi : EventBehaviour {

	public Image backLightText;
	public Image backText;
	public TextUGUIScale text;

	public Animation anim;
	private bool _isAnim;

	private Color _startColorImg;
	private Color _startColorTxt;

	public Color defaultColorWord;
	public Color tryColorWord;
	public Color repeatColorWord;
	public Color falseColorWorld;
	public Color conchTryColorWorld;
	public Color conchRepeatColorWorld;

	public RectTransform okIcon;
	public RectTransform cancelIcon;
	public RectTransform conchIcon;

	private bool isVisualWord;

	public AnimationHelper animHelp;
	public PlayGamePlay gamePlay;

	protected override void Awake() {
		base.Awake();
		_startColorImg = backText.color;
		_startColorTxt = text.textUi.color;

		animHelp.OnEvent1 += () => {
			gamePlay.StartConchMover(conchIcon.transform.position);
		};
	}

	private void OnEnable() {
		ClearText();
	}
	
	[ExEventHandler(typeof(GameEvents.OnChangeWord))]
	void ChangeWord(GameEvents.OnChangeWord word) {

		if (isVisualWord) return;

		if(anim.isPlaying) anim.Stop();

		HideAll();

		text.SetText(word.word.ToUpper(), PlayerManager.Instance.company.actualCompany);
		backText.rectTransform.sizeDelta = new Vector2(text.textUi.preferredWidth+50, backText.rectTransform.sizeDelta.y);
		backText.color = _startColorImg;
		text.textUi.color = _startColorTxt;

		if (hideCoroutine != null) {
			StopCoroutine(hideCoroutine);
		}

	}

	public void ClearText() {
		text.SetText("");
		backText.rectTransform.sizeDelta = new Vector2(0, backText.rectTransform.sizeDelta.y);
		HideAll();
	}

	public void HideAll() {
		okIcon.gameObject.SetActive(false);
		cancelIcon.gameObject.SetActive(false);
		conchIcon.gameObject.SetActive(false);
		backLightText.gameObject.SetActive(false);
	}

	public void FadeHideCompleted() {
		_isAnim = false;
		HideAll();
		ClearText();
	}
	
	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnWordSelect))]
	public void OnSelectWord(ExEvent.GameEvents.OnWordSelect selectWord) {

		if (hideCoroutine != null)
			StopCoroutine(hideCoroutine);

		if (selectWord.word.Length > 1) {

			switch (selectWord.select) {
				case SelectWord.yes:
					PlayWordAppearAudio();
					text.textUi.color = tryColorWord;
					break;
				case SelectWord.repeat:
					PlayWordCoorectAudio();
					okIcon.gameObject.SetActive(true);
					okIcon.anchoredPosition = new Vector2(text.textUi.preferredWidth / 2 + 20, okIcon.anchoredPosition.y);
					text.textUi.color = repeatColorWord;
					break;
				case SelectWord.no:
					PlayWordWrongAudio();
					cancelIcon.gameObject.SetActive(true);
					cancelIcon.anchoredPosition = new Vector2(text.textUi.preferredWidth / 2 + 20, cancelIcon.anchoredPosition.y);
					text.textUi.color = falseColorWorld;
					break;
				case SelectWord.conchRepeat:
					PlayWordCoorectAudio();
					backLightText.gameObject.SetActive(true);
					okIcon.gameObject.SetActive(true);
					okIcon.anchoredPosition = new Vector2(text.textUi.preferredWidth / 2 + 20, okIcon.anchoredPosition.y);
					text.textUi.color = conchRepeatColorWorld;
					backLightText.rectTransform.sizeDelta = new Vector2(text.textUi.preferredWidth + 230,
						backLightText.rectTransform.sizeDelta.y);
					break;
				case SelectWord.conchYes:
					PlayWordSecretAudio();
					backLightText.gameObject.SetActive(true);
					conchIcon.gameObject.SetActive(true);
					okIcon.anchoredPosition = new Vector2(text.textUi.preferredWidth / 2 + 20, okIcon.anchoredPosition.y);
					text.textUi.color = conchTryColorWorld;
					backLightText.rectTransform.sizeDelta = new Vector2(text.textUi.preferredWidth + 230,
						backLightText.rectTransform.sizeDelta.y);
					break;
			}
		}

		anim.Play("fadeHide");
		_isAnim = true;
		
		hideCoroutine = StartCoroutine(HideTransperent());
	}

	private Coroutine hideCoroutine;
	IEnumerator HideTransperent() {

		isVisualWord = true;
		yield return new WaitForSeconds(0.2f);
		isVisualWord = false;
	}
	
	public AudioClipData wordAppearAudio;
	public void PlayWordAppearAudio() {
		AudioManager.PlayEffects(wordAppearAudio, AudioMixerTypes.effectUi);
	}
	public AudioClipData wordCorrectAudio;
	public void PlayWordCoorectAudio() {
		AudioManager.PlayEffects(wordCorrectAudio, AudioMixerTypes.effectUi);
	}
	public AudioClipData wordSecretAudio;
	public void PlayWordSecretAudio() {
		AudioManager.PlayEffects(wordSecretAudio, AudioMixerTypes.effectUi);
	}
	public List<AudioClipData> wordWrongAudio;
	public void PlayWordWrongAudio() {
		AudioManager.PlayEffects(wordWrongAudio[Random.Range(0, wordWrongAudio.Count)], AudioMixerTypes.effectUi);
	}
}
