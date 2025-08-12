using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// Энергетическая панель
/// </summary>
public class EnergyDialog : PanelUi {

  private bool isClose;             // Флаг закрытия

	public string[] productArray;

	protected override void OnEnable() {
		base.OnEnable();
    isClose = false;
  }

  protected override void OnDisable() {
		base.OnDisable();
    isClose = false;
		if (OnClose != null)	OnClose();

	}

  /// <summary>
  /// Запуск процесса закрытия
  /// </summary>
  public void Close() {
    isClose = true;
    GetComponent<Animator>().SetTrigger("close");
  }

  /// <summary>
  /// Успешное подтверждение работы
  /// </summary>
  public void OnCloseEvent() {
    if(isClose) {
      isClose = false;
      gameObject.SetActive(false);
    }
  }
  /// <summary>
  /// Клик по кнопке с покупкой энергии
  /// </summary>
  /// <param name="numInArray"></param>
  public void ButtonByeEnergy(int numInArray) {

		Assets.Scripts.Billing.BillingController.Instance.ByeProduct(productArray[numInArray], () => {
			//EnergySales.Instance.OnConfirm = Close;
			//EnergySales.Instance.DialogButtonBye(numInArray);
		});

	}
	
	public override void BackButton() {
		Close();
	}


}
