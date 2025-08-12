using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/*
 * Класс по контролю меню апгрейда
 * 
 */
public class UpgrabeMenuController : MonoBehaviour {

    public GameObject[] upgradeItemStamina; // Изображение элемента
    public GameObject[] upgradeItemSpeed; // Изображение элемента
    public GameObject[] upgradeItemLounch; // Изображение элемента
    public GameObject[] upgradeItemHealth; // Изображение жизней

    public Button StaminaAddButton;     
    public Button SpeedAddButton;
    public Button LounchAddButton;
    public Button HealthAddButton;

    public GameObject StoreObject;
    private StoreController store;

    private int stamina;
    private int speed;
    private int lounch;
    private int health;

    StoreItems[] storeItem;
    
    void Start()
    {
        store = StoreObject.GetComponent<StoreController>();
        UpdateThis();
    }

    // Публичная функция для вызова обновления извне
    public void UpdateThisPanel()
    {
        UpdateThis();
    }

    // Внутрення функция обновления
    private void UpdateThis()
    {
        stamina = PlayerPrefs.GetInt("stamina");
        speed = PlayerPrefs.GetInt("speed");
        lounch = PlayerPrefs.GetInt("lounch");
        health = PlayerPrefs.GetInt("health");

        UpdateArray(ref upgradeItemStamina,stamina);
        UpdateArray(ref upgradeItemSpeed,speed);
        UpdateArray(ref upgradeItemLounch, lounch);
        UpdateArray(ref upgradeItemHealth, health);

        storeItem = StoreObject.GetComponent<StoreController>().storeItem;
        bool staminaButton = false;
        bool speedButton = false;
        bool lounchButton = false;
        bool healthButton = false;

        int coins = UserManager.coins;

        for (int i = 0; i < storeItem.Length; i++) //изем достепные для покупкистаты
        {
            //Если есть доступные статы для покупки, активируем кнопку
            if (storeItem[i].group.ToString() == "stamina"
                && PlayerPrefs.GetInt("stamina") < storeItem[i].level 
                && storeItem[i].price <= coins
                )
                staminaButton = true;

            if (storeItem[i].group.ToString() == "speed"
                && PlayerPrefs.GetInt("speed") < storeItem[i].level
                && storeItem[i].price <= coins
                )
                speedButton = true;

            if (storeItem[i].group.ToString() == "lounch"
                && PlayerPrefs.GetInt("lounch") < storeItem[i].level
                && storeItem[i].price <= coins
                )
                lounchButton = true;

            if (storeItem[i].group.ToString() == "health"
                && PlayerPrefs.GetInt("health") < storeItem[i].level
                && storeItem[i].price <= coins
                )
                healthButton = true;
        }

        if (staminaButton)
            StaminaAddButton.interactable = true;
        else
            StaminaAddButton.interactable = false;

        if (speedButton)
            SpeedAddButton.interactable = true;
        else
            SpeedAddButton.interactable = false;

        if (lounchButton)
            LounchAddButton.interactable = true;
        else
            LounchAddButton.interactable = false;

        if (healthButton)
            HealthAddButton.interactable = true;
        else
            HealthAddButton.interactable = false;
    }
    // Обновляем отображение
    private void UpdateArray(ref GameObject[] array, int count)
    {
        for (int i = 0; i < array.Length; i++)
            if (i+1 <= count)
                array[i].SetActive(true);
            else
                array[i].SetActive(false);
    }

    private void ChangeStat(int id, string name)
    {
        int firstItemInStore = store.storeItem.Length; //позиция первой покупки в машагазине

        for (int ii = 0; ii < store.storeItem.Length; ii++)
        {
            if (store.storeItem[ii].group.ToString() == name && firstItemInStore > ii)
                firstItemInStore = ii;
        }

        int stat = PlayerPrefs.GetInt(name);

        if (stat >= 5) // Если все куплено
            return;

        int ids = (firstItemInStore - 1) + stat + id; // предыдущая позиция в машазине + ужекупленное + новый

        store.DialogBuy(ids);

        UpdateThis();
    }

    public void ChangeHealth()
    {
        ChangeStat(1, "health");
    }

    public void ChangeStamina()
    {
        ChangeStat(1, "stamina");
    }

    public void ChangeSpeed()
    {
        ChangeStat(1, "speed");
    }

    public void ChangeLounch()
    {
        ChangeStat(1, "lounch");
    }
}
