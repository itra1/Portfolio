using Game.Scripts.Managers;
using UnityEngine;
using Scripts.GameItems.Platforms;
using System.Collections.Generic;
using static Game.GameItems.Platforms.PlatformSettings;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Constructor
{
#if UNITY_EDITOR

	[CustomEditor(typeof(LevelConstructor))]
	public class LevelConstructorEditor :Editor
	{
		private LevelConstructor _script;
		private int _platformCount;
		private int _indexFormation;
		private void OnEnable()
		{
			_script = (LevelConstructor)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			_ = EditorGUILayout.BeginHorizontal();
			_platformCount = EditorGUILayout.IntField("Platform count", _platformCount);
			if (GUILayout.Button("Spawn platforms"))
				_script.SpawnPlatforms(_platformCount);
			EditorGUILayout.EndHorizontal();

			_ = EditorGUILayout.BeginHorizontal();
			_indexFormation = EditorGUILayout.IntField("Index formation", _indexFormation);
			if (GUILayout.Button("Load formation"))
				_script.Load(_indexFormation);
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("Clear"))
				_script.Clear();

			if (GUILayout.Button("Save"))
				_script.Save();
		}
	}

#endif

	public class LevelConstructor :MonoBehaviour
	{
		[SerializeField] private SceneComponents _sceneComponents;
		[SerializeField] private GameSettings _gameSettings;
		[SerializeField] private GamePlatform _prefab;

		private List<GamePlatform> _gamePlatforms = new();

		public LevelConstructor()
		{
		}

#if UNITY_EDITOR
		public void SpawnPlatforms(int count)
		{
			Clear();

			for (int i = 0; i < _gamePlatforms.Count; i++)
			{
				if (_gamePlatforms[i] != null)
					DestroyImmediate(_gamePlatforms[i]);
			}
			_gamePlatforms.Clear();

			Vector3 targetPosition = Vector3.zero;

			for (int i = 0; i < count; i++)
			{
				var inst = Instantiate(_prefab, _sceneComponents.PlatformParent);
				inst.transform.SetLocalPositionAndRotation(targetPosition, Quaternion.identity);
				_gamePlatforms.Add(inst);
				targetPosition.y -= _gameSettings.PlatformSettings.PlatformDistance;
			}
		}

		internal void Clear()
		{
			for (int i = 0; i < _gamePlatforms.Count; i++)
			{
				DestroyImmediate(_gamePlatforms[i]);
			}
			_gamePlatforms.Clear();
		}

		internal void Save()
		{
			var spawnPlatform = _sceneComponents.PlatformParent.GetComponentsInChildren<GamePlatform>();
			Debug.Log(spawnPlatform.Length);

			LevelSpawnSettings spawnItem = new()
			{
				Platforms = new()
			};

			for (int i = 0; i < spawnPlatform.Length; i++)
			{
				spawnItem.Platforms.Add(new PlatformFormationItem() { UUID = spawnPlatform[i].Formation.ActiveFormationUUID, Rotation = spawnPlatform[i].transform.rotation });
			}
			_gameSettings.PlatformSettings.LevelSpawn.Add(spawnItem);
		}

		internal void Load(int indexFormation)
		{
			var formation = _gameSettings.PlatformSettings.LevelSpawn[indexFormation];

			Vector3 targetPosition = Vector3.zero;
			for (int i = 0; i < formation.Platforms.Count; i++)
			{
				var inst = Instantiate(_prefab, _sceneComponents.PlatformParent);
				inst.gameObject.SetActive(true);
				inst.transform.SetLocalPositionAndRotation(targetPosition, formation.Platforms[i].Rotation);
				inst.Formation.SetFormation(formation.Platforms[i].UUID);
				targetPosition.y -= _gameSettings.PlatformSettings.PlatformDistance;
			}
		}
#endif

	}
}
