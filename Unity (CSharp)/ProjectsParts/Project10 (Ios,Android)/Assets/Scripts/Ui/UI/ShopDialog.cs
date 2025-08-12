using System;
using UnityEngine;
using UnityEngine.UI;
using ExEvent;

/// <summary>
/// Диалоговое окно перед входом в магазин
/// </summary>
public class ShopDialog : PanelUi {

	public Action<shopTypes,Vector3> OnAction;
	private bool isActive;

	protected override void OnEnable() {
		base.OnEnable();
		isActive = true;
		AudioManager.PlayEffect(shopOpen, AudioMixerTypes.runnerEffect);
		InitCounts();
	}

	public AudioClip shopOpen;

	#region Счетчики
	
	public GameObject mountsCount;
	public GameObject armorCount;
	public GameObject powerCount;
	public GameObject upgradesCount;

	/// <summary>
	/// Инифиализация счетчиков
	/// </summary>
	void InitCounts() {
		SetCounrs(Shop.Products.ProductType.cloth, armorCount);
		SetCounrs(Shop.Products.ProductType.gadget, powerCount);
		SetCounrs(Shop.Products.ProductType.upgrade, upgradesCount);
		SetCounrs(Shop.Products.ProductType.mount, mountsCount);
	}
	/// <summary>
	/// Инифиализация одного счетчика
	/// </summary>
	/// <param name="shopType">Категория товара</param>
	/// <param name="counter">Ссылка на объект счетчика</param>
	void SetCounrs(Shop.Products.ProductType productType, GameObject counter) {
		int elementsCount = Config.GetFolowsCount(productType);

		if(elementsCount == 0) {
			counter.gameObject.SetActive(false);
		} else {
			counter.gameObject.SetActive(true);
			counter.transform.GetComponentInChildren<Text>().text = elementsCount.ToString();
		}
	}

	#endregion

	#region Buttons

	/// <summary>
	/// Реакция на кнопку Петов
	/// </summary>
	public void MountsButton(Transform pos) {
		ShopPage(shopTypes.mounts, pos.position);
	}

	/// <summary>
	/// Реакция на кнопку Армора
	/// </summary>
	public void ArmorsButton(Transform pos) {
		ShopPage(shopTypes.clothes, pos.position);
	}
	/// <summary>
	/// Реакция на кнопку расходников
	/// </summary>
	public void PowerUpsButton(Transform pos) {
		ShopPage(shopTypes.powers, pos.position);
	}

	/// <summary>
	/// Реакция на кнопку прокачки
	/// </summary>
	public void UpgradesButton(Transform pos) {
		ShopPage(shopTypes.upgrades, pos.position);
	}

	/// <summary>
	/// Реакция на кнопку Зооолота
	/// </summary>
	public void GooldsButton(Transform pos) {
		ShopPage(shopTypes.golds, pos.position);
	}

	void ShopPage(shopTypes needType, Vector3 positionButton) {
		UiController.ClickButtonAudio();

		if (!isActive) return;
		isActive = false;

		if (OnAction != null) OnAction(needType, positionButton);
	}
	
	/// <summary>
	/// Реакция на кнопку закрытия
	/// </summary>
	public void CloseButton() {
		GetComponent<Animator>().SetTrigger("close");
		UiController.ClickButtonAudio();
		Invoke("ClosePanel", 0.5f);
	}

	/// <summary>
	/// Закрытие панели
	/// </summary>
	void ClosePanel() {
		RunnerController.Instance.ShowRunnerMenu(true);
		gameObject.SetActive(false);
	}

	#endregion
	
	public override void BackButton() {
		CloseButton();
	}
}
