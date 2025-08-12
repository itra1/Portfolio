using UnityEngine;
using TMPro;
using it.Network.Rest;
using it.UI.Elements;
using Garilla;

namespace it.Popups
{
	public class SmartHudPopup : PopupBase
	{
		[SerializeField] protected TextMeshProUGUI _nameLabel;
		[SerializeField] protected it.UI.Avatar _avatar;

		[SerializeField] private TextMeshProUGUI _titleLabel;
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

		protected UserLimited _user;

		protected virtual string _titleName => "popup.mySmartHud.title";

		protected override void EnableInit()
		{
			base.EnableInit();
		}

		public virtual void SetData(UserLimited user, UserStat stat)
		{
			_user = user;
			if (_avatar != null && user.user_profile != null)
			{
				_avatar.SetDefaultAvatar();
				if(!string.IsNullOrEmpty(user.user_profile.avatarUrl))
					_avatar.SetAvatar(user.user_profile.avatarUrl);
			}
			if (_nameLabel != null)
			{
				_nameLabel.text = user.nickname;
				RectTransform rtName = _nameLabel.GetComponent<RectTransform>();
				rtName.sizeDelta = new Vector2(_nameLabel.preferredWidth, rtName.sizeDelta.y);
			}
			//GameController.GetUser(user.Id, ShowInfo);
			//gameObject.SetActive(true);
			//Request("holdem");

			if (stat != null)
				PrintStat(stat);
			else
				Request("holdem");

		}

		protected override void Localize()
		{
			base.Localize();
			if (_titleLabel != null)
			{
				_titleLabel.text = I2.Loc.LocalizationManager.GetTermTranslation(_titleName);
				RectTransform tRect = _titleLabel.GetComponent<RectTransform>();
				tRect.sizeDelta = new Vector2(_titleLabel.preferredWidth, tRect.sizeDelta.y);
			}
		}


		public void Request(string section)
		{
			if (_user == null) return;
			//Lock(true);
			it.Api.UserApi.GetStatistic(_user.id, section, (result) =>
			{
				//Lock(false);
				if (result.IsSuccess)
					PrintStat(result.Result);
			});
		}

		private void PrintStat(it.Network.Rest.UserStat stat)
		{
			if (_handsLabel != null)
				_handsLabel.text = ((int)stat.distributions_count).ToString();
			if (_vpipBar != null)
				_vpipBar.SetData((float)stat.vpip / 100f);
			if (_pfrbar != null)
				_pfrbar.SetData((float)stat.pfr / 100f);
			if (_3betBar != null)
				_3betBar.SetData((float)stat.ats / 100f);

			if (_cbBar != null)
				_cbBar.SetData((float)stat.continuation_bet / 100f);
			if (_fcbBar != null)
				_fcbBar.SetData((float)stat.fold_continuation_bet / 100f);
			if (_ccbBar != null)
				_ccbBar.SetData((float)stat.call_continuation_bet / 100f);
			if (_rcbBar != null)
				_rcbBar.SetData((float)stat.raise_continuation_bet / 100f);
			if (_wtBar != null)
				_wtBar.SetData((float)stat.showdown_participate / 100f);
			if (_wsdBar != null)
				_wsdBar.SetData((float)stat.showdown_win / 100f);
		}

		public void WhatIsLinkTouch()
		{
			LinkManager.OpenUrl("whatSmartHud");
		}

	}
}