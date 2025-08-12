using UnityEngine;
using Zenject;

namespace Game.Scripts.Providers.DailyMissions.Settings
{
	[CreateAssetMenu(fileName = "DailyMissionSettingsInstaller", menuName = "Providers/DailyMission/Settings")]
	public partial class DailyMissionSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private DailyMissionSettings _missions;
		[SerializeField] private DailyMissionColorSettings _colors;
		[SerializeField] private DailyMissionIconeSettings _icons;
		public override void InstallBindings()
		{
			_ = Container.Bind<DailyMissionSettings>().FromInstance(_missions).AsSingle().NonLazy();
			_ = Container.Bind<DailyMissionColorSettings>().FromInstance(_colors).AsSingle().NonLazy();
			_ = Container.Bind<DailyMissionIconeSettings>().FromInstance(_icons).AsSingle().NonLazy();
		}
	}
}
