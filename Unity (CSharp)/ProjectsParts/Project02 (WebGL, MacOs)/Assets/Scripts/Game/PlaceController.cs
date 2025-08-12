using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class PlaceController : MonoBehaviour
{
	public int PlaceCount => PlayerPlaces.Count;
	public TablePlaceTypes TablePlace => _tablePlace;

	public List<Transform> PlayerPlacesTakeSit;
	public List<Transform> PlayerPlaces;
	public List<DropCards> CardDrop;

	public it.UI.Elements.Bets MoveBet;
	public List<it.UI.Elements.Bets> Bets;
	public List<Image> Dealers;

	[SerializeField] private TablePlaceTypes _tablePlace;

	[System.Serializable]
	public class DropCards
	{
		public List<DropCardsItem> Items;

		[System.Serializable]
		public class DropCardsItem
		{
			public int Count;
			public RectTransform Item;
		}

	}

	private void Awake()
	{
		PlayerPlaces[0].parent.SetAsLastSibling();
	}

	public void ClearBetAndDealer()
	{
		foreach (var item in Bets) item.gameObject.SetActive(false);
		foreach (var item in Dealers) item.gameObject.SetActive(false);
	}

}