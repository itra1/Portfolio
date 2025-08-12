using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace it.UI.Settings
{
  public class AudioPage : PageBase
  {
	 [SerializeField]
	 private ScrollerItem _musicItem;
	 [SerializeField]
	 private ScrollerItem _effectsItem;

	 protected override void OnEnable()
	 {
		base.OnEnable();
		_musicItem.OnChangeEvent = SetMusic;
		GetMusic();
		_effectsItem.OnChangeEvent = SetEffects;
		GetEffects();
	 }

	 public void SetMusic(float value)
	 {
		it.Game.Managers.GameManager.Instance.GameSettings.AudioMusic = value;
	 }

	 public void GetMusic()
	 {
		_musicItem.SetValue(it.Game.Managers.GameManager.Instance.GameSettings.AudioMusic);
	 }

	 public void SetEffects(float value)
	 {
		it.Game.Managers.GameManager.Instance.GameSettings.AudioEffects = value;
	 }

	 public void GetEffects()
	 {
		_effectsItem.SetValue(it.Game.Managers.GameManager.Instance.GameSettings.AudioEffects);
	 }

  }
}