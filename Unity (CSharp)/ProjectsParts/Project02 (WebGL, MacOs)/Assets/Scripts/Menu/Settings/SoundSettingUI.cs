using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingUI : SettingsPage
{
	[SerializeField] private Slider SliderBackground;
	[SerializeField] private Slider SliderDealer;
	[SerializeField] private Slider SliderEffect;
	[SerializeField] private Toggle soundOnToggle;
	[SerializeField] private Toggle soundOffToggle;
	[SerializeField] private Toggle muteCriticalToggle;
	private SoundGet sound;

	public void Show()
	{
		sound = GameHelper.UserProfile.sound.Clone();
		if (sound.on)
		{
			soundOnToggle.isOn = true;
		}
		else
		{
			soundOffToggle.isOn = true;
		}
		muteCriticalToggle.isOn = sound.mute_critical_alert;
		SliderBackground.value = sound.background / 100f;
		SliderDealer.value = sound.dealer_voice / 100f;
		SliderEffect.value = sound.sound_effect / 100f;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void SetVolumeBackground(float vl)
	{
		sound.background = (int)(vl * 100);
	}
	public void SetVolumeDealer(float vl)
	{
		sound.dealer_voice = (int)(vl * 100);
	}
	public void SetVolumeEffect(float vl)
	{
		sound.sound_effect = (int)(vl * 100);
	}

	public void SoundOnOff(bool on)
	{
		sound.on = on;
	}

	public void MuteCriticalOn(bool on)
	{
		sound.mute_critical_alert = on;
	}

	public override void Apply()
	{
		SoundManager.instance.SetVolumeBackground(sound.background / 100f);
		SoundManager.instance.SetVolumeDealer(sound.dealer_voice / 100f);
		SoundManager.instance.SetVolumeEffect(sound.sound_effect / 100f);
		SoundManager.instance.SoundOnOff(sound.on);
		SoundManager.instance.MuteCriticalOn(sound.mute_critical_alert);

		userProfilePost.sound = sound;
		base.Apply();
	}

}
