#if FACEBOOK_SERVICE
using Facebook.Unity;
#endif
using System;
using System.Collections;

using UnityEngine;

namespace Core.Engine.Services.FBService
{
#if FACEBOOK_SERVICE
	public class FacebookShare : IFacebookShare
	{
		public void Share(string url)
		{
			FB.ShareLink(
			new Uri(url),
			callback: ShareCallback
			);
		}

		private void ShareCallback(IShareResult result)
		{
			if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
			{
				Debug.Log("ShareLink Error: " + result.Error);
			}
			else if (!String.IsNullOrEmpty(result.PostId))
			{
				// Print post identifier of the shared content
				Debug.Log(result.PostId);
			}
			else
			{
				// Share succeeded without postID
				Debug.Log("ShareLink success!");
			}
		}
	}
#endif
}