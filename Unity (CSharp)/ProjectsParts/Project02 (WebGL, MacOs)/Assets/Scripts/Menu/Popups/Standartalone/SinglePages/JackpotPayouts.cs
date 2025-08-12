using System;
using System.Collections.Generic;
using it.Main.SinglePages;
using it.Network.Rest;
using it.UI.Elements;
using UnityEngine;
using UnityEngine.UI;

public class JackpotPayouts : MonoBehaviour
{
	[SerializeField] private ScrollRect _scrollRect;
	[SerializeField] private GameObject parent;
	[SerializeField] private PayoutItem item;

	List<PayoutItem> records = new List<PayoutItem>();
	private PoolList<PayoutItem> _itemsPooler;

	private void Awake()
	{
		item.gameObject.SetActive(false);
	}

	public void SetData(JackpotInfoResponse jackpotResponse)
	{
		if (jackpotResponse == null)
			return;
		//if (jackpotResponse != null && jackpotResponse.blinds.Length > 0)
		//{
		//	GenerateItemAlternately(jackpotResponse);
		//}

		if (_itemsPooler == null)
			_itemsPooler = new PoolList<PayoutItem>(item.gameObject, parent.transform);

		_itemsPooler.HideAll();
		for (int i = 0; i < jackpotResponse.blinds.Length; i++)
		{
			var winner = _itemsPooler.GetItem();
			winner.UseData(jackpotResponse.blinds[i], jackpotResponse.amount);
			winner.gameObject.SetActive(true);
			//records.Add(winner);
			RectTransform recordRect = item.GetComponent<RectTransform>();
			var vlg = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
			vlg.padding.top = 6;
			vlg.padding.bottom = 6;
			_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x,
					(recordRect.rect.height + 6) * (i+1));
		}

	}

	//private void GenerateItemAlternately(JackpotInfoResponse response)
	//{
	//	for (int i = 0; i < response.blinds.Length; i++)
	//	{
	//		SpawnItem(response.blinds[i], response.amount);
	//	}


	//	//for (int i = 0; i < 7; i++)
	//	//{
	//	//	SpawnItem(response, i, "Omaha");
	//	//	SpawnItem(response, i, "Holdem");
	//	//}
	//}

	//private void SpawnItem(JackpoBlinds record, decimal valueJackpot)
	//{
	//	var winner = Instantiate(item, parent.transform);
	//	winner.UseData(record, valueJackpot);
	//	winner.gameObject.SetActive(true);
	//	records.Add(winner);
	//	RectTransform recordRect = item.GetComponent<RectTransform>();
	//	var vlg = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
	//	vlg.padding.top = 6;
	//	vlg.padding.bottom = 6;
	//	_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x,
	//			(recordRect.rect.height + 6) * records.Count);
	//}

	//private void OnEnable()
	//{
	//	ClearRecords();
	//}

	//private void ClearRecords()
	//{
	//	for (int i = 0; i < records.Count; i++)
	//	{
	//		Destroy(records[i].gameObject);
	//	}

	//	records.Clear();
	//}
}