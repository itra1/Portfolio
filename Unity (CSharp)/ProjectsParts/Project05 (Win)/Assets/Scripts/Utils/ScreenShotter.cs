using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Utils
{
  public class ScreenShotter : MonoBehaviourBase
  {
	 public string path;
	 public int scale = 1;

	 [ContextMenu("Screenshot")]
	 public void ScreenShoot()
	 {
		ScreenCapture.CaptureScreenshot(path, scale);
	 }

  }
}