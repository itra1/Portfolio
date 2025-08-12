using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZbCatScene;

public class CatSceneUi : UiDialog {

	public Action<bool> OnNext;

	public GameObject leftNameBlock;
	public I2.Loc.Localize leftNameText;

	public GameObject rightNameBlock;
	public I2.Loc.Localize rightNameText;

	public GameObject upArrow;
	public GameObject downArrow;
	public GameObject nextArrow;

	public RectTransform mainTextContent;
	public I2.Loc.Localize mainText;
	private RectTransform _mainTextRect;
	private Text _mainText;

	private float _maxHright = 72f;
	private float _deltaOffset = 20f;
	//private float _maxHright = 77.5f;

	public void SetData(ZbCatScene.CatBlockBehaviour catBlock ) {
		
		leftNameBlock.SetActive(catBlock.leftHero != HeroType.none);
		rightNameBlock.SetActive(catBlock.rightHero != HeroType.none);

		if (catBlock.leftHero != HeroType.none) {
			HeroInfo leftHero = CatSceneManager.Instance.library.hero.Find(x => x.type == catBlock.leftHero);
			leftNameText.Term = leftHero.name;
		}
		if (catBlock.rightHero != HeroType.none) {
			HeroInfo rightHero = CatSceneManager.Instance.library.hero.Find(x => x.type == catBlock.rightHero);
			rightNameText.Term = rightHero.name;
		}
		mainText.Term = "CatScene." +(catBlock as CatBlockDialog).dialog;

		if (_mainText == null)
			_mainText = mainText.GetComponent<Text>();

		if (_mainTextRect == null)
			_mainTextRect = mainText.GetComponent<RectTransform>();
		
		_mainTextRect.sizeDelta = new Vector2(_mainTextRect.sizeDelta.x, _mainText.preferredHeight);
		mainTextContent.sizeDelta = new Vector2(mainTextContent.sizeDelta.x, _mainTextRect.sizeDelta.y);
		
		Offset(0);
	}

	public void UpButton() {
		UIController.ClickPlay();
		Offset(-1);
	}

	public void DownButton() {
		UIController.ClickPlay();
		Offset(-1);
	}

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Space))
      NextButton();
  }

  private void Offset(float offset) {

		Vector2 ancor = new Vector2(mainTextContent.anchoredPosition.x, mainTextContent.anchoredPosition.y + offset * _deltaOffset);

		if (ancor.y < 0) ancor.y = 0;

		if (_mainTextRect.sizeDelta.y > _maxHright && ancor.y > 0 && ancor.y - _maxHright > -_maxHright)
			ancor.y = -_maxHright;

		mainTextContent.anchoredPosition = ancor;
		upArrow.SetActive(_mainTextRect.sizeDelta.y > _maxHright);
		nextArrow.SetActive(ancor.y - _maxHright >= -_maxHright);
		downArrow.SetActive(ancor.y - _maxHright < -_maxHright);
	}

	public void NextButton() {
		UIController.ClickPlay();
		if (OnNext != null) OnNext(false);
	}

}
