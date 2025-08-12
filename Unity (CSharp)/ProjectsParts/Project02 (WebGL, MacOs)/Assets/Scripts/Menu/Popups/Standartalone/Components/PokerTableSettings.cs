using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
[CreateAssetMenu(fileName = "TableGameSettings", menuName = "")]
public class PokerTableSettings : ScriptableObject
{
	[Header("Тип игры")]
	public PokerTableType tableType;
	[Header("Наименование blind")]
	public BlindNameType blindNameType;
	[Header("Время на ход")]
	public int[] timesSteps = new int[3] { 15, 20, 30 };

	[Header("Минимальное количество игроков")]
	[SerializeField]
	public int MinPlayers = 2;
	[Header("Максимальное количество игроков")]
	[SerializeField]
	public int MaxPlayers = 6;
	[Header("Выбранное число игроков")]
	public int currentPlayers;
	[Header("Выбранное время хода")]
	public int currentActionTime;
	[Header("Blinds")]
	[SerializeField]
	public List<Blind> blinds;
	[Header("Идентификатор игры")]
	public string id;
}
public enum BlindStakes
{
	Micro,
	Average,
	Hight
}
public enum PokerTableType
{
	NLH,
	PLO4,
	PLO5,
	PLO4HiLo,
	PLO5HiLo,
	OFC,
	ShortDeck,
	Montana,
	DealerChois,
	Memphis,
	PLO6,
	PLO7
}
public enum BlindNameType
{
	Blinds,
	Ante
}

