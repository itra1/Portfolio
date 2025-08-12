

using Cysharp.Threading.Tasks;

using UnityEngine.SocialPlatforms;
using UnityEngine;
using Providers.User.Save;
using Providers.Biometric.Common;

namespace Providers.Biometric
{
	public class BiometricProvider : IBiometricProvider
	{
		private JBiometricScript _script;
		private BiometricSave _save;
		private const string biometricSave = "biometricSave";
		public BiometricProvider()
		{
			_script = JBiometricScript.Instance();
			Load();
		}

		public bool CanAuthenticate => _script.CanAuthenticate();
		public bool HasSystemDialog => _script.HasSystemDialog();

		public bool SuccessAuth => _save.SuccessAuth;

		public async UniTask<bool> Authenticate()
		{
			bool onComplete = false;
			bool isAuth = false;
			if (!HasSystemDialog)
				_script.StartAuthenticateWithoutDialog(
					() =>
					{
						isAuth = true;
						onComplete = true;
					}, (errorMessage, isCanceled) =>
					{
						onComplete = true;
					}, (helper) =>
						{
						});
			else
				_script.StartAuthenticate(
					"Авторизация", () =>
					{
						isAuth = true;
						onComplete = true;
					}, (errorMessage, isCanceled) =>
					{
						onComplete = true;
					});

			await UniTask.WaitUntil(() => onComplete);

			return isAuth;
		}

		public void SetAccess(bool isAccess){
			_save.SuccessAuth = isAccess;
			Save();
		}

		private void Save()
		{
			string dataString = Newtonsoft.Json.JsonConvert.SerializeObject(_save);
			PlayerPrefs.SetString(biometricSave, dataString);
		}

		private void Load()
		{
			if (!PlayerPrefs.HasKey(biometricSave))
			{
				_save = new();
				return;
			}

			_save = Newtonsoft.Json.JsonConvert.DeserializeObject<BiometricSave>(PlayerPrefs.GetString(biometricSave));

		}

	}
}
