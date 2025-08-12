using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUi : UiPanel {

	public Action OnNext;
	public Action OnBack;
	public Text secretWordsText;
	public Text coinsCountText;
	public Animation animComp;

	public GameObject locker;

	public GameObject starsBlock;
	public GameObject nextButton;

	public List<Star> starList;
	
	public AudioClipData levelCompletedAudio;

	public Transform octopus;

	private bool octopusMove = false;

	public void PlayLevelCompletedAudio() {
		AudioManager.PlayEffects(levelCompletedAudio, AudioMixerTypes.effectUi);
	}

	protected override void OnEnable() {
		base.OnEnable();
		PlayerManager.Instance.company.GetSaveLevel();

		starsBlock.gameObject.SetActive(!PlayerManager.Instance.company.isBonusLevel);
		nextButton.gameObject.SetActive(!PlayerManager.Instance.company.isBonusLevel);

		if (!PlayerManager.Instance.company.isBonusLevel) {
			starList.ForEach((elem) => {
				elem.active.SetActive(false);
				elem.deactive.SetActive(true);
			});
		}
		
		Show();
	}

	private GameCompany.Level _alvl;
	private GameCompany.Save.Level _lvl;

	public void SetData(GameCompany.Level alvl, GameCompany.Save.Level lvl) {

		_alvl = alvl;
		_lvl = lvl;
		
		int totalSecret = 0;
		int openSecret = 0;

		for (int i = 0; i < alvl.words.Count; i++) {
			if (!alvl.words[i].primary) {
				totalSecret++;
				if (lvl.words.Exists(x => x.word == alvl.words[i].word))
					openSecret++;
			}
		}
		secretWordsText.text = String.Format("{0}/{1}", openSecret, totalSecret);
	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		animComp.Play("showBack");
	}

	public void NextButton() {
		AudioManager.Instance.library.PlayClickAudio();
		if (OnNext != null) OnNext();
	}

	public void BackButton() {
		AudioManager.Instance.library.PlayClickAudio();
		if(octopusMove) return;
		if (OnBack != null) OnBack();
	}

	public override void ShowComplited() {
		//PlayLevelCompletedAudio();
		base.ShowComplited();
		int starCount = _alvl.GetCountStar();

		StartCoroutine(ShowStar(starCount));
	}

	IEnumerator ShowStar(int count) {

		yield return new WaitForSeconds(0.2f);
		for (int i = 0; i < count; i++) {
			starList[i].anim.Play("show");
			yield return new WaitForSeconds(0.14f);
		}

	}

	public void BackShowCompleted() {

		octopusMove = true;
		//OctopusController.Instance.PlayHide(() => {
			OctopusController.Instance.graphic.transform.position = octopus.position;
			OctopusController.Instance.ToFrontOrder();
			OctopusController.Instance.PlayShow(() => {
				octopusMove = false;
				StartCoroutine(PlayHappy());
				animComp.Play("show");
			});
		//});
		
	}

	IEnumerator PlayHappy() {
		yield return null;
		OctopusController.Instance.Happy();
	}

	IEnumerator MoveOctopus(Action onComplete) {

		Transform moveTransform = OctopusController.Instance.graphic;

		bool isMove = true;

		while (isMove) {
			Vector3 newPosition = moveTransform.position + (octopus.position - moveTransform.position).normalized * 15 * Time.deltaTime;
			if ((octopus.position - moveTransform.position).magnitude >
			    (newPosition - moveTransform.position).magnitude) {
				moveTransform.position = newPosition;
			}
			else {
				moveTransform.position = octopus.position;
				isMove = false;
			}
			yield return null;
		}

		onComplete();
	}

	public override void ManagerClose() {
		if (isClosing) return;
		if (locker.activeInHierarchy) return;

		BackButton();
	}

	[System.Serializable]
	public struct Star {
		public Animation anim;
		public GameObject active;
		public GameObject deactive;
	}
}
