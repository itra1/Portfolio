
namespace Core.Engine.Services.FBService
{
#if FACEBOOK_SERVICE
	public interface IFacebookLogin
	{
		bool IsLogin { get; }

		void Login();

	}
#endif
}
