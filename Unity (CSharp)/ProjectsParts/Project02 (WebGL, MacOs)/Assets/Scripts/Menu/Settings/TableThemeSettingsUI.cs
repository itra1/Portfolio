using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TableThemeSettingsUI : SettingsPage
{
	[SerializeField] private TMP_Dropdown dropdownFront;
	[SerializeField] private TMP_Dropdown dropdownBack;

	private TableTheme tableTheme;

	public void Show()
	{
		tableTheme = GameHelper.UserProfile.table_theme;
		//tableTheme = new TableTheme(tableTheme.front_deck, tableTheme.back_deck, tableTheme.felt, tableTheme.background);
		tableTheme = new TableTheme() { back_deck = tableTheme.back_deck, front_deck = tableTheme.front_deck, background = tableTheme.background, felt = tableTheme.felt };

		dropdownFront.ClearOptions();
		List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
		for (int i = 0; i < CardLibrary.Instance.CardsPacks.Count; i++)
		{
			optionDatas.Add(new TMP_Dropdown.OptionData(CardLibrary.Instance.CardsPacks[i].type.ToString(), CardLibrary.Instance.CardsPacks[i].icon));
		}
		dropdownFront.AddOptions(optionDatas);

		dropdownBack.ClearOptions();
		optionDatas.Clear();
		for (int i = 0; i < CardLibrary.Instance.BackPacks.Count; i++)
		{
			optionDatas.Add(new TMP_Dropdown.OptionData(CardLibrary.Instance.BackPacks[i].type.ToString(), CardLibrary.Instance.BackPacks[i].icon));
		}
		dropdownBack.AddOptions(optionDatas);

		SetFront(int.Parse(tableTheme.front_deck) - 1);
		SetBack(int.Parse(tableTheme.back_deck) - 1);
		SetFelt("default");
		SetBackground("default");
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void SetFront(int n)
	{
		tableTheme.front_deck = (n + 1).ToString();
		dropdownFront.value = n;
	}

	public void SetBack(int n)
	{
		tableTheme.back_deck = (n + 1).ToString();
		dropdownBack.value = n;
	}

	public void SetFelt(string txt)
	{
		tableTheme.felt = txt;
	}

	public void SetBackground(string txt)
	{
		tableTheme.background = txt;
	}


	public override void Apply()
	{
		CardLibrary.SetPackName((CardLibrary.CardsFrontType)(int.Parse(tableTheme.front_deck) - 1), (CardLibrary.CardsBackType)(int.Parse(tableTheme.back_deck) - 1));

		userProfilePost.table_theme = tableTheme;
		base.Apply();
	}
}
