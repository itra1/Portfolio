using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Providers.User
{
	public interface IUserAuth
	{
		public void SetAuthData(string login, string password);
		public (string, string) GetAuthData();
		public void ClearAuthData();
	}
}
