using UnityEngine;
using Zenject;

namespace Game.Scripts.Providers.Addressable.Settings
{
	[CreateAssetMenu(fileName = "AddressableSettingsInstaller", menuName = "Installers/AddressableSettings")]
	public class AddressableSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private AddressableSettings _settings;

		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<AddressableSettings>().FromInstance(_settings).AsSingle().NonLazy();
		}
	}
}
