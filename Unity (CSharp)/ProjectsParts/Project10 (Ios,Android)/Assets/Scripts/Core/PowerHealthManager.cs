using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроллер жизней через силу
/// </summary>
public class PowerHealthManager : HealthManagerBase {
	
	// Урон при получениее повреждения за единицу
	[SerializeField]
	private float damageDelta;
	public override void Init() {

		ClothesBonus bonus = Config.GetActiveCloth(ClothesSets.heart);
		maxValue = _maxValue + UserManager.Instance.livePerk * livePerkValue + (bonus.full ? livePerkValue : 0);
		actualValue = maxValue;
		ChangeEvent();
	}

	public override void LiveChange(float delta) {
		actualValue += delta * damageDelta;
		if (_maxValue < actualValue)
			actualValue = _maxValue;
		ChangeEvent();
	}

}
