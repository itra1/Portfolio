using Settings;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Zenject;

namespace Editor
{
    public static class EditorAssetsMenu
    {
        [MenuItem("Assets/Actualize prefab settings &#p")]
        [ContextMenu("Actualize prefab settings")] 
        private static void ActualizePrefabSettings()
        {
            var container = StaticContext.Container;
            var uiSettings = container.TryResolve<IUISettings>();
			
            if (uiSettings == null)
            {
                Debug.LogError("You need to create UI settings first");
                return;
            }
			
            var prefabSettings = container.TryResolve<IPrefabSettings>();
			
            if (prefabSettings == null)
            {
                Debug.LogError("You need to create prefab settings first");
                return;
            }
			
            prefabSettings.Actualize(uiSettings);
        }
		
        [MenuItem("Assets/Forced Recompilation &#f")]
        private static void RecompileForcibly()
        { 
            CompilationPipeline.RequestScriptCompilation();
        }
    }
}