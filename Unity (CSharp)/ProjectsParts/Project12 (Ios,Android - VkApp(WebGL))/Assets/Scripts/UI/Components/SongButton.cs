using System;
using Engine.Scripts.Timelines;
using itra.Animations.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Components
{
	public class SongButton : MonoBehaviour
	{
		[SerializeField] protected TextMeshProUGUI m_SongName;
		[SerializeField] protected Button m_Button;
		[SerializeField] private Image _border;
		[SerializeField] private ImageSpriteToggleUnitaskAnim _eqAnimation;
		[SerializeField] private Color _selectColor;
		[SerializeField] private Color _deselectColor;

		protected int _Index;
		protected RhythmTimelineAsset m_Song;
		private Image _baseImage;

		private Action<bool> OnPlay;

		public int Index => _Index;
		public bool IsSelected { get; private set; }
		public RhythmTimelineAsset Song => m_Song;
		public Button Button => m_Button;
		public TextMeshProUGUI SongName => m_SongName;

		public void Initialize(RhythmTimelineAsset song, int index, Action<SongButton> onClickAction, Action<bool> onPlay)
		{
			m_Song = song;
			_Index = index;
			_baseImage = GetComponent<Image>();

			m_SongName.text = $"{m_Song.Authour} - {m_Song.FullName}";
			OnPlay = onPlay;
			m_Button.onClick.AddListener(() => onClickAction.Invoke(this));
			_border.gameObject.SetActive(false);
			_eqAnimation.gameObject.SetActive(false);
		}

		public void SetSelected(bool isSelected)
		{

			// Уже отключен
			if (!IsSelected && !isSelected)
				return;

			// Если отключается
			if (!isSelected)
			{
				_eqAnimation.Stop();
				_border.gameObject.SetActive(false);
				_eqAnimation.gameObject.SetActive(false);
			}

			// Выполняется повторный клик
			if (isSelected && IsSelected)
			{
				if (!_eqAnimation.IsPlaying)
				{
					_eqAnimation.Play();
				}
				else
				{
					_eqAnimation.Stop();
				}
				OnPlay?.Invoke(_eqAnimation.IsPlaying);
				return;
			}
			if (isSelected && !IsSelected)
			{
				_border.gameObject.SetActive(true);
				_eqAnimation.gameObject.SetActive(true);
				_eqAnimation.Play();
			}

			IsSelected = isSelected;
			_baseImage.color = IsSelected ? _selectColor : _deselectColor;
		}
	}
}
