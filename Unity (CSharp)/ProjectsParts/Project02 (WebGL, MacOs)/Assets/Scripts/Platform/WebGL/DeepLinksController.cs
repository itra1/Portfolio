using System.Collections;

using UnityEngine;


public class DeepLinksController : MonoBehaviour
{
	public static DeepLinksController Instance { get; private set; }
	public string DeeplinkURL;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Application.deepLinkActivated += OnDeepLinkActivated;
			if (!string.IsNullOrEmpty(Application.absoluteURL))
			{
				// Cold start and Application.absoluteURL not null so process Deep Link.
				OnDeepLinkActivated(Application.absoluteURL);
			}
			// Initialize DeepLink Manager global variable.
			else DeeplinkURL = "";
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
		OnDeepLinkActivated(Application.absoluteURL);
		Debug.Log(Application.absoluteURL);
		//it.Logger.Log($"[DeepLinksController] url {Application.absoluteURL}");
	}
	private void OnDeepLinkActivated(string url)
	{
		// Update DeepLink Manager global variable, so URL can be accessed from anywhere.
		DeeplinkURL = url;
		if (url.Contains("https://dev."))
			AppConfig.IsDevApp = true;

		// Decode the URL to determine action. 
		// In this example, the app expects a link formatted like this:
		// unitydl://mylink?scene1
		string subUrl = url.Split("?"[0])[1];
		//it.Logger.Log($"[DeepLinksController] subUrl {subUrl}");
		var properties = subUrl.Split('&');

		foreach (var prop in properties)
		{
			if (prop.Contains("ref"))
			{
				string refVal = prop.Split("=")[1];
				//it.Logger.Log($"[DeepLinksController] ref {refVal}");
				PlayerPrefs.SetString("promo", refVal);
			}
			if (prop == "log")
			{
				AppConfig.IsLog = true;
			}
			if (prop == "disableAudio")
			{
				AppConfig.DisableAudio = true;
			}
			if (prop == "devServer")
			{
				AppConfig.DevServer = true;
			}
			if (prop.Contains("serverApi"))
			{
				AppConfig.CustomServer = prop.Split("=")[1];
			}
			if (prop.Contains("serverWS"))
			{
				AppConfig.CustomServerWS = prop.Split("=")[1];
			}
		}

	}
}