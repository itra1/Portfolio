using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
[CreateAssetMenu(fileName = "TableGameSettings", menuName = "")]
public class PokerTableSettings : ScriptableObject
{
	[Header("��� ����")]
	public PokerTableType tableType;
	[Header("������������ blind")]
	public BlindNameType blindNameType;
	[Header("����� �� ���")]
	public int[] timesSteps = new int[3] { 15, 20, 30 };

	[Header("����������� ���������� �������")]
	[SerializeField]
	public int MinPlayers = 2;
	[Header("������������ ���������� �������")]
	[SerializeField]
	public int MaxPlayers = 6;
	[Header("��������� ����� �������")]
	public int currentPlayers;
	[Header("��������� ����� ����")]
	public int currentActionTime;
	[Header("Blinds")]
	[SerializeField]
	public List<Blind> blinds;
	[Header("������������� ����")]
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

