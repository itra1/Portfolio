using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LetterIsland : MonoBehaviour {

	public Action<int> OnClick;

	public Animation anim;

	public SpriteRenderer backGraphic;
	public TextMeshProScale letterText;

	private string _letter;
	private int _num;

	private bool _isHint;
	public bool isHint { get { return _isHint; } }

	private bool _isCoin;

	public bool isCoin {
		get { return _isCoin; }
		set {
			_isCoin = value;
			coin.gameObject.SetActive(_isCoin);
		}
	}
	public SpriteRenderer coin;

	public string letter {
		get { return _letter; }
	}

	public bool isOpen = false;

	private void OnEnable() {
		isOpen = false;
		backGraphic.gameObject.SetActive(false);
		letterText.gameObject.SetActive(false);
	}

	public void SetData(int num, string letter) {
		_num = num;
		_letter = letter;
		_isHint = false;
		this.letterText.SetText(letter, PlayerManager.Instance.company.actualCompany);
	}

	public void SetStatus(bool isOpen, bool isAnim = false, bool isAutoDeactive = false) {
		this.isOpen = isOpen;

		if (_isCoin) {
			isCoin = false;
			if (!isAutoDeactive) {
				var panel = (PlayGamePlay) UIManager.Instance.GetPanel(UiType.game);
				panel.StartCoinsMover(transform.position);
			}
		}

		if (isAnim)
			Animated();
	}

	public void Animated(bool isFirst = false) {

		if (isFirst) {
			backGraphic.gameObject.SetActive(true);
			PlayOpenLetterAudio();

			transform.localScale = Vector3.one;
			anim.Play("firstShow");
			return;
		}

		if (isOpen)
			ShowAnim();

		if (_isHint)
			SetHint(false);

		/*
		Color cl = backGraphic.color;
		backGraphic.color = new Color(cl.r, cl.g, cl.b, (isOpen ? 1 : .4f));
		Color clt = letterText.color;
		letterText.color = new Color(clt.r, clt.g, clt.b, 1);
		letterText.gameObject.SetActive(isOpen);
		*/
	}

	public void SetHint(bool isFirst) {
		if (this.isOpen) return;
		_isHint = true;


		if (!isFirst) {
			PlayOpenLetterAudio();
			anim.Play("hint");
		}
		/*
		Color clt = letterText.color;
		letterText.color = new Color(clt.r, clt.g, clt.b, .5f);
		letterText.gameObject.SetActive(true);
		*/
	}

	private void OnDisable() {
		SetStatus(false, false, true);
	}

	public void Click() {
		if (OnClick != null) OnClick(_num);
	}

	public void ShowAnim() {
		PlayOpenLetterAudio();
		anim.Play(_isHint ? "showHint" : "show");
	}

	public List<AudioClipData> openLetterAudio;
	public void PlayOpenLetterAudio() {
		AudioManager.PlayEffects(openLetterAudio[UnityEngine.Random.Range(0, openLetterAudio.Count)], AudioMixerTypes.effectUi);
	}

}
