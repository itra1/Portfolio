using Cysharp.Threading.Tasks;
using Providers.Network.Materials;

namespace Providers.User
{
	public interface IUserProvider
	{
		bool IsAuthorized { get; }

		UniTask Initialize();
		UniTask RunUI();

		public void SetUserData(UserData userData);

		public string Token { get;}

	}
}
