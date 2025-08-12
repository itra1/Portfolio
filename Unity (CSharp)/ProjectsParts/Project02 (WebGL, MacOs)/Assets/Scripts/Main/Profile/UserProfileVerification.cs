using System.Collections;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;
using it.Popups;

namespace it.UI
{
	public class UserProfileVerification : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _verifiedDescriptionLabel;
		[SerializeField] private TextMeshProUGUI _noVerifiedDescriptionLabel;
		[SerializeField] private RectTransform _verifiedFlag;
		[SerializeField] private RectTransform _increadeButton;
		[SerializeField] private RectTransform _verifiedIcone;

		private void OnEnable()
		{
			FillData();

			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
		}

		private void UserProfileUpdate(com.ootii.Messages.IMessage handle)
		{
			FillData();
		}

		private void FillData()
		{

			bool isVerified = UserController.User.verified;
			if (_increadeButton != null)
				_increadeButton.gameObject.SetActive(!isVerified);
			if (_verifiedFlag != null)
				_verifiedFlag.gameObject.SetActive(isVerified);
			if (_verifiedIcone != null)
				_verifiedIcone.gameObject.SetActive(isVerified);
			_verifiedDescriptionLabel.gameObject.SetActive(isVerified);
			if (_noVerifiedDescriptionLabel != null)
				_noVerifiedDescriptionLabel.gameObject.SetActive(!isVerified);
		}

		public void IncreaseTouch()
		{
			GetAuthToken();
		}
		private void GetAuthToken()
		{
			it.Api.UserApi.GetVerifiedToken((token) =>
			{
				OpenPage(token);
			}, (error) =>
			{

			});
		}

		private void OpenPage(string token)
		{
			//#if UNITY_STANDALONE
			//			//var asset = Resources.Load<TextAsset>("sumbusHtmlPage");
			//			var asset = (TextAsset)Garilla.ResourceManager.GetResource<TextAsset>("sumbusHtmlPage");

			//			var HTML = asset.text.Replace("$REPLACE_TOKEN", token);

			//			var popup = it.Main.PopupController.Instance.ShowPopup<it.Popups.BrowserPopup>(PopupType.Browser);
			//			popup.SetHtml(HTML);
			//			popup.OnClose += (p) =>
			//			{
			//				StartCoroutine(CorUpdata());
			//			};
			////#elif UNITY_WEBGL
			////			openUrl($"https://garilla-web.com/fu3478fhf3kuv.php?lang=ru&token={token}");
			//#else
			string url = "https://garilla-web.com/php/fu3478fhf3kuv.php?lang=ru&token=" + token;

#if UNITY_WEBGL
			ConfirmPopup panel = Main.PopupController.Instance.ShowPopup<ConfirmPopup>(PopupType.Confirm);
			panel.SetDescriptionString("message.info.openUrlToProcess".Localized());
			panel.OnConfirm = () =>
			{
				Application.OpenURL(url);
			};
#else
			Application.OpenURL(url);
#endif

			//#endif
		}

		IEnumerator CorUpdata()
		{
			yield return new WaitForSeconds(5);
			UserController.Instance.GetUserProfile();
		}

#if UNITY_WEBGL
		[DllImport("__Internal")]
		private static extern void openUrl(string url);
#endif

	}
}