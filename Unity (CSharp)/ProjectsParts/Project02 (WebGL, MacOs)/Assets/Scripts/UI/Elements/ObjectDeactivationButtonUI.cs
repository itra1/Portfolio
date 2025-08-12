using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace it.UI.Elements
{
  public class ObjectDeactivationButtonUI : ButtonUI
  {
    [SerializeField] private GameObject _normalObject;
		[SerializeField] private Graphic[] _coloredGraphic;
		[SerializeField] private Graphic[] _graphics;
		[SerializeField] private Color _activeColor = Color.white;
		[SerializeField] private Color _disactiveColor = Color.gray;
		[SerializeField] private string _colorProperty;
		[ColorUsage(false, true)]
		[SerializeField] private Color _colorLight;

		public override void NormalState()
		{
			base.NormalState();
			DefSet();
			if (!string.IsNullOrEmpty(_colorProperty))
				for (int i = 0; i < _graphics.Length; i++)
					_graphics[i].material.SetColor(_colorProperty, Color.black);
		}

		public override void HighlightedState()
		{
			base.HighlightedState();
			DefSet();
			if (!string.IsNullOrEmpty(_colorProperty))
				for (int i = 0; i < _graphics.Length; i++)
					_graphics[i].material.SetColor(_colorProperty, _colorLight);
		}
		public override void SelectState()
		{
			base.HighlightedState();
			DefSet();
			if (!string.IsNullOrEmpty(_colorProperty))
				for (int i = 0; i < _graphics.Length; i++)
					_graphics[i].material.SetColor(_colorProperty, Color.black);
		}

		public override void DownState()
		{
			base.DownState();
			DownsSet();
			if (!string.IsNullOrEmpty(_colorProperty))
				for (int i = 0; i < _graphics.Length; i++)
					_graphics[i].material.SetColor(_colorProperty, _colorLight);
		}

		public override void DisableState()
		{
			base.DisableState();
			if (!string.IsNullOrEmpty(_colorProperty))
{
				for (int i = 0; i < _graphics.Length; i++)
					_graphics[i].material.SetColor(_colorProperty, _colorLight);
			}
			else
			{
				DefSet();
			}
		}

		private void DefSet()
		{
			_normalObject.SetActive(true);
			_normalObject.transform.SetAsLastSibling();
			ConfirmGraphic();
		}
		private void DownsSet()
		{
			_normalObject.SetActive(false);
			ConfirmGraphic();
		}

		private void ConfirmGraphic()
		{
			for (int i = 0; i < _coloredGraphic.Length; i++)
				_coloredGraphic[i].color = IsInteractable() ? _activeColor : _disactiveColor;
		}
	}
}