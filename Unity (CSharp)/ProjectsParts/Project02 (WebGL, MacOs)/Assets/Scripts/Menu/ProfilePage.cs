using Garilla;

using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfilePage : MonoBehaviour, MobilePageBase
{
	[Space]
	[SerializeField] private TextMeshProUGUI _nickname;
	[SerializeField] private it.Inputs.CurrencyLabel _balance;
	[SerializeField] private it.UI.Avatar _avatar;
	[SerializeField] private ScrollRect _scroll;
	[SerializeField] private RectTransform _validationBlock;


	private void Awake()
	{

#if UNITY_IOS

		if (_scroll != null && _validationBlock != null && _validationBlock.gameObject.activeSelf)
		{
			_validationBlock.gameObject.SetActive(false);
			_scroll.content.sizeDelta = new Vector2(_scroll.content.sizeDelta.x, _scroll.content.sizeDelta.y - _validationBlock.rect.height);
		}

#endif
	}

	private void Start()
	{
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, UserAuth);
		FillData();


	}

	private void OnDestroy()
	{
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, UserAuth);

	}

	private void UserAuth(com.ootii.Messages.IMessage handle)
	{
		FillData();
	}

	private void FillData()
	{
		if (UserController.User == null)
		{
			_avatar.SetDefaultAvatar();
			_nickname.text = "User";
			if (_balance)
				_balance.SetValue(0);
			return;
		}

		_avatar.SetAvatar(UserController.User.AvatarUrl);
		_nickname.text = UserController.User.nickname;
		if (_balance)
			_balance.SetValue((float)UserController.User.user_wallet.amount);
	}


	public void Init()
	{
		//#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		//		//        var userInfo = GameController.UserInfo;
		//		//        Amount.text = userInfo.user_wallet.amount.ToString();
		//#endif
		//#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_IOS
		//        var userInfo = GameController.UserInfo;
		//        var userProfile = GameController.UserProfile;
		//		_nickname.text = userInfo.Nickname;
		//        Amount.text = userInfo.UserWallet.Amount.ToString();

		//        Avatar.sprite = SpriteManager.instance.avatarDefault;
		//        if (userProfile != null)
		//        {
		//            if(userProfile.AvatarUrl == null || userProfile.AvatarUrl.Length <= 0)
		//            {
		//                userProfile.SetAvatar(GameController.Reference.AvatarCategories[0].Avatars[0]);
		//                GameController.UserProfile.SetAvatar(GameController.Reference.AvatarCategories[0].Avatars[0]);
		//            }
		//            StartCoroutine(SetImage(userProfile.AvatarUrl));
		//        }
		//#endif
	}

	//IEnumerator SetImage(string url)
	//{
	//	UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
	//	yield return request.SendWebRequest();
	//	if (request.result != UnityWebRequest.Result.Success)
	//		it.Logger.Log(request.error);
	//	else
	//	{
	//		Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
	//		Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
	//		Avatar.overrideSprite = sprite;
	//	}
	//}

	public void LogoutButtonTouch()
	{
		UserController.Instance.Logout();
	}

	public void NetworkButtonTouch()
	{
		it.Main.PopupController.Instance.ShowPopup(PopupType.Network);
	}

	public void SupportButtonTouch()
	{
		LinkManager.OpenUrl("liveChat");
	}

}
