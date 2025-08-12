using System.Collections;
using TMPro;
using UnityEngine;

namespace it.UI.Elements
{
	public class FilterSwitchButtonUI : ButtonUI
	{
		public UnityEngine.Events.UnityAction OnSelectAction;

		[SerializeField] private Color _defaultColor = Color.white;
		[SerializeField] private Color _selectColor = new Color(0.7764706f, 0.5490196f, 0.2627451f, 1);
		[SerializeField] private Color _colorLight = new Color(0.7764706f, 0.5490196f, 0.2627451f, 1);
		[SerializeField] private FontStyles _hoverStyleLight = FontStyles.Underline;
		[SerializeField] private TextMeshProUGUI _label;
		[SerializeField] private GameObject _selectImage;

		public Color StartColor { get => _startColor; set => _startColor = value; }
		public bool IsSelect
		{
			get => _isSelect; 
			set
			{
				_isSelect = value;

				if (_isSelect)
				{
					OnSelectAction?.Invoke();
					if (_selectImage != null)
						_selectImage.gameObject.SetActive(true);
					StartColor = _selectColor;
				}else
				{
					if (_selectImage != null)
						_selectImage.gameObject.SetActive(false);
					StartColor = _defaultColor;
				}

				switch (_state)
				{
					case State.normal:
						NormalState();
						break;
					case State.hightlight:
						HighlightedState();
						break;
					case State.down:
						DownState();
						break;
				}

			}
		}

		private Color _startColor;
		private FontStyles _startStyles;
		private bool _isSelect;
		private State _state;

		private enum State{
	normal, hightlight,down
		}

		protected override void Init()
		{
			_state = State.normal;
			base.Init();
			if (_label == null)
				_label = GetComponentInChildren<TextMeshProUGUI>();
			_startColor = _label.color;
			_startStyles = _label.fontStyle;
		}
		public override void Click()
		{
			if (!interactable) return;
			base.Click();
		}

		public override void NormalState()
		{
			if (_label == null)
				Init();

			_state = State.normal;
			_label.fontStyle = _startStyles;
			_label.color = _startColor;

			//if (!IsSelect && _selectImage != null)
			//	_selectImage.gameObject.SetActive(false);
		}

		public override void HighlightedState()
		{
			if (_label == null)
				Init();
			_state = State.hightlight;
			_label.fontStyle = _hoverStyleLight;
			_label.color = _colorLight;

			//if (_selectImage != null)
			//	_selectImage.gameObject.SetActive(true);
		}
		public override void DownState()
		{
			if (_label == null)
				Init();

			_state = State.down;
			_label.fontStyle = _hoverStyleLight;
			_label.color = _colorLight;
		}
	}
}