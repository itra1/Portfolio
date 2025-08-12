using UnityEngine;
using System.Collections;

public class StorePerksItem : MonoBehaviour {

	public int StoreItemId; // Идентификатор товара из магазина
    private PerksController perks;

    // Купить элемент, по идентификатору покупки
    public void ByeThisItem()
    {
        perks = GameObject.Find("Perks").GetComponent<PerksController>();
        perks.ByeItem(StoreItemId);
    }
}
