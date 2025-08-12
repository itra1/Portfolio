using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace it.UI
{
	public class SettingsPage_old : MainContentPage
	{
		[SerializeField] private GameObject _developPage;

		//private List<SettingsBodyPage> _pages = new List<SettingsBodyPage>();


		private void OnEnable()
		{
			//MyAvatarButton();
		}

		//public void SetPage(SettingsPageType pagetype)
		//{
		//	GetComponentInChildren<SettingsLeftPanel>().SetPage(pagetype);

		//	if (_pages.Count == 0)
		//		_pages = GetComponentsInChildren<SettingsBodyPage>(true).ToList();

		//	bool existsPage = false;
		//	for (int i = 0; i < _pages.Count; i++)

		//	{
		//		if (_pages[i].Page == pagetype)
		//		{
		//			existsPage = true;
		//			_pages[i].gameObject.SetActive(true);
		//		}
		//		else
		//			_pages[i].gameObject.SetActive(false);
		//	}

		//	_developPage.SetActive(!existsPage);

		//}


		//public void MyAvatarButton()
		//{
		//	SetPage(SettingsPageType.MyAvatar);
		//}

		//public void MyWalletButton()
		//{
		//	SetPage(SettingsPageType.MyWallet);
		//}

		//public void BalanceHistoryButton()
		//{
		//	SetPage(SettingsPageType.BalanceHistory);
		//}

		//public void VerifyIdButton()
		//{
		//	SetPage(SettingsPageType.VerifyId);
		//}

		//public void PokerStatusticButton()
		//{
		//	SetPage(SettingsPageType.PokerStatistics);
		//}

		//public void MyRankButton()
		//{
		//	SetPage(SettingsPageType.MyRank);
		//}

		//public void PlayerRanksButton()
		//{
		//	SetPage(SettingsPageType.PlayerRanks);
		//}

		//public void MyRanksHistoryButton()
		//{
		//	SetPage(SettingsPageType.MyRanksHistory);
		//}

		//public void BettingButton()
		//{
		//	SetPage(SettingsPageType.Betting);
		//}

		//public void LanguageButton()
		//{
		//	SetPage(SettingsPageType.Language);
		//}

		//public void SoundButton()
		//{
		//	SetPage(SettingsPageType.Sound);
		//}

		//public void SwitchChipDisplayButton()
		//{
		//	SetPage(SettingsPageType.SwitchChipDisplay);
		//}

		//public void TableThemeButton()
		//{
		//	SetPage(SettingsPageType.TableTheme);
		//}

		//public void TimeBankButton()
		//{
		//	SetPage(SettingsPageType.TimeBank);
		//}

	}
}