using UnityEngine;
using Zenject;

namespace Game.Scripts.Providers.Shop.Settings
{
	[CreateAssetMenu(fileName = "ShopSettingsInstaller", menuName = "Providers/Shop/ShopSettings")]
	public class ShopSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private ShopSettings _settings;

		public override void InstallBindings()
		{
			_ = Container.Bind<ShopSettings>().FromInstance(_settings).AsSingle().NonLazy();
		}
	}
}
