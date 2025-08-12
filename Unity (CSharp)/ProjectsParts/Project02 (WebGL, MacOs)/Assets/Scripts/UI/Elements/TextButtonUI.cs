using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace it.UI.Elements
{
	public class TextButtonUI : ButtonUI
	{
		[SerializeField] private Color _colorLight = new Color(0.7764706f, 0.5490196f, 0.2627451f, 1);
		[SerializeField] private FontStyles _hoverStyleLight = FontStyles.Normal;
		[SerializeField] private TextMeshProUGUI _label;
		private Color _startColor;
		private FontStyles _startStyles;

		public Color StartColor
		{
			get => _startColor;
			set
			{
				_startColor = value;
				DoStateTransition(currentSelectionState, false);
			}
		}
		public Color ColorLight { get => _colorLight; set => _colorLight = value; }

		public override void Click()
		{
			if (!interactable) return;
			base.Click();
		}

		protected override void Init()
		{
			base.Init();
			if (_label == null)
				_label = GetComponentInChildren<TextMeshProUGUI>();
			_startColor = _label.color;
			_startStyles = _label.fontStyle;
		}


		public override void NormalState()
		{
			if (_label == null)
				Init();

			_label.fontStyle = _startStyles;
			_label.color = _startColor;
		}

		public override void HighlightedState()
		{
			if (_label == null)
				Init();
			_label.fontStyle = _hoverStyleLight;
			_label.color = _colorLight;
		}
		public override void DownState()
		{
			if (_label == null)
				Init();
			_label.fontStyle = _hoverStyleLight;
			_label.color = _colorLight;
		}

		public override void SelectState()
		{
			//NormalState();
		//	EventSystem.current.SetSelectedGameObject(null);
		}

	}
}