using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TimeBankSettingsPage : MonoBehaviour
{
	[SerializeField] private it.UI.Elements.GraphicButtonUI _applyButton;
	[SerializeField] private PropertyStruct[] _propertyes;

	private string _keySlug;

	[System.Serializable]
	public struct PropertyStruct{
		public Toggle Toggle;
		public string Slug;
	}

	private void OnEnable()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, UserLogin);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, UserLogin);

		if (UserController.IsLogin){
			_keySlug = UserController.User.user_profile.time_bank_type.slug;
		}
		
		CheckChange();
		ConfirmChange();
		if (_applyButton != null)
			_applyButton.interactable = false;

		for (int i = 0; i < _propertyes.Length;i++){
			int index = i;
			_propertyes[index].Toggle.onValueChanged.RemoveAllListeners();
			_propertyes[index].Toggle.onValueChanged.AddListener((val) =>
			{
				if (!val) return;
				_keySlug = _propertyes[index].Slug;
				ConfirmChange();
				_applyButton.interactable = true;

			});
		}

		if(!string.IsNullOrEmpty(_keySlug)){
			_keySlug = _propertyes[0].Slug;
		}

	}

	private void ConfirmChange(){

		for (int j = 0; j < _propertyes.Length; j++)
		{
			_propertyes[j].Toggle.isOn = _propertyes[j].Slug == _keySlug;
		}
		CheckChange();
	}

	private void OnDisable()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, UserLogin);
		#if !UNITY_STANDALONE
		ApplyButton();
		#endif
	}

	private void SettingsUpdateEvent(com.ootii.Messages.IMessage handle)
	{
		CheckChange();
	}
	private void UserLogin(com.ootii.Messages.IMessage handle)
	{
		if (!UserController.IsLogin) return;

		_keySlug = UserController.User.user_profile.time_bank_type.slug;

		ConfirmChange();
	}

	private void CheckChange()
	{
		if (UserController.IsLogin && _keySlug != UserController.User.user_profile.time_bank_type.slug)
		{
		if(_applyButton != null)
			_applyButton.interactable = true;
			return;
		}
		if (_applyButton != null)
			_applyButton.interactable = false;
	}


	public void ApplyButton()
	{
		if (UserController.User == null) return;

		var prof = UserController.User.user_profile.PostProfile;
		var lib = UserController.ReferenceData.time_bank_types.Find(x => x.slug == _keySlug);
		prof.time_bank_type_id = lib.id;

		UserController.Instance.UpdateProfile(prof,(prof)=>{

			if (_applyButton != null)
				_applyButton.interactable = false;
		});
	}

}
