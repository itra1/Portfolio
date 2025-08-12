using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Providers.User
{
	public interface IUserProviderRequests
	{
		UniTask<(bool, object)> AuthorizationRequest(string login, string password);

	}
}
