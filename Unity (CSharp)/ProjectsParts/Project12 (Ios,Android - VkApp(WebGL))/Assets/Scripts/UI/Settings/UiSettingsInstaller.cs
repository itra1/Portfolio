using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Settings
{
	[CreateAssetMenu(fileName = "UiSettingsInstaller", menuName = "Installers/UiSettings")]
	class UiSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private UiSettings _uiSettings;
		[SerializeField] private ShopUiSettings _shopSettings;

		public override void InstallBindings()
		{
			_ = Container.Bind<UiSettings>().FromInstance(_uiSettings).NonLazy();
			_ = Container.Bind<ShopUiSettings>().FromInstance(_shopSettings).NonLazy();
		}
	}
}
