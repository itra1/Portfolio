using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Providers.User
{
	public partial class UserProvider : IUserProviderRequests
	{
		public async UniTask<(bool, object)> AuthorizationRequest(string login, string password)
		{
			(bool result, object response) = await _api.Authorization(login, password);
			return (result, response);
		}
	}
}
