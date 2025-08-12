using UnityEngine;
using Zenject;

namespace Game.Scripts.Providers.DailyBonuses.Settings
{
	[CreateAssetMenu(fileName = "DailyBonusSettingsInstaller", menuName = "Providers/DailyBonus/Settings")]
	public class DailyBonusSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private DailyBonusSettings _dailyBonusSettings;

		public override void InstallBindings()
		{
			_ = Container.Bind<DailyBonusSettings>().FromInstance(_dailyBonusSettings).AsSingle().NonLazy();
		}
	}
}
