using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorConfirmUi : UiPanel {

	private List<GiftElement> instalceList = new List<GiftElement>();
	public Action OnComplete;
	private bool _isReady;
	public Animation packAnim;
	public RectTransform elementsParent;
	public Transform takeBlock;

	public List<RectTransform> targetPositionsReady;
	public GiftElement giftAnyLetter;
	public GiftElement giftFirstLetter;
	public GiftElement giftFirstWord;

	public GameObject fullClick;

	private int showCount;
	
	protected override void OnEnable() {
		base.OnEnable();
		PlayGiftIdleAudio();
		_isReady = true;
		takeBlock.localScale = Vector3.one;
	}

	protected override void OnDisable() {
		base.OnDisable();
		instalceList.ForEach(x => x.gameObject.SetActive(false));
	}

	public ParticleSystem paticles;

	public void TakeButton() {

		if (!_isReady) return;
		_isReady = false;

		takeBlock.GetComponent<Animation>().Play("hide");

		giftIdleAudioPoint.Stop(0.5f);

		packAnim.Play("hide");
		paticles.Play();

		packAnim.GetComponent<AnimationHelper>().OnEvent1 = StartShow;

		//OnComplete();
		//Hide(() => {
		//	gameObject.SetActive(false);
		//	//DailyBonus.Instance.GetGift();
		//});
	}

	private void HideDialog() {
		OnComplete();
	}


	private void StartShow() {

		CalcGifts();
		CalcPosition();
		showCount = _showList.Count;
		ShowElements();
	}

	List<RectTransform> targetPoints = new List<RectTransform>();
	private void CalcPosition() {
		targetPoints.Clear();
		switch (_showList.Count) {
			case 1:
				targetPoints.Add(targetPositionsReady[0]);
				break;
			case 2:
				targetPoints.Add(targetPositionsReady[1]);
				targetPoints.Add(targetPositionsReady[2]);
				break;
			case 3:
				targetPoints.Add(targetPositionsReady[0]);
				targetPoints.Add(targetPositionsReady[1]);
				targetPoints.Add(targetPositionsReady[2]);
				break;
		}
	}

	private bool isHide;
	public void HideInInspector() {
		if (isHide) return;
		isHide = true;
		HideDialog();
		fullClick.SetActive(false);
	}

	private void MoveCompleteToWidget() {
		showCount--;

		if(showCount == 0)
			HideInInspector();
	}

	private void WainAndGo() {
		fullClick.SetActive(true);

		if (giftFirstWord.gameObject.activeInHierarchy) {
			giftFirstWord.MoveComplete = MoveCompleteToWidget;
			giftFirstWord.toWindet = true;
			giftFirstWord.FindTargetWindet();
		}
		if (giftAnyLetter.gameObject.activeInHierarchy) {
			giftAnyLetter.MoveComplete = MoveCompleteToWidget;
			giftAnyLetter.toWindet = true;
			giftAnyLetter.FindTargetWindet();
		}
		if (giftFirstLetter.gameObject.activeInHierarchy) {
			giftFirstLetter.MoveComplete = MoveCompleteToWidget;
			giftFirstLetter.toWindet = true;
			giftFirstLetter.FindTargetWindet();
		}



	}

	private void ShowElements() {

		if (_showList.Count == 0) {
			Invoke("WainAndGo", 1);
			

			return;
		}

		ShowParam par = _showList[0];
		_showList.RemoveAt(0);

		GiftElement elem = null;

		switch (par.type) {
			case GiftElement.Type.hintWord:
				elem = giftFirstWord;
				break;
			case GiftElement.Type.hintAnyletter:
				elem = giftAnyLetter;
				break;
			default:
				elem = giftFirstLetter;
				break;
		}

		PlayGamePlay gp = (PlayGamePlay)UIManager.Instance.GetPanel(UiType.game);
		elem.Show(par.type, par.count, gp, gp);
		//elem.transform.SetParent(transform);
		elem.targetPosition = targetPoints[0];

		targetPoints.RemoveAt(0);

		//elem.targetPosition = startAncor;
		elem.transform.localScale = Vector3.one;
		elem.GetComponent<RectTransform>().anchoredPosition = packAnim.GetComponent<RectTransform>().anchoredPosition;
		elem.gameObject.SetActive(true);
		elem.MoveComplete = ShowElements;
	}

	public void CalcGifts() {

		for (int i = 0; i < 5; i++) {

			int num = UnityEngine.Random.Range(0, 3);

			HintType useType = (HintType)num;

			switch (useType) {
				case HintType.anyLetter:
					PlayerManager.Instance.hintAnyLetter++;
					AddElementToList(GiftElement.Type.hintAnyletter);
					break;
				case HintType.firstLetter:
					PlayerManager.Instance.hintFirstLetter++;
					AddElementToList(GiftElement.Type.hintLetter);
					break;
				case HintType.firstWord:
					PlayerManager.Instance.hintFirstWord++;
					AddElementToList(GiftElement.Type.hintWord);
					break;
			}
		}
	}

	private void AddElementToList(GiftElement.Type type) {

		if (!_showList.Exists(x => x.type == type)) {
			_showList.Add(new ShowParam() { type = type, count = 1 });
		} else {
			ShowParam newElem = _showList.Find(x => x.type == type);
			_showList.Remove(newElem);
			newElem.count++;
			_showList.Add(newElem);
		}

	}

	private List<ShowParam> _showList = new List<ShowParam>();

	[System.Serializable]
	public struct ShowParam {
		public GiftElement.Type type;
		public int count;
	}

	public AudioClipData giftIdleAudio;
	private AudioPoint giftIdleAudioPoint;

	public void PlayGiftIdleAudio() {
		giftIdleAudioPoint = AudioManager.PlayEffects(giftIdleAudio, AudioMixerTypes.effectUi);
	}

	public override void ManagerClose() {
		/*
		if (_isReady) {
			TakeButton();
			return;
		}

		HideInInspector();
		*/
	}
}
