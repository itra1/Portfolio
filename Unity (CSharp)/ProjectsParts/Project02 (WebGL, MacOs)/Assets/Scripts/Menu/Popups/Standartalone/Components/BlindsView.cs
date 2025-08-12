using it.UI.Elements;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Network.Rest;
using System.Collections.Generic;
using System.Linq;
 

public class BlindsView : MonoBehaviour, IPokerTableSettings
{

	[SerializeField]private SliderScrollHandler scrollHandler;
	[SerializeField] private float step;
	[SerializeField] private TextMeshProUGUI textBlinds;
	[SerializeField] private TextMeshProUGUI textMinMaxBlinds;
	[SerializeField] private TextMeshProUGUI textBlindStake;
	[SerializeField] private TextMeshProUGUI blindNameText;
	public int blindsCount;
	private PokerTableSettings currentSettings;
	private Blind currentBlind;
	private TableOptions _tableOptions;
	public string BlindStakeName => textBlindStake.text;
	public void SetSettings(PokerTableSettings settings)
	{
		if (settings != null)
		{
			scrollHandler.GetComponent<Slider>().value = 0;
			blindsCount = settings.blinds.Count;
			var slider = scrollHandler.GetComponent<Slider>();
			slider.maxValue = blindsCount - 1;
			slider.wholeNumbers = true;
			if (blindsCount > 0)
			{
				scrollHandler.Speed = 1;
				slider.onValueChanged.AddListener(GetValue);
			}

			currentSettings = settings;
			blindNameText.text = currentSettings.blindNameType.ToString() + ":";
			currentBlind = currentSettings.blinds[0];
			UpdateView(currentBlind);
		}

	}
	public void SetSettings(TableOptions tableOptions)
	{
		_tableOptions = tableOptions;
		if (tableOptions.levels.Count > 0)
		{
			currentSettings = new PokerTableSettings();
			currentSettings.blinds = UpdateCurrentBlinds(tableOptions.levels);
		}

		scrollHandler.GetComponent<Slider>().value = 0;
		blindsCount = currentSettings.blinds.Count;
		var slider = scrollHandler.GetComponent<Slider>();
		slider.maxValue = blindsCount - 1;
		slider.wholeNumbers = true;
		if (blindsCount > 0)
		{
			scrollHandler.Speed = 1;
			slider.onValueChanged.AddListener(GetValue);
		}
		//blindNameText.text = currentSettings.blindNameType.ToString() + ":";
		currentBlind = currentSettings.blinds[0];
		UpdateView(currentBlind);


	}
	private List<Blind> UpdateCurrentBlinds(Dictionary<string, Level> blindLevels)
	{
		List<Blind> res = new List<Blind>();

		foreach (KeyValuePair<string, Level> entry in blindLevels)
		{
			// do something with entry.Value or entry.Key
			var bl = new Blind();
			bl.type = Blind.BlindType.Multi;
			bl.minBuyIn = entry.Value.min_buy_in;
			bl.maxBuyIn = entry.Value.max_buy_in;
			bl.minValue = entry.Value.small_blind;
			bl.maxValue = entry.Value.big_blind;
			bl.LevelName = entry.Value.name;
			bl.level_id = entry.Key;
			res.Add(bl);
		}
		return res.OrderBy(x => x.minBuyIn).ToList();
	}
	public string GetStakeName()
	{
		return textBlindStake.text;
	}
	public string GetLevelID()
	{
		return currentBlind.level_id;
	}
	private void GetValue(float value)
	{
		currentBlind = currentSettings.blinds[(int)value];
		UpdateView(currentBlind);
	}

	private void UpdateView(Blind currentBlind)
	{
		RectTransform blindNameTextRect = blindNameText.GetComponent<RectTransform>();
		//textBlinds.text = currentBlind.Blinds();
		if (_tableOptions.id == "short-deck")
		{
			textBlinds.text = currentBlind.AnteBuyIin();
			blindNameText.text = "popup.createTable.ante".Localized();
		}
		else
		{
			textBlinds.text = currentBlind.Blinds();
			blindNameText.text = "popup.createTable.blinds".Localized();
		}
		blindNameTextRect.sizeDelta = new Vector2(blindNameText.preferredWidth, blindNameTextRect.sizeDelta.y);

		textMinMaxBlinds.text = currentBlind.MinMaxBuyIin();
		textBlindStake.text = currentBlind.StakeName();

	}
}
