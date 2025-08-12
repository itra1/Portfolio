using System;
using UnityEngine;
using UnityEngine.UI;

public class RatingMap : PanelUi {

	Animator animComponent;
	public AudioClip openClip;
	public AudioClip closeClip;

	protected override void OnEnable() {
		animComponent = GetComponent<Animator>();
		AudioManager.PlayEffect(openClip, AudioMixerTypes.mapEffect);
		InitFb();
		ChangeLanuage();
	}

	public void CloseButton() {
		animComponent.SetTrigger("hidePanel");
		AudioManager.PlayEffect(closeClip, AudioMixerTypes.mapEffect);
		Invoke("CloseThis", 0.6f);
	}

	void CloseThis() {
		if (OnClose != null) OnClose();
		gameObject.SetActive(false);
	}


	#region Facebook

	void InitFb() {
		FacebookAuth();
	}

	public void FBLogin() {
#if PLUGIN_FACEBOOK
		UiController.ClickButtonAudio();
		FBController.instance.FBlogin(FacebookAuth);
#endif
	}

	// калбак авторизации

	void FacebookAuth() {
		if (FBController.CheckFbLogin)
			GetComponent<Animator>().SetBool("fbAuth", true);
		else
			GetComponent<Animator>().SetBool("fbAuth", false);
	}


	public void FbInvate() {

		UiController.ClickButtonAudio();
#if PLUGIN_FACEBOOK
		FBController.instance.InvateFriendFb();
#endif
	}

	public void ShareResult() {
#if PLUGIN_FACEBOOK
		FBController.ShareResult();
#endif
	}

	#endregion


	#region Lanuage

	public Text contentTitleRating;
	public Text friendsInfoText;
	public Text loginFb;


	void ChangeLanuage() {
		contentTitleRating.text = LanguageManager.GetTranslate("stat_Rating");
		friendsInfoText.text = LanguageManager.GetTranslate("stat_FriendsInfo");
		loginFb.text = LanguageManager.GetTranslate("stat_LoginFb");
	}

	#endregion

	public override void BackButton() {
		CloseButton();
	}

}
