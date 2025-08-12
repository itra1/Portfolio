using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;
using UnityEngine.UI;

public class HintMenuButton : EventBehaviour {

	private int _count;

	public GameObject countIcon;
	public GameObject coinsIcon;
	public Text countText;

	public HintType type;

	public Text priceText;

	private void Start() {

		switch (type) {
				case HintType.anyLetter:
				priceText.text = Properties.Instance.hintAnyLetterPrice.ToString();
				break;
			case HintType.firstWord:
				priceText.text = Properties.Instance.hintFirstWordPrice.ToString();
				break;
			case HintType.firstLetter:
				priceText.text = Properties.Instance.hintFirstLetterPrice.ToString();
				break;
		}

	}

	private void OnEnable() {

		switch (type) {
			case HintType.anyLetter: {
					Change(PlayerManager.Instance.hintAnyLetter);
					break;
				}
			case HintType.firstLetter: {
					Change(PlayerManager.Instance.hintFirstLetter);
					break;
				}
			case HintType.firstWord: {
					Change(PlayerManager.Instance.hintFirstWord);
					break;
				}
		}
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.HintEnyLetterChange))]
	public void OnHintEnyLetterChange(ExEvent.PlayerEvents.HintEnyLetterChange evnt) {
		if (type == HintType.anyLetter)
			Change(evnt.count);
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.HintFirstLetterChange))]
	public void OnHintFirstLetterChange(ExEvent.PlayerEvents.HintFirstLetterChange evnt) {
		if (type == HintType.firstLetter)
			Change(evnt.count);
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.HintFirstWordChange))]
	public void OnHintFirstWordChange(ExEvent.PlayerEvents.HintFirstWordChange evnt) {
		if (type == HintType.firstWord)
			Change(evnt.count);
	}

	private void Change(int cnt) {
		this._count = cnt;

		coinsIcon.SetActive(!PlayerManager.Instance.unlimitedHint && cnt <= 0);
		countIcon.SetActive(!PlayerManager.Instance.unlimitedHint && cnt > 0);
		countText.text = cnt.ToString();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.SetUnlimitedHint))]
	public void SetUnlimitedHint(ExEvent.PlayerEvents.SetUnlimitedHint evnt) {
		coinsIcon.SetActive(false);
		countIcon.SetActive(false);
	}

  public void HintFirstLetter() {
    AudioManager.Instance.library.PlayClickAudio();
    PlayerManager.Instance.HintFirstLetter();
    AudioManager.Instance.library.PlayClickAudio();
  }

  public void HintOpenEnyLetter() {
    AudioManager.Instance.library.PlayClickAudio();
    PlayerManager.Instance.HintAnyLetter();
    AudioManager.Instance.library.PlayClickAudio();
  }

  public void HintOpenFirstWord() {
    AudioManager.Instance.library.PlayClickAudio();
    PlayerManager.Instance.HintFirstWord();
    AudioManager.Instance.library.PlayClickAudio();
  }

  public Animation animComp;

	public void PlayBauns() {
		AudioManager.Instance.library.PlayGetBonusAudio();
		animComp.Play("bauns");
	}


  public void BaunsComplete() {
	}

}
