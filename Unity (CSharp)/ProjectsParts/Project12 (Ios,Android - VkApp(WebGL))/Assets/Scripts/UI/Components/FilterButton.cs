using Engine.Scripts.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.UI.Components
{
	public class FilterButton : MonoBehaviour
	{
		[HideInInspector] public UnityEvent<DifficultyTrack> OnClick = new();

		[SerializeField] private DifficultyTrack _dificultyTrack;
		[SerializeField] private TMP_Text _titleLabel;
		[SerializeField] private GameObject _light;

		private bool _isSelected;

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				_light.SetActive(_isSelected);
				_titleLabel.color = _isSelected ? ColorActive : ColorDisactive;
			}
		}
		public DifficultyTrack DificultyTrack => _dificultyTrack;
		private Color ColorActive => Color.white;
		private Color ColorDisactive => new(1, 1, 1, 0.5f);

		private void Awake()
		{
			var button = GetComponent<Button>();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => OnClick?.Invoke(_dificultyTrack));
		}
	}
}
