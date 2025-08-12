using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Core.Engine.Services.FBService
{
#if FACEBOOK_SERVICE
	public interface IFacebookPermissions
	{
		List<string> Permissions { get; }
	}
#endif
}