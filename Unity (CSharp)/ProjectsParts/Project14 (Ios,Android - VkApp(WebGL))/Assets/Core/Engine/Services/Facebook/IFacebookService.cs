using Core.Engine.Services.Common;

namespace Core.Engine.Services.FBService
{
	public interface IFacebookService : IService
	{
#if FACEBOOK_SERVICE

		IFacebookInit Init { get; }
		IFacebookLogin Login { get; }

#endif

	}

}
