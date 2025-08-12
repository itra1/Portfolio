using Engine.Scripts.Base;
using Game.Scripts.Providers.Avatars.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class ProfileAvatar : MonoBehaviour, IInjection
	{
		[HideInInspector] public UnityEvent<string> OnClick = new();

		[SerializeField] private Button _selfButton;
		[SerializeField] private Image _avatar;
		[SerializeField] private Transform _border;
		[SerializeField] private Transform _check;
		[SerializeField] private Transform _currentLabel;

		private AvatarSource _avatarSettings;
		private bool _isSelected;
		private bool _isCurrent;

		public string Uuid => _avatarSettings.Uuid;

		public bool IsCurrent
		{
			get => _isCurrent;
			set
			{
				_isCurrent = value;
				ConfirmVisual();
			}
		}
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				ConfirmVisual();
			}
		}

		[Inject]
		private void Build()
		{
			_selfButton.onClick.RemoveAllListeners();
			_selfButton.onClick.AddListener(SelfButtonTouch);
		}

		private void SelfButtonTouch() => OnClick?.Invoke(_avatarSettings.Uuid);

		public void SetData(AvatarSource avatar)
		{
			_avatarSettings = avatar;
			_avatar.sprite = _avatarSettings.Avatar50Round10;
			IsCurrent = false;
			IsSelected = false;
			ConfirmVisual();
		}

		private void ConfirmVisual()
		{
			_currentLabel.gameObject.SetActive(IsCurrent);
			_border.gameObject.SetActive(IsSelected);
			_check.gameObject.SetActive(IsSelected && !IsCurrent);
		}
	}
}
