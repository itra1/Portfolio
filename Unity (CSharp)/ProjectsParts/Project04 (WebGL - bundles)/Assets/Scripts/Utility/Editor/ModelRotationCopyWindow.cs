using UnityEngine;
using UnityEditor;
using System.Collections;

public class ModelRotationCopyWindow : EditorWindow 
{
	GameObject sourceObj;

	GameObject destObj;

	[MenuItem ("Window/Mixamo Rotation Copy")]
	static void Init () 
	{
		ModelRotationCopyWindow window = (ModelRotationCopyWindow)EditorWindow.GetWindow(typeof(ModelRotationCopyWindow));
		window.Show();
	}
	
	void OnGUI()
	{
		GUILayout.Label("Sorce Settings", EditorStyles.boldLabel);
		sourceObj = (GameObject)EditorGUILayout.ObjectField("Source object", sourceObj, typeof(GameObject), true);

		GUILayout.Label("Target Settings", EditorStyles.boldLabel);
		destObj = (GameObject)EditorGUILayout.ObjectField("Target object", destObj, typeof(GameObject), true);

		GUILayout.Label("Operations", EditorStyles.boldLabel);
		if (GUILayout.Button("CopyPose"))
		{
			CopyBodyRotations(sourceObj.transform, destObj.transform);
		}
		if (GUILayout.Button("CopyPosition"))
		{
			destObj.transform.eulerAngles = sourceObj.transform.eulerAngles;
			destObj.transform.position = sourceObj.transform.position;
		}
	}

	protected void CopyBodyRotations(Transform src, Transform dest)
	{
		foreach (Transform tr in src)
		{
			Transform destImage = dest.Find(tr.name);
			if (destImage == null)
			{
				if (tr.GetComponent<Renderer>() == null)
					continue;

				GameObject imageObject = (GameObject)Instantiate(tr.gameObject);
				imageObject.name = tr.gameObject.name;
				imageObject.transform.parent = dest.transform;
				imageObject.transform.localPosition = tr.localPosition;
				imageObject.transform.localEulerAngles = tr.localEulerAngles;
				continue;
			}

			destImage.localEulerAngles = tr.localEulerAngles;

			CopyBodyRotations(tr, destImage);
		}
	}
}
