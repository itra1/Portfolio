using it.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTypeGameUI : MonoBehaviour
{
	[SerializeField] private string nameRule;
	[SerializeField] private MainPagesType PageType;
	[SerializeField] private List<GameType> typeGame = new List<GameType>();
	[SerializeField] private int maxPlayers = 9;
	[SerializeField] private bool isPrivate;
	[SerializeField] private bool allOrNothing;

	void Start()
	{

	}
	public void SelectGame()
	{

		//GetComponentInParent<MainPanel>().LobbyButton(GetNavigationData());

	}

	//public NavigationButtonData GetNavigationData()
	//{
	//	return new NavigationButtonData
	//	{
	//		Name = nameRule,
	//		PageType = PageType,
	//		//TypeGame = typeGame,
	//		MaxPlayers = maxPlayers,
	//		IsPrivate = isPrivate,
	//		AllOrNothing = allOrNothing
	//	};
	//}

	//public List<TypeGame> GetGameType()
	//{
	//	return typeGame;
	//}

	public void SetPrivacy(bool _isPrivate)
	{
		isPrivate = _isPrivate;
	}
}
