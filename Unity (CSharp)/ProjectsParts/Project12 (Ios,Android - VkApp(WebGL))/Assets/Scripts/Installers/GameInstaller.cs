using Game.Scripts.Controllers.AccuracyLabels;
using Game.Scripts.Controllers.AccuracyLabels.Factorys;
using Game.Scripts.Controllers.Inputs;
using Game.Scripts.Managers;
using Game.Scripts.Providers.Saves;
using Zenject;

namespace Game.Scripts.Installers
{
	public class GameInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<AccuracyLabelsFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<AccuracyController>().AsSingle().NonLazy();

			_ = Container.BindInterfacesTo<GameHandler>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<PauseHandler>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<SaveProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<GameStandartInput>().AsSingle().NonLazy();
		}
	}
}
