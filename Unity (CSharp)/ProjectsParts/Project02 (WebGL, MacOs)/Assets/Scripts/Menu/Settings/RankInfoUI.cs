using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 

public class RankInfoUI : MonoBehaviour
{
    [SerializeField] private string[] names;
    [SerializeField] private Sprite[] sprites;

    [Space]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI cashback;
    [SerializeField] private TextMeshProUGUI climb;    
    [SerializeField] private TextMeshProUGUI period;
    [SerializeField] private TextMeshProUGUI timebank;

    public void Show(Rank rank)
    {
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == rank.name)
            {
                image.sprite = sprites[i];
                break;
            }
        }

        cashback.text = rank.cashback + "%";
        climb.text = $"<b>{rank.ClimbSp} SPs</b> {"TO_CLIMB".Localized()}\n" +
            $"<b>{rank.MaintainSp} SPs</b> {"TO_MAINTAIN".Localized()}";
        period.text = $"{rank.period} Days";
        timebank.text = $"{"TIME_BANK".Localized()} - <b>{rank.timebank} min</b>\n" +
            $"{"ADD_MORE".Localized()} {rank.timebank} min - <b>$ {rank.timebank_cost_usd}</b>";
    }
}
