using System.Collections.Generic;

using UnityEngine;

namespace it.UI.Settings
{
  public class VideoPage : PageBase
  {

	 [SerializeField]
	 private SwitchItem _fpsItem;
	 [SerializeField]
	 private SwitchItem _qualityItem;

	 protected override void OnEnable()
	 {
		base.OnEnable();

		_fpsItem._selectEvent = SetFps;
		GetFps();
		_qualityItem._selectEvent = SetQuality;
		GetQuality();

	 }

	 public void SetFps(int index)
	 {
		int frames = 0;

		switch (index)
		{
		  case 1:
			 frames = 30;
			 break;
		  case 2:
			 frames = 60;
			 break;
		  case 0:
		  default:
			 frames = -1;
			 break;
		}
		it.Game.Managers.GameManager.Instance.GameSettings.VideoTargetFps = frames;
	 }

	 public void GetFps()
	 {
		int fps = it.Game.Managers.GameManager.Instance.GameSettings.VideoTargetFps;

		int index = 0;

		switch (fps)
		{
		  case 30:
			 index = 1;
			 break;
		  case 60:
			 index = 2;
			 break;
		  case -1:
		  default:
			 index = 0;
			 break;
		}

		_fpsItem.SetIndex(index);
	 }

	 public void SetQuality(int index)
	 {
		it.Game.Managers.GameManager.Instance.GameSettings.VideoQualityLevel = index;
	 }

	 public void GetQuality()
	 {
		_qualityItem.SetIndex(it.Game.Managers.GameManager.Instance.GameSettings.VideoQualityLevel);
	 }
  }
}