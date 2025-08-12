using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Garilla
{
	public class ResourceManager : MonoBehaviour
	{
		private static ResourceManager _instance;
		private Dictionary<string, Object> _dictionary = new Dictionary<string, Object>();
		private Dictionary<string, Object[]> _dictionaryList = new Dictionary<string, Object[]>();

		public static ResourceManager Instance
		{
			get
			{
				if (_instance == null)
					Init();
				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

		public static void Init()
		{
			if (_instance != null) return;
			GameObject go = GameObject.Find("Resource Manager");
			if (go == null)
			{
				go = new GameObject("Resource Manager");
				_instance = go.AddComponent<ResourceManager>();
				return;
			}
			_instance = go.GetComponent<ResourceManager>();
		}

		public static T GetResource<T>(string path) where T : Object
		{
			return (T)Instance._GetResource<T>(path);
		}
		private Object _GetResource<T>(string path) where T : Object
		{

			if (_dictionary.ContainsKey(path))
				return _dictionary[path];

			var obj = Resources.Load(path);
			_dictionary.Add(path, obj);
			return obj;
		}
		public static T[] GetResourceAll<T>(string path) where T : Object
		{
			return (T[])Instance._GetResourceAll<T>(path);
		}
		private T[] _GetResourceAll<T>(string path) where T : Object
		{
			Object[] ojsects;
			if (_dictionaryList.ContainsKey(path))
			{
				ojsects = _dictionaryList[path];
				T[] result = new T[ojsects.Length];
				for (int i = 0; i < ojsects.Length; i++)
					result[i] = (T)ojsects[i];
				return result;
			}

			var obj = Resources.LoadAll<T>(path);
			Object[] save = new T[obj.Length];
			for (int i = 0; i < obj.Length; i++)
				save[i] = (T)obj[i];

			_dictionaryList.Add(path, save);

			return obj;
		}

	}
}