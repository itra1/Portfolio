using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UserWallentItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Amount;
    [SerializeField] private TextMeshProUGUI WalletType;
    [SerializeField] private TextMeshProUGUI Description;
    [SerializeField] private TextMeshProUGUI DateAndTime;

    public void Show (UserWalletTransaction userWallet)
    {
        if(Amount != null) Amount.text = "$" + userWallet.amount.ToString();
        //if (WalletType != null) WalletType.text = userWallet.WalletTransactionableType;
        if (Description != null) Description.text = userWallet.system_comment;
        if (DateAndTime != null && DateTime.TryParse(userWallet.created_at, out DateTime dateTime)) DateAndTime.text = dateTime.ToString();
    }
}
