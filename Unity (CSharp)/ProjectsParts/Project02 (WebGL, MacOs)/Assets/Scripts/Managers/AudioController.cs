using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


public class AudioController : Singleton<AudioController>
{
	[SerializeField] private AudioMixer _baseMixer;
	[SerializeField] private AudioMixer _effectsMixer;
	[SerializeField] private AudioMixer _backgroundsMixer;
	[SerializeField] private AudioMixerSnapshot _baseOn;
	[SerializeField] private AudioMixerSnapshot _baseOff;

	private bool _isOn;
	private int _effectsValue;
	private int _backgroundValue;

	public bool IsOn { get => _isOn; set => _isOn = value; }
	public int EffectsValue { get => _effectsValue; set => _effectsValue = value; }
	public int BackgroundValue { get => _backgroundValue; set => _backgroundValue = value; }

	private void OnEnable()
	{
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
		Load();
	}

	private void OnDisable()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
	}

	private void UserProfileUpdate(com.ootii.Messages.IMessage handle)
	{
		ConfirmProfile();
	}

	public void Save(){
		PlayerPrefs.SetInt("soundOn", _isOn? 1 : 0);
		PlayerPrefs.SetInt("soundEffects", _effectsValue);
		PlayerPrefs.SetInt("soundBackground", _backgroundValue);
	}

	public void Load(){
		_isOn = PlayerPrefs.GetInt("soundOn", 1) == 1;
		_effectsValue = PlayerPrefs.GetInt("soundEffects", 100);
		_backgroundValue = PlayerPrefs.GetInt("soundBackground",100);
		SoundChange(_isOn);
		ChangeEffects(_effectsValue);
		ChangeBackground(_backgroundValue);
	}

	public void ConfirmProfile(){
		var soundValue = UserController.User.user_profile.sound;
		_isOn = soundValue.on;
		_effectsValue = soundValue.sound_effect;
		_backgroundValue = soundValue.background;
		SoundChange(_isOn);
		ChangeEffects(_effectsValue);
		ChangeBackground(_backgroundValue);
		Save();
	}

	public void SoundChange(bool isActive){
		if (isActive)
			_baseOn.TransitionTo(0.1f);
		else
			_baseOff.TransitionTo(0.1f);
	}

	public void ChangeEffects(int value){
		_effectsMixer.SetFloat("ValumeBase", -80 + 0.8f*value);
	}

	public void ChangeBackground(int value)
	{
		_backgroundsMixer.SetFloat("ValumeBase", -80 + 0.8f * value);
	}


}