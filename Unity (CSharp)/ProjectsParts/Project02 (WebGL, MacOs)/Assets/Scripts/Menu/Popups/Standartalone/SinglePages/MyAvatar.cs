using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace it.Main.SinglePages
{
	public class MyAvatar : SinglePage
	{

		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private SettingsAvatarButton _prefab;
		[SerializeField] private it.UI.Elements.GraphicButtonUI _applyButton;

		private PoolList<SettingsAvatarButton> _poolList;
		private List<SettingsAvatarButton> _inctancesList = new List<SettingsAvatarButton>();
		private int _selectIndex = -1;

		protected override void EnableInit()
		{
			base.EnableInit();
			_applyButton.interactable = false;
			if (_poolList == null)
				_poolList = new PoolList<SettingsAvatarButton>(_prefab.gameObject, _scrollRect.content);
			_inctancesList.Clear();

			_poolList.HideAll();

			SpawnAvatars();
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserReferenceUpdate, UserReferenceUpdate);
		}

		protected void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserReferenceUpdate, UserReferenceUpdate);
		}

		private void UserReferenceUpdate(com.ootii.Messages.IMessage handle)
		{
			SpawnAvatars();
		}
		private void SpawnAvatars()
		{
			if (UserController.ReferenceData == null) return;

			_poolList.HideAll();
			var categoryes = UserController.ReferenceData.avatar_categories;

			int index = -1;

			for (int c = 0; c < categoryes.Length; c++)
			{
				var cat = categoryes[c];

				for (int a = 0; a < cat.avatars.Length; a++)
				{
					index++;
					int ind = index;
					var avatar = cat.avatars[a];

					var item = _poolList.GetItem();

					item.IsSelect = UserController.User.user_profile.avatar != null && UserController.User.user_profile.avatar.id == avatar.id;
					item.SetAvatar(avatar);

					item.OnClick = (ava) =>
					{
						//if (_selectIndex == ind)
						//{
						//_selectIndex = -1;
						//_applyButton.interactable = false;
						//}
						//else
						//{
						_selectIndex = ind;
						_applyButton.interactable = true;
						//}

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

			//_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, h);

		}
		public void CancelButtonTouch()
		{
			Hide();
		}

		public void ApplyButtonTouch()
		{
			var prof = UserController.User.user_profile.PostProfile;
			prof.avatar_id = _inctancesList[_selectIndex].AvatarObject.id;
			_applyButton.interactable = false;
			UserController.Instance.UpdateProfile(prof,(result)=> {
				Hide();
			}, (error)=> {
				Hide();
			});

		}


	}
}