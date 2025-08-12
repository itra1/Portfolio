using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceSettingsUI : MonoBehaviour
{
    [SerializeField] private Transform Content;
    [SerializeField] private UserWallentItemUI UserWallentPref;

    [Space]
    private IndexRequestMetaData HistoryMetaData = new IndexRequestMetaData();
    private List<UserWallentItemUI> WallentItemUIs = new List<UserWallentItemUI>();

    public void Show()
    {

    }

    public void GetHistoryWallent()
    {
        WallentItemUIs.ForEach(x => { Destroy(x.gameObject); });
        WallentItemUIs.Clear();
        GameHelper.GetWalletTransactions(HistoryMetaData, HistoryListCreate);
    }

    void HistoryListCreate(UserWalletTransactionRespone transactionRespone)
    {
        List<UserWalletTransaction> userWallets = transactionRespone.data;
        for (int i = 0; i < userWallets.Count; i++)
		{
			UserWallentItemUI itemUI = Instantiate(UserWallentPref, Content);
            itemUI.Show(userWallets[i]);
            WallentItemUIs.Add(itemUI);
        }
    }
}
