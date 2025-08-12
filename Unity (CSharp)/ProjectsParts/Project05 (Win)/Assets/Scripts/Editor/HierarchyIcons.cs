using UnityEngine;
using System.Collections;
using UnityEditor;
namespace it.Editor
{
#if UNITY_EDITOR

  [InitializeOnLoad]
  public class HierarchyIcons : MonoBehaviour
  {
	 static Texture2D EnvironmentHI;
	 static Texture2D SpawnPointHI;

	 static HierarchyIcons()
	 {

		EnvironmentHI = AssetDatabase.LoadAssetAtPath("Assets/Gizmos/Environment Icon.png", typeof(Texture2D)) as Texture2D;
		SpawnPointHI = AssetDatabase.LoadAssetAtPath("Assets/Gizmos/SpawnPoint.png", typeof(Texture2D)) as Texture2D;

#if UNITY_EDITOR
		EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
		EditorApplication.RepaintHierarchyWindow();
#endif
	 }

	 static void HierarchyItemCB(int instanceId, Rect selectionRect)
	 {
		EnvironmentIcons(instanceId, selectionRect);
		SpawnPointsIcons(instanceId, selectionRect);
	 }

	 private static void EnvironmentIcons(int instanceId, Rect selectionRect)
	 {

		var environmentObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;

		if (environmentObject == null)
		  return;

		if (EnvironmentHI != null && environmentObject.GetComponent<it.Game.Environment.Environment>() != null)
		{
		  var iconRect = new Rect(selectionRect);
		  iconRect.x = iconRect.width + (selectionRect.x - 16);
		  iconRect.width = 16;
		  iconRect.height = 16;
		  GUI.DrawTexture(iconRect, EnvironmentHI);
		}
	 }

	 private static void SpawnPointsIcons(int instanceId, Rect selectionRect)
	 {

		var environmentObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;

		if (environmentObject == null)
		  return;

		if (SpawnPointHI != null && environmentObject.GetComponent<it.Game.Environment.Handlers.SpawnPoint>() != null)
		{
		  var iconRect = new Rect(selectionRect);
		  iconRect.x = iconRect.width + (selectionRect.x - 16);
		  iconRect.width = 16;
		  iconRect.height = 16;
		  GUI.DrawTexture(iconRect, SpawnPointHI);
		}
	 }

  }

#endif

}