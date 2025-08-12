using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Core.Engine.Services.FBService
{
#if FACEBOOK_SERVICE
	public class FacebookPermissions: IFacebookPermissions
	{
		public List<string> Permissions => new List<string>
		{
			"email"
			,"public_profile"
		};
	}
#endif
}