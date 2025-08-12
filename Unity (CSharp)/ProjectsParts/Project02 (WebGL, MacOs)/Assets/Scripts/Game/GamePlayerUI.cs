using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using it.Network.Rest;
using Garilla.Games;

public class GamePlayerUI : MonoBehaviour
{
	[NonSerialized] public TablePlayerSession TablePlayerSession;

	[Header("Labels")]
	[SerializeField] private TextMeshProUGUI StateText;

	[Header("Containers")]
	[SerializeField] private GameObject StateContainer;

	[Space]
	[SerializeField] private Image Avatar;
	[SerializeField] private Image Country;
	[SerializeField] public Timer Timer;

	[Space]
	[SerializeField] private GameObject CardsContainer;
	[SerializeField] private GameObject ClosedCardsContainer;
	private List<Transform> CardPlaces;
	[SerializeField] private List<Transform> CardPlaces2Max;
	[SerializeField] private List<Transform> CardPlaces5Max;
	[SerializeField] private List<GameObject> CardClosedPlaces2Max;
	[SerializeField] private List<GameObject> CardClosedPlaces5Max;
	[SerializeField] private GameCardUI GameCardUIPrefab;
	[SerializeField] private PlayerInfoPanelUI PlayerInfoPanelUI;

	//[SerializeField] private GameObject WinStateImage;

	private List<GameCardUI> cards = new List<GameCardUI>();
	private DistributionSharedDataPlayer dataPlayer;
	[HideInInspector] public UserLimited user;
	private UserStat _userStat;

	public void Init(TablePlayerSession tablePlayerSession, bool isShowDown = false)
	{
		this.TablePlayerSession = tablePlayerSession;
		Init(tablePlayerSession.user, tablePlayerSession.amount, tablePlayerSession.amountBuffer, isShowDown);
	}

	public void Init(DistributionSharedDataPlayer dataPlayer, DistributionEvent active_event,
			DistributionCard[] distributionCards, bool isPreFlop, bool isShowDown, bool WinSee = true)
	{
		ClearState();
		this.dataPlayer = dataPlayer;
		Init(dataPlayer.user, dataPlayer.amount, TablePlayerSession != null ? TablePlayerSession.amountBuffer : 0);
		_userStat = dataPlayer.user_stat;
		InitCards(distributionCards, dataPlayer.combinations, isShowDown);
		if (active_event != null && active_event.user_id == dataPlayer.user.id)
		{
			if (active_event.calltime_at != null)
			{
				var endTime = DateTime.Parse(active_event.calltime_at);
				var diff = (endTime - GameHelper.NowTime).TotalSeconds;
				if (diff > 0) StartTimer(endTime);
			}
			else
			{
				it.Logger.Log("active_event.calltime_at == null");
			}
		}
		else
		{
			Timer.StopTimer();
		}

		InitState(dataPlayer.ActiveStageStateName);
		if (WinSee) ShowWin();
	}

	[ContextMenu("ShowTimer")]
	public void ShowTimer()
	{
		StartTimer(GameHelper.NowTime.AddSeconds(15));
	}

	public void StartTimer(DateTime dateTime)
	{
		Timer.StartTimer(dateTime);
	}
	public void ClearState()
	{
		Timer.StopTimer();
		StateContainer.SetActive(false);
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		//WinStateImage.SetActive(false);
#endif
	}

	public void ClearStateWithoutWin()
	{
		Timer.StopTimer();
	}

	private void ShowWin()
	{


#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS

		// WinStateImage.SetActive(true);
#endif
		StateContainer.SetActive(dataPlayer.IsWin);
		StateText.text = "<color=#E9B069>Win</color>";
		if (dataPlayer.IsWin)
		{
			Timer.StopTimer();
		}

	}

	void Init(UserLimited user, decimal amount, decimal amountBuffer, bool isShowDown = false)
	{
		this.user = user;
		PlayerInfoPanelUI.Init(user, amount, amountBuffer);

		var path = $"CountryFlags/{user.country.short_title}";
		var spCountry = (Sprite)Garilla.ResourceManager.GetResource<Sprite>(path);
		//var spCountry = Resources.Load<Sprite>(path);
		if (spCountry != null)
		{
			Country.sprite = spCountry;
		}

		Avatar.sprite = SpriteManager.instance.avatarDefault;
		if (TablePlayerSession != null && !string.IsNullOrEmpty(TablePlayerSession.user.AvatarUrl) && user.AvatarUrl.Length > 0) StartCoroutine(SetImage(user.AvatarUrl));

		CardsContainer.SetActive(isShowDown);
		ClosedCardsContainer.SetActive(false);
	}

	private void InitCards(DistributionCard[] distributionCards, List<DistributionPlayerCombination> combinations, bool isShowDown)
	{
		var isShowCard = user.id == GameHelper.UserInfo.id || isShowDown;
		CardsContainer.SetActive(isShowCard);
		ClosedCardsContainer.SetActive(!isShowCard);

		if (distributionCards.Length > 2) CardPlaces = CardPlaces5Max;
		else CardPlaces = CardPlaces2Max;
		for (int i = 0; i < CardClosedPlaces2Max.Count; i++)
		{
			CardClosedPlaces2Max[i].SetActive(distributionCards.Length <= 2 && distributionCards.Length > i);
		}
		for (int i = 0; i < CardClosedPlaces5Max.Count; i++)
		{
			CardClosedPlaces5Max[i].SetActive(distributionCards.Length > 2 && distributionCards.Length > i);
		}


		foreach (var item in cards) Destroy(item.gameObject);
		cards = new List<GameCardUI>();

		for (int i = 0; i < distributionCards.Length; i++)
		{
			if (distributionCards[i].IsFolded)
			{
				CardsContainer.SetActive(false);
				ClosedCardsContainer.SetActive(false);
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
				CardPlaces[i].gameObject.SetActive(false);
#endif
				break;
			}

			if (i >= CardPlaces.Count)
			{
				it.Logger.Log("Количество карт превышает количество свободных мест на столе");
				break;
			}

			var cardPanel = Instantiate(GameCardUIPrefab, CardPlaces[i]);
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			CardPlaces[i].gameObject.SetActive(true);
#endif
			cardPanel.Init(distributionCards[i], combinations, combinations, dataPlayer.IsWin);
			cards.Add(cardPanel);
		}
	}

	public void OpenCards()
	{
		CardsContainer.SetActive(true);
		ClosedCardsContainer.SetActive(false);
	}

	private void InitState(string state)
	{
		StateContainer.SetActive(true);
		switch (state)
		{
			case "fold":
				CardsContainer.SetActive(false);
				ClosedCardsContainer.SetActive(false);
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
				for (int i = 0; i <CardPlaces.Count; i++)
                CardPlaces[i].gameObject.SetActive(false);
#endif
				StateText.text = "Fold";
				break;
			case "small-blind":
				StateText.text = "Small-blind";
				break;
			case "big-blind":
				StateText.text = "Big-blind";
				break;
			case "all-in":
				StateText.text = "All-in";
				break;
			case "call":
				StateText.text = "Call";
				break;
			case "check":
				StateText.text = "Check";
				break;
			case "raise":
				StateText.text = "Raise";
				break;
			case "bet":
				StateText.text = "Bet";
				break;
			case "re-raise":
				StateText.text = "Re-raise";
				break;
			case "cap":
				StateText.text = "Cap";
				break;
			case "need-call":
			case "active":
			case "last-active":
			default:
				StateContainer.SetActive(false);
				break;
		}
	}

	IEnumerator SetImage(string url)
	{
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
		yield return request.SendWebRequest();
		if (request.result != UnityWebRequest.Result.Success)
			it.Logger.Log(request.error);
		else
		{
			Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
			Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
			Avatar.sprite = sprite;
		}
	}

	public void ShowInfo()
	{
		//if (it.UI.GamePanel.Instance != null)
		//	it.UI.GamePanel.Instance.OpenUserInfo(user, _userStat, new UserNote());
	}

	public void Clear()
	{
		ClearState();
		CardsContainer.SetActive(false);
		ClosedCardsContainer.SetActive(false);
	}
}
