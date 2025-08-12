using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapon
{
  

public class RadiatorWeapon : Bullet {

	List<Collider2D> damageList = new List<Collider2D>();

	public override void OnEnable() {
		base.OnEnable();
		damageList.Clear();
	}

	public override void OnTriggerEnter2D(Collider2D col) {
		if(!damageList.Exists(x => x == col))
			base.OnTriggerEnter2D(col);
	}

	protected override void DamageObject(Collider2D col) {
		base.DamageObject(col);
		damageList.Add(col);
	}

}

}