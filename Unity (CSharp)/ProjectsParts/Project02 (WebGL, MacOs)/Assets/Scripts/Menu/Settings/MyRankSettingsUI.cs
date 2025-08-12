using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRankSettingsUI : MonoBehaviour
{
    [SerializeField] private RankInfoUI rankNow;
    [SerializeField] private RankInfoUI rankNext;

    public void Show()
    {
        //RankRecord[] rankRecords = GameController.UserProfile.RankRecords;
        //Rank[] allRanks = GameController.Reference.Ranks;
        //Rank rank = rankRecords[0].Rank;

        //if (rankNow) rankNow.Show(rank);
        //if (rankNext)
        //{
        //    rankNext.Show(rank);
        //    for (int i = 0; i < allRanks.Length - 1; i++)
        //    {
        //        if (allRanks[i].name == rank.name)
        //        {
        //            rankNext.Show(allRanks[i + 1]);
        //            break;
        //        }
        //    }
        //}
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
