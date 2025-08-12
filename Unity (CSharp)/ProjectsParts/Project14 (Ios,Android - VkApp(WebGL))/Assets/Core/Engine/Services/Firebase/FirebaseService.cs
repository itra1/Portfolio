using Zenject;

namespace Core.Engine.Services.FireBService
{
#if FIREBASE_SERVICE
	public class FirebaseService : IFirebaseService, IInitializable
	{
		public FirebaseService()
		{
			//_init = init;
		}
		public void Initialize() {

			//_init.Init();
		}
	}
#endif
}