using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankHistorySettingsUI : MonoBehaviour
{
    [SerializeField] private RankHistoryItem Item;
    [SerializeField] private Transform Content;

    public void Show()
    {
        RankRecord[] rankRecords = GameHelper.UserProfile.rank_records;

        for (int i = 0; i < rankRecords.Length; i++)
		{
			var item = Instantiate(Item, Content);
            item.Show(rankRecords[i]);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
