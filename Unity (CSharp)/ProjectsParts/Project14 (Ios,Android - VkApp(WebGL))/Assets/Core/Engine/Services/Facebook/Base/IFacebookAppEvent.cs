using System.Collections;

using UnityEngine;

namespace Core.Engine.Services.FBService
{
	public interface IFacebookAppEvent
	{
		public void AppEvent(string eventName, string contentId, string description, float progress);
	}
}