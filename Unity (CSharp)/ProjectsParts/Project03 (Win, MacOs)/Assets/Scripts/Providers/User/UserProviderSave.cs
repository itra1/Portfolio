using Providers.User.Save;

using UnityEngine;

namespace Providers.User
{
	public partial class UserProvider
	{
		private UserSaveData _save;
		private const string userSave = "userSave";

		public void ClearAuthData()
		{
			_save.AuthLogin = null;
			_save.AuthPassword = null;
			_biometric.SetAccess(false);
			PlayerPrefs.DeleteKey(userSave);
		}

		public (string, string) GetAuthData()
		{
			if (_save == null)
				return (null, null);

			return (_save.AuthLogin, _save.AuthPassword);
		}

		public void SetAuthData(string login, string password)
		{
			_save.AuthLogin = login;
			_save.AuthPassword = password;
			_biometric.SetAccess(true);
			Save();
		}

		private void Load()
		{
			if (!PlayerPrefs.HasKey(userSave))
			{
				_save = new();
				return;
			}

			_save = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSaveData>(PlayerPrefs.GetString(userSave));

		}

		private void Save()
		{
			string dataString = Newtonsoft.Json.JsonConvert.SerializeObject(_save);
			PlayerPrefs.SetString(userSave, dataString);
		}
	}
}
