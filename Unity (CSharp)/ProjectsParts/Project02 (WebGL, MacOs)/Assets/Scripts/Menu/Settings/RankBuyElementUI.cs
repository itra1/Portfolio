using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RankBuyElementUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI TopText;
    [SerializeField] private TextMeshProUGUI DownText;
    private PlayerRankSettingsUI playerRankSettingsUI;
    private Rank rank;

    public void Show(Rank rank, PlayerRankSettingsUI playerRankSettingsUI)
    {
        this.playerRankSettingsUI = playerRankSettingsUI;
        this.rank = rank;
        Title.text = rank.name;
        TopText.text = $"30 qty = {rank.period}"; //TO DO
        DownText.text = $"+30 qty = {rank.timebank_cost_usd}"; //TO DO
    }

    public void Buy()
    {
        playerRankSettingsUI.Buy(rank);
    }
}
