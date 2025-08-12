using System.Collections.Generic;
using Game.Base;
using Game.Providers.Smiles.Handlers;
using Game.Providers.Ui.Presenters.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class GamePlaySmilesToolPanel : MonoBehaviour, IInjection, IUiVisibleHandler
	{
		[SerializeField] private Button _openButton;
		[SerializeField] private GamePlayToolItem _prefab;
		[SerializeField] private RectTransform _panelRect;
		[SerializeField] private RectTransform _contentRect;

		private readonly List<GamePlayToolItem> _toolsItem = new();
		private ISmileHndler _smileHelper;
		private bool _isVisiblePanel = false;

		[Inject]
		private void Constructor(ISmileHndler smileHelper)
		{
			_smileHelper = smileHelper;

			_openButton.onClick.RemoveAllListeners();
			_openButton.onClick.AddListener(OpenButtonTouch);
		}

		public void Show()
		{
			if (_toolsItem.Count == 0)
				CreateButtons();

			SetVisiblePanel(false);
		}

		public void Hide()
		{
		}

		private void CreateButtons()
		{
			for (int i = 0; i < _smileHelper.SmilesList.Count; i++)
			{
				var element = _smileHelper.SmilesList[i];
				var instance = MonoBehaviour.Instantiate(_prefab, _contentRect);
				_toolsItem.Add(instance);
				instance.SetData(element.Uuid, element.Icone);

				var buttonComponent = instance.GetComponent<Button>();
				buttonComponent.onClick.RemoveAllListeners();
				buttonComponent.onClick.AddListener(() =>
				{
					_smileHelper.EmitSmileHandler(true, element.Uuid);
				});
			}
		}

		private void OpenButtonTouch()
		{
			SetVisiblePanel(!_isVisiblePanel);
		}

		private void SetVisiblePanel(bool isVisible)
		{
			_isVisiblePanel = isVisible;
			_panelRect.gameObject.SetActive(_isVisiblePanel);
		}
	}
}
