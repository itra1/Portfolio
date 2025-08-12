using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using System.Globalization;


namespace it.Game.Panels
{
	public class LeaderboardPanel : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _titleLavel;
		[SerializeField] private TextMeshProUGUI _valueLabel;

		string sprKey = "AoN";
		public void SetTable(it.Network.Rest.Table table)
		{

			var elem = it.Settings.GameSettings.GameNames.Find(x => x.GameType == (GameType)table.game_rule_id);

			if (!UserController.ReferenceData.create_vip_table_options.ContainsKey(elem.Slug))
				elem = it.Settings.GameSettings.GameNames.Find(x => x.GameType == GameType.Holdem);

			if (elem != null)
			{
				var opt = UserController.ReferenceData.create_vip_table_options[elem.Slug];
				foreach (var key in opt.levels.Keys)
				{
					//if (opt.levels[key].min_buy_in == table.BuyInMin && opt.levels[key].max_buy_in == table.BuyInMax)
					//	sprKey = opt.levels[key].name;
					if (opt.levels[key].small_blind <= table.SmallBlindSize /*&& opt.levels[key].max_buy_in == table.BuyInMax*/)
						sprKey = opt.levels[key].name;
				}
			}

			RectTransform titleRect = _titleLavel.GetComponent<RectTransform>();
			_titleLavel.text = sprKey.ToUpper();
			titleRect.sizeDelta = new Vector2(_titleLavel.preferredWidth, titleRect.sizeDelta.y);
			if (sprKey == "Micro")
			{
				_titleLavel.text = "singlePage.leaderBoard.navigations.micro".Localized().ToUpper();
				_valueLabel.text = $"{StringConstants.CURRENCY_SYMBOL} {it.Helpers.Currency.String(210, false)}";
			}
			if (sprKey == "Average")
			{
				_titleLavel.text = "singlePage.leaderBoard.navigations.average".Localized().ToUpper();
				_valueLabel.text = $"{StringConstants.CURRENCY_SYMBOL} {it.Helpers.Currency.String(350, false)}";
			}
			if (sprKey == "High")
			{
				_titleLavel.text = "singlePage.leaderBoard.navigations.high".Localized().ToUpper();
				_valueLabel.text = $"{StringConstants.CURRENCY_SYMBOL} {it.Helpers.Currency.String(1750, false)}";
			}

		}

		public void ClickButtonTouch()
		{
			if (sprKey == "Micro")
				PlayerPrefs.SetString(StringConstants.BUTTON_LEADERBOARD_MICRO, "");
			if (sprKey == "Average")
				PlayerPrefs.SetString(StringConstants.BUTTON_LEADERBOARD_AVERAGE, "");
			if (sprKey == "High")
				PlayerPrefs.SetString(StringConstants.BUTTON_LEADERBOARD_HIGH, "");
#if UNITY_STANDALONE
			StandaloneController.Instance.FocusMain();
#endif
		}

	}
}