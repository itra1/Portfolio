#if FACEBOOK_SERVICE
using Facebook.Unity;
#endif
using System.Collections;

using UnityEngine;

namespace Core.Engine.Services.FBService
{
#if FACEBOOK_SERVICE
	public class FacebookInit: IFacebookInit
	{
		public bool IsInitialized => FB.IsInitialized;

		public void Init()
		{
			if (FB.IsInitialized)
			{
				FB.Init(InitCallback, OnHideUnity);
			}
			else
			{
				FB.ActivateApp();
			}
		}

		private void InitCallback()
		{
			if (FB.IsInitialized)
			{
				FB.ActivateApp();
			}
			else
			{
				Debug.Log("Failed to Initialize the Facebook SDK");
			}
		}

		private void OnHideUnity(bool isGameShown)
		{
			if (!isGameShown)
			{
				Time.timeScale = 0;
			}
			else
			{
				Time.timeScale = 1;
			}
		}

	}
#endif
}