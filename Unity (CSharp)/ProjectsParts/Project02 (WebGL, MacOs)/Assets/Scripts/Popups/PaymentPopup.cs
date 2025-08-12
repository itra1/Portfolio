using System;
using System.Collections;
using System.Collections.Generic;
 
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaymentPopup : MonoBehaviour
{
    [Space, Header("SelectPayment")]
    private Transform ContentCardsNow;
    [SerializeField] private Transform ContentCards1;
    [SerializeField] private Transform ContentCards2;
    [SerializeField] private Toggle ToggleCardPref;
    [SerializeField] private ToggleGroup ToggleGroupCard;
    private List<PaymentBody.Requisites> Cards = new List<PaymentBody.Requisites>();
    private List<GameObject> CardsToggle = new List<GameObject>();
    private int SelectCard = -1;

    [SerializeField] private TextMeshProUGUI USDText;
    [SerializeField] private TextMeshProUGUI AvailibleText;
    private int Availible;
    [SerializeField] private TMP_InputField NumberDepositField;

    [Space, Header("Buttons")]
    [SerializeField] private Button[] buttonsWindow;

    [Space, Header("Deposit details")]
    [SerializeField] private TextMeshProUGUI MinDepositText;
    [SerializeField] private TextMeshProUGUI MaxDepositText;
    [SerializeField] private TextMeshProUGUI FreeDepositText;
    [SerializeField] private TextMeshProUGUI DailyDepositText;

    [Space, Header("Deposit")]
    [SerializeField] private TMP_InputField AmmountDepositField;

    [Space, Header("Withdrawal")]
    [SerializeField] private TMP_InputField AmmountWithdrawalField;

    [Space, Header("History")]
    [SerializeField] private TMP_InputField DataFromField;
    [SerializeField] private TMP_InputField DataToField;

    [Space, Header("AddCardWindow")]
    [SerializeField] private GameObject AddCardWindow;
    [SerializeField] private TMP_InputField HolderField;
    [SerializeField] private TMP_InputField NumberField;
    [SerializeField] private TMP_InputField YearField;
    [SerializeField] private TMP_InputField MonthField;
    [SerializeField] private TMP_InputField CvvField;



    [SerializeField] private Action<PaymentBody> callback;

    [SerializeField] private Image metodPaymentImage;

    private int WindowNow;
    private int count;

    private IndexRequestMetaData HistoryMetaData;

    public void Show(Action<PaymentBody> callback)
    {
        this.callback = callback;

        USDText.text = GameHelper.UserInfo.user_wallet.amount.ToString();
        AvailibleText.text = $"<color=#C68C43>{"AVAILABLE".Localized()}</color>: $ {Availible.ToString()}";
        SelectWindow(1);

        CardsToggle.ForEach(x => { Destroy(x.gameObject); });
        CardsToggle.Clear();
        for (int i = 0; true; i++)
        {
            if (PlayerPrefs.HasKey("CARD_HOLDER" + i))
            {
                Cards.Add(new PaymentBody.Requisites(PlayerPrefs.GetString("CARD_HOLDER" + i), PlayerPrefs.GetString("CARD_NUMBER" + i),
                    PlayerPrefs.GetInt("CARD_MONTH" + i), PlayerPrefs.GetInt("CARD_YEAR" + i), PlayerPrefs.GetInt("CVV" + i)));
                CreateCardObject(i);
            }
            else
            {
                break;
            }
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Submit()
    {
        if (SelectCard == -1 || SelectCard >= Cards.Count)
        {
            return;
        }

        if (WindowNow == 1)
        {
            callback(new PaymentBody(count, Cards[SelectCard]));
            Hide();
        }
        else
        {
            //Вывод средств
        }
    }

    public void AddCardWindowOpen()
    {
        AddCardWindow.SetActive(true);
    }
    public void AddCardWindowHide()
    {
        AddCardWindow.SetActive(false);
    }
    public void CreateCard()
    {
        if (HolderField.text.Length == 0)
        {
            return;
        }

        if (NumberField.text.Length < 16)
        {
            return;
        }

        if (YearField.text.Length < 2 || !int.TryParse(YearField.text, out int year))
        {
            return;
        }

        if (MonthField.text.Length < 2 || !int.TryParse(MonthField.text, out int month))
        {
            return;
        }

        if (CvvField.text.Length < 3 || !int.TryParse(CvvField.text, out int cvv))
        {
            return;
        }

        Cards.Add(new PaymentBody.Requisites(HolderField.text, NumberField.text, month, year, cvv));
        int n = Cards.Count - 1;
        CreateCardObject(n);

        AddCardWindowHide();
    }
    void CreateCardObject(int n)
    {
        Transform ContentCards = ContentCardsNow == ContentCards2 ? ContentCards1 : ContentCards2;
        ContentCardsNow = ContentCards;
		Toggle toggleCard = Instantiate(ToggleCardPref, ContentCards);
        toggleCard.group = ToggleGroupCard;
        toggleCard.onValueChanged.AddListener((bl) =>
        {
            if (bl)
            {
                SelectPayCard(n);
            }
        });
        toggleCard.transform.SetAsFirstSibling();
        toggleCard.isOn = true;
        CardsToggle.Add(toggleCard.gameObject);

        it.Logger.Log($"AddCard: {Cards[n].cardHolder}\n{Cards[n].cardNumber}\n{Cards[n].expireYear} {Cards[n].expireMonth}\n{Cards[n].cvv}");

        PlayerPrefs.SetString("CARD_HOLDER" + n, Cards[n].cardHolder);
        PlayerPrefs.SetString("CARD_NUMBER" + n, Cards[n].cardNumber);
        PlayerPrefs.SetInt("CARD_MONTH" + n, Cards[n].expireMonth);
        PlayerPrefs.SetInt("CARD_YEAR" + n, Cards[n].expireYear);
        PlayerPrefs.SetInt("CVV" + n, Cards[n].cvv);
    }

    public void ClickCurrency(string currency)
    {
        it.Logger.Log($"ClickCurrency {currency}");
    }

    public void AmmountEdit(string txt)
    {
        int.TryParse(txt, out count);
    }

    public void PlusFieldAmmount(int add)
    {
        count += add;
        AmmountDepositField.text = count.ToString();
        AmmountWithdrawalField.text = count.ToString();
    }

    public void SelectPayCard(int n)
    {
        SelectCard = n;
        if (n != -1) NumberDepositField.text = Cards[SelectCard].cardNumber;
    }

    public void NumberEdit(string txt)
    {

    }

    void HistoryDateSet(DateTime dateFrom, DateTime dateTo)
    {
        it.Logger.Log($"SetHistory {dateFrom.ToShortDateString()} - {dateTo.ToShortDateString()}");
    }

    public void SetHistoryDateFrom(string txt)
    {
        if (DateTime.TryParse(txt, out DateTime date))
        {
            DataFromField.text = DateTime.Parse(txt).ToShortDateString();
            it.Logger.Log("DateFrom " + date.ToShortDateString());
        }
        else if (DateTime.TryParse(txt + "/01", out date))
        {
            DataFromField.text = date.ToShortDateString();
            it.Logger.Log("DateFrom " + date.ToShortDateString());
        }
    }

    public void SetHistoryDateTo(string txt)
    {
        if (DateTime.TryParse(txt, out DateTime date))
        {
            DataToField.text = DateTime.Parse(txt).ToShortDateString();
            it.Logger.Log("DateTo " + date.ToShortDateString());
        }
        else if (DateTime.TryParse(txt + "/01", out date))
        {
            DataToField.text = date.ToShortDateString();
            it.Logger.Log("DateTo " + date.ToShortDateString());
        }
    }

    public void SortHistory(bool usd)
    {

    }

    public void SelectWindow(int n)
    {
        WindowNow = n;
    }
    public void SelectWindowOpen (int n)
    {
        buttonsWindow[n - 1].onClick.Invoke();
    }

    public void PaymentImage(Image image)
    {
        metodPaymentImage.sprite = image.sprite;
    }
}



