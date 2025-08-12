using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMiniMenu : MonoBehaviour {

	public Animation anim;

	private string _showAnim = "show";
	private string _hideAnim = "hide";

	private bool _isOpen;

	public GameObject shopBtn;
	public GameObject firstLetterBtn;
	public GameObject anyLetterBtn;
	public GameObject anyWordBtn;
	public GameObject firstLetterBtnGraph;
	public GameObject anyLetterBtnGraph;
	public GameObject anyWordBtnGraph;

	private void OnEnable() {
		HideComplete();
		isLocked = 0;
    Close(true);
  }
	
	public void Click() {
		if (anim.isPlaying) return;
		AudioManager.Instance.library.PlayClickAudio();
		_isOpen = !_isOpen;
		anim.Play((_isOpen? _showAnim: _hideAnim));

		if (_isOpen) {
			shopBtn.gameObject.SetActive(true);
			firstLetterBtn.gameObject.SetActive(true);
			anyLetterBtn.gameObject.SetActive(true);
			anyWordBtn.gameObject.SetActive(true);
		}

	}
	private int isLocked = 0;
	public void Open(bool isLocked = false) {

		if (isLocked)
			this.isLocked++;
		if (_isOpen) return;

		_isOpen = true;
		anim.Play(_showAnim);
		if (_isOpen) {
			shopBtn.gameObject.SetActive(true);
			firstLetterBtn.gameObject.SetActive(true);
			anyLetterBtn.gameObject.SetActive(true);
			anyWordBtn.gameObject.SetActive(true);
		}
	}

  private void OnDisable() {
    Close(true);
  }

  public void Close(bool isLocked = false) {

		if (!_isOpen) return;

		if (isLocked)
			this.isLocked--;

    if (this.isLocked <= 0) this.isLocked = 0;

    if (this.isLocked > 0) return;

		_isOpen = false;
		anim.Play(_hideAnim);
	}
	
	public void HintFirstLetter() {
		AudioManager.Instance.library.PlayClickAudio();
		PlayerManager.Instance.HintFirstLetter();
		Click();
	}

	public void HintOpenEnyLetter() {
		AudioManager.Instance.library.PlayClickAudio();
		PlayerManager.Instance.HintAnyLetter();
		Click();
	}

	public void HintOpenFirstWord() {
		AudioManager.Instance.library.PlayClickAudio();
		PlayerManager.Instance.HintFirstWord();
		Click();
	}

	public void ShopButton() {
		AudioManager.Instance.library.PlayClickAudio();
		GameManager.Instance.Shop();
		Click();
	}

	public void HideComplete() {
		shopBtn.gameObject.SetActive(false);
		firstLetterBtn.gameObject.SetActive(false);
		anyLetterBtn.gameObject.SetActive(false);
		anyWordBtn.gameObject.SetActive(false);
	}

}
