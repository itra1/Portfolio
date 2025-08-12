using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
public class MobileWelcomAvatarChoice : MonoBehaviour
{
	public TextMeshProUGUI Name;
	public TextMeshProUGUI Contry;
	public Image Avatar;
	public Image Flag;

	[SerializeField] private AvatarSelectButtonUI currentAvatar;
	void Start()
	{
		if (GameHelper.UserProfile.avatar_url == null || GameHelper.UserProfile.avatar_url.Length > 0)
		{
			gameObject.SetActive(false);
			return;
		}
		var userInfo = GameHelper.UserInfo;
		var userProfile = GameHelper.UserProfile;
		Name.text = userInfo.nickname;

		SetCountryText((int)userInfo.country.id);
		StartCoroutine(SetFlagImage((int)userInfo.country.id));
		//if (userInfo.avatar_url != null && userInfo.avatar_url.Length > 0) StartCoroutine(SetImage(userInfo.avatar_url));
		Avatar.sprite = SpriteManager.instance.avatarDefault;
		if (userProfile != null)
		{
			if (userProfile.avatar_url == null || userProfile.avatar_url.Length <= 0)
			{
				userProfile.SetAvatar(UserController.ReferenceData.avatar_categories[0].avatars[0]);
				GameHelper.UserProfile.SetAvatar(UserController.ReferenceData.avatar_categories[0].avatars[0]);
			}
			StartCoroutine(SetImage(userProfile.avatar_url));
		}
	}
	IEnumerator SetImage(string url)
	{
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
		yield return request.SendWebRequest();
		if (request.result != UnityWebRequest.Result.Success)
			it.Logger.Log(request.error);
		else
		{
			Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
			Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
			Avatar.overrideSprite = sprite;
		}
	}

	IEnumerator SetFlagImage(int flagid)
	{
		if (flagid == 4)
		{
			//Flag.sprite = Resources.Load<Sprite>("CountryFlags/RU");
			Flag.sprite = (Sprite)Garilla.ResourceManager.GetResource<Sprite>("CountryFlags/RU");
		}
		else
		{
			Flag.gameObject.SetActive(false);
		}
		yield return null;
	}


	void SetCountryText(int gp_country_id)
	{
		if (gp_country_id == 4)
		{
			Contry.text = "Россия";
		}
	}

	UserProfilePost userProfilePost;
	public void ChooseAvatar(AvatarObject avat)
	{
		var userProfile = GameHelper.UserProfile;

		userProfile.SetAvatar(avat);
		userProfilePost = userProfile.PostProfile;
		GameHelper.UserProfile.SetAvatar(avat);
		GameHelper.PostUserProfile(userProfilePost, SettingsUI.SetProfileInfo);
	}
	public void SetAvatartPreview(int avatarId, string avatarUri)
	{
		StartCoroutine(SetImage(avatarUri));
	}
}
