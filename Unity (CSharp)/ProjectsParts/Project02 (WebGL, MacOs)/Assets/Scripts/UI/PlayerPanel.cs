using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.UI;

namespace it.Main
{
	public class PlayerPanel : MonoBehaviour
	{
		[SerializeField] private it.UI.Avatar _avatar;
		[SerializeField] private TextMeshProUGUI _nickname;
		[SerializeField] private it.Inputs.CurrencyLabel _walletValueLabel;
		[SerializeField] private TextMeshProUGUI _ticketValueLabel;

		private void Awake()
		{
			_avatar.SetDefaultAvatar();

			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, (handle) =>
			{
				Init();
			});
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserBalanceUpdate, (handle) =>
			{
				Init();
			});

		}

		public void Init()
		{
			_nickname.text = UserController.User.nickname;
			_walletValueLabel.SetValue("{0}", (decimal)UserController.User.user_wallet.amount);
			_avatar.SetAvatar(UserController.User.AvatarUrl);
			//_walletValueLabel.text = UserController.User.UserWallet.Amount.ToString();
		}

		public void AvatarButton()
		{
			UiManager.GetPanel<MainPanel>().UserProfileButton();
		}

		public void CasherButton()
		{
			PopupController.Instance.ShowPopup(PopupType.Cashier);
		}
		public void BonusButton()
		{
			//PopupController.Instance.ShowPopup(PopupType.WelcomeBonus);
			SinglePageController.Instance.Show(SinglePagesType.WelcomeBonus);
		}
		public void EventsButton()
		{
			//PopupController.Instance.ShowPopup(PopupType.WelcomeBonus);
			SinglePageController.Instance.Show(SinglePagesType.Events);
		}
		public void LeaderboardButton()
		{
			//PopupController.Instance.ShowPopup(PopupType.Leaderboard);
			SinglePageController.Instance.Show(SinglePagesType.Leaderboard);
		}
	}
}