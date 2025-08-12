using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HardCashUi : MonoBehaviour {

	public Text valueHardCash;
	// Use this for initialization
	void Start () {
		UserManager.HardCashChange += ChangeHardCash;
		ChangeHardCash(UserManager.Instance.HardCash);
	}
	private void OnDestroy() {
		UserManager.HardCashChange -= ChangeHardCash;
	}

	void ChangeHardCash(int value) {
		valueHardCash.text = Utils.RoundValueString(value);
	}

  public void ButtonAdd() {

    ShopDialog panel = UiController.GetUi<ShopDialog>();
    panel.Show();
    panel.SetPage(ShopDialog.Pages.backs);

  }

}
