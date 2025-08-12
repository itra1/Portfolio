using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditRun {

	public class RunBlockManagerWindow : EditorWindow {

		private Vector2 _scrollListPosition = Vector2.zero;
		private List<RunBlock> runBlockList;

		private GUIStyle boxStyle;
		private GUIStyle paddingStyle;

		private bool mapEditorMode = false;

		public int mainToolBar = 0;
		public string[] mainToolBarStrings = new string[] { "Блоки карт", "Уровни", "Очередность уровней" };

		protected void OnEnable() {

			InitMapBlock();
			InitMapList();
			InitLevelOrder();

			RunBlockEditPrefs.PreloadEditorPrefs();

			this.titleContent = new GUIContent("Редактор карты");
			EditorGUIUtility.labelWidth = 300f;

			mapEditorMode = EditorPrefs.GetBool("mapEditorMode", false);

			this.boxStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box);
			this.paddingStyle = new GUIStyle();
			this.paddingStyle.padding = new RectOffset(3, 3, 3, 3);

		}

		private void OnDisable() {
			RunBlockEditPrefs.SaveEditorPrefs();
		}

		public void OnGUI() {

			EditorGUILayout.BeginVertical();
			mainToolBar = GUILayout.Toolbar(mainToolBar, mainToolBarStrings);
			EditorGUILayout.EndHorizontal();

			switch (mainToolBar) {
				case 0:
					DrawMapBlocks();
					break;
				case 1:
					DrawMaps();
					break;
				case 2:
					DrawOrderLevel();
					break;
			}

		}

		#region Очередность уровней

		private LevelDataOrders _levelOrder;

		void InitLevelOrder() {
			List<LevelDataOrders> tmpLevelOrder = GetAllPrefabsOfType<LevelDataOrders>();
			_levelOrder = tmpLevelOrder[0];
		}


		void DrawOrderLevel() {

			if (_levelOrder == null) return;

			GUILayout.Space(8f);
			
			if (GUILayout.Button("Сохранить очередь")) {
				_levelOrder.SaveData();
			}

			if (GUILayout.Button("Добавить уровень в очередь")) {

				LevelDataSelectWindow selectWindows = (LevelDataSelectWindow)EditorWindow.GetWindow(typeof(LevelDataSelectWindow));
				selectWindows.onSelect = (elem) => {

					LevelDataOrders.LevelDataOrderStruct dos = new LevelDataOrders.LevelDataOrderStruct();
					dos.levelObject = elem;

					_levelOrder.orderLevels.Add(dos);
					Repaint();
				};
			}

			DrawOrderLevelsList();
		}

		void DrawOrderLevelsList() {
			
			_scrollListPosition = GUILayout.BeginScrollView(_scrollListPosition);

			float thumbnailMaxHeight = 5;
			RectOffset thumbnailPadding = new RectOffset(6, 6, 6, 3);
			float labelHeight = 8;

			RectOffset padding = this.paddingStyle.padding;

			int orderNum = 0;

			foreach (var elementItem in _levelOrder.orderLevels) {

				var element = elementItem.levelObject;

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
				GUI.Label(new Rect(padding.left * 2, controlRect.y, 400, 18), String.Format("{0} - {1} ({2})", (orderNum+1), (String.IsNullOrEmpty(element.title) ? "Обычный блок" : element.title), element.name));
				GUI.color = Color.white;

				if (GUI.Button(new Rect((controlRect.width - 58f), (controlRect.y + 1f), 40f, 20f), "Вниз")) {
					ChangeOrderLevels(orderNum, 1);
				} else if (GUI.Button(new Rect((controlRect.width - 110f), (controlRect.y + 1f), 50f, 20f), "Вверх")) {
					ChangeOrderLevels(orderNum, -1);
				} else if (GUI.Button(new Rect((controlRect.width - 13f), (controlRect.y + 1f), 20f, 20f), "X")) {
					_levelOrder.orderLevels.RemoveAt(orderNum);
					EditorUtility.SetDirty(_levelOrder);
					Repaint();
				} else if (Event.current.type == EventType.MouseUp && controlRect.Contains(Event.current.mousePosition)) { }

				GUILayout.Space(8f);
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(3f);
				orderNum++;
			}
			GUILayout.EndScrollView();
		}


		void ChangeOrderLevels(int index, int increment) {

			LevelDataOrders.LevelDataOrderStruct activeElem = _levelOrder.orderLevels[index];
			
			if ((index == 0 && increment < 0) || (index == _levelOrder.orderLevels.Count - 1 && increment > 0)) return;
			_levelOrder.orderLevels.RemoveAt(index);
			_levelOrder.orderLevels.Insert(index + increment, activeElem);
			Repaint();
			EditorUtility.SetDirty(_levelOrder);
		}

		#endregion

		#region Уровни

		private LevelData _activeLevelData;

		public int mapBar = 0;
		public string[] mapBarToolBarStrings = new string[] { "Список", "Редактор" };

		private List<LevelData> runList;

		/// <summary>
		/// Отрисовка списка карт
		/// </summary>
		private void DrawMaps() {

			GUILayout.Space(8f);
			GUILayout.BeginHorizontal();
			_activeLevelData = EditorGUILayout.ObjectField("Выбор карты", this._activeLevelData, typeof(LevelData), false) as LevelData;
			if (GUILayout.Button("Отменить")) {
				CancelSelect();
        DeactiveLevelBlock();
        DeactiveBlock();
      }

			GUILayout.EndHorizontal();

			EditorGUILayout.BeginVertical();
			mapBar = GUILayout.Toolbar(mapBar, mapBarToolBarStrings);
			EditorGUILayout.EndHorizontal();

			switch (mapBar) {
				case 0:
					DrawListMapLevel();
					break;
				case 1:
					DrawEditorMapLevel();
					break;
			}

		}


		void CancelSelect() {
			DeactiveMapBlock();
			DeactiveBlock();
      DeactiveAllInspector();
		}

		void DrawListMapLevel() {

			_scrollListPosition = GUILayout.BeginScrollView(_scrollListPosition);

			float thumbnailMaxHeight = 30;
			RectOffset thumbnailPadding = new RectOffset(6, 6, 6, 3);
			float labelHeight = 8;
      
      RectOffset padding = this.paddingStyle.padding;

			foreach (var element in runList) {

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
				GUI.Label(new Rect(padding.left * 2, controlRect.y, controlRect.width - 200, 18), String.Format("{0} ({1})", (String.IsNullOrEmpty(element.title) ? "Обычный блок" : element.title), element.name));
        GUI.Label(new Rect(padding.left * 2, controlRect.y+15, controlRect.width - 200, 40), element.description);
        GUI.color = Color.white;

        if (GUI.Button(new Rect((controlRect.width - 190f), (controlRect.y + 1f), 60f, 20f), "Выбрать")) {
          OpenDialogLevel(element);
        }

        if (GUI.Button(new Rect((controlRect.width - 127f), (controlRect.y + 1f), 55f, 20f), "Запуск")) {
          EditorPlayLevel(element);
        }

        if (GUI.Button(new Rect((controlRect.width - 190f), (controlRect.y + 25f), 90f, 20f), "Дублировать")) {
					DublicateLevelBlock(element);
        }

        if (GUI.Button(new Rect((controlRect.width - 98f), (controlRect.y + 25f), 80f, 20f), "Установить")) {
          CancelSelect();
          SetMap(element);
          OpenDialogLevel(element);
        }

        if (GUI.Button(new Rect((controlRect.width - 13f), (controlRect.y + 25f), 20f, 20f), "X")) {
					DeleteLevel(element);
				}

        if (Event.current.type == EventType.MouseUp && controlRect.Contains(Event.current.mousePosition)) { }

				GUILayout.Space(8f);
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(3f);
			}
			GUILayout.EndScrollView();
		}

		void EditorPlayLevel(LevelData level) {
			CancelSelect();
			GameManager.activeLevelData = level;
			EditorUtility.SetDirty(GameManager.Instance);
			EditorApplication.isPlaying = true;

		}

		void DrawEditorMapLevel() {

			GUILayout.Space(8f);

			if (_activeLevelData == null) {
				GUILayout.Label("Не выбран уровень");
				return;
			}

			_activeLevelData.title = EditorGUILayout.TextField("Название", _activeLevelData.title);
			_activeLevelData.gameMode = (GameMode)EditorGUILayout.EnumPopup("Режим игры", _activeLevelData.gameMode);
			_activeLevelData.location = (GameLocation)EditorGUILayout.EnumPopup("Локация", _activeLevelData.location);
			_activeLevelData.moveVector = (MoveVector)EditorGUILayout.EnumPopup("Направление движения", _activeLevelData.moveVector);
			_activeLevelData.gameFormat = (GameMechanic)EditorGUILayout.EnumPopup("Формат игры", _activeLevelData.gameFormat);

			if (GUILayout.Button("Добавить блок")) {
				RunBlockSelectWindows selectWindows = (RunBlockSelectWindows)EditorWindow.GetWindow(typeof(RunBlockSelectWindows));
				selectWindows.onSelect = (elem) => {
					_activeLevelData.runBlocks.Add(elem);
					Repaint();
				};
			}

			DrawMapBlockInLevel();

			if (GUILayout.Button("Сохранить")) {
				EditorUtility.SetDirty(_activeLevelData);
			}

		}

		void DrawMapBlockInLevel() {

			_scrollListPosition = GUILayout.BeginScrollView(_scrollListPosition);

			float thumbnailMaxHeight = 5;
			RectOffset thumbnailPadding = new RectOffset(6, 6, 6, 3);
			float labelHeight = 8;

			RectOffset padding = this.paddingStyle.padding;

			int indexElement = 0;
			foreach (var element in _activeLevelData.runBlocks) {

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
				GUI.Label(new Rect(padding.left * 2, controlRect.y, 400, 18), String.Format("{0} ({1})", (String.IsNullOrEmpty(element.title) ? "Обычный блок" : element.title), element.name));
				GUI.color = Color.white;

				if (GUI.Button(new Rect((controlRect.width - 58f), (controlRect.y + 1f), 40f, 20f), "Вниз")) {
					ChangeOrderBlockInLevel(indexElement, 1);
				} else if (GUI.Button(new Rect((controlRect.width - 110f), (controlRect.y + 1f), 50f, 20f), "Вверх")) {
					ChangeOrderBlockInLevel(indexElement, -1);
				} else if (GUI.Button(new Rect((controlRect.width - 13f), (controlRect.y + 1f), 20f, 20f), "X")) {
					_activeLevelData.runBlocks.RemoveAt(indexElement);
					EditorUtility.SetDirty(_activeLevelData);
					Repaint();
				} else if (Event.current.type == EventType.MouseUp && controlRect.Contains(Event.current.mousePosition)) { }

				GUILayout.Space(8f);
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(3f);
				indexElement++;
			}
			GUILayout.EndScrollView();

		}

		void ChangeOrderBlockInLevel(int index, int increment) {
			BlockBase runBlock = _activeLevelData.runBlocks[index];
			if ((index == 0 && increment < 0) || (index == _activeLevelData.runBlocks.Count - 1 && increment > 0)) return;
			_activeLevelData.runBlocks.RemoveAt(index);
			_activeLevelData.runBlocks.Insert(index + increment, runBlock);
			Repaint();
			EditorUtility.SetDirty(_activeLevelData);
			SetMap(_activeLevelData);
		}

		void DeleteLevel(LevelData map) {

			DialogConfirm.ShowDialog(String.Format("Действительно хотите удалить блок {0}?", map.title), () => {
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(map));
				LoadPrefabsMapsList();
			}, null);

		}

		private void DeactiveBlock() {
			//LevelBlockSpawner.Instance.DestroyObjects();
			//_activeLevelData = null;
		}

		void InitMapList() {
			LoadPrefabsMapsList();
		}

		void LoadPrefabsMapsList() {
			runList = GetAllPrefabsOfType<LevelData>();

			if (!String.IsNullOrEmpty(RunBlockEditPrefs.mapBlockEditon)) {
				LevelData elem = runList.Find(x => x.name == RunBlockEditPrefs.mapBlockEditon);
				if (elem != null) SetMap(elem);
			}
		}

		void SetMap(LevelData map) {
			LevelBlockSpawn.Instance.DestroyObjects();
			_activeLevelData = map;
      
			RunBlockEditPrefs.mapBlockEditon = _activeLevelData.name;
			LevelBlockSpawn.SpawnEditor(_activeLevelData);
      map.Init();

    }

    private void OpenDialogLevel(LevelData map) {

      DeactiveAllInspector();
      if (levelDataEditorWindow == null)
        levelDataEditorWindow = (LevelDataWindow)EditorWindow.GetWindow(typeof(LevelDataWindow));
      levelDataEditorWindow.activeLevelData = map;
    }

		void DublicateLevelBlock(LevelData levelData) {

			string path = AssetDatabase.GetAssetPath(levelData);
			path = path.Substring(0, path.Length - (levelData.name.Length + 6)) + levelData.name + " " + (runList.Count + 1) + ".asset";

			LevelData asset = ScriptableObject.CreateInstance("LevelData") as LevelData;
			asset.runBlocks = new List<BlockBase>(levelData.runBlocks);
			asset.title = levelData.title + "(копия)";
			asset.gameMode = levelData.gameMode;
			asset.location = levelData.location;
			asset.moveVector = levelData.moveVector;
			asset.gameFormat = levelData.gameFormat;
			asset.sceneStart = levelData.sceneStart;
			AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(path));

			LoadPrefabsMapsList();
		}

		#endregion

		#region Блоки

		private RunBlock _activeRunBlock;

		public int mapBlocksBar = 0;
		public string[] mapBlocksToolBarStrings = new string[] { "Список", "Редактор" };

		/// <summary>
		/// Отрисовка блоков
		/// </summary>
		private void DrawMapBlocks() {
			GUILayout.Space(8f);
			GUILayout.BeginHorizontal();
			_activeRunBlock = EditorGUILayout.ObjectField("Выбор блока", this._activeRunBlock, typeof(RunBlock), false) as RunBlock;
			if (GUILayout.Button("Отменить")) {

				DeactiveMapBlock();
				DeactiveBlock();
        DeactiveAllInspector();
      }

			GUILayout.EndHorizontal();

			EditorGUILayout.BeginVertical();
			mapBlocksBar = GUILayout.Toolbar(mapBlocksBar, mapBlocksToolBarStrings);
			EditorGUILayout.EndHorizontal();

			switch (mapBlocksBar) {
				case 0:
					DrawListMapBlock();
					break;
				case 1:
					DrawMamBlockEditor();
					break;
			}
		}

		private void InitMapBlock() {
			LoadPrefabsMapsBlocks();
		}

		private void DrawMamBlockEditor() {
			GUILayout.Space(8f);

			if (_activeRunBlock == null) {
				GUILayout.Label("Не выбран блок");
				return;
			}

			_activeRunBlock.title = EditorGUILayout.TextField("Название", _activeRunBlock.title);

			//if (GUILayout.Button("Сохранить")) EditorUtility.SetDirty(_activeRunBlock);
			if (GUILayout.Button("Сохранить")) _activeRunBlock.SaveData();
			if (GUILayout.Button("Удалить все объекты")) _activeRunBlock.DeleteObject();
			if (GUILayout.Button("Очистить все объекты")) _activeRunBlock.DeleteObject(true);
			if (GUILayout.Button("Загрузить объекты")) _activeRunBlock.LoadObject();

		}

		void DrawStatus() {

			GUILayout.BeginHorizontal();

			Rect controlRect = GUILayoutUtility.GetRect(0.0f, 20, GUILayout.ExpandWidth(true));
			GUI.Box(new Rect(controlRect.x - 4, controlRect.y - 4, controlRect.width + 8, controlRect.height + 8), "", this.boxStyle);

			if (mapEditorMode) {
				GUI.color = Color.red;
				GUI.Label(new Rect(controlRect.x + 4, controlRect.y + 4, 400, 18), "Включен режим редакции");
				GUI.color = Color.white;
			} else {
				GUI.color = Color.black;
				GUI.Label(new Rect(controlRect.x + 4, controlRect.y + 4, 400, 18), "Отключен режим редакции");
				GUI.color = Color.white;
			}
			GUILayout.EndHorizontal();
		}


		void DrawListMapBlock() {

			_scrollListPosition = GUILayout.BeginScrollView(_scrollListPosition);

			float thumbnailMaxHeight = 5;
			RectOffset thumbnailPadding = new RectOffset(6, 6, 6, 3);
			float labelHeight = 8;

			RectOffset padding = this.paddingStyle.padding;

			foreach (var element in runBlockList) {

				Color boxColor = new Color(0.897f, 0.897f, 0.897f, 1f);

				EditorGUILayout.Space();
				GUILayout.Space(3f);
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(8f);

				float thumbnailHeightWithPadding = thumbnailMaxHeight + thumbnailPadding.top + thumbnailPadding.bottom;
				Rect controlRect = GUILayoutUtility.GetRect(0.0f, (thumbnailHeightWithPadding + labelHeight), GUILayout.ExpandWidth(true));
				//Rect clickRect = new Rect(controlRect.x, controlRect.y, controlRect.width, (controlRect.height));

				GUI.color = boxColor;
				GUI.Box(new Rect(controlRect.x - padding.left, controlRect.y - padding.top, controlRect.width + (padding.left + padding.right), controlRect.height + (padding.top + padding.bottom)), "", this.boxStyle);
				GUI.color = Color.white;

				GUI.color = Color.black;
				GUI.Label(new Rect(padding.left * 2, controlRect.y, 400, 18), String.Format("{0} ({1})", (String.IsNullOrEmpty(element.title) ? "Обычный блок" : element.title), element.name));
				GUI.color = Color.white;

				if (GUI.Button(new Rect((controlRect.width - 98f), (controlRect.y + 1f), 80f, 20f), "Установить")) {
					CancelSelect();
					SetMapBlocks(element);
				} else if (GUI.Button(new Rect((controlRect.width - 190f), (controlRect.y + 1f), 90f, 20f), "Дублировать")) {
					DublicateMapBlock(element);
				} else if (GUI.Button(new Rect((controlRect.width - 13f), (controlRect.y + 1f), 20f, 20f), "X")) {
					DeleteMap(element);
				} else if (Event.current.type == EventType.MouseUp && controlRect.Contains(Event.current.mousePosition)) { }

				GUILayout.Space(8f);
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(3f);
			}
			GUILayout.EndScrollView();
		}

		private void DeactiveMapBlock() {
			if (_activeRunBlock == null) return;
			_activeRunBlock.DestroyExistsObjects();
			_activeRunBlock = null;
		}

    private void DeactiveLevelBlock() {
      if (_activeLevelData == null) return;
      _activeLevelData.DestroyExistsObjects();
      _activeLevelData = null;
    }

    void StartEdit() {

			if (_activeRunBlock == null) return;

			mapEditorMode = true;
			EditorPrefs.SetBool("mapEditorMode", mapEditorMode);
		}

		void EndEdit() {
			mapEditorMode = false;
			EditorPrefs.SetBool("mapEditorMode", mapEditorMode);
		}

		void SetMapBlocks(RunBlock map) {
			_activeRunBlock = map;
			RunBlockEditPrefs.mapBlockEditon = _activeRunBlock.name;
			_activeRunBlock.Init();
      //Selection.objects = new UnityEngine.Object[] { _activeRunBlock };
      //GameManager mm = GameObject.Find("Map").GetComponent<GameManager>();
      //mm.SetMap(map);
      DeactiveAllInspector();
      if (mapBlockEditorWindow == null)
        mapBlockEditorWindow = (MapBlockEditorWindow)EditorWindow.GetWindow(typeof(MapBlockEditorWindow));
      mapBlockEditorWindow.activeRunBlock = _activeRunBlock;
    }

    LevelDataWindow levelDataEditorWindow;
    MapBlockEditorWindow mapBlockEditorWindow;
    
    private void DeactiveAllInspector() {

      if (levelDataEditorWindow != null)
        levelDataEditorWindow.Close();
      if (mapBlockEditorWindow != null)
        mapBlockEditorWindow.Close();
    }


		void DeleteMap(RunBlock map) {

			DialogConfirm.ShowDialog(String.Format("Действительно хотите удалить блок {0}?", map.title), () => {
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(map));
				LoadPrefabsMapsBlocks();
			}, null);

		}

		void DublicateMapBlock(RunBlock runBlock) {

			string path = AssetDatabase.GetAssetPath(runBlock);
			path = path.Substring(0, path.Length - (runBlock.name.Length + 6)) + runBlock.name + " " + (runBlockList.Count + 1) + ".asset";

			RunBlock asset = ScriptableObject.CreateInstance("RunBlock") as RunBlock;
			asset.saveObject = new List<SpawnObjectReady>(runBlock.saveObject);
			asset.title = runBlock.title + "(копия)";
			asset.objectLibrary = runBlock.objectLibrary;
			AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(path));

			LoadPrefabsMapsBlocks();
		}

		void LoadPrefabsMapsBlocks() {
			runBlockList = GetAllPrefabsOfType<RunBlock>();

			if (!String.IsNullOrEmpty(RunBlockEditPrefs.mapBlockEditon)) {
				RunBlock elem = runBlockList.Find(x => x.name == RunBlockEditPrefs.mapBlockEditon);
				if (elem != null) SetMapBlocks(elem);
			}
		}

		public List<T> GetAllPrefabsOfType<T>() where T : class {
			List<T> list = new List<T>();
			var allPrefabs = GetAllPrefabs();
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
				if (s.Contains(".asset")) result.Add(s);
			}
			return result.ToArray();
		}
		#endregion

	}

}