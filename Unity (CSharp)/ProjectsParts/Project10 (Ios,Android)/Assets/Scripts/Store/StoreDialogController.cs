using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoreDialogController : MonoBehaviour {

    private StoreController store; // Ссылка на магазин

    public Text MoneyCount; // ссылка на интерфейстное число стоимости покупки
    public Text BuyDescriptions; // Ссылка на интерфейсную часть описания покупки

    private int BuyItemId; // Идентификатор рассматриваемой покупки

    void Start()
    {
       // gameObject.SetActive(false);
    }
    
    public void ShowDialogBuy(int id, int price, string descr)
    {
        gameObject.SetActive(true);
        BuyItemId = id;
        MoneyCount.text = price.ToString();
        BuyDescriptions.text = descr;
    }

    public void Buy()
    {
        store = GameObject.Find("Store").GetComponent<StoreController>();
        store.BuyCommit(BuyItemId);
        BuyItemId = -1;
        gameObject.SetActive(false);
    }

    public void NoBuy()
    {
        gameObject.SetActive(false);
    }

    public void CloseThisDialog()
    {
        Debug.Log("No button");
        //gameObject.transform.parent.gameObject.SetActive(false);
    }

}
