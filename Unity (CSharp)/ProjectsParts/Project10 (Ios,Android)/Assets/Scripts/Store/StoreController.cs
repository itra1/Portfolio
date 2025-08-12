using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Тиры покупок
public enum StoreItemType { stamina, speed, lounch, armorBoxex, magnet, saves, weapon, access, jump, health };

[System.Serializable]
public struct StoreItems {
	public int price; // Цена покупки
	public string description; //Описание покупки
	public StoreItemType group; // группв
	public int level; // уровень в группе
}
public class StoreController : MonoBehaviour {

	public GameObject DialogUI;     // Диалог покупки
	private StoreDialogController dialog; // Диалоговое окно
	public Text MoneyCount; // Монеты
	public GameObject UpgradeMenu; //Менюсо статами
	public GameObject perks; //Меню с модифвкациями

	public StoreItems[] storeItem; // Массив покупок

	void Start() {
		dialog = DialogUI.GetComponent<StoreDialogController>();
		UpdateStoreInterface();

	}

	// Диалоговое окно с покупкой
	public void DialogBuy(int ident) {
		dialog.ShowDialogBuy(ident, storeItem[ident].price, storeItem[ident].description);
	}

	// Подтверждение выполнения покупки
	public void BuyCommit(int ident) {
		StoreItems sales = storeItem[ident];

		if(UserManager.coins < sales.price) // Возврат при недостатке монет
			return;

		int val = PlayerPrefs.GetInt(sales.group.ToString());
		PlayerPrefs.SetInt(sales.group.ToString(), val + 1);
		UserManager.coins -= sales.price;

		UpdateStoreInterface();
	}

	public void UpdateStoreInterface() {
		// Обновляем состояние панели после покупки или загрузки

		// Записьо монетах
		MoneyCount.text = UserManager.coins.ToString();
		// панель со статами
		UpgradeMenu.GetComponent<UpgrabeMenuController>().UpdateThisPanel();
		//Панель с модификаторами
		perks.GetComponent<PerksController>().UpdateGroups();
	}

	public int GetPriceItem(int id) {
		return storeItem[id].price;
	}
}
