using System.Linq;

using UnityEditor;

using UnityEngine;

using Zenject;

namespace Core.Engine.Helpers
{
#if UNITY_EDITOR
	public static class ContextHelpers
	{
		public static bool AddMonoInstaller<T>(string assetPath) where T : MonoInstaller
		{
			GameObject goPrefab = null;
			try
			{
				goPrefab = PrefabUtility.LoadPrefabContents(assetPath);

				var installer = goPrefab.GetComponent<T>();

				if (installer == null)
					installer = goPrefab.AddComponent<T>();

				var context = goPrefab.GetComponent<ProjectContext>();

				var componentList = context.Installers.ToList();
				if (!componentList.Contains(installer))
					componentList.Add(installer);
				context.Installers = componentList;

				PrefabUtility.SaveAsPrefabAsset(goPrefab, assetPath);
				PrefabUtility.UnloadPrefabContents(goPrefab);
			}
			catch
			{
				if (goPrefab != null)
				{
					PrefabUtility.SaveAsPrefabAsset(goPrefab, assetPath);
					PrefabUtility.UnloadPrefabContents(goPrefab);
				}
				return false;
			}
			return true;
		}
		public static bool RemoveMonoInstaller<T>(string assetPath) where T : MonoInstaller
		{
			GameObject goPrefab = null;
			try
			{
				goPrefab = PrefabUtility.LoadPrefabContents(assetPath);

				var installer = goPrefab.GetComponent<T>();

				if (installer == null)
				{
					PrefabUtility.UnloadPrefabContents(goPrefab);
					return true;
				}

				var context = goPrefab.GetComponent<ProjectContext>();

				var componentList = context.Installers.ToList();
				componentList.Remove(installer);
				context.Installers = componentList;

				Object.DestroyImmediate(installer);

				PrefabUtility.SaveAsPrefabAsset(goPrefab, assetPath);
				PrefabUtility.UnloadPrefabContents(goPrefab);
			}
			catch
			{
				if (goPrefab != null)
				{
					PrefabUtility.SaveAsPrefabAsset(goPrefab, assetPath);
					PrefabUtility.UnloadPrefabContents(goPrefab);
				}
				return false;
			}
			return true;
		}
	}
#endif
}