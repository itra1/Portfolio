using UnityEngine;

public class MenuGamePlay : UiDialog {
	
	public System.Action OnReadXml;
	public System.Action OnCompany;
	public System.Action OnSurvival;
	public System.Action OnCrusade;
	public System.Action OnArena;
	public System.Action OnProfile;
	public System.Action OnShop;
	public System.Action OnOptions;
	public System.Action OnExit;

  [SerializeField]
	private GameObject arenaButton;

  [SerializeField]
  private I2.Loc.Localize companiaText;

	private void OnEnable() {

    arenaButton.SetActive(false);
#if OPTION_ARENA
    arenaButton.SetActive(true);
#endif

    companiaText.Term = LevelsManager.Instance.LevelsList[0].Status != PointSatus.blocked ? "menu.continue" : "menu.compain";
  }

	/// <summary>
	/// Кнопка компании
	/// </summary>
	public void CompanuButton() {
		//ButtonCancel();
		//return;

		UIController.ClickPlay();
		if (OnCompany != null) OnCompany();

	}

	public void ButtonCancel() {
		UIController.RejectPlay();
	}

	/// <summary>
	/// Кнопка сурвивал игры
	/// </summary>
	public void SurvivalButton() {
		UIController.ClickPlay();
		if (OnSurvival != null) OnSurvival();
	}

	/// <summary>
	/// Кнопка крестового похода
	/// </summary>
	public void CrusadeButton() {
		UIController.ClickPlay();
		if (OnCrusade != null) OnCrusade();
	}

	public void ArenaButton() {
		UIController.ClickPlay();
		if (OnArena != null) OnArena();
	}

	/// <summary>
	/// Кнопка профиля
	/// </summary>
	public void ProfileButton() {
		UIController.ClickPlay();
		if (OnProfile != null) OnProfile();
	}

	/// <summary>
	/// Кнопка Магазина
	/// </summary>
	public void ShopButton() {
		UIController.ClickPlay();
		if (OnShop != null) OnShop();
	}

	/// <summary>
	/// Кнопка опций
	/// </summary>
	public void OptionsButton() {
		UIController.ClickPlay();
		if (OnOptions != null) OnOptions();
	}

	/// <summary>
	/// Кнопка выхода
	/// </summary>
	public void ExitButton() {
		UIController.ClickPlay();
		if (OnExit != null) OnExit();
	}

	public void ReadXml() {
		if (OnReadXml != null) OnReadXml();
	}

}
