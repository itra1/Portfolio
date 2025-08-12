using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using it.Network.Rest;

public class TableListItem : MonoBehaviour
{
    [SerializeField] private Image imageSelect;
    [SerializeField] private TMP_Text tableName;
    [SerializeField] private TMP_Text bet;
    [SerializeField] private TMP_Text buyIn;
    [SerializeField] private TMP_Text waits;
    [SerializeField] private Image[] players;
    [SerializeField] private Sprite placeOn;
    [SerializeField] private Sprite placeOff;

    [SerializeField] private TMP_Text playersCountLabel;

    private int playerCount;

    private Action<Table> checkCallback;
    private Table table;

    private Vector2 BaseItemSizeDelta; //width/height of element w/o table panel

    private int currentPlayersCount;
    private int maxPlayersCount;

    public void SetTableInfo(Table table)
    {
        BaseItemSizeDelta = GetComponent<RectTransform>().sizeDelta;
        this.table = table;
        UpdateItem();
    }
    
    public void SetCheckCallback(Action<Table> checkCallback)
    {
        this.checkCallback = checkCallback;
    }

    private void UpdateItem()
    {
        tableName.text = table.name;
        bet.text = $"${(int)table.SmallBlindSize} / {table.big_blind_size}"; //отсутствует на сайте
        buyIn.text = $"${table.BuyInMinEURO} - {table.BuyInMaxEURO}";
        waits.text = "0";

        playerCount = table.current_players_count;
        if (playerCount > 8) playerCount = 8;
#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_IOS
		for (var i = 0; i < players.Length; i++)
        {
            players[i].sprite = i < playerCount ? placeOn : placeOff;
            
            players[i].gameObject.SetActive(i < table.MaxPlayers);
        }
#endif

#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS

		//playersCountLabel.text = playerCount + "/" + table.maxPlayers;
#endif
#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_IOS

		imageSelect.gameObject.SetActive(TablesUIManager.Instance.GetSelectTable() != null && table.id == TablesUIManager.Instance.GetSelectTable().id);
#endif
	}
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
	public void OpenTableRepresentation ()
    {
        //TablesUIManager.Inst.TableSetParent(gameObject.GetComponent<RectTransform>());
    }

#endif
	public void Check()
    {
        checkCallback?.Invoke(table);
#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_IOS
		imageSelect.gameObject.SetActive(true);
#endif
	}

	public void Uncheck()
    {
#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_IOS
		imageSelect.gameObject.SetActive(false);
#endif
	}
}
