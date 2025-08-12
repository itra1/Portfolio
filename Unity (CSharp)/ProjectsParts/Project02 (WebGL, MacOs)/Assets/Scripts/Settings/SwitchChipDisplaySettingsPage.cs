using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SwitchChipDisplaySettingsPage : MonoBehaviour
{
	[SerializeField] private it.UI.Elements.GraphicButtonUI _applyButton;
	[SerializeField] private Color _chipColor;
	[SerializeField] private Color _bbColor;
	[SerializeField] private TextMeshProUGUI _textLabel;
	[SerializeField] private PropertyStruct[] _propertyes;

	private int _chipValue;

	[System.Serializable]
	public struct PropertyStruct
	{
		public Toggle Toggle;
	}

	private void OnEnable()
	{

		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.SettingsUpdate, SettingsUpdateEvent);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, SettingsUpdateEvent);

		_chipValue = SettingsController.Instance.Chip;
		CheckChange();
		ConfirmChange();

		for (int i = 0; i < _propertyes.Length; i++)
		{
			int index = i;
			_propertyes[index].Toggle.onValueChanged.RemoveAllListeners();
			_propertyes[index].Toggle.onValueChanged.AddListener((val) =>
			{
				if (!val) return;
				_chipValue = index;
				ConfirmChange();

			});
		}

	}
	private void OnDisable()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.SettingsUpdate, SettingsUpdateEvent);
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, SettingsUpdateEvent);
#if !UNITY_STANDALONE
		AppleButtonTouch();
		#endif
	}
	private void SettingsUpdateEvent(com.ootii.Messages.IMessage handle)
	{
		_chipValue = SettingsController.Instance.Chip;
		CheckChange();
	}
	private void CheckChange()
	{
		if (_chipValue != SettingsController.Instance.Chip)
		{
			_applyButton.interactable = true;
			return;
		}
		_applyButton.interactable = false;
	}

	private void ConfirmChange()
	{

		for (int j = 0; j < _propertyes.Length; j++)
		{
			_propertyes[j].Toggle.isOn = _chipValue == j;
		}

		_textLabel.color = _chipValue == 0 ? _bbColor : _chipColor;
		_textLabel.text = _chipValue == 0 ? "25 BB" : "1000";
		CheckChange();
	}

	public void AppleButtonTouch()
	{
		SettingsController.Instance.Chip = _chipValue;
	}


}
