using System;
using UnityEditor;
using UnityEngine;

namespace EditRun {
  public class LevelDataWindow: EditorWindow {

    private LevelData _activeLevel;
    private Vector2 _scrollListPosition = Vector2.zero;
    private GUIStyle boxStyle;
    private GUIStyle paddingStyle;

    public LevelData activeLevelData {
      get {
        return _activeLevel;
      }
      set {
        _activeLevel = value;
        Debug.Log(_activeLevel.title);
        Repaint();
      }
    }

    private void OnEnable() {
      titleContent = new GUIContent("Уровень");

      boxStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box);
      paddingStyle = new GUIStyle();
      paddingStyle.padding = new RectOffset(3, 3, 3, 3);
    }

    private void OnGUI() {

      if (activeLevelData == null) return;
      DrawEditorMapLevel();
    }

    void DrawEditorMapLevel() {

      GUILayout.Space(8f);

      if (_activeLevel == null) {
        GUILayout.Label("Не выбран уровень");
        return;
      }

      if (GUILayout.Button("Запустить")) {

        GameManager.activeLevelData = _activeLevel;
        EditorUtility.SetDirty(GameManager.Instance);
        EditorApplication.isPlaying = true;
      }

      _activeLevel.title = EditorGUILayout.TextField("Название", _activeLevel.title);
      _activeLevel.description = EditorGUILayout.TextField("Описание", _activeLevel.description, GUILayout.ExpandHeight(true), GUILayout.Height(50));
      _activeLevel.gameMode = (GameMode)EditorGUILayout.EnumPopup("Режим игры", _activeLevel.gameMode);
      _activeLevel.location = (GameLocation)EditorGUILayout.EnumPopup("Локация", _activeLevel.location);
      _activeLevel.moveVector = (MoveVector)EditorGUILayout.EnumPopup("Направление движения", _activeLevel.moveVector);
      _activeLevel.gameFormat = (GameMechanic)EditorGUILayout.EnumPopup("Формат игры", _activeLevel.gameFormat);
      _activeLevel.region = (RegionType)EditorGUILayout.EnumPopup("Регион", _activeLevel.region);
      _activeLevel.healthType = (HealthType)EditorGUILayout.EnumPopup("Тип жизней", _activeLevel.healthType);
      EditorGUILayout.LabelField("Дистанция: " + _activeLevel.levelDistantion);

      if (GUILayout.Button("Добавить блок")) {
        RunBlockSelectWindows selectWindows = (RunBlockSelectWindows)EditorWindow.GetWindow(typeof(RunBlockSelectWindows));
        selectWindows.onSelect = (elem) => {
          _activeLevel.runBlocks.Add(elem);
          Repaint();
        };
      }

      DrawMapBlockInLevel();

      if (GUILayout.Button("Сохранить")) {
        EditorUtility.SetDirty(_activeLevel);
      }

    }

    void DrawMapBlockInLevel() {

      _scrollListPosition = GUILayout.BeginScrollView(_scrollListPosition);

      float thumbnailMaxHeight = 5;
      RectOffset thumbnailPadding = new RectOffset(6, 6, 6, 3);
      float labelHeight = 8;

      RectOffset padding = paddingStyle.padding;

      int indexElement = 0;
      _activeLevel.runBlocks.RemoveAll(x => x == null);
      foreach (var element in _activeLevel.runBlocks) {

        Color boxColor = new Color(0.897f, 0.897f, 0.897f, 1f);

        EditorGUILayout.Space();
        GUILayout.Space(3f);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(8f);

        float thumbnailHeightWithPadding = thumbnailMaxHeight + thumbnailPadding.top + thumbnailPadding.bottom;
        Rect controlRect = GUILayoutUtility.GetRect(0.0f, (thumbnailHeightWithPadding + labelHeight), GUILayout.ExpandWidth(true));

        GUI.color = boxColor;
        GUI.Box(new Rect(controlRect.x - padding.left, controlRect.y - padding.top, controlRect.width + (padding.left + padding.right), controlRect.height + (padding.top + padding.bottom)), "", boxStyle);
        GUI.color = Color.white;

        GUI.color = Color.black;
        GUI.Label(new Rect(padding.left * 2, controlRect.y, 400, 18), String.Format("{0} ({1})", (String.IsNullOrEmpty(element.title) ? "Обычный блок" : element.title), element.name));
        GUI.color = Color.white;

        if (GUI.Button(new Rect((controlRect.width - 58f), (controlRect.y + 1f), 40f, 20f), "Вниз")) {
          ChangeOrderBlockInLevel(indexElement, 1);
        } else if (GUI.Button(new Rect((controlRect.width - 110f), (controlRect.y + 1f), 50f, 20f), "Вверх")) {
          ChangeOrderBlockInLevel(indexElement, -1);
        } else if (GUI.Button(new Rect((controlRect.width - 13f), (controlRect.y + 1f), 20f, 20f), "X")) {
          _activeLevel.runBlocks.RemoveAt(indexElement);
          EditorUtility.SetDirty(_activeLevel);
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
      BlockBase runBlock = _activeLevel.runBlocks[index];
      if ((index == 0 && increment < 0) || (index == _activeLevel.runBlocks.Count - 1 && increment > 0)) return;
      _activeLevel.runBlocks.RemoveAt(index);
      _activeLevel.runBlocks.Insert(index + increment, runBlock);
      Repaint();
      EditorUtility.SetDirty(_activeLevel);
      SetMap(_activeLevel);
    }

    void SetMap(LevelData map) {
      LevelBlockSpawn.Instance.DestroyObjects();
      _activeLevel = map;

      //Selection.objects = new UnityEngine.Object[] { _activeLevelData };
      RunBlockEditPrefs.mapBlockEditon = _activeLevel.name;
      LevelBlockSpawn.SpawnEditor(_activeLevel);
      //GameManager mm = GameObject.FindObjectOfType<GameManager>();;
      //mm.SetMap(map);

    }

  }
}