using System.Collections;
using UnityEngine;
using TMPro;
using it.Main;

namespace it.UI.Profiles
{
	public class UserProfileMain : MonoBehaviour
	{
		[SerializeField] private Avatar _avatar;
		[SerializeField] private GameObject _nicknameEditButtpn;
		[SerializeField] private TextMeshProUGUI _regionLabel;
		[SerializeField] private TextMeshProUGUI _nicknameLabel;
		[SerializeField] private TextMeshProUGUI _emailLabel;
		[SerializeField] private TextMeshProUGUI _phoneLabel;
		[SerializeField] private GameObject _editPhoneButton;

		public void OnEnable()
		{
			if (_editPhoneButton != null)
				_editPhoneButton.SetActive(true);

			ConfirmValue();
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
		}

		private void UserProfileUpdate(com.ootii.Messages.IMessage handle)
		{
			ConfirmValue();
		}

		// Реакция на событие редактирования аватара
		public void AvatarEditTouck()
		{
			it.Main.SinglePageController.Instance.Show(SinglePagesType.MyAvatar);
		}

		public void EditPhoneButtonTouch()
		{
			PopupController.Instance.ShowPopup(PopupType.PhoneEdit);

		}

		public void EditNicknameButtonTouch()
		{
			PopupController.Instance.ShowPopup(PopupType.NicknameEdit);

		}

		private void ConfirmValue()
		{
			_nicknameEditButtpn.gameObject.SetActive(UserController.User.can_change_nickname);
			_regionLabel.text = UserController.User.country.title;
			_nicknameLabel.text = UserController.User.nickname;
			_emailLabel.text = UserController.User.email;
			_phoneLabel.text = UserController.User.phone;
			_avatar.SetAvatar(UserController.User.AvatarUrl);
			_phoneLabel.enabled = true;
		}

	}
}