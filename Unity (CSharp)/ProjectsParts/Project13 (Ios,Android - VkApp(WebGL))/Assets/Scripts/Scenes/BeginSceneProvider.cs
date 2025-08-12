using Game.Base;
using Game.Game.Base;
using Game.Game.Elements.Scenes;
using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Settings;
using Game.Providers.Ui.Windows.Base;
using UnityEngine;
using Zenject;

namespace Game.Scenes
{
	public class BeginSceneProvider : MonoBehaviour, ISceneCanvas, IWorldParent, IPopupsParent, IWindowsParent, IItemsAnimationParent
	{
		[SerializeField] private Transform _world;
		[SerializeField] private RectTransform _baseRect;
		[SerializeField] private RectTransform _windows;
		[SerializeField] private RectTransform _popups;
		[SerializeField] private RectTransform _splash;
		[SerializeField] private RectTransform _debug;
		[SerializeField] private ItemsAnimationPanel _itemsAnimationPanel;
		[SerializeField] private GameScene _gameScene;

		public RectTransform PopupsParent => _popups;
		public RectTransform WindowsParent => _windows;
		public RectTransform SplashParent => _splash;
		public RectTransform DebugParent => _debug;
		public Transform WorldParent => _world;
		public GameScene GameScene => _gameScene;
		public ItemsAnimationPanel ItemsAnimationPanel => _itemsAnimationPanel;

		[Inject]
		public void Constructor(DiContainer container)
		{
			var componentsDebug = _debug.GetComponentsInChildren<IInjection>();
			for (var i = 0; i < componentsDebug.Length; i++)
			{
				container.Inject(componentsDebug[i]);
			}
			container.Inject(_itemsAnimationPanel);
		}
	}
}
