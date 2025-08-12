using UnityEngine;
using Zenject;

namespace Game.Providers.Shop.Settings
{
	[CreateAssetMenu(fileName = "ShopSettings", menuName = "App/Create/Settings/ShopSettings")]
	public class ShopSettingsScriptableObject : ScriptableObjectInstaller
	{
		[SerializeField] private ShopSettings _shopSettings;

		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<ShopSettings>().FromInstance(_shopSettings);
		}
	}
}
