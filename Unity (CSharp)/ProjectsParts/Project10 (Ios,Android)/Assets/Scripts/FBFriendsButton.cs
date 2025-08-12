using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

public class FBFriendsButton : MonoBehaviour {

	public System.Action OnLoginFb;
	public System.Action OnFacebook;

	public GameObject loginButton;
	public GameObject invateButton;

	private void OnEnable() {
		Apikbs.OnLogin += FacebookAuth;

		loginButton.gameObject.SetActive(!Apikbs.isLogin);
		invateButton.gameObject.SetActive(Apikbs.isLogin);

	}

	private void OnDisable() {
		Apikbs.OnLogin -= FacebookAuth;
	}

	void InitFb() {
		if (OnLoginFb != null) OnLoginFb();
		GetComponent<Animator>().SetTrigger("forsLogin");
		if (FBController.CheckFbLogin) {
			GetComponent<Animator>().SetBool("login", true);
		}
	}

	public void FBLogin() {
#if PLUGIN_FACEBOOK
		UiController.ClickButtonAudio();
		FBController.fbLogin(FacebookAuth);
#endif
	}

	// калбак авторизации
	void FacebookAuth() {
		GetComponent<Animation>().Play("auth");
	}

	public void InviteFriends() {
#if PLUGIN_FACEBOOK
			FBController.instance.InvateFriendFb();
#endif
	}

	public void AnumMiddle() {

		loginButton.gameObject.SetActive(true);
		invateButton.gameObject.SetActive(true);
	}

}
