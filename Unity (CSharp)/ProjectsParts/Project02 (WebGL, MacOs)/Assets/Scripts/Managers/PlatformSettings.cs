using Garilla.Games;
using Garilla.WebGL;
using it.Network.Rest;
using it.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace it.Settings
{
	public abstract class PlatformSettings<T> : ScriptableObject where T : ScriptableObject
	{

		private static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null)
					_instance = (T)Garilla.ResourceManager.GetResource<T>("PlatformSettings");

				return _instance;
			}
		}

		//public static List<GameInfoData> GameInfoPrefabs => Instance._gameInfoPrefabs;
		[SerializeField] protected List<GameInfoData> _gameInfoPrefabs;

		[System.Serializable]
		public class GameInfoData
		{
			public string PrefabName;
			public List<GameType> GameTypes;
			public bool IsAllOrNofing;
			public bool IsDealerChoise;
		}

		//[ContextMenu("Read data info")]
		//public static void ReadDataInfo()
		//{
		//	//for (int i = 0; i < GameSettings.GameInfoPrefabs.Count; i++)
		//	//{
		//	//	_gameInfoPrefabs.Add(new GameInfoData()
		//	//	{
		//	//		GameTypes = new List<GameType>(GameSettings.GameInfoPrefabs[i].GameTypes),
		//	//		IsAllOrNofing = GameSettings.GameInfoPrefabs[i].IsAllOrNofing,
		//	//		IsDealerChoise = GameSettings.GameInfoPrefabs[i].IsDealerChoise,
		//	//		PrefabName = GameSettings.GameInfoPrefabs[i].PrefabName
		//	//	}); ;
		//	//}

		//}

	}
}