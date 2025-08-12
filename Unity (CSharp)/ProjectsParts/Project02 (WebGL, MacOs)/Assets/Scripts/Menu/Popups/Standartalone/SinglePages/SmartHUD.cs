using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Api;
using System.Linq;
using System;
using it.UI.Elements;
 
using Garilla;
using Garilla.Main;

namespace it.Main.SinglePages
{
	public class SmartHUD : SinglePage, TargetCaruselNavigationMenu
	{
		[SerializeField] private Image[] _lockImage;
		[SerializeField] private Image _icone;
		//[SerializeField] private TMP_Dropdown _gameDropdown;
		//[SerializeField] private TMP_Dropdown _cupDropdown;
		[SerializeField] private SmartHudCircleBar _vpipBar;
		[SerializeField] private SmartHudCircleBar _pfrbar;
		[SerializeField] private SmartHudCircleBar _3betBar;
		[SerializeField] private TextMeshProUGUI _handsLabel;

		[SerializeField] private SmartHudCircleBar _cbBar;
		[SerializeField] private SmartHudCircleBar _fcbBar;
		[SerializeField] private SmartHudCircleBar _ccbBar;
		[SerializeField] private SmartHudCircleBar _rcbBar;
		[SerializeField] private SmartHudCircleBar _wtBar;
		[SerializeField] private SmartHudCircleBar _wsdBar;

		private List<Settings.GameSettings.GaneType> _settings = new List<Settings.GameSettings.GaneType>();

		//string[] cupOptions = new string[]{
		//		"None"
		//	,"singlePage.pokerStatistic.cup.RegularTournaments"
		//	,"singlePage.pokerStatistic.cup.TurboTournaments"
		//	,"singlePage.pokerStatistic.cup.ShortDeckTournaments"
		//	,"singlePage.pokerStatistic.cup.StelliteTournaments"
		//	,"singlePage.pokerStatistic.cup.SundayTournaments"
		//	,"singlePage.pokerStatistic.cup.GarillaPokerCUP"
		//};

		private void Awake()
		{
			//_cupDropdown.ClearOptions();

			//List<TMP_Dropdown.OptionData> optionsCup = new List<TMP_Dropdown.OptionData>();

			//for (int i = 0; i < cupOptions.Length; i++)
			//	optionsCup.Add(new TMP_Dropdown.OptionData(name = cupOptions[i].Localized()));

			//_cupDropdown.AddOptions(optionsCup);
			//_cupDropdown.value = 0;

			//_gameDropdown.ClearOptions();
			//List<TMP_Dropdown.OptionData> optionsGames = new List<TMP_Dropdown.OptionData>();

			//for (int i = 0; i < it.Settings.GameSettings.Games.Count; i++)
			//	if (it.Settings.GameSettings.Games[i].IsSmartHud)
			//	{
			//		_settings.Add(it.Settings.GameSettings.Games[i]);
			//		optionsGames.Add(new TMP_Dropdown.OptionData(name = it.Settings.GameSettings.Games[i].Name));
			//	}

			//_gameDropdown.AddOptions(optionsGames);
			//_gameDropdown.value = 0;

			//_gameDropdown.onValueChanged.RemoveAllListeners();
			//_gameDropdown.onValueChanged.AddListener((val) =>
			//{
			//	if (_icone != null)
			//		_icone.sprite = _settings[val].RectIcone;
			//	Request(_settings[val].SlugRequest);
			//});

		}

		protected override void EnableInit()
		{
			Request("holdem");
		}

		public void Request(string section)
		{
			Lock(true);
			it.Api.UserApi.GetStatistic(UserController.User.id, section, (result) =>
			{
				Lock(false);
				if (result.IsSuccess)
					PrintStat(result.Result);
			});
		}

		private void PrintStat(it.Network.Rest.UserStat stat)
		{
			_handsLabel.text = ((int)stat.distributions_count).ToString();
			_vpipBar.SetData((float)stat.vpip / 100f);
			_pfrbar.SetData((float)stat.pfr / 100f);
			_3betBar.SetData((float)stat.ats / 100f);

			_cbBar.SetData((float)stat.continuation_bet / 100f);
			_fcbBar.SetData((float)stat.fold_continuation_bet / 100f);
			_ccbBar.SetData((float)stat.call_continuation_bet / 100f);
			_rcbBar.SetData((float)stat.raise_continuation_bet / 100f);
			_wtBar.SetData((float)stat.showdown_participate / 100f);
			_wsdBar.SetData((float)stat.showdown_win / 100f);
		}

		private void Lock(bool isLock)
		{
			for (int i = 0; i < _lockImage.Length; i++)
				_lockImage[i].gameObject.SetActive(isLock);
		}

		public void WhatIsLinkTouch()
		{
			LinkManager.OpenUrl("whatSmartHud");
		}

		public void SelectFromCaruselMenu(string type)
		{
			Request(type);
		}
	}
}