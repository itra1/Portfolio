#pragma strict

function OnGUI()
{
	if (GUILayout.Button("Take"))
	{
		ScreenCapture.CaptureScreenshot(Application.dataPath + "1.png", 1);
	}
}