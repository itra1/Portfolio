using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using it.Network.Rest;

public class PlayerInfoPanelUI : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Bank;
    public TextMeshProUGUI BankBuffer;

    public void Init(UserLimited user, decimal amount, decimal amountBuffer)
    {
        Name.text = user.nickname;
        Bank.text = "<color=#E9B069>$</color>" + amount.ToString(CultureInfo.InvariantCulture);
        BankBuffer.gameObject.SetActive(amountBuffer > 0);
        BankBuffer.text = amountBuffer.ToString(CultureInfo.InvariantCulture);
    }
}
