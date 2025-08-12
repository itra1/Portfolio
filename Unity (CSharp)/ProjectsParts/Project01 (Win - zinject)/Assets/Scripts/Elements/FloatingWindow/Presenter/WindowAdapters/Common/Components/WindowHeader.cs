using System;
using Elements.Windows.Base.Presenter.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components
{
	public class WindowHeader : HiddenBarBase
	{
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private Image _icon;
		[SerializeField] private Button _closeButton;
		
		public event Action Closed;
		
		private void Awake() => _closeButton.onClick.AddListener(OnCloseButtonClick);
		private void OnDestroy() => _closeButton.onClick.RemoveListener(OnCloseButtonClick);
		
		public void SetTitle(string value) => _title.text = value;
		public void SetIconSprite(Sprite sprite) => _icon.sprite = sprite;
		
		private void OnCloseButtonClick() => Closed?.Invoke();
	}
}
