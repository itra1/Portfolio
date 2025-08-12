using it.Managers;
using it.Network.Rest;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerifyIdSettingsPage : MonoBehaviour
{
	[SerializeField] private it.UI.Elements.GraphicButtonUI _increateButton;

	public void IncreaseTouch()
	{
		GetAuthToken();
	}

	private void GetAuthToken()
	{

		string url = "/profile/verificationToken";

		it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
		{
#if UNITY_EDITOR
			it.Logger.Log("Get Auth Token " + result);
#endif

			//var data = (AuthToken)it.Helpers.ParserHelper.Parse(typeof(AuthToken), result);
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthToken>(result);

			it.Logger.Log("token " + data.data);
			OpenPage(data.data);

		},
	 (error) =>
	 {
		 it.Logger.LogError("Logout error " + error + " | Request: " + url);
		 //onError?.Invoke(error);
		 //OutputError();
		 return;
	 });
	}

	public class AuthToken
	{
		public string data;
	}

	private void OpenPage(string token){
		//var asset = Resources.Load<TextAsset>("sumbusHtmlPage");
		//var asset = (TextAsset)Garilla.ResourceManager.GetResource<TextAsset>("sumbusHtmlPage");

		//var HTML = asset.text.Replace("$REPLACE_TOKEN", token);

		//var popup = it.Main.PopupController.Instance.ShowPopup<it.Popups.BrowserPopup>(PopupType.Browser);
		//popup.SetHtml(HTML);

	}

}
