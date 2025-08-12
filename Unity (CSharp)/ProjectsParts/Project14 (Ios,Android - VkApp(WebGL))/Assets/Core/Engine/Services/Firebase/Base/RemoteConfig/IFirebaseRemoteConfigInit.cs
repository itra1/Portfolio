
namespace Core.Engine.Services.FireBService.RemoteConfig
{
#if FIREBASE_SERVICE
	public interface IFirebaseRemoteConfigInit
	{
		void Init();
	}
#endif
}
