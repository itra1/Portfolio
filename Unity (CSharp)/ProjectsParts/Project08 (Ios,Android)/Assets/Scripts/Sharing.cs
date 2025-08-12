using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Sharing : MonoBehaviour {
	public string ScreenshotName = "screenshot.png";

	public bool useDeffImage;

	public void Share(string text) {
		FirebaseManager.Instance.LogEvent("friend_question");
		ShareScreenshotWithText(text);
	}


	public void ShareScreenshotWithText(string text) {

		if (useDeffImage) {
			NativeShare.Share(text, Application.streamingAssetsPath + "/Icone.png", "", "", "image/png", true, "");
		}
		else {
			string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
			if (File.Exists(screenShotPath)) File.Delete(screenShotPath);
			ScreenCapture.CaptureScreenshot(ScreenshotName);

			StartCoroutine(delayedShare(screenShotPath, text));
		}

	}

	//CaptureScreenshot runs asynchronously, so you'll need to either capture the screenshot early and wait a fixed time
	//for it to save, or set a unique image name and check if the file has been created yet before sharing.
	IEnumerator delayedShare(string screenShotPath, string text) {
		while (!File.Exists(screenShotPath)) {
			yield return new WaitForSeconds(.05f);
		}

		NativeShare.Share(text, screenShotPath, "", "", "image/png", true, "");
	}
}
