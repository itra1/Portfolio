using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using it.Network.Rest;

public class SettingsPage : MonoBehaviour
{
	protected UserProfilePost userProfilePost;
	protected UserProfilePost userProfilePostStart;

	protected virtual void OnEnable()
	{
		userProfilePostStart = GameHelper.UserProfile.PostProfile;

	}
	public virtual void Cancel()
	{
		GameHelper.PostUserProfile(userProfilePostStart, SettingsUI.SetProfileInfo);
	}

	public virtual void Apply()
	{
		GameHelper.PostUserProfile(userProfilePost, SettingsUI.SetProfileInfo);
	}
}

public class SettingsUI : MonoBehaviour
{
	[SerializeField] private Transform Content;
	[SerializeField] private List<SettingsPage> _pagesList;

	private GameObject PrefabNow;

	public static SettingsUI instance;

	public void Awake()
	{
		instance = this;
	}

	public void Init()
	{

		instance = this;
		//#if !UNITY_ANDROID && !UNITY_WEBGL || UNITY_IOS
		//		SelectAvatarShow(true);
		//#endif
		SetProfileInfo(GameHelper.UserProfile);
	}

	public void Show()
	{
		if (gameObject.activeInHierarchy)
			return;

		instance = this;
		SetProfileInfo(GameHelper.UserProfile);
		//#if !UNITY_ANDROID && !UNITY_WEBGL || UNITY_IOS
		//		SelectAvatarShow(true);
		//#endif

		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public static void SetProfileInfo(UserProfile profile)
	{
		it.Logger.Log("Profile Info applyed");
		//userProfile = profile;
		//userProfilePost = profile.PostProfile;

		ProfilePage[] uIProfiles = FindObjectsOfType<ProfilePage>();
		for (int i = 0; i < uIProfiles.Length; i++)
		{
			uIProfiles[i].Init();
		}
	}


}
