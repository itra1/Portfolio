using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSaleDialog : PanelUi {

  private bool isClose;             // Флаг закрытия

  public string[] productArray;

  protected override void OnEnable() {
    base.OnEnable();
    isClose = false;
  }

  protected override void OnDisable() {
    base.OnDisable();
    isClose = false;
    if (OnClose != null) OnClose();

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
    if (isClose) {
      isClose = false;
      gameObject.SetActive(false);
    }
  }
  /// <summary>
  /// Клик по кнопке с покупкой энергии
  /// </summary>
  /// <param name="numInArray"></param>
  public void ButtonByeEnergy(int numInArray) {

    if(numInArray == 0) {

      if (GoogleAdsMobile.Instance.IsLoaded) {
        GoogleAdsMobile.Instance.ShowRewardVideo(() => {
          var product = Assets.Scripts.Billing.BillingController.Instance.productLidt.Find(x => x is Assets.Scripts.Billing.HeartVideoProduct);
          product.Bye();
        });
      }
      return;
    }

    Assets.Scripts.Billing.BillingController.Instance.ByeProduct(productArray[numInArray], () => {
      Close();
      //EnergySales.Instance.OnConfirm = Close;
      //EnergySales.Instance.DialogButtonBye(numInArray);
    });

  }

  public override void BackButton() {
    Close();
  }



}
