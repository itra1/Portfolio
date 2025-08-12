using UnityEngine;
using Zenject;

namespace Game.Scripts.Providers.Premiums.Settings
{
	[CreateAssetMenu(fileName = "PremiumSettingsInstaller", menuName = "Providers/Premium/Settings")]
	public class PremiumSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private PremiumSettings _settinga;

		public override void InstallBindings()
		{
			_ = Container.Bind<PremiumSettings>().FromInstance(_settinga).AsSingle().NonLazy();
		}
	}
}
