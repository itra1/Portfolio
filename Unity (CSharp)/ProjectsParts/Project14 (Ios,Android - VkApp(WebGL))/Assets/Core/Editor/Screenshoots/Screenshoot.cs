using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Core.Editor.Screenshoots
{
	public class Screenshoot
	{
		[MenuItem("App/Screenshoot")]
		public static void Make()
		{
			var writeDir = new DirectoryInfo(Application.dataPath);
			var parentDir = writeDir.Parent;

			var targetDir = parentDir.CreateSubdirectory("screenshoots");

			var fuleName = DateTime.Now.ToString("yyyy_MM_dd__H_mm_ss_fff");

			var path = $"{targetDir.FullName}\\{fuleName}.png";

			ScreenCapture.CaptureScreenshot(path, 1);
		}
	}
}
