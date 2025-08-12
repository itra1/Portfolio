using Game.Base;
using Game.Settings;
using Scripts.Common;
using Scripts.Common.Blobs;
using Scripts.GameItems.Platforms;
using Scripts.Workers;
using UnityEngine;
using Zenject;

namespace Scripts.Installers
{
	public class GameInstaller :MonoInstaller
	{

		[Inject]
		private GamePrefabSettings.Prefabs _gamePrefabs;
		private SceneComponents _sceneComponents;
		public override void InstallBindings()
		{
			_sceneComponents = (SceneComponents)FindAnyObjectByType(typeof(SceneComponents));

			_ = Container.Bind(typeof(SceneComponents)).FromInstance(_sceneComponents).AsSingle().NonLazy();

			_ = Container.Bind<SkinWorker>().AsSingle().NonLazy();
			_ = Container.Bind<ThemeWorker>().AsSingle().NonLazy();
			_ = Container.Bind<PlatformFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<PlatformSpawner>().AsSingle().NonLazy();
			_ = Container.Bind<BlobSpawner>().AsSingle().NonLazy();
			_ = Container.BindFactory<Color, IPlatform, Blob, Blob.Factory>()
			.FromPoolableMemoryPool<Color, IPlatform, Blob, BlobPool>(poolBinder => poolBinder
				.WithInitialSize(10)
				.FromComponentInNewPrefab(_gamePrefabs.Blob));
			_ = Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();

			Container.Inject(_sceneComponents.Player);
			Container.Inject(_sceneComponents.ResetButton);
		}

		public class BlobPool :MonoPoolableMemoryPool<Color, IPlatform, IMemoryPool, Blob> { }
	}
}
