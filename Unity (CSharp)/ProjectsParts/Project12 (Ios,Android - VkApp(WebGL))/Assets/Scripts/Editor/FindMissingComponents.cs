using UnityEditor;
using UnityEngine;

namespace Game.Editor.Scripts.EventReceivers
{
	public class FindMissingComponents
	{
		[MenuItem("App/Find Missing Components")]
		static void FindMissing()
		{
			bool exists = false;
			foreach (var prefab in AssetDatabase.FindAssets("t:Prefab"))
			{
				string path = AssetDatabase.GUIDToAssetPath(prefab);
				GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

				if (obj != null)
				{
					foreach (Component component in obj.GetComponents<Component>())
					{
						if (component == null)
						{
							Debug.LogError($"Missing component in {path}");
							exists = true;
							break;
						}
					}
				}
			}
			if (!exists)
				Debug.Log($"No exists prefabs with missing components");
		}
	}
}
