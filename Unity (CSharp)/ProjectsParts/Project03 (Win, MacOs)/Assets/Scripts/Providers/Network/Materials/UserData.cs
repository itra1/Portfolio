
namespace Providers.Network.Materials
{
	[System.Serializable]
	public class UserData
	{
		public string id;
		public string login;
		public string password;
		public bool remeberMe;
		public string email;
		public int blocked;
		public string block_reason;
		public bool verified;
		public TokenData token;
	}
}
