using System.Linq;
using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.Audio;
using Core.Engine.Components.Skins;
using Core.Engine.Signals;
using Core.Engine.uGUI.Elements;
using Core.Engine.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.Engine.uGUI.Screens
{
	[PrefabName(ScreenTypes.Skins)]
	public class SkinsScreen :Screen
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private SkinPanel _skinItemPrefab;

		private SignalBus _signalBus;
		private ISkinProvider _skinProvider;
		private PrefabPool<SkinPanel> _skinPool;

		[Inject]
		public void Initiate(SignalBus signalbus, ISkinProvider skinProvider)
		{
			_signalBus = signalbus;
			_skinProvider = skinProvider;

			_skinPool = new(_skinItemPrefab, _scrollRect.content);
			_skinItemPrefab.gameObject.SetActive(false);

			signalbus.Subscribe<SetSkinSignal>(SetSkinSignal);
		}

		private void OnEnable()
		{
			SpawnItems();
		}

		private void SetSkinSignal()
		{
			if (!gameObject.activeInHierarchy)
				return;

			SpawnItems();
		}

		private void SpawnItems()
		{
			_skinPool.HideAll();

			var skins = _skinProvider.Items.OrderByDescending(x => x.IsDefault).ToList();
			var count = 0;

			for (var i = 0; i < skins.Count; i++)
			{
				if (!skins[i].ReadyToSelect && !skins[i].IsDefault)
					continue;
				var item = skins[i];
				var elem = _skinPool.GetItem();
				elem.Set(_skinProvider, item);
				elem.RT.anchoredPosition = new(elem.RT.anchoredPosition.x, -(count * (elem.RT.rect.height + 5)));
				elem.gameObject.SetActive(true);
				count++;
			}
			_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, count * (_skinItemPrefab.RT.rect.height + 5));
		}

		public void BackButtonTouch()
		{
			PlayAudio.PlaySound("click");
			_signalBus.Fire(new UGUIButtonClickSignal() { Name = ButtonTypes.FirstMenuOpen });
		}
	}
}
