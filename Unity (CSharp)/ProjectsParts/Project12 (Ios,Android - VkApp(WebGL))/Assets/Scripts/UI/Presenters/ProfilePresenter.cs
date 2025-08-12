using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Avatars.Base;
using Game.Scripts.Providers.Profiles.Common;
using Game.Scripts.UI.Components;
using Game.Scripts.UI.Presenters.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.UI.Presenters
{
	public class ProfilePresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent<string> OnSaveEvent = new();
		[HideInInspector] public UnityEvent OnCloseEvent = new();

		[SerializeField] private ProfileShortPanel _profilePanel;
		[SerializeField] private List<ProfileAvatar> _maleAvatars;
		[SerializeField] private List<ProfileAvatar> _femaleAvatars;
		[SerializeField] private Button _saveButton;
		[SerializeField] private Button _closeButton;

		private string _avatarSource;
		private List<AvatarSource> _maleAvatarsSource;
		private List<AvatarSource> _femaleAvatarsSource;
		private IProfile _profile;

		public void SetAvatars(List<AvatarSource> maleAvatars, List<AvatarSource> femaleAvatars)
		{
			_maleAvatarsSource = maleAvatars;
			_femaleAvatarsSource = femaleAvatars;

			for (int i = 0; i < _maleAvatars.Count; i++)
			{
				_maleAvatars[i].SetData(_maleAvatarsSource[i]);
				_maleAvatars[i].OnClick.AddListener(AvatarClick);
			}

			for (int i = 0; i < _femaleAvatars.Count; i++)
			{
				_femaleAvatars[i].SetData(_femaleAvatarsSource[i]);
				_femaleAvatars[i].OnClick.AddListener(AvatarClick);
			}
		}

		public void SetProfile(IProfile profile)
		{
			_profile = profile;

			SetCurrentProfile();
			AvatarClick(_profile.AvatarUuid);
		}

		public override async UniTask<bool> Initialize()
		{
			if (!await base.Initialize())
				return false;

			_saveButton.onClick.AddListener(SaveButtonTouch);
			_closeButton.onClick.AddListener(() => OnCloseEvent?.Invoke());

			return false;
		}

		private void AvatarClick(string avatarUuid)
		{
			_avatarSource = avatarUuid;
			for (int i = 0; i < _maleAvatars.Count; i++)
				_maleAvatars[i].IsSelected = _maleAvatars[i].Uuid == _avatarSource;

			for (int i = 0; i < _femaleAvatars.Count; i++)
				_femaleAvatars[i].IsSelected = _femaleAvatars[i].Uuid == _avatarSource;

			_saveButton.interactable = _avatarSource != _profile.AvatarUuid;
		}

		private void SetCurrentProfile()
		{
			for (int i = 0; i < _maleAvatars.Count; i++)
				_maleAvatars[i].IsCurrent = _maleAvatars[i].Uuid == _profile.AvatarUuid;

			for (int i = 0; i < _femaleAvatars.Count; i++)
				_femaleAvatars[i].IsCurrent = _femaleAvatars[i].Uuid == _profile.AvatarUuid;
		}

		public void SaveButtonTouch()
		{
			OnSaveEvent?.Invoke(_avatarSource);
		}
	}
}
