using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace MapConfiguration {

	public class EditorMapWindow : EditorWindow {

		public MapBehaviour mapBehaviour;
		public MapBehaviour _mapBehaviourInst;

		private List<MapLink> mapLinkList;

		public int mainToolBar = 0;
		public string[] mainToolBarStrings = new string[] { "Список", "Редактор" };

		private GUIStyle boxStyle;
		private GUIStyle paddingStyle;
		Vector2 scrollListPosition = Vector2.zero;

		private EditorMode editorMode;

		[Flags]
		public enum EditorMode {
			none = 0,
			gride = 1
		}

		public void OnEnable() {
			this.titleContent.text = "Rедактором карты";
			LoadPrefabs();

			EditorGUIUtility.labelWidth = 300f;

			this.boxStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box);
			this.paddingStyle = new GUIStyle();
			this.paddingStyle.padding = new RectOffset(3, 3, 3, 3);
			EditorSceneManager.sceneOpened += LoadMap;
		}

		private void OnDisable() {
			SetDefault();
			EditorSceneManager.sceneOpened -= LoadMap;
		}

		private void OnGUI() {

			GUILayout.BeginHorizontal();

			GUILayout.Space(8f);
			mapBehaviour = EditorGUILayout.ObjectField("Карта", this.mapBehaviour, typeof(MapBehaviour), false) as MapBehaviour;

			GUILayout.EndHorizontal();

			EditorGUILayout.BeginVertical();
			mainToolBar = GUILayout.Toolbar(mainToolBar, mainToolBarStrings);
			EditorGUILayout.EndHorizontal();

			switch (mainToolBar) {
				case 0:
					DrawList();
					break;
				case 1:
					if (mapBehaviour != null)
						DrawTools();
					break;
			}
		}

		void DrawList() {

			scrollListPosition = GUILayout.BeginScrollView(scrollListPosition);

			float thumbnailMaxHeight = 5;
			RectOffset thumbnailPadding = new RectOffset(6, 6, 6, 3);
			float labelHeight = 8;

			RectOffset padding = this.paddingStyle.padding;

			foreach (var element in mapLinkList) {

				if (element.name.Contains("Template")) continue;

				Color boxColor = new Color(0.897f, 0.897f, 0.897f, 1f);

				EditorGUILayout.Space();
				GUILayout.Space(3f);
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(8f);

				float thumbnailHeightWithPadding = thumbnailMaxHeight + thumbnailPadding.top + thumbnailPadding.bottom;
				Rect controlRect = GUILayoutUtility.GetRect(0.0f, (thumbnailHeightWithPadding + labelHeight), GUILayout.ExpandWidth(true));

				GUI.color = boxColor;
				GUI.Box(new Rect(controlRect.x - padding.left, controlRect.y - padding.top, controlRect.width + (padding.left + padding.right), controlRect.height + (padding.top + padding.bottom)), "", this.boxStyle);
				GUI.color = Color.white;

				GUI.color = Color.black;
				GUI.Label(new Rect(padding.left * 3, controlRect.y+2, 400, 15), String.Format("{0} ({1})", element.mapTitle, element.mapId));
				GUI.color = Color.white;

				if (GUI.Button(new Rect((controlRect.width - 98f), (controlRect.y + 1f), 70f, 20f), "Загрузить")) {
					SetMap(element);
				} else if (GUI.Button(new Rect((controlRect.width - 190f), (controlRect.y + 1f), 90f, 20f), "Дублировать")) {
					DublicateMap(element);
				} else if (GUI.Button(new Rect((controlRect.width - 13f), (controlRect.y + 1f), 20f, 20f), "X")) {
					DeleteMap(element);
				} else if (Event.current.type == EventType.MouseUp && controlRect.Contains(Event.current.mousePosition)) { }

				GUILayout.Space(8f);
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(3f);
			}
			GUILayout.EndScrollView();
		}

		void LoadMap(Scene scene, OpenSceneMode lsm) {

			mapBehaviour = GameObject.FindObjectOfType<MapBehaviour>();

			var mng = GameObject.Find("Managers");

			var mm = mng.GetComponent<MapManager>();


			if (mm.map != null) {
				//PrefabUtility.DisconnectPrefabInstance(map.gameObject);
				DestroyImmediate(mm.map.gameObject);
			}
			

			//_mapBehaviourInst = mm.SetMap(map);
			_mapBehaviourInst = mapBehaviour;
			mm.map = _mapBehaviourInst;
			_mapBehaviourInst.source = mapBehaviour;
			_mapBehaviourInst.DrawGride();

		}

		void LoadPrefabs() {
			mapLinkList = GetAllPrefabsOfType<MapLink>();
			mapLinkList = mapLinkList.OrderBy(x => x.mapId).ToList();
		}

		void SetMap(MapLink map) {

			EditorSceneManager.OpenScene(map.localPath + "map" + map.mapId + ".unity", OpenSceneMode.Additive);
			
			/*
			mapBehaviour = map;

			var mng = GameObject.Find("Managers");

			var mm = mng.GetComponent<MapManager>();


			if (mm.map != null) {
				//PrefabUtility.DisconnectPrefabInstance(map.gameObject);
				DestroyImmediate(mm.map.gameObject);
			}

			GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(map.gameObject);
			inst.transform.SetParent(mng.GetComponent<GameManager>().world);

			//_mapBehaviourInst = mm.SetMap(map);
			_mapBehaviourInst = inst.GetComponent<MapBehaviour>();
			mm.map = _mapBehaviourInst;
			_mapBehaviourInst.source = mapBehaviour;
			_mapBehaviourInst.DrawGride();
			*/
		}

		void DeleteMap(MapLink map) {

			DialogConfirm.ShowDialog("Действительно хотите удалить карту " + map.mapTitle, () => {
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(map));
				LoadPrefabs();
			}, null);

		}

		void DublicateMap(MapLink map) {

			string path = AssetDatabase.GetAssetPath(map);
			path = path.Substring(0, path.Length - (map.name.Length + 7));
			AssetDatabase.CreateAsset(map, path);
			//GameObject newMap = Instantiate(map);
			//newMap.GetComponent<MapLink>().mapTitle += " (копия)";
			//newMap.GetComponent<MapLink>().mapId = map.mapId + 1;
			//newMap.name = "Map" + (map.mapId + 1).ToString();
			//PrefabUtility.CreatePrefab(path + newMap.name + ".prefab", newMap);

			//DestroyImmediate(newMap);
			LoadPrefabs();
		}


		private static string GetSavePath(string title, string defaultName, string extension, string message) {
			string path = "Assets";
			UnityEngine.Object obj = Selection.activeObject;

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
		public List<T> GetAllPrefabsOfType<T>() where T : class {
			List<T> list = new List<T>();
			var allPrefabs = GetAllAssets();
			foreach (var single in allPrefabs) {
				T obj = AssetDatabase.LoadAssetAtPath(single, typeof(T)) as T;
				if (obj != null) {
					//Debug.Log("Found prefab of class: "+typeof(T)+" : "+obj);
					list.Add(obj);
				}
			}
			return list;
		}
		public static string[] GetAllPrefabs() {
			string[] temp = AssetDatabase.GetAllAssetPaths();
			List<string> result = new List<string>();
			foreach (string s in temp) {
				if (s.Contains(".prefab")) result.Add(s);
			}
			return result.ToArray();
		}
		public static string[] GetAllAssets() {
			string[] temp = AssetDatabase.GetAllAssetPaths();
			List<string> result = new List<string>();
			foreach (string s in temp) {
				if (s.Contains(".asset")) result.Add(s);
			}
			return result.ToArray();
		}

		bool isEditor = false;

		void DrawTools() {

			GUILayout.Space(8f);

			if (_mapBehaviourInst == null) return;

			//_mapBehaviourInst.mapId = EditorGUILayout.IntField("Идентификатор", _mapBehaviourInst.mapId);
			//_mapBehaviourInst.mapTitle = EditorGUILayout.TextField("Название карты", _mapBehaviourInst.mapTitle);
			_mapBehaviourInst.cellWidth = EditorGUILayout.IntField("Количество ячеек по ширине", _mapBehaviourInst.cellWidth);
			_mapBehaviourInst.cellLength = EditorGUILayout.IntField("Количество ячеек по длинне", _mapBehaviourInst.cellLength);
			_mapBehaviourInst.cellSize = EditorGUILayout.FloatField("Размер ячейки", _mapBehaviourInst.cellSize);
			_mapBehaviourInst.startGridePoint = EditorGUILayout.Vector3Field("Точка генерации", _mapBehaviourInst.startGridePoint);

			GUILayout.Space(8f);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Перерисовать сетку")) {
				//mapBehaviour.DrawGride();
				_mapBehaviourInst.DrawGride();
			}
			if (GUILayout.Button("Сохранить карту")) {
				_mapBehaviourInst.SaveObject();
				mapBehaviour = _mapBehaviourInst.source;
				LoadPrefabs();
				//mapBehaviour.SaveObject();
			}
			EditorUtility.SetDirty(_mapBehaviourInst);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();

			switch (editorMode) {
				case EditorMode.none:
					DrawFullNavigate();
					break;
				case EditorMode.gride:
					DrawGrideEditor();
					break;
			}

			GUILayout.EndHorizontal();
		}

		void DrawFullNavigate() {
			if (GUILayout.Button("Включить редактирование сетки")) {
				SetGrideMode();
			}
		}


		void DrawGrideEditor() {
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Отключить редактирование")) {
				SetDefault();
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(2f);

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Переотрисовать сетку")) {
				GameManager mm = GameObject.Find("Managers").GetComponent<GameManager>();
				mm.DrawGride();
			}
			if (GUILayout.Button("Блокирование ячейку")) {
				Debug.Log("Блокирование ячейку");
			}
			if (GUILayout.Button("Открытие ячейку")) {
				Debug.Log("Открытие ячейку");
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		void SetGrideMode() {
			editorMode = EditorMode.gride;
		}

		void SetDefault() {
			editorMode = EditorMode.none;
		}

	}
}