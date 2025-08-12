using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorUiGamePlay : ExEvent.EventBehaviour {

	public GameObject panel;

	public GameObject spearArmorIcon;
	public GameObject breackArmorIcon;
	public GameObject barrierArmorIcon;
	public GameObject hendingArmorIcon;
	int defendStones;
	int breackReturn;
	int defendSpear;
	int defendHending;

	void Start() {
		InitArmor();
	}


	void InitArmor() {

		spearArmorIcon.SetActive(false);
		breackArmorIcon.SetActive(false);
		barrierArmorIcon.SetActive(false);
		hendingArmorIcon.SetActive(false);

		int summ = 0;

		ClothesBonus defendbarrier = Config.GetActiveCloth(ClothesSets.defendBarrier);
		defendStones = 0 + (defendbarrier.head ? 1 : 0) + (defendbarrier.spine ? 1 : 0) + (defendbarrier.accessory ? 1 : 0);
		summ += defendStones;

		ClothesBonus beackClothes = Config.GetActiveCloth(ClothesSets.noBreack);
		breackReturn = 0 + (beackClothes.head ? 1 : 0) + (beackClothes.spine ? 1 : 0) + (beackClothes.accessory ? 1 : 0);
		summ += breackReturn;

		ClothesBonus spearClothes = Config.GetActiveCloth(ClothesSets.noAirAttack);
		defendSpear = 0 + (spearClothes.head ? 1 : 0) + (spearClothes.spine ? 1 : 0) + (spearClothes.accessory ? 1 : 0);
		summ += defendSpear;

		ClothesBonus hendingClothes = Config.GetActiveCloth(ClothesSets.noHendingBarrier);
		defendHending = 0 + (hendingClothes.head ? 1 : 0) + (hendingClothes.spine ? 1 : 0) + (hendingClothes.accessory ? 1 : 0);
		summ += defendHending;

		if (summ > 0)
			panel.SetActive(true);

		if (defendStones > 0) {
			barrierArmorIcon.SetActive(true);
		}
		if (breackReturn > 0) {
			breackArmorIcon.SetActive(true);
		}
		if (defendSpear > 0) {
			spearArmorIcon.SetActive(true);
		}
		if (defendHending > 0) {
			hendingArmorIcon.SetActive(true);
		}
		CalcScaleArmorIcon();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.SetArmorPlayer))]
	public void ArmorActiv(ExEvent.GameEvents.SetArmorPlayer eventData) {
		
		bool recalcPosition = false;

		if (eventData.cloth == ClothesSets.defendBarrier && defendStones > 0) {
			defendStones--;
			if (defendStones > 0)
				barrierArmorIcon.GetComponent<Animator>().SetTrigger("active");
			else {
				barrierArmorIcon.GetComponent<Animator>().SetTrigger("deactive");
				recalcPosition = true;
			}
		}

		if (eventData.cloth == ClothesSets.noBreack && breackReturn > 0) {
			breackReturn--;
			if (breackReturn > 0)
				breackArmorIcon.GetComponent<Animator>().SetTrigger("active");
			else {
				breackArmorIcon.GetComponent<Animator>().SetTrigger("deactive");
				recalcPosition = true;
			}
		}

		if (eventData.cloth == ClothesSets.noAirAttack && defendSpear > 0) {
			defendSpear--;
			if (defendSpear > 0)
				spearArmorIcon.GetComponent<Animator>().SetTrigger("active");
			else {
				spearArmorIcon.GetComponent<Animator>().SetTrigger("deactive");
				recalcPosition = true;
			}
		}

		if (eventData.cloth == ClothesSets.noHendingBarrier && defendHending > 0) {
			defendHending--;
			if (defendHending > 0)
				hendingArmorIcon.GetComponent<Animator>().SetTrigger("active");
			else {
				hendingArmorIcon.GetComponent<Animator>().SetTrigger("deactive");
				recalcPosition = true;
			}
		}

		if (recalcPosition) InvokeRecalcPosition();
	}

	void InvokeRecalcPosition() {
		Invoke("RecalcPositionArmorIcon", 0.5f);
	}

	float displayArmorDol;

	void CalcScaleArmorIcon() {
		int activeHeart = (int)RunnerController.Instance.healthManager.actualValue - 1;

		//Vector2 lastHeart = hearthItems[activeHeart].GetComponent<RectTransform>().position + healthObject.GetComponent<RectTransform>().localPosition;
		Vector2 lastHeart = Vector2.zero;

		displayArmorDol = Camera.main.pixelWidth / 100;
		Vector2 clothPanel = panel.GetComponent<RectTransform>().localPosition;
		panel.GetComponent<RectTransform>().localPosition = new Vector3(lastHeart.x, clothPanel.y);
		RecalcPositionArmorIcon();
	}


	void RecalcPositionArmorIcon() {
		int num = 0;

		Vector3 thisPos;

		if (defendStones > 0) {
			num++;
			thisPos = barrierArmorIcon.GetComponent<RectTransform>().anchoredPosition;
			barrierArmorIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3((displayArmorDol * 6 * num) - (displayArmorDol * 2), thisPos.y, thisPos.z);
		}
		if (breackReturn > 0) {
			num++;
			thisPos = breackArmorIcon.GetComponent<RectTransform>().anchoredPosition;
			breackArmorIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3((displayArmorDol * 6 * num) - (displayArmorDol * 2), thisPos.y, thisPos.z);
		}
		if (defendSpear > 0) {
			num++;
			thisPos = spearArmorIcon.GetComponent<RectTransform>().anchoredPosition;
			spearArmorIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3((displayArmorDol * 6 * num) - (displayArmorDol * 2), thisPos.y, thisPos.z);
		}
		if (defendHending > 0) {
			num++;
			thisPos = hendingArmorIcon.GetComponent<RectTransform>().anchoredPosition;
			hendingArmorIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3((displayArmorDol * 6 * num) - (displayArmorDol * 2), thisPos.y, thisPos.z);
		}

	}

}
