using System.Collections.Generic;
using Game.Base;
using Game.Game.Elements.Weapons.Factorys;
using Game.Providers.Profile;
using Game.Providers.Ui.Presenters.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class GamePlayWeaponToolPanel : MonoBehaviour, IInjection, IUiVisibleHandler
	{
		[HideInInspector] public UnityAction<string> OnSelectWeapon;

		[SerializeField] private Button _openButton;
		[SerializeField] private GamePlayToolItem _prefab;
		[SerializeField] private RectTransform _panelRect;
		[SerializeField] private RectTransform _contentRect;

		private readonly List<GamePlayToolItem> _toolsItem = new();
		private bool _isVisiblePanel = false;
		private IWeaponFactory _weaponsFactory;
		private IProfileProvider _profileProvider;

		[Inject]
		public void Constructor(IProfileProvider profileProvider, IWeaponFactory weaponsFactory)
		{
			_weaponsFactory = weaponsFactory;
			_profileProvider = profileProvider;
			_openButton.onClick.RemoveAllListeners();
			_openButton.onClick.AddListener(OpenButtonTouch);
		}

		public void Show()
		{
			SetVisiblePanel(false);

			CreateButtons();
		}

		public void Hide()
		{
		}

		private void CreateButtons()
		{
			for (int i = 0; i < _toolsItem.Count; i++)
				Destroy(_toolsItem[i].gameObject);
			_toolsItem.Clear();

			var prefabRect = _prefab.transform as RectTransform;
			float height = prefabRect.rect.height + 10;
			int countWeapons = _profileProvider.Weapons.Count;


			foreach (var item in _profileProvider.Weapons)
			{
				var weapon = _weaponsFactory.PrefabsList.Find(x => x.Type == item.Name);
				var instance = MonoBehaviour.Instantiate(_prefab, _contentRect);
				_toolsItem.Add(instance);
				instance.SetData(item.Name, weapon.Icone, item.IsUnlumited ? -1 : item.Count);

				var buttonComponent = instance.GetComponent<Button>();
				buttonComponent.onClick.RemoveAllListeners();
				buttonComponent.onClick.AddListener(() =>
				{
					//_playerGameHelper.ChangeWeapon(item.Name);
					OnSelectWeapon?.Invoke(item.Name);
				});
			}
			_panelRect.sizeDelta = new(_panelRect.sizeDelta.x, height * countWeapons + 10);
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
