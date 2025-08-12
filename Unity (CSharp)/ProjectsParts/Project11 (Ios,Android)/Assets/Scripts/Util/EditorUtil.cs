using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
#endif
/// <summary>
/// Набор утилит
/// </summary>
public class EditorUtil {

#if UNITY_EDITOR

	/// <summary>
	/// Поиск префабов по типу
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static List<T> GetAllPrefabsOfType<T>() where T : class {
		List<T> list = new List<T>();
		var allPrefabs = GetAllPrefabs();
		foreach (var single in allPrefabs) {
			T obj = AssetDatabase.LoadAssetAtPath(single, typeof(T)) as T;
			if (obj != null) {
				list.Add(obj);
			}
		}
		return list;
	}

	/// <summary>
	/// Получить все префабы
	/// </summary>
	/// <returns></returns>
	public static string[] GetAllPrefabs() {
		string[] temp = AssetDatabase.GetAllAssetPaths();
		List<string> result = new List<string>();
		foreach (string s in temp) {
			result.Add(s);
		}
		return result.ToArray();
	}

	/// <summary>
	/// Создание ассета
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="title">Заголовок</param>
	/// <param name="defaultName">Стандартное имя</param>
	/// <param name="extension">Хз что это</param>
	/// <param name="message">Сообщение при сохранении</param>
	public static void CreateAsset<T>(string title, string defaultName, string extension = "asset", string message = "Create a new instance.") where T : ScriptableObject {

		string assetPath = GetSavePath(title, defaultName, extension, message);

		if (string.IsNullOrEmpty(assetPath))
			return;

		T asset = ScriptableObject.CreateInstance(typeof(T).Name) as T;
		AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
		AssetDatabase.Refresh();
	}

	/// <summary>
	/// Выбор пути для сохранения
	/// </summary>
	/// <param name="title">Заголовок</param>
	/// <param name="defaultName">Стандартное имя</param>
	/// <param name="extension">Хз что это</param>
	/// <param name="message">Сообщение при сохранении</param>
	/// <returns></returns>
	private static string GetSavePath(string title, string defaultName, string extension, string message) {
		string path = "Assets";
		Object obj = Selection.activeObject;

		if (obj != null) {
			path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

			if (path.Length > 0) {
				if (!System.IO.Directory.Exists(path)) {
					string[] pathParts = path.Split("/"[0]);
					pathParts[pathParts.Length - 1] = "";
					path = string.Join("/", pathParts);
				}
			}
		}

		return EditorUtility.SaveFilePanelInProject(title, defaultName, extension, message, path);
	}

#endif

}
