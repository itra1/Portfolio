using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DailyBonusUi : UiPanel {

	public Action OnBack;

	public Animation anim;

	public List<DailyBonusGift> giftList;
	public List<Transform> ancorList;

	public GameObject giftRegion;
	public Transform octopus;

	public GameObject rotationElement;
	public GameObject giftsParent;

	public GiftElement gift;

	private bool _readyTouch = true;

	private bool isReadyGift;

	protected override void OnEnable() {
		base.OnEnable();
		isReadyGift = true;
		rotationElement.gameObject.SetActive(false);
		giftsParent.SetActive(false);
		//OctopusController.Instance.ToFrontOrder();
		giftList.ForEach(x => x.transform.localScale = Vector3.one);

		OctopusController.Instance.PlayHide(() => {
			OctopusController.Instance.graphic.transform.position = octopus.position;
			OctopusController.Instance.ToFrontOrder();
			OctopusController.Instance.PlayShow(() => {
				OctopusController.Instance.Happy();
				rotationElement.gameObject.SetActive(true);
				giftsParent.SetActive(true);
				OnReady();
			});
		});


		Show(() => {
		});
	}

	IEnumerator MoveOctopus(Action onComplete) {

		Transform moveTransform = OctopusController.Instance.graphic;

		bool isMove = true;

		while (isMove) {
			Vector3 newPosition = moveTransform.position + (octopus.position - moveTransform.position).normalized * 15 * Time.deltaTime;
			if ((octopus.position - moveTransform.position).magnitude >
					(newPosition - moveTransform.position).magnitude) {
				moveTransform.position = newPosition;
			} else {
				moveTransform.position = octopus.position;
				isMove = false;
			}
			yield return null;
		}

		onComplete();
	}

	public void OnReady() {
		List<int> contAncor = new List<int>();

		for (int i = 0; i < giftList.Count; i++) {

			int rand = 0;
			do {
				rand = Random.Range(0, giftList.Count);
			} while (contAncor.Contains(rand));

			contAncor.Add(rand);

			giftList[i].Set(rand, ancorList[rand], (num) => {
				UseGift();
				//Hide(() => {
				//	UseGift();
				//});
			});
			giftList[i].gameObject.SetActive(true);
		}

		giftRegion.SetActive(true);

	}

	public void UseGift() {

		if (!_readyTouch) return;

		if (!isReadyGift) return;
		isReadyGift = false;

		giftList.ForEach(x=>x.GetComponent<Animation>().Play("giftHide"));

		gift.transform.position = octopus.position;

		var useG = gift.pars[Random.Range(0, gift.pars.Count)];
		PlayGamePlay gp = (PlayGamePlay)UIManager.Instance.GetPanel(UiType.game);
		gift.Show(useG.type, gp, gp);
		gift.gameObject.SetActive(true);
		SaveData(useG);
		
		gift.MoveComplete = () => {
			DailyBonus.Instance.GetGift();
		};
		
		//gameObject.SetActive(false);
		//DailyBonus.Instance.GetGift();
	}

	private void SaveData(GiftElement.GiftElementParam par) {
		switch (par.type) {
				case GiftElement.Type.hintAnyletter:
				PlayerManager.Instance.hintAnyLetter += par.count;
				break;
			case GiftElement.Type.hintLetter:
				PlayerManager.Instance.hintFirstLetter += par.count;
				break;
			case GiftElement.Type.hintWord:
				PlayerManager.Instance.hintFirstWord += par.count;
				break;
			default:
				PlayerManager.Instance.coins += par.count;
				break;
		}
	}

	public void TakeButton() {

		return;
		if (isReadyGift) return;

		DailyBonus.Instance.GetGift();
		/*
		Hide(() => {
			gameObject.SetActive(false);
		});
		*/
	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		anim.Play("show");
	}

	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		anim.Play("hide");
	}

	public void TakeElement() {
		
	}

	public override void ManagerClose() {
		TakeButton();
	}
}
