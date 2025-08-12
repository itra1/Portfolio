using Zenject;
using UnityEngine;
#if FACEBOOK_SERVICE
using Facebook.Unity;
#endif
namespace Core.Engine.Services.FBService
{
	public class FacebookService : IFacebookService

#if FACEBOOK_SERVICE
	, IInitializable
#endif
	{
#if FACEBOOK_SERVICE

		public IFacebookInit Init => _init;
		public IFacebookLogin Login => _login;

		private IFacebookInit _init;
		private IFacebookLogin _login;
		private IFacebookPermissions _permissions;

		public FacebookService(
			IFacebookInit fbInit
		, IFacebookLogin fbLogin
		, IFacebookPermissions fbPermissions
		)
		{
			_init = fbInit;
			_login = fbLogin;
			_permissions = fbPermissions;
		}

		public void Initialize()
		{
			_init.Init();
		}

#endif
	}

}