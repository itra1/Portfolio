using System;
using System.Collections.Generic;
using it.Main.SinglePages;
using it.Network.Rest;
using it.UI.Elements;
using UnityEngine;
using UnityEngine.UI;

public class JackpotWinner : MonoBehaviour
{
	[SerializeField] private ScrollRect _scrollRect;
	[SerializeField] private GameObject parent;
	[SerializeField] private JackpotWinnersItem item;
	[SerializeField] private GameObject noData;

	private List<JackpotWinnersItem> records = new List<JackpotWinnersItem>();
	private PoolList<JackpotWinnersItem> _pooler;

	private void Awake()
	{
		item.gameObject.SetActive(false);
	}

	public void SetData(JackpotInfoResponse jackpotResponse)
	{
		if (jackpotResponse != null && jackpotResponse.winners.Length > 0)
		{
			noData.SetActive(false);

			if (_pooler == null)
				_pooler = new PoolList<JackpotWinnersItem>(item, parent.transform);
			_pooler.HideAll();

			var vlg = _scrollRect.content.GetComponent<VerticalLayoutGroup>();

			float separate = 0;
			float upPadding = 0, downSPadding = 0;
			if (vlg != null)
			{
				separate = vlg.spacing;
				upPadding = vlg.padding.top;
				downSPadding = vlg.padding.bottom;
			}

			for (int index = 0; index < jackpotResponse.winners.Length; index++)
			{
				//AddItem(jackpotResponse, index);
				var winner = _pooler.GetItem();
				winner.UseData(jackpotResponse, index);
				records.Add(winner);
			}
			RectTransform iRect = item.gameObject.GetComponent<RectTransform>();
			_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, upPadding + downSPadding + jackpotResponse.winners.Length * (iRect.rect.height + separate));
		}
		else
		{
			noData.SetActive(true);
		}
	}

}