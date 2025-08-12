#if FACEBOOK_SERVICE
using Facebook.Unity;
#endif
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Core.Engine.Services.FBService
{
#if FACEBOOK_SERVICE
	public class FacebookAppEvent : IFacebookAppEvent
	{

		public void AppEvent(string eventName, string contentId, string description, float progress)
		{
			var tutParams = new Dictionary<string, object>();
			tutParams[AppEventParameterName.ContentID] = contentId;
			tutParams[AppEventParameterName.Description] = description;
			tutParams[AppEventParameterName.Success] = progress.ToString();

			FB.LogAppEvent(
					AppEventName.CompletedTutorial,
					parameters: tutParams
			);
		}

	}
	#endif
}