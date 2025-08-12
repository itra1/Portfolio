using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchChipSettingUI : SettingsPage
{
	[SerializeField] private Toggle onToggle;
	[SerializeField] private Toggle offToggle;

	[Space]
	[SerializeField] private Toggle bbToggle;
	[SerializeField] private Toggle usdToggle;

	[Space]
	[SerializeField] private Toggle alternativeToggle;

	private SwitchChipDisplay chipDisplay;

	public void Show()
	{
		chipDisplay = GameHelper.UserProfile.switch_chip_display;
		//chipDisplay = new SwitchChipDisplay(chipDisplay.on, chipDisplay.chip_display, chipDisplay.alternative_chip);
		chipDisplay = new SwitchChipDisplay() { on = chipDisplay.on, chip_display = chipDisplay.chip_display, alternative_chip = chipDisplay.alternative_chip };

		if (chipDisplay.on) onToggle.isOn = true;
		else offToggle.isOn = true;

		if (chipDisplay.chip_display == "bb") bbToggle.isOn = true;
		else usdToggle.isOn = true;

		alternativeToggle.isOn = chipDisplay.alternative_chip;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void SetOn(bool on)
	{
		chipDisplay.on = on;
	}

	public void SetChipDisplay(string chip)
	{
		chipDisplay.chip_display = chip;
	}

	public void SetAlternative(bool bl)
	{
		chipDisplay.alternative_chip = bl;
	}

	public override void Apply()
	{
		userProfilePost.switch_chip_display = chipDisplay;
		base.Apply();
	}
}
