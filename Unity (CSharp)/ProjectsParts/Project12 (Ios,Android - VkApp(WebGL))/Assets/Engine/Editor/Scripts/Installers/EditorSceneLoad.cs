using Game.Editor.Installers;
using UnityEditor;

namespace Game.Editor.Scripts.Installers
{
	[InitializeOnLoad]
	public class EditorSceneLoad
	{
		private static EngineEditorInstaller editor;
		static EditorSceneLoad()
		{
			EditorApplication.hierarchyChanged += OnSceneLoaded;
		}

		static void OnSceneLoaded()
		{
			if (editor == null)
			{
				editor = new();
				editor.Initiate();
			}
		}
	}
}
