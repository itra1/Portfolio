using System.Collections;
using UnityEngine;


public class SettingsController : Singleton<SettingsController>
{
	public const string PP_TIMEBANK = "settings.TimeBank";
	public const string PP_CHIP = "settings.Chip";

	private int _timeBank;
	private int _chip;

	public int TimeBank
	{
		get => _timeBank; 
		set
		{
			if (_timeBank == value)
				return;
			_timeBank = value;
			PlayerPrefs.SetInt(PP_TIMEBANK, _timeBank);
			it.Logger.Log("Settings set = " + _timeBank);
			PlayerPrefs.Save();
			EmitUpdate();
		}
	}
	public int Chip
	{
		get => _chip;
		set
		{
			if (_chip == value)
				return;
			_chip = value;
			PlayerPrefs.SetInt(PP_CHIP, _chip);
			it.Logger.Log("Chip set = " + _chip);
			PlayerPrefs.Save();
			EmitUpdate();
		}
	}

	private void EmitUpdate()
	{
		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.SettingsUpdate);

	}

	private void Awake()
	{
		_timeBank = PlayerPrefs.GetInt(PP_TIMEBANK, 2);
		_chip = PlayerPrefs.GetInt(PP_CHIP, 0);
	}

}