using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using it.Network.Rest;
using it.UI;
using Garilla.Games;

public class ChinaGameUIPlayer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[NonSerialized] public TablePlayerSession TablePlayerSession;

	[SerializeField] private TextMeshProUGUI StateText;

	[Header("Containers")]
	[SerializeField] private GameObject FantasyCardsContainer;
	[SerializeField] private GameObject FantasyButtonsContainer;
	[SerializeField] private GameObject FantasyIcon;
	[SerializeField] private GameObject StateContainer;

	[Space]
	[SerializeField] private PlayerGameIcone Avatar;

	[Space]
	[SerializeField] private GameObject DistributionCardsContainer;
	[SerializeField] private GameObject GridCardsContainer;
	[SerializeField] private GameCardUI GameCardUIPrefab;
	[SerializeField] private PlayerInfoPanelUI PlayerInfoPanelUI;

	[Header("Card Places Collections")]
	[SerializeField] private List<RectTransform> DistributionCardPlaces;
	[SerializeField] private List<RectTransform> GridCardPlaces;
	[SerializeField] private List<RectTransform> FantasyCards;

	private List<GameCardUI> cards = new List<GameCardUI>();
	private ChinaDistributionSharedDataPlayer dataPlayer;
	private UserLimited user;
	private GameCardUI currentFocusObjecs;
	private Transform currentDragObjecs;
	private bool isMeActive = false;
	private bool isActiveState = false;
	private bool isPreFlop = false;
	private bool isFantasy = false;

	[ContextMenu("Init")]
	public void InitFantasyCards()
	{
		GridCardPlaces = new List<RectTransform>();
		DistributionCardPlaces = new List<RectTransform>();
		FantasyCards = new List<RectTransform>();

		if (FantasyCardsContainer != null) foreach (RectTransform t in FantasyCardsContainer.transform) FantasyCards.Add(t);
		foreach (RectTransform t in GridCardsContainer.transform) GridCardPlaces.Add(t);
		foreach (RectTransform t in DistributionCardsContainer.transform) DistributionCardPlaces.Add(t);
	}

	public void Init(TablePlayerSession tablePlayerSession, bool isShowDown = false)
	{
		this.TablePlayerSession = tablePlayerSession;
		Init(tablePlayerSession.user, tablePlayerSession.amount, tablePlayerSession.amountBuffer, isShowDown);
	}

	public void Init(ChinaDistributionSharedDataPlayer dataPlayer, DistributionEvent active_event,
			List<DistributionCard> distributionCards, bool isPreFlop, bool isShowDown)
	{
		ClearState();
		this.isPreFlop = isPreFlop;
		this.dataPlayer = dataPlayer;
		this.isFantasy = dataPlayer.IsFantasy;

		isMeActive = active_event != null;
		//isMeActive = active_event != null && active_event.user_id == dataPlayer.user.id;
		Init(dataPlayer.user, dataPlayer.amount, TablePlayerSession != null ? TablePlayerSession.amountBuffer : 0);
		InitCards(distributionCards, dataPlayer.combinations, isShowDown);
		if (isMeActive)
		{
			if (active_event.calltime_at != null && DateTime.TryParse(active_event.calltime_at, out var endTime))
			{
				var diff = (endTime - GameHelper.NowTime).TotalSeconds;
				if (diff > 0) Avatar.StartTimer(endTime);
				else
				{
					it.Logger.Log("active_event.calltime_at older");
				}
			}
			else
			{
				it.Logger.Log("Date calltime error " + active_event.calltime_at);
			}
		}
		else
		{
			Avatar._timer.StopTimer();
		}

		InitState(dataPlayer.ActiveStageStateName);
		Avatar.ShowWin();
	}

	[ContextMenu("ShowTimer")]
	public void ShowTimer()
	{
		Avatar.StartTimer(GameHelper.NowTime.AddSeconds(15));
	}

	//private void StartTimer(DateTime dateTime)
	//{
	//	Avatar.Timer.StartTimer(dateTime);
	//}

	public void ClearState()
	{
		Avatar.ClearState();
		//Avatar.Timer.StopTimer();
		//StateContainer.SetActive(false);
		ClearDragAndDrop();
	}

	public void ClearStateWithoutWin()
	{
		Avatar._timer.StopTimer();
		ClearDragAndDrop();
	}

	//private void ShowWin()
	//{
	//	StateContainer.SetActive(dataPlayer.IsWin);
	//	StateText.text = "Win";
	//	if (dataPlayer.IsWin)
	//	{
	//		Avatar.Timer.StopTimer();
	//		DistributionCardsContainer.SetActive(false);
	//	}
	//}

	private void Init(UserLimited user, decimal amount, decimal amountBuffer, bool isShowDown = false)
	{
		this.user = user;
		PlayerInfoPanelUI.Init(user, amount, amountBuffer);

		Avatar.SetFlag(user.country.flag);

		Avatar.Avatar.SetDefaultAvatar();
		if (TablePlayerSession.user.AvatarUrl != null && user.AvatarUrl.Length > 0)
			Avatar.Avatar.SetAvatar(user.AvatarUrl);

		DistributionCardsContainer.SetActive(false);
		SetFantasyVisibility(false);
	}

	private void InitCards(List<DistributionCard> distributionCards, List<DistributionPlayerCombination> combinations,
			bool isShowDown)
	{
		SetFantasyVisibility(IsMe/* && isMeActive */&& isFantasy);

		var isShowCard = IsMe || isPreFlop;

		ClearDistributionCards();

		var indexDistributionCard = 0;
		var placesCard = IsMe && isFantasy ? FantasyCards : DistributionCardPlaces;
		DistributionCardsContainer.SetActive(!IsMe || !isFantasy);
		for (int i = 0; i < distributionCards.Count; i++)
		{
			if (distributionCards[i].IsFolded) continue;

			if (!distributionCards[i].IsFreeDistribution)//карты уже разложены по сетке
			{
				var positionInGrid = GetPositionByRowAndPlace(distributionCards[i].CpRow, distributionCards[i].CpPosition);
				var card = InitCard(distributionCards[i], combinations, GridCardPlaces[positionInGrid]);

				continue;
			}

			if (indexDistributionCard >= placesCard.Count)
			{
				it.Logger.Log("Количество карт превышает количество свободных слотов " + distributionCards[i]);
				continue;
			}

			placesCard[indexDistributionCard].gameObject.SetActive(true);

			var cardPanel = InitCard(distributionCards[i], combinations, placesCard[indexDistributionCard]);
			if (IsMe) cardPanel.SetBlockingState(isMeActive);
			if (!isShowCard) cardPanel.SetCloseState();
			cards.Add(cardPanel);

			indexDistributionCard++;
		}

		for (int j = indexDistributionCard; j < placesCard.Count; j++)
		{
			if (isMeActive && j < 4) continue;
			placesCard[j].gameObject.SetActive(false);
		}
	}

	private GameCardUI InitCard(DistributionCard distributionCard, List<DistributionPlayerCombination> combinations, RectTransform parent)
	{
		var cardPanel = Instantiate(GameCardUIPrefab, parent);
		cardPanel.Init(distributionCard, combinations,combinations, true, true);
		cardPanel.SetSize(parent);
		return cardPanel;
	}

	private void ClearDistributionCards()
	{
		foreach (var item in GridCardPlaces)
		{
			foreach (Transform tr in item.transform)
			{
				Destroy(tr.gameObject);
			}
		}
		foreach (var item in cards) Destroy(item.gameObject);
		cards = new List<GameCardUI>();
	}

	public void OpenCards()
	{

	}

	private void InitState(string state)
	{
		StateContainer.SetActive(true);

		Avatar.InitState(state);

		if(state == "fold")
		{
			DistributionCardsContainer.SetActive(false);
			GridCardsContainer.SetActive(false);
		}

	}

	private void SetFantasyVisibility(bool isVisible)
	{
		if (FantasyButtonsContainer != null)
		{
			FantasyCardsContainer.SetActive(isVisible);
			FantasyIcon.SetActive(isVisible);
			DistributionCardsContainer.SetActive(!isVisible);
#if UNITY_ANDROID || UNITY_WEBGL
			// GameInitManager.instance.EnableMobileChinaPanel(!isVisible);
#endif

		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{

		if (!IsMe || !isMeActive || currentDragObjecs != null || !isActiveState) return;

		var activeCards = dataPlayer.IsFantasy ? FantasyCards : DistributionCardPlaces;
		RectTransform itemActiveCard = null;
		RectTransform itemGridCard = null;

		if (IsCollectionContainsPoint(activeCards, eventData.position, true, out itemActiveCard) ||
				IsCollectionContainsPoint(GridCardPlaces, eventData.position, true, out itemGridCard))
		{

			var item = itemActiveCard != null ? itemActiveCard : itemGridCard;
			currentFocusObjecs = item.GetChild(0).GetComponent<GameCardUI>();
			if (!currentFocusObjecs.GetInfoCard.under_user_control) return;

			currentDragObjecs = Instantiate(currentFocusObjecs.transform, transform);
			currentDragObjecs.transform.position = eventData.position;

			currentFocusObjecs.gameObject.SetActive(false);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (currentDragObjecs == null) return;
		currentDragObjecs.transform.position = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (currentDragObjecs == null) return;

		if (IsCollectionContainsPoint(GridCardPlaces, eventData.position, false, out RectTransform item))
		{
			currentFocusObjecs.transform.SetParent(item);
			currentFocusObjecs.transform.localPosition = Vector3.zero;

			isActiveState = false;
			ChinaCardRequestBody body = new ChinaCardRequestBody(currentFocusObjecs.GetInfoCard.id, GetRowIndex(item.GetSiblingIndex()),
					GetPlaceIndex(item.GetSiblingIndex()));
			GameHelper.ChinaMoveCard(TablePlayerSession.table_id, body, card =>
			{
				isActiveState = true;
			}, s =>
			{
				isActiveState = true;
			});
		}

		ClearDragAndDrop();
	}

	private void ClearDragAndDrop()
	{
		if (currentFocusObjecs != null) currentFocusObjecs.gameObject.SetActive(true);
		if (currentDragObjecs != null)
		{
			DestroyImmediate(currentDragObjecs.gameObject);
			currentDragObjecs = null;
		}
	}

	private int GetPositionByRowAndPlace(int row, int place)
	{
		switch (row)
		{
			case 1: return 9 + place;
			case 2: return 4 + place;
			case 3: return -1 + place;
		}

		return 0;
	}

	private int GetRowIndex(int position)
	{
		if (position > 9) return 1;//верхний ряд
		if (position < 5) return 3;//нижний ряд
		return 2;//средний ряд
	}

	private int GetPlaceIndex(int position)
	{
		if (position > 9) return position - 9;//верхний ряд
		if (position < 5) return position + 1;//нижний ряд
		return position - 4;//средний ряд
	}

	private bool IsCollectionContainsPoint(List<RectTransform> collection, Vector2 point, bool isAddedCheck, out RectTransform obj)
	{
		foreach (var item in collection)
		{
			if (!isAddedCheck && item.childCount > 0) continue;
			if (isAddedCheck && item.childCount == 0) continue;

			if (RectTransformUtility.RectangleContainsScreenPoint(item, point))
			{
				obj = item;
				return true;
			}
		}

		obj = null;
		return false;
	}

	public bool IsMe => user.id == GameHelper.UserInfo.id;

	public bool IsFullGrid()
	{
		return GridCardPlaces.All(item => item.childCount != 0);
	}

	public void SetStateButtons(bool isActiveState)
	{
		this.isActiveState = isActiveState;
		if (!isActiveState)
		{
			ClearDragAndDrop();
		}
	}

	public int GetLostDistributionCardCount()
	{
		return cards.Count;
	}
}