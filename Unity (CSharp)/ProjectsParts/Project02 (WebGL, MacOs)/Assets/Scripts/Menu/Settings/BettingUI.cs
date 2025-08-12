using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BettingUI : SettingsPage
{
	[SerializeField] private TMP_Dropdown[] OpeningDropdown;
	[SerializeField] private string[] OpeningsTxt;

	[Space]
	[SerializeField] private TMP_Dropdown[] NormalDropdown;
	List<string> NormalsTxt = new List<string>();

	[Space]
	[SerializeField] private Toggle ToggleRound;
	private Betting betting;

	public void Show()
	{
		betting = GameHelper.UserProfile.betting;
		//betting = new Betting(betting.button1, betting.button2, betting.button3, betting.button4, betting.dont_round_to_nearest_blind);
		betting = new Betting() { button1 = betting.button1, button2 = betting.button2, button3 = betting.button3, button4 = betting.button4, dont_round_to_nearest_blind = betting.dont_round_to_nearest_blind };

		for (int i = 0; i < OpeningDropdown.Length; i++)
		{
			OpeningDropdown[i].ClearOptions();
			List<string> vs = new List<string>();
			for (int a = 0; a < OpeningsTxt.Length; a++)
			{
				vs.Add(OpeningsTxt[a]);
			}
			OpeningDropdown[i].AddOptions(vs);
		}
		for (int i = 0; i < OpeningsTxt.Length; i++)
		{
			if (betting.button1.opening_bet_size == OpeningsTxt[i])
			{
				OpeningDropdown[0].value = i;
			}
			if (betting.button2.opening_bet_size == OpeningsTxt[i])
			{
				OpeningDropdown[1].value = i;
			}
			if (betting.button3.opening_bet_size == OpeningsTxt[i])
			{
				OpeningDropdown[2].value = i;
			}
			if (betting.button4.opening_bet_size == OpeningsTxt[i])
			{
				OpeningDropdown[3].value = i;
			}
		}

		for (int i = 1; i <= 100; i++)
		{
			NormalsTxt.Add(i.ToString() + "%");
		}
		for (int i = 0; i < NormalDropdown.Length; i++)
		{
			NormalDropdown[i].ClearOptions();
			NormalDropdown[i].AddOptions(NormalsTxt);
		}
		NormalDropdown[0].value = int.Parse(betting.button1.normal_bet_size.Replace("%", "")) - 1;
		NormalDropdown[1].value = int.Parse(betting.button2.normal_bet_size.Replace("%", "")) - 1;
		NormalDropdown[2].value = int.Parse(betting.button3.normal_bet_size.Replace("%", "")) - 1;
		NormalDropdown[3].value = int.Parse(betting.button4.normal_bet_size.Replace("%", "")) - 1;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void SetOpeningButton1(int n)
	{
		betting.button1.opening_bet_size = OpeningsTxt[n];
	}
	public void SetNormalButton1(int n)
	{
		betting.button1.normal_bet_size = NormalsTxt[n];
	}

	public void SetOpeningButton2(int n)
	{
		betting.button2.opening_bet_size = OpeningsTxt[n];
	}
	public void SetNormalButton2(int n)
	{
		betting.button2.normal_bet_size = NormalsTxt[n];
	}

	public void SetOpeningButton3(int n)
	{
		betting.button3.opening_bet_size = OpeningsTxt[n];
	}
	public void SetNormalButton3(int n)
	{
		betting.button3.normal_bet_size = NormalsTxt[n];
	}

	public void SetOpeningButton4(int n)
	{
		betting.button4.opening_bet_size = OpeningsTxt[n];
	}
	public void SetNormalButton4(int n)
	{
		betting.button4.normal_bet_size = NormalsTxt[n];
	}

	public override void Apply()
	{
		userProfilePost.betting = betting;
		base.Apply();
	}
}
