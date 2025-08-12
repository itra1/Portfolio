using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace it.UI.Elements
{
	public class ObjectChangeButtonUI : ButtonUI
	{
		[SerializeField] private GameObject _normalObject;
		[SerializeField] private GameObject _downObject;
		[SerializeField] private Color _activeColor = Color.white;
		[SerializeField] private Color _disactiveColor = Color.gray;
		[SerializeField] private Graphic[] _coloredGraphic;
		[SerializeField] private Graphic[] _graphics;
		[SerializeField] private string _colorProperty;
		[SerializeField] private bool _hoverActivate;
		[ColorUsage(false,true)]
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
			if(_hoverActivate)
				DownsSet();
			else
				DefSet();
			if (!string.IsNullOrEmpty(_colorProperty))
				for (int i = 0; i < _graphics.Length; i++)
					_graphics[i].material.SetColor(_colorProperty, _colorLight);
		}
		public override void SelectState()
		{
			//base.HighlightedState();
			//DefSet();
			//if (!string.IsNullOrEmpty(_colorProperty))
			//	for (int i = 0; i < _graphics.Length; i++)
			//		_graphics[i].material.SetColor(_colorProperty, Color.black);
			NormalState();
			try
			{
				//UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
			}
			catch { }
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
			}else
			{
				DefSet();
			}
		}

		private void DefSet()
		{
			if(_normalObject != null) _normalObject.SetActive(true);
			if(_downObject != null) _downObject.SetActive(false);
			if (_normalObject != null) _normalObject.transform.SetAsLastSibling();
			ConfirmGraphic();
		}
		private void DownsSet()
		{
			if (_normalObject != null) _normalObject.SetActive(false);
			if (_downObject != null) _downObject.SetActive(true);
			if (_downObject != null) _downObject.transform.SetAsLastSibling();
			ConfirmGraphic();
		}

		private void ConfirmGraphic()
		{
			for (int i = 0; i < _coloredGraphic.Length; i++)
				_coloredGraphic[i].color = IsInteractable() ? _activeColor : _disactiveColor;
		}

	}

}