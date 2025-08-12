using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Менеджер жизней через силу
/// </summary>
public class EnergyHealthManager : HealthManagerBase {
	
	// Урон при получениее повреждения за единицу
	[SerializeField]
	private float damageDelta;

	// Скорость утечки значение жизней
	[SerializeField]
	private float deltaSecong;

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

	private void Update() {
		
		if (Tutorial.TutorialController.isActive) return;
		if((RunnerController.Instance.runnerPhase & (RunnerPhase.run | RunnerPhase.boost)) == 0) return;

		if (actualValue <= 0) actualValue = 0;

		actualValue -= deltaSecong * Time.deltaTime;
		ChangeEvent();
	}



}
