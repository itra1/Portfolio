using Zenject;

namespace Core.Engine.Installers
{
	public class FirebaseInstaller : MonoInstaller
	{
#if FIREBASE_SERVICE
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<FirebaseRemoteConfig>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<FirebaseService>().AsSingle().NonLazy();
		}

		private void Awake()
		{
			Container.Resolve<IFirebaseRemoteConfigInit>();
			Container.Resolve<IFirebaseService>();
		}

#endif

	}
}