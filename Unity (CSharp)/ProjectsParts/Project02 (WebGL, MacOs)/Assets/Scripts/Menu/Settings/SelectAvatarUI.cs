using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAvatarUI : SettingsPage
{
	[SerializeField] private Transform Content;
	[SerializeField] private SettingsAvatarButton _prefab;

	[SerializeField] private ScrollRect _scrollRect;

	private List<SettingsAvatarButton> _inctancesList = new List<SettingsAvatarButton>();
	[SerializeField] private Toggle[] toggles;

	private AvatarObject avatar;

	[SerializeField] private bool isWelcomeScreen = false;
	[SerializeField] private MobileWelcomAvatarChoice mobileWelcomAvatarChoice;
	[SerializeField] private WelcomeAvatarChoice welcomeAvatarChoice;
	private int _selectIndex = -1;

	private PoolList<SettingsAvatarButton> _poolList;



	public void Show()
	{
		OpenAvatarList();

		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserReferenceUpdate, UserReferenceUpdate);
	}
	private void UserReferenceUpdate(com.ootii.Messages.IMessage handle)
	{

		OpenAvatarList();
	}

	public void OpenAvatarList()
	{
		it.Logger.Log("open avatar list");
		if (UserController.ReferenceData == null) return;

		if (_poolList == null)
			_poolList = new PoolList<SettingsAvatarButton>(_prefab.gameObject, _scrollRect.content);
		_inctancesList.Clear();

		_poolList.HideAll();

		var categoryes = UserController.ReferenceData.avatar_categories;

		int index = -1;

		for (int c = 0; c < categoryes.Length; c++)
		{
			it.Logger.Log(c);
			var cat = categoryes[c];

			for (int a = 0; a < cat.avatars.Length; a++)
			{
				index++;
				int ind = index;
				var avatar = cat.avatars[a];

				var item = _poolList.GetItem();

				item.IsSelect = false;
				item.SetAvatar(avatar);

				item.OnClick = (ava) =>
				{
					if (_selectIndex == ind)
					{
						_selectIndex = -1;

					}
					else
					{
						_selectIndex = ind;

					}

					for (int i = 0; i < _inctancesList.Count; i++)
					{
						_inctancesList[i].IsSelect = i == _selectIndex;
					}
				};
				_inctancesList.Add(item);
			}
		}

		int rowCount = (int)Mathf.Ceil(_inctancesList.Count / 5f);

		var gl = _scrollRect.content.GetComponent<GridLayoutGroup>();

		float h = (rowCount * gl.cellSize.y) + ((rowCount - 1) * gl.spacing.y);

		_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, h);

	}

	public void SelectAvatar()
	{
		var prof = UserController.User.user_profile.PostProfile;
		prof.avatar_id = _inctancesList[_selectIndex].AvatarObject.id;
		UserController.Instance.UpdateProfile(prof);
	}
	public void SelectAvatar(AvatarObject avatarSelect)
	{
		if (avatarSelect == null) return;
		avatar = avatarSelect;

		//  SelectTest();
	}

	/* private void SelectTest()
	 {
			 if (avatar == null) return;
			 for (int i = 0; i < _inctancesList.Count; i++)
			 {
					 if (isWelcomeScreen)
					 {
//#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
//            mobileWelcomAvatarChoice.SetAvatartPreview(avatar.id, avatar.url);
//#else
							 welcomeAvatarChoice.SetAvatartPreview(avatar.Id, avatar.Url);
//#endif
					 }
					 _inctancesList[i].SetSelect(_inctancesList[i].avatar.Id == avatar.Id);
			 }
	 }*/


	public override void Apply()
	{
		if (!isWelcomeScreen)
		{
			userProfilePost.avatar_id = avatar.id;
			base.Apply();
		}
		else
		{
			ulong idAva = avatar == null ? 0 : avatar.id;
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			mobileWelcomAvatarChoice.ChooseAvatar(UserController.ReferenceData.GetAvatarObject(idAva));
#else
            welcomeAvatarChoice.ChooseAvatar(UserController.ReferenceData.GetAvatarObject(idAva));
#endif
		}
	}
}