using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PetDialogs : Singleton<PetDialogs> {

	public System.Action OnClick;

	public List<PetDialogParam> textParam;
	
	public LocalizationUiText text;
	public GameObject dialog;

	private bool _isOpen;
	private PetDialogType _activeType;

	public Animation tapHelper;

	private void OnEnable() {
		dialog.GetComponent<AnimationHelper>().OnEvent1 = Close;
	}

	public void Close() {
		dialog.SetActive(false);
	}
	
	public void ClickDialog(bool force = false) {

		//if (_activeType == PetDialogType.conchHelp && !force) return;

		if (OnClick != null) OnClick();

		AudioManager.Instance.library.PlayClickAudio();
		dialog.GetComponent<Animation>().Play("hide");
		_isOpen = false;

		tapHelper.Play("hide");

	}

	public void ShowDialog(PetDialogType type, System.Action OnConfirm) {
		this.OnClick = OnConfirm;
		PetDialogParam elem = textParam.Find(x => x.type == type);
		dialog.SetActive(true);
		text.SetCode(elem.text);
		PlayOpenAudio();
		dialog.GetComponent<Animation>().Play("show");
		_isOpen = true;
		_activeType = type;
		/*
		if (type == PetDialogType.conchHelp) {
			PlayGamePlay pp = (PlayGamePlay) UIManager.Instance.GetPanel(UiType.game);
			pp.SetCounchPointer(true, () => {
				ClickDialog(true);
			});
		}
		*/

		if (elem.tapHelper) {
			tapHelper.gameObject.SetActive(true);
			tapHelper.Play("show");
		}
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintAnyLetterReady))]
	private void OnHintAnyLetterReady(ExEvent.GameEvents.OnHintAnyLetterReady hint) {
		if (_activeType != PetDialogType.getHelp || !_isOpen) return;
		ClickDialog();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintFirstLetterReady))]
	private void OnHintFirstLetterReady(ExEvent.GameEvents.OnHintFirstLetterReady hint) {
		if (_activeType != PetDialogType.getHelp || !_isOpen) return;
		ClickDialog();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintFirstWordReady))]
	private void OnHintFirstWordReady(ExEvent.GameEvents.OnHintFirstWordReady hint) {
		if (_activeType != PetDialogType.getHelp || !_isOpen) return;
		ClickDialog();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnAlphaDown))]
	private void OnAlphaDown(ExEvent.GameEvents.OnAlphaDown hint) {
		if (_activeType != PetDialogType.getHelp || !_isOpen) return;
		ClickDialog();
	}

	public AudioClipData openAudio;

	public void PlayOpenAudio() {
		AudioManager.PlayEffects(openAudio, AudioMixerTypes.effectUi);
	}

}

public enum PetDialogType {
	tutorStart, tutor1End, getHelp, tutorial2End, conchHelp
}

[System.Serializable]
public struct PetDialogParam {
	public PetDialogType type;
	public bool tapHelper;
	public string text;
}