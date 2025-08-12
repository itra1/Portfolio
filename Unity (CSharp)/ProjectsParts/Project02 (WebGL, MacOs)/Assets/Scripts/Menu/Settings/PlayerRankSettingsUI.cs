using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRankSettingsUI : MonoBehaviour
{
    [SerializeField] private RankBuyElementUI[] buyElementUIs;

    public void Show()
    {
        //Rank[] ranks = GameController.Reference.Ranks;
        //for (int i = 0; i < ranks.Length && i < buyElementUIs.Length; i++)
        //{
        //    buyElementUIs[i].Show(ranks[i], this);
        //}
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Buy(Rank rank)
    {
        it.Logger.Log($"Buy rank {rank}");
    }
}
