using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoftCashUi : MonoBehaviour {

	public Text valueSoftCash;
	// Use this for initialization
	void Start() {
		UserManager.SoftCashChange += ChangeSoftCash;
		ChangeSoftCash(UserManager.Instance.Gold);
	}

	private void OnDestroy() {
		UserManager.SoftCashChange -= ChangeSoftCash;
	}

	void ChangeSoftCash(int value) {
		valueSoftCash.text = Utils.RoundValueString(value);
  }

  public void ButtonAdd() {

    ShopDialog panel = UiController.GetUi<ShopDialog>();
    panel.Show();
    panel.SetPage(ShopDialog.Pages.gold);

  }
}
