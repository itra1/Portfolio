using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUiGamePlay : ExEvent.EventBehaviour {

	public GameObject panel;

	public Animation anim;

	public List<WeaponData> weaponList;

	[System.Serializable]
	public struct WeaponData {
		public WeaponTypes type;
		public GameObject icon;
	}

	public Text countText;

	private WeaponTypes activeWeapon;
	private int bulletCount;
	
	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.WeaponActiveChange))]
	public void SetWeapon(ExEvent.GameEvents.WeaponActiveChange eventData) {

		if (eventData.count == 0) {
			activeWeapon = WeaponTypes.none;
			weaponList.ForEach(x=>x.icon.SetActive(false));
			bulletCount = eventData.count;
			countText.text = bulletCount.ToString();
			return;
		}

		if (eventData.first || (eventData.weapon != activeWeapon && !eventData.first) || bulletCount == 0) {
			weaponList.ForEach(x => x.icon.SetActive(eventData.weapon == x.type));
			activeWeapon = eventData.weapon;

		}
		anim.Play("weaponChange");
		//if (animComp != null && animComp.enabled) animComp.SetTrigger("weapon");
		bulletCount = eventData.count;
		countText.text = bulletCount.ToString();

	}


}
