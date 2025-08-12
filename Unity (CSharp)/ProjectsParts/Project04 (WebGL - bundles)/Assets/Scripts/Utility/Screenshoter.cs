using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class Screenshoter : MonoBehaviour 
{
	public Camera workingCamera;

	public HexNet net;

	public Vector2 screenSize;

	// Use this for initialization
	void Start()
	{
		if (!Application.isPlaying)
			return;

		InitializeCamera();
		StartCoroutine(MakeMap());
	}

	void InitializeCamera()
	{
		workingCamera.transform.position = workingCamera.transform.TransformPoint(new Vector3(0.0f, 0.0f, -100.0f));
	}

	public IEnumerator MakeMap()
	{
		Vector3 startPosition = transform.position;

		transform.position = net.GetCellPos3D(0, 0);

		Debug.Log(2.0f * Mathf.Abs(transform.position.x - startPosition.x) + " " + 2.0f * (Mathf.Abs(transform.position.z - startPosition.z)) * 115.3f / 100.0f);

		yield return new WaitForSeconds(5);

		for (int i = 0; i < screenSize.x; i++) 
		{
			for (int j = 0; j < screenSize.y; j++) 
			{
				transform.position = startPosition;
				transform.position = transform.TransformPoint(new Vector3(2.0f * workingCamera.orthographicSize * (i + 0.5f), 0.0f, -(2.0f * workingCamera.orthographicSize + 15.3f) * (j + 0.5f)));

				ScreenCapture.CaptureScreenshot("e:\\1\\map" + i + "_" + j + ".png");

				yield return new WaitForSeconds(1);
			}
        }
	}

	public void Update()
	{
		if (Application.isPlaying)
			return;

		Vector3 leftTop = transform.position;
		Vector3 rightTop = transform.TransformPoint(new Vector3(2.0f * workingCamera.orthographicSize * screenSize.x, 0.0f, 0.0f));
		Vector3 rightBottom = transform.TransformPoint(new Vector3(2.0f * workingCamera.orthographicSize * screenSize.x, 0.0f, -2.0f * workingCamera.orthographicSize * screenSize.y));
		Vector3 leftBottom = transform.TransformPoint(new Vector3(0.0f, 0.0f, -2.0f * workingCamera.orthographicSize * screenSize.y));

		Debug.DrawLine(leftTop, rightTop);
		Debug.DrawLine(rightTop, rightBottom);
		Debug.DrawLine(rightBottom, leftBottom);
		Debug.DrawLine(leftBottom, leftTop);
	}
}
