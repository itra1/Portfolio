using System;
using System.Collections.Generic;
using UnityEngine;

namespace it.UI.Settings
{
  public class GamePage : PageBase
  {
	 [SerializeField]
	 private SwitchItem _localizeItem;

	 protected override void OnEnable()
	 {
		base.OnEnable();

		_localizeItem._selectEvent = SetLocalization;
		GetLocalization();

	 }

	 public void SetLocalization(int index)
	 {
		string code = "en";

		switch (index)
		{
		  case 1:
			 code = "ru";
			 break;
		  case 2:
			 code = "es";
			 break;
		  case 0:
		  default:
			 code = "en";
			 break;
		}

		it.Game.Managers.GameManager.Instance.GameSettings.GameLanguageCode = code;
	 }

	 public void GetLocalization()
	 {
		string localCode = it.Game.Managers.GameManager.Instance.GameSettings.GameLanguageCode;

		int index = 0;

		switch (localCode)
		{
		  case "ru":
			 index = 1;
			 break;
		  case "es":
			 index = 2;
			 break;
		  case "en":
		  default:
			 index = 0;
			 break;
		}

		_localizeItem.SetIndex(index);
	 }

  }
}