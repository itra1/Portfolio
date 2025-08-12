using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace it.Popups
{
	public class MyAvatarsPopup : PopupBase
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private SettingsAvatarButton _prefab;
		[SerializeField] private it.UI.Elements.GraphicButtonUI _applyButton;

		private bool _fromRegistration;
		private PoolList<SettingsAvatarButton> _poolList;
		private List<SettingsAvatarButton> _inctancesList = new List<SettingsAvatarButton>();
		private int _selectIndex = -1;
		public bool FromRegistration { get => _fromRegistration; set => _fromRegistration = value; }

		protected override void EnableInit()
		{
			_applyButton.interactable = false;

			SpawnAvatars();

			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserReferenceUpdate, UserReferenceUpdate);

		}

		protected override void OnDisable()
		{
			base.OnDisable();
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserReferenceUpdate, UserReferenceUpdate);
		}

		private void UserReferenceUpdate(com.ootii.Messages.IMessage handle)
		{

			SpawnAvatars();
		}

		private void SpawnAvatars()
		{
			it.Logger.Log("spawn avatars");

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
							_applyButton.interactable = false;
						}
						else
						{
							_selectIndex = ind;
							_applyButton.interactable = true;
						}

						for (int i = 0; i < _inctancesList.Count; i++)
						{
							_inctancesList[i].IsSelect = i == _selectIndex;
						}
					};
					_inctancesList.Add(item);
				}
			}

			float countinRow = 5;

#if UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
			countinRow = 3;
			#endif

			int rowCount = (int)Mathf.Ceil(_inctancesList.Count / countinRow);

			var gl = _scrollRect.content.GetComponent<GridLayoutGroup>();

			float h = (rowCount * gl.cellSize.y) + ((rowCount - 1) * gl.spacing.y) + gl.padding.top + gl.padding.bottom;

			_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, h);

		}

		public void SkipButton()
		{

			if (_fromRegistration)
			{
				it.Main.PopupController.Instance.ShowPopup(PopupType.Welcome);

				Hide();
			}

		}

		public void ApplyButton()
		{
			var prof = UserController.User.user_profile.PostProfile;
			prof.avatar_id = _inctancesList[_selectIndex].AvatarObject.id;
			UserController.Instance.UpdateProfile(prof);

			if (_fromRegistration)
			{
				it.Main.PopupController.Instance.ShowPopup(PopupType.Welcome);
			}

			Hide();
		}


		public void BackToAuth()
		{
			Hide();
			it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
		}

	}
}