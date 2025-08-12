using Engine.Scripts.Base;
using Game.Scripts.Controllers;
using Game.Scripts.Providers.Avatars;
using Game.Scripts.Providers.Profiles;
using Game.Scripts.Providers.Profiles.Common;
using Game.Scripts.Providers.Profiles.Handlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class ProfileShortPanel : MonoBehaviour, IInjection
	{
		[SerializeField] private Image _avatar;
		[SerializeField] private TMP_Text _nickname;
		[SerializeField] private TMP_Text _levelLabel;
		[SerializeField] private Button _debToolButton;

		private IProfileProvider _profileProvider;
		private IAvatarsProvider _avatarProvider;
		private IUiNavigator _uiNavigator;
		private IProfileLevelHandler _profileLevelHandler;
		private IApplicationSettingsController _applicationSettingsController;

		[Inject]
		public void Constructor(
			IProfileProvider profileProvider,
			IAvatarsProvider avatarPrrovider,
			IUiNavigator uiNavigator,
			IProfileLevelHandler profileLevelHandler,
			IApplicationSettingsController applicationSettingsController
		)
		{
			_profileProvider = profileProvider;
			_avatarProvider = avatarPrrovider;
			_uiNavigator = uiNavigator;
			_profileLevelHandler = profileLevelHandler;
			_applicationSettingsController = applicationSettingsController;

			SetProfile(_profileProvider.Profile);

			_profileLevelHandler.OnLevelChangeEvent.AddListener(OnLevelChangeEventListener);

			_debToolButton.onClick.RemoveAllListeners();
			if (_applicationSettingsController.ApplicationSettings.DevMode)
			{
				_debToolButton.gameObject.SetActive(true);
				_debToolButton.onClick.AddListener(DevToolButtonTouch);
			}
			else
			{
				_debToolButton.gameObject.SetActive(false);
			}
		}

		private void OnLevelChangeEventListener(int level)
		{
			SetLevel(level);
		}

		private void DevToolButtonTouch()
		{
			var develoopPanel = _uiNavigator.GetController<Controllers.DevelopPresenterController>();
			_ = develoopPanel.Open();
		}

		private void OnEnable()
		{
			_profileProvider.OnProfileChange.AddListener(ProfileChangeEvent);
		}

		private void OnDisable()
		{
			_profileProvider.OnProfileChange.AddListener(ProfileChangeEvent);
		}

		private void ProfileChangeEvent(IProfile profile)
		{
			SetProfile(profile);
		}

		public void SetProfile(IProfile profile)
		{
			_avatar.sprite = _avatarProvider.GetAvatar(profile.AvatarUuid);
			_nickname.text = profile.UserName;
			SetLevel(profile.Level);
		}

		private void SetLevel(int level)
		{
			_levelLabel.text = level.ToString();
		}
	}
}
