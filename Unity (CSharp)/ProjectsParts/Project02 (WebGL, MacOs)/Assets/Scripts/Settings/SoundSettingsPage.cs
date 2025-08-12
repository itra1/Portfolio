using System.Collections;

using UnityEngine;
using UnityEngine.UI;


public class SoundSettingsPage : MonoBehaviour
{
	[SerializeField] private it.UI.Elements.GraphicButtonUI _appluButton;
	[SerializeField] private Slider _bpmSlider;
	[SerializeField] private Slider _backgroundSlider;
	[SerializeField] private Slider _soundEffectSlider;
	[SerializeField] private it.UI.Elements.MoveToggle _onOffSoundToggle;

	private UserProfilePost _pp;
	private bool _isOn;
	private int _effectsValue;
	private int _backgroundValue;

	private void OnEnable()
	{
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.SettingsUpdate, SettingsUpdateEvent);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, SettingsUpdateEvent);

		//_userProfile = UserController.Instance.User.UserProfile;
		if (UserController.IsLogin)
		{
			_pp = UserController.User.user_profile.PostProfile;
			_isOn = _pp.sound.on;
			_effectsValue = _pp.sound.sound_effect;
			_backgroundValue = _pp.sound.background;
		}
		else
		{
			_isOn = AudioController.Instance.IsOn;
			_effectsValue = AudioController.Instance.EffectsValue;
			_backgroundValue = AudioController.Instance.BackgroundValue;
		}

		ConfirmValue();
		Subscribe();
		if (_appluButton != null)
			_appluButton.interactable = false;
	}

	private void OnDisable()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.SettingsUpdate, SettingsUpdateEvent);
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, SettingsUpdateEvent);

		AudioController.Instance.SoundChange(_isOn);
		AudioController.Instance.ChangeBackground(_effectsValue);
		AudioController.Instance.ChangeEffects(_backgroundValue);
#if !UNITY_STANDALONE
		AppleButton();
#endif
	}

	private void Subscribe()
	{

		_onOffSoundToggle.OnChangeValue.RemoveAllListeners();
		_onOffSoundToggle.OnChangeValue.AddListener((val) =>
		{
			_pp.sound.on = val;
			AudioController.Instance.SoundChange(val);
			ValueChange();

		});

		if (_bpmSlider != null)
		{
			_bpmSlider.onValueChanged.RemoveAllListeners();
			_bpmSlider.onValueChanged.AddListener((val) =>
			{
				_pp.sound.dealer_voice = (int)val;
				//AudioController.Instance.SoundChange(val);
				ValueChange();
			});
		}
		_backgroundSlider.onValueChanged.RemoveAllListeners();
		_backgroundSlider.onValueChanged.AddListener((val) =>
		{
			_pp.sound.background = (int)val;
			AudioController.Instance.ChangeBackground((int)val);
			ValueChange();
		});
		_soundEffectSlider.onValueChanged.RemoveAllListeners();
		_soundEffectSlider.onValueChanged.AddListener((val) =>
		{
			_pp.sound.sound_effect = (int)val;
			AudioController.Instance.ChangeEffects((int)val);
			ValueChange();
			//DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_BUTTON_CLICK, 1);
		});


	}

	private void SettingsUpdateEvent(com.ootii.Messages.IMessage handle)
	{
		if (UserController.IsLogin)
			_pp = UserController.User.user_profile.PostProfile;
		_isOn = _pp.sound.on;
		_effectsValue = _pp.sound.sound_effect;
		_backgroundValue = _pp.sound.background;
	}

	private void ConfirmValue()
	{
		_onOffSoundToggle.IsOn = _pp.sound.on;

		if (_bpmSlider != null)
		{
			_bpmSlider.minValue = 70;
			_bpmSlider.maxValue = 120f;
			_bpmSlider.value = _pp.sound.dealer_voice;
		}
		_backgroundSlider.minValue = 0;
		_backgroundSlider.maxValue = 100f;
		_backgroundSlider.value = _pp.sound.background;

		_soundEffectSlider.minValue = 0;
		_soundEffectSlider.maxValue = 100f;
		_soundEffectSlider.value = _pp.sound.sound_effect;
	}

	private void ValueChange()
	{
		it.Logger.Log("value changed");
		if (_appluButton != null)
			_appluButton.interactable = true;
	}

	public void AppleButton()
	{
		_isOn = _pp.sound.on;
		_effectsValue = _pp.sound.sound_effect;
		_backgroundValue = _pp.sound.background;
		AudioController.Instance.Save();

		if (!UserController.IsLogin) return;
		UserController.Instance.UpdateProfile(_pp);

	}



}