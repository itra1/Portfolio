using DG.Tweening;
using Game.Base;
using Game.Providers.Audio.Base;
using Game.Providers.Audio.Handlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Popups.Elements
{
	/// <summary>
	/// Переключатель
	/// </summary>
	public class ToggleElement : MonoBehaviour, IInjection
	{
		public UnityEngine.Events.UnityEvent<bool> OnChange = new();

		[SerializeField] private Image _back;
		[SerializeField] private Image _point;
		[SerializeField] private Vector2 _ancorActive;
		[SerializeField] private Vector2 _ancorDisable;
		[SerializeField] private TMP_Text _handleLabel;

		private bool? _isActive = null;
		private AudioHandler _audioHandler;

		public bool IsActive
		{
			get => _isActive.Value;
			set
			{
				if (_isActive == value)
					return;
				_isActive = value;
				if (_isActive.Value)
					SetActiveState();
				else
					SetDisableState();
			}
		}
		[Inject]
		public void Constructor(AudioHandler audioHandler)
		{
			_audioHandler = audioHandler;
		}

		public void OnDisable()
		{
			_isActive = null;
		}

		public void Touch()
		{
			IsActive = !IsActive;
			_ = _audioHandler.PlayRandomClip(IsActive ? SoundNames.UiOn : SoundNames.UiOff);
			OnChange?.Invoke(IsActive);
		}

		public void SetActiveState()
		{
			_handleLabel.text = "ON";
			var targetColor = new Color(1, 1, 1, 1);
			if (!gameObject.activeInHierarchy)
			{
				_back.color = targetColor;
				_point.rectTransform.anchoredPosition = _ancorActive;
				return;
			}

			_ = _back.DOColor(targetColor, 0.25f);
			_ = _point.rectTransform.DOAnchorPos(_ancorActive, 0.25f);
		}

		public void SetDisableState()
		{
			_handleLabel.text = "OFF";
			var targetColor = new Color(1, 1, 1, 0);
			if (!gameObject.activeInHierarchy)
			{
				_back.color = targetColor;
				_point.rectTransform.anchoredPosition = _ancorDisable;
				return;
			}

			_ = _back.DOColor(targetColor, 0.25f);
			_ = _point.rectTransform.DOAnchorPos(_ancorDisable, 0.25f);
		}
	}
}