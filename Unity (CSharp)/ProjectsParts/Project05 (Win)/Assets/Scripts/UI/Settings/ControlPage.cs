using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Game.Managers;
using UnityEngine.UIElements;

namespace it.UI.Settings
{
  public class ControlPage : PageBase
  {

	 public void KeyboardPage()
	 {
		GetComponentInParent<Settings>().SetPage(Settings.PageType.keyboard, false);
	 }

	 public void GamepadPage()
	 {
		GetComponentInParent<Settings>().SetPage(Settings.PageType.gamepad, false);
	 }


  }
}