using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Network.Rest;

[CreateAssetMenu(fileName = "CardLibrary", menuName = "Tools/Card Library", order = 1)]
public class CardLibrary : ScriptableObject
{

//#if UNITY_WEBGL
//	public static CardLibrary Instance => Garilla.WebGL.WebGLResources.CardLibrary;
//#else
	private static CardLibrary _instance;
	public static CardLibrary Instance
	{
		get
		{
			if (_instance == null)
			{
				//_instance = Resources.Load<CardLibrary>("CardLibrary");
				_instance = (CardLibrary)Garilla.ResourceManager.GetResource<CardLibrary>("CardLibrary");
				CardLibrary.SetPackName(PackFront, PackBack);
			}

			return _instance;
		}
	}
//#endif

	public List<Sprite> Spades;
	public List<Sprite> Hearts;
	public List<Sprite> Diamonds;
	public List<Sprite> Clubs;
	public List<Sprite> Backs;

	public CardsFrontPack MiniStruct;
	public CardsFrontPack MicroStruct;

	[Space, Header("Packs")]
	[SerializeField] public List<CardsFrontPack> CardsPacks;
	[SerializeField] public List<CardsBackPack> BackPacks;
	private static CardsFrontType PackFront;
	private static CardsBackType PackBack;

	public Sprite GetCard(DistributionCard card)
	{
		if (card != null && card.card != null && card.card.card_type.card_value != null) return GetCard(card.card.card_type);

		//closed card
		return GetCardBack();
	}

	public Sprite GetCardBack()
	{
		return Backs[0];
	}

	public Sprite GetCardMini(DistributionCard card)
	{
		var numberCard = 0;
		if (card.card.card_type.card_value.honneur == null || card.card.card_type.card_value.honneur.Length == 0)
		{
			var num = card.card.card_type.card_value.number;
			if (num == null) it.Logger.Log("card.card_value.number == null");
			var number = num ?? 0;
			numberCard = number - 1;
		}
		else
		{
			switch (card.card.card_type.card_value.honneur)
			{
				case "jack":
					{
						numberCard = 10;
						break;
					}
				case "queen":
					{
						numberCard = 11;
						break;
					}
				case "king":
					{
						numberCard = 12;
						break;
					}
			}
		}

		return GetCardFromStruct(MiniStruct, numberCard, card.card.card_type.card_suit.system_id);
	}

	public Sprite GetCardMicro(CardType card)
	{
		var numberCard = 0;
		if (card.card_value.honneur == null || card.card_value.honneur.Length == 0)
		{
			var num = card.card_value.number;
			if (num == null) it.Logger.Log("card.card_value.number == null");
			var number = num ?? 0;
			numberCard = number - 1;
		}
		else
		{
			switch (card.card_value.honneur)
			{
				case "jack":
					{
						numberCard = 10;
						break;
					}
				case "queen":
					{
						numberCard = 11;
						break;
					}
				case "king":
					{
						numberCard = 12;
						break;
					}
			}
		}

		return GetCardFromStruct(MicroStruct, numberCard, card.card_suit.system_id);
	}

	public Sprite GetCard(CardType card)
	{
		var numberCard = 0;
		if (card.card_value.honneur == null || card.card_value.honneur.Length == 0)
		{
			var num = card.card_value.number;
			if (num == null) it.Logger.Log("card.card_value.number == null");
			var number = num ?? 0;
			numberCard = number - 1;
		}
		else
		{
			switch (card.card_value.honneur)
			{
				case "jack":
					{
						numberCard = 10;
						break;
					}
				case "queen":
					{
						numberCard = 11;
						break;
					}
				case "king":
					{
						numberCard = 12;
						break;
					}
			}
		}

		return GetCard(numberCard, card.card_suit.system_id);
	}

	public Sprite GetCardFromStruct(CardsFrontPack pack, int numberCard, string suit)
	{
		if (numberCard < 0 || numberCard >= Spades.Count)
		{
			it.Logger.Log("Номер карты не существует: " + numberCard);
			return GetCardBack();
		}

		switch (suit)
		{
			case "spades":
				{
					return pack.cardsSpades[numberCard];
				}
			case "hearts":
				{
					return pack.cardsHearts[numberCard];
				}
			case "diamonds":
				{
					return pack.cardsDiamonds[numberCard];
				}
			case "cross":
				{
					return pack.cardsClubs[numberCard];
				}
			default:
				{
					it.Logger.Log("Не найден suit карты: " + suit);
					return GetCardBack();
				}
		}
	}

	public Sprite GetCard(int numberCard, string suit)
	{
		if (numberCard < 0 || numberCard >= Spades.Count)
		{
			it.Logger.Log("Номер карты не существует: " + numberCard);
			return GetCardBack();
		}

		switch (suit)
		{
			case "spades":
				{
					return Spades[numberCard];
				}
			case "hearts":
				{
					return Hearts[numberCard];
				}
			case "diamonds":
				{
					return Diamonds[numberCard];
				}
			case "cross":
				{
					return Clubs[numberCard];
				}
			default:
				{
					it.Logger.Log("Не найден suit карты: " + suit);
					return GetCardBack();
				}
		}
	}



	public static void SetPackName(CardsFrontType front, CardsBackType back)
	{
		PackFront = front;
		PackBack = back;

		if (Instance != null)
		{
			for (int i = 0; i < Instance.CardsPacks.Count; i++)
			{
				if (Instance.CardsPacks[i].type == front)
				{
					Instance.SetFrontPack(i);
					break;
				}
			}
			for (int i = 0; i < Instance.BackPacks.Count; i++)
			{
				if (Instance.BackPacks[i].type == back)
				{
					Instance.SetBackPack(i);
					break;
				}
			}
		}
	}

	private void SetFrontPack(int n)
	{
		Spades = CardsPacks[n].cardsSpades;
		Hearts = CardsPacks[n].cardsHearts;
		Diamonds = CardsPacks[n].cardsDiamonds;
		Clubs = CardsPacks[n].cardsClubs;
	}

	private void SetBackPack(int n)
	{
		Backs = BackPacks[n].cardBacks;
	}

	public enum CardsFrontType { Default, ColorIcon, ColorCards, Mini, Micro }
	[System.Serializable]
	public struct CardsFrontPack
	{
		public CardsFrontType type;
		public List<Sprite> cardsSpades;
		public List<Sprite> cardsHearts;
		public List<Sprite> cardsDiamonds;
		public List<Sprite> cardsClubs;
		public Sprite icon;
	}

	public enum CardsBackType { Default, two, three, four };
	[System.Serializable]
	public struct CardsBackPack
	{
		public CardsBackType type;
		public List<Sprite> cardBacks;
		public Sprite icon;
	}

}
