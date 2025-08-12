using Editor.Build.Windows;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	public static class EditorToolsMenu
	{
		[MenuItem("Tools/Build/Build window", false, 0)]
		private static void OpenWindow()
		{
			var buildWindow = ScriptableObject.CreateInstance<BuildWindow>();
			buildWindow.Show();
			_ = buildWindow.InitializeIfNot();
		}
	}
}