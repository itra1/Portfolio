using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultDropItem : MonoBehaviour {

	public Action OnShow;

	public Sprite silverCoin;
	public Sprite goldCoin;
	public RectTransform iconBlock;
	public RectTransform weaponBlock;

	public Image coinIcon;
	public Image weaponIcon;
	public Text countText;
	public AspectRatioFitter ratioFilter;

	public RectTransform block;

	Drop dropItem;

	public void ShowComplited() {
		if (OnShow != null) OnShow();
	}

	public void SetData(Drop drop) {
		countText.rectTransform.pivot = new Vector2(0, 0.5f);
		Debug.Log("DropTyp = " + drop.type);

		switch (drop.type) {
			case DropType.coins:
				coinIcon.sprite = silverCoin;
				break;
			case DropType.superCoins:
				coinIcon.sprite = goldCoin;
				break;
			case DropType.weapon:
				weaponIcon.sprite = Game.User.UserWeapon.Instance.weaponsManagers.Find(x => x.GetComponent<Game.Weapon.WeaponManager>().weaponType == drop.weaponType).GetComponent<Game.Weapon.WeaponManager>().Icon;
				break;
		}
		ratioFilter.aspectRatio = coinIcon.sprite.rect.width / coinIcon.sprite.rect.height;

		countText.text = "+ " + drop.count.ToString();
		
		switch (drop.type) {
			case DropType.coins:
			case DropType.superCoins:
				iconBlock.gameObject.SetActive(true);
				weaponBlock.gameObject.SetActive(false);
				countText.rectTransform.anchoredPosition = new Vector2(31, countText.rectTransform.anchoredPosition.y);
				block.sizeDelta = new Vector2(iconBlock.sizeDelta.x + countText.preferredWidth, block.sizeDelta.y);
				break;
			case DropType.weapon:
				iconBlock.gameObject.SetActive(false);
				weaponBlock.gameObject.SetActive(true);
				countText.rectTransform.anchoredPosition = new Vector2(47, countText.rectTransform.anchoredPosition.y);
				block.sizeDelta = new Vector2(weaponBlock.sizeDelta.x + countText.preferredWidth, block.sizeDelta.y);
				break;
		}
		
	}

}
