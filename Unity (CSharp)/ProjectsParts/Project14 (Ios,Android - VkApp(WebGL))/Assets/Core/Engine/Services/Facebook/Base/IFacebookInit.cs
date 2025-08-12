using System.Collections;

using UnityEngine;

namespace Core.Engine.Services.FBService
{
#if FACEBOOK_SERVICE
	public interface IFacebookInit
	{
		bool IsInitialized { get; }

		void Init();

	}
#endif
}