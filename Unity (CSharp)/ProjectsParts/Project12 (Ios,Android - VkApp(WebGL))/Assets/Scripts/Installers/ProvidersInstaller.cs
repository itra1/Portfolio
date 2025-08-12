using Game.Scripts.Providers.Avatars;
using Game.Scripts.Providers.DailyBonuses;
using Game.Scripts.Providers.DailyMissions;
using Game.Scripts.Providers.DailyMissions.Factorys;
using Game.Scripts.Providers.Leaderboard;
using Game.Scripts.Providers.Premiums;
using Game.Scripts.Providers.Songs;
using Game.Scripts.Providers.Songs.Helpers;
using Game.Scripts.Providers.Timers;
using Zenject;

namespace Game.Scripts.Installers
{
	public class ProvidersInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<TimersProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<DailyBonusProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<DailyMissionFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<DailyMissionProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<AvatarsProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<LeaderboardProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<PremiumProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<SongsProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<SongsHelper>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<SongSaveHelper>().AsSingle().NonLazy();
		}
	}
}
