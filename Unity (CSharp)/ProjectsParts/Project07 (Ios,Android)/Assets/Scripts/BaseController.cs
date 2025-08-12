using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseController : MonoBehaviour {

  // Use this for initialization
  void Start () {
		OpenBaseUi();
    GameManager.Instance.PlayBackGroundSound("BackGround/menuBack");
  }

	public void OpenBaseUi() {
		BaseUi panel = UiController.GetUi<BaseUi>();
		panel.gameObject.SetActive(true);
		panel.onSetting = OpenSettingDialog;
		panel.onShop = OpenShopDialog;
		panel.onArsenal = OpenArsenalDialog;
		panel.onJournal = OpenJournalDialog;
		panel.onBattle = OpenBattle;
		panel.onMap = OpenMap;
	}

	public void OpenMap() {
    GameManager.Instance.LoadScene("Map");
	}

	void OpenSettingDialog() {
		SettingDialog panel = UiController.GetUi<SettingDialog>();
		panel.Show();
	}

	void OpenShopDialog() {
		ShopDialog panel = UiController.GetUi<ShopDialog>();
		panel.Show();
	}

	void OpenArsenalDialog() {
		ArsenalDialog panel = UiController.GetUi<ArsenalDialog>();
		panel.Show();
	}

	void OpenJournalDialog() {
		JournalDialog panel = UiController.GetUi<JournalDialog>();
		panel.Show();
	}

	void OpenBattle() {

    if (!UserEnergy.Instance.ExistsEnergy)
      return;

    GameManager.Instance.StartBattle(UserManager.Instance.SelectLevel != UserManager.Instance.CompleteLastLevel ? UserManager.Instance.SelectLevel : UserManager.Instance.CompleteLastLevel +1);

  }

}
