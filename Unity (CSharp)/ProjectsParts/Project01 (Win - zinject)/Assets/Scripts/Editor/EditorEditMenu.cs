using UnityEditor;

namespace Editor
{
    public static class EditorEditMenu
    {
        [MenuItem("Edit/Clear All EditorPrefs")]
        private static void ClearAllEditorPrefs()
        {
            if (EditorUtility.DisplayDialog("Clear all editor preferences.",
                    "Are you sure you want to delete all the editor preferences? " +
                    "This action cannot be undone.", "Yes", "No"))
            {
                EditorPrefs.DeleteAll();
            }
        }
    }
}