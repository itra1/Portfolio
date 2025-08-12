using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShopProduct : ShopProductBehaviour {

	public int bulletCount;
	public Game.Weapon.WeaponType weaponType;

	public override float count {
		get { return bulletCount; }
	}

	public override Sprite Icon {
		get {
			return base.Icon != null 
				? base.Icon 
				: Game.User.UserWeapon.Instance.weaponsManagers.Find(x => x.weaponType == weaponType).IconActive;
		}
	}

	public override bool Bye() {
		if (!CheckBuy()) return false;

		Game.User.UserManager.Instance.silverCoins.Value -= price;
		UIController.ParametrUpPlay();
	  Game.User.UserWeapon.Instance.AddBullet(weaponType, bulletCount);

		base.Bye();

		return true;

	}

}
