using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Core.Engine.uGUI.Elements
{
	/// <summary>
	/// Переключатель
	/// </summary>
	public class ToggleElement : MonoBehaviour
	{
		public UnityEngine.Events.UnityEvent<bool> OnChange;

		[SerializeField] private Color _activeColor;
		[SerializeField] private Color _disableColor;
		[SerializeField] private Image _back;
		[SerializeField] private Image _point;
		[SerializeField] private Vector2 _ancorActive;
		[SerializeField] private Vector2 _ancorDisable;

		private void Awake()
		{
			//SetDisableState();
		}

		public bool IsActive
		{
			get => _isActive;
			set
			{
				if (_isActive == value) return;
				_isActive = value;
				if (_isActive)
					SetActiveState();
				else
					SetDisableState();
			}
		}

		private bool _isActive = false;

		public void Touch()
		{
			IsActive = !IsActive;

			OnChange?.Invoke(IsActive);
		}

		public void SetActiveState()
		{
			if (!gameObject.activeSelf)
				gameObject.SetActive(true);

			_back.DOColor(_activeColor, 0.25f);
			_point.rectTransform.DOAnchorPos(_ancorActive, 0.25f);
		}

		public void SetDisableState()
		{
			if (!gameObject.activeSelf)
				gameObject.SetActive(true);
			_back.DOColor(_disableColor, 0.25f);
			_point.rectTransform.DOAnchorPos(_ancorDisable, 0.25f);
		}

	}
}