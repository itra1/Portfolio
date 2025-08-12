using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour {

	public Text valueEnergy;
	// Use this for initialization
	void Start() {
    UserEnergy.EnergyChange += ChangeEnergy;
		ChangeEnergy(UserEnergy.Instance.energy);
	}

	private void OnDestroy() {
    UserEnergy.EnergyChange -= ChangeEnergy;
	}

	void ChangeEnergy(float value) {
		valueEnergy.text = Utils.RoundValueString(value);
  }

  public void ButtonAdd() {

    ShopDialog panel = UiController.GetUi<ShopDialog>();
    panel.Show();
    panel.SetPage(ShopDialog.Pages.energy);

  }

}
