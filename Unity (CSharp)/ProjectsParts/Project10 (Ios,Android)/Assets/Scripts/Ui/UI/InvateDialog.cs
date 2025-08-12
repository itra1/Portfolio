using System;
using UnityEngine;
using UnityEngine.UI;
#if PLUGIN_FACEBOOK
using Facebook.Unity;
#endif
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Dialoglist {
	public string idFb;
	public string name;
	public string url;
	public int type;
	public int coinsCount;
}

public class InvateDialog : MonoBehaviour {

	public Action OnClose;

	public GameObject userPanel;
	public GameObject coinsPanel;

	public Color greenColor;
	public Color yelowColor;
	public string inGameText;
	public string dubleText;
	public Image roundImage;
	public Text acceptBtn;

	public Text nameUser;
	public Text statusUser;
	public Text textCoins;

	public void Close() {
		UiController.ClickButtonAudio();
		GetComponent<Animator>().SetTrigger("close");
	}

	public void CloseEvent() {
		if (OnClose != null) OnClose();
	}

	public void Show(Dialoglist dial) {
#if PLUGIN_FACEBOOK
		HideAll();

		acceptBtn.text = LanguageManager.GetTranslate("inv_Accept");

		if (dial.type == 2) {
			userPanel.SetActive(true);
			FBController.FBGetFriendName(dial.idFb, ParseName);
			statusUser.text = LanguageManager.GetTranslate(inGameText);
			statusUser.color = yelowColor;
			FBController.FBGetFriendAvatar(dial.idFb, GetAvatar);
		} else if (dial.type == 1) {
			userPanel.SetActive(true);
			FBController.FBGetFriendName(dial.idFb, ParseName);
			statusUser.text = LanguageManager.GetTranslate(dubleText);
			statusUser.color = greenColor;
			FBController.FBGetFriendAvatar(dial.idFb, GetAvatar);
		} else if (dial.type == 0) {
			coinsPanel.SetActive(true);
			textCoins.text = dial.name;
		}
#endif
	}

#if PLUGIN_FACEBOOK
	public void ParseName(IGraphResult result) {
		Dictionary<string, object> dict = (Dictionary<string, object>)result.ResultDictionary;
		nameUser.text = dict["name"].ToString();
	}
	
	void GetAvatar(IGraphResult pict) {
		Debug.Log(pict.RawResult);
		Dictionary<string, object> dict = (Dictionary<string, object>)((Dictionary<string, object>)pict.ResultDictionary)["data"];
		StartCoroutine(DownloadUserAvatar(dict["url"].ToString()));
	}
#endif

	IEnumerator DownloadUserAvatar(string avaUrl) {
		Debug.Log(avaUrl);
		WWW www = new WWW(avaUrl);
		yield return www;
		roundImage.GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, 40, 40), new Vector2(0, 0));
	}

	void HideAll() {
		roundImage.GetComponent<Image>().sprite = null;
		userPanel.SetActive(false);
		coinsPanel.SetActive(false);
	}

	public void ConfirmCoins() {
		UiController.ClickButtonAudio();
		Close();
	}

}
