using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Leguar.TotalJSON;
using UnityEngine.Audio;

namespace it.Game.Managers
{
  /// <summary>
  /// Настройки игры
  /// </summary>
  public class GameSettings : MonoBehaviourBase
  {
	 [SerializeField]
	 private AudioMixer _musicMixer;
	 [SerializeField]
	 private AudioMixer _effectsMixer;

	 private int _videoQualityLevel;
	 public int VideoQualityLevel
	 {
		get => _videoQualityLevel;
		set
		{
		  if (_videoQualityLevel == value)
			 return;
		  _videoQualityLevel = value;
		  ConfirmSettings();
		}
	 }

	 private int _videoTargetFps;
	 public int VideoTargetFps
	 {
		get => _videoTargetFps;
		set
		{
		  if (_videoTargetFps == value)
			 return;
		  _videoTargetFps = value;
		  ConfirmSettings();
		}
	 }

	 private string _gameLanguageCode;
	 public string GameLanguageCode
	 {
		get => _gameLanguageCode;
		set
		{
		  if (_gameLanguageCode == value)
			 return;
		  _gameLanguageCode = value;
		  ConfirmSettings();
		}
	 }

	 private float _audioMusic;
	 public float AudioMusic
	 {
		get => _audioMusic;
		set
		{
		  if (_audioMusic == value)
			 return;
		  _audioMusic = value;
		  ConfirmAudio();
		}
	 }

	 private float _audioEffects;
	 public float AudioEffects
	 {
		get => _audioEffects;
		set
		{
		  if (_audioEffects == value)
			 return;
		  _audioEffects = value;
		  ConfirmAudio();
		}
	 }

	 private void Awake()
	 {
		Load();
		ConfirmSettings();
	 }

	 private void Load()
	 {
		if (!PlayerPrefs.HasKey("Settings"))
		{
		  LoadDefauldData();
		  Save();
		}


		string saveDataString = PlayerPrefs.GetString("Settings");
		JSON saveData = JSON.ParseString(saveDataString);

		if (saveData.ContainsKey("video_qualityLevel"))
		  _videoQualityLevel = saveData.GetInt("video_qualityLevel");
		else
		  _videoQualityLevel = 2;

		if (saveData.ContainsKey("video_targetFps"))
		  _videoTargetFps = saveData.GetInt("video_targetFps");
		else
		  _videoTargetFps = 0;

		if (saveData.ContainsKey("game_languageCode"))
		  _gameLanguageCode = saveData.GetString("game_languageCode");
		else
		  _gameLanguageCode = "en";

		if (saveData.ContainsKey("audio_music"))
		  _audioMusic = saveData.GetFloat("audio_music");
		else
		  _audioMusic = 1;

		if (saveData.ContainsKey("audio_effects"))
		  _audioEffects = saveData.GetFloat("audio_effects");
		else
		  _audioEffects = 1;

		ConfirmSettings();
		InvokeEndFrame(ConfirmAudio);
	 }

	 private void Save()
	 {
		JSON saveData = new JSON();
		saveData.Add("video_qualityLevel", VideoQualityLevel);
		saveData.Add("video_targetFps", VideoTargetFps);
		saveData.Add("game_languageCode", GameLanguageCode);
		saveData.Add("audio_music", AudioMusic);
		saveData.Add("audio_effects", AudioEffects);

		string saveString = saveData.CreateString();
		PlayerPrefs.SetString("Settings", saveString);
		PlayerPrefs.Save();
	 }

	 private void ConfirmSettings()
	 {
		QualitySettings.SetQualityLevel(_videoQualityLevel);
		Application.targetFrameRate = _videoTargetFps;
		I2.Loc.LocalizationManager.CurrentLanguageCode = GameLanguageCode;
		ConfirmAudio();
		Save();
	 }

	 private void ConfirmAudio()
	 {
		_musicMixer.SetFloat("musicVal", -80 + (_audioMusic * 80));
		_effectsMixer.SetFloat("effectsVal", -80 + (_audioEffects * 80));
	 }

	 private void LoadDefauldData()
	 {
		_videoQualityLevel = 2;
		_videoTargetFps = 0;
		_gameLanguageCode = "en";
		_audioMusic = 1;
		_audioEffects = 1;
	 }

	 /// <summary>
	 /// Открытие панели с настройками
	 /// </summary>
	 public void OpenSettingsPanel(UnityEngine.Events.UnityAction onClose)
	 {
		var panel = UiManager.GetPanel<it.UI.Settings.Settings>();
		panel.gameObject.SetActive(true);

		panel.onDisable = () =>
		{
		  ConfirmSettings();
		  Save();
		  onClose?.Invoke();
		};

		panel.onOkButtonClick = () =>
		{
		  ConfirmSettings();
		  panel.gameObject.SetActive(false);
		};
		panel.onCancelButtonClick = () =>
		{
		  Load();
		  ConfirmSettings();
		  panel.gameObject.SetActive(false);
		};

		//panel.onQualityLevelChange = (level) =>
		//{
		//  QualityLevel = level;
		//  ConfirmSettings();
		//};

		//panel.onFpsChange = (fps) =>
		//{
		//  TargetFps = fps;
		//  ConfirmSettings();
		//};
	 }

  }

}