using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditRun {

  public class MapBlockEditorWindow: EditorWindow {

    private RunBlock _activeRunBlock;

    private GUIStyle paddingStyle;
    private GUIStyle boxStyle;
    private Vector2 scrollViewOffset = Vector2.zero;
    private SpawnType groupType = SpawnType.none;
    private int m_SelectedSpriteInstanceID = 0;

    private static Color spriteBoxNormalColor = new Color(0.897f, 0.897f, 0.897f, 1f);
    private static Color spriteBoxHighlightColor = new Color(0.798f, 0.926f, 0.978f, 1f);

    private void OnEnable() {
      this.titleContent = new GUIContent("Уровень");
      this.boxStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box);

      this.paddingStyle = new GUIStyle();
      this.paddingStyle.padding = new RectOffset(3, 3, 3, 3);
    }

    public EditRun.RunBlock activeRunBlock {
      get {
        return _activeRunBlock;
      }
      set {
        _activeRunBlock = value;
        Repaint();
      }
    }

    private void OnGUI() {

      if (!_activeRunBlock)
        return;
      DrawInspector();
      EditorGUILayout.Space();
      this.DrawSaveButton();
      EditorGUILayout.Space();

      if (GUILayout.Button("Сохранить")) {
        EditorUtility.SetDirty(this._activeRunBlock);
      }
    }

    private void DrawInspector() {
      this._activeRunBlock.title = EditorGUILayout.TextField("Название", this._activeRunBlock.title);
      EditorGUILayout.LabelField("Дистанция блока = " + this._activeRunBlock.blockDistantion.ToString() + " м");
      EditorGUILayout.LabelField("Количество блоков = " + this._activeRunBlock.saveObject.Count);
      EditorGUILayout.Space();
      DrawGrideObject();
    }

    private void DrawSaveButton() {

      if (GUILayout.Button("Сохранить объекты")) _activeRunBlock.SaveData();
      if (GUILayout.Button("Удалить все объекты")) _activeRunBlock.DeleteObject();
      if (GUILayout.Button("Очистить все объекты")) _activeRunBlock.DeleteObject(true);
      if (GUILayout.Button("Загрузить объекты")) _activeRunBlock.LoadObject();

    }

    private void DrawGrideObject() {


      Rect controlRect = EditorGUILayout.GetControlRect();
      GUI.Label(controlRect, "Object (" + this._activeRunBlock.objectLibrary.objectList.Count + ")", EditorStyles.boldLabel);

      EditorGUILayout.BeginVertical(this.boxStyle);

      this.scrollViewOffset = EditorGUILayout.BeginScrollView(this.scrollViewOffset, GUILayout.Height(500));

      if (this._activeRunBlock.objectLibrary != null) {

        if (groupType == SpawnType.none)
          this.DrawGroup();
        else
          this.DrawSpritesObject();
      }

      EditorGUILayout.EndScrollView();

      EditorGUILayout.EndVertical();

    }


    private void DrawGroup() {
      List<GroupSpawnParametr> objectLibrary = this._activeRunBlock.objectLibrary.groupList;

      EditorGUILayout.BeginHorizontal();
      GUILayout.Space(6f);

      GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
      labelStyle.fontStyle = FontStyle.Normal;
      labelStyle.alignment = TextAnchor.MiddleCenter;

      RectOffset padding = this.paddingStyle.padding;
      float thumbnailMaxHeight = 80;
      float labelHeight = 20f;
      float selectedExtensionHeight = 27f;

      RectOffset thumbnailPadding = new RectOffset(6, 6, 6, 3);
      EditorGUILayout.BeginVertical();

      foreach (GroupSpawnParametr oneObject in objectLibrary) {

        bool isSelected = this.IsSelected(oneObject.GetHashCode());
        Color boxColor = (isSelected ? spriteBoxHighlightColor : spriteBoxNormalColor);

        GUILayout.Space(6f);
        EditorGUILayout.BeginHorizontal(this.paddingStyle);

        float thumbnailHeight = thumbnailMaxHeight;
        if (oneObject.sprite != null)
          thumbnailHeight = (oneObject.sprite.rect.height > thumbnailMaxHeight) ? thumbnailMaxHeight : oneObject.sprite.rect.height;

        float thumbnailHeightWithPadding = thumbnailHeight + thumbnailPadding.top + thumbnailPadding.bottom;

        // Generate a working rect for the control
        Rect controlRect = GUILayoutUtility.GetRect(0.0f, (thumbnailHeightWithPadding + labelHeight + (isSelected ? (selectedExtensionHeight + padding.top) : 0f)), GUILayout.ExpandWidth(true));


        // Determine the click rect
        Rect clickRect = new Rect(controlRect.x, controlRect.y, controlRect.width, (controlRect.height - (isSelected ? (selectedExtensionHeight + padding.top) : 0f)));

        GUI.color = boxColor;
        GUI.Box(new Rect(controlRect.x - padding.left, controlRect.y - padding.top, controlRect.width + (padding.left + padding.right), controlRect.height + (padding.top + padding.bottom)), "", this.boxStyle);
        GUI.color = Color.white;

        // Draw the thumbnail
        if (oneObject.sprite != null)
          this.DrawThumbnail(oneObject.sprite, thumbnailHeight, controlRect, thumbnailPadding);

        GUI.color = Color.black;
        GUI.Label(new Rect(controlRect.x, (controlRect.y + thumbnailHeightWithPadding + 1f), controlRect.width, labelHeight), oneObject.title, labelStyle);
        GUI.color = Color.white;

        if (Event.current.type == EventType.MouseUp && clickRect.Contains(Event.current.mousePosition)) {
          groupType = oneObject.type;
          Repaint();
        }

        EditorGUILayout.EndHorizontal();

      }
      EditorGUILayout.EndVertical();
      EditorGUILayout.EndHorizontal();
    }


    private void DrawSpritesObject() {
      List<SpawnObjectInfo> objectLibrary = this._activeRunBlock.objectLibrary.objectList;

      EditorGUILayout.BeginHorizontal();
      GUILayout.Space(6f);

      GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
      labelStyle.fontStyle = FontStyle.Normal;
      labelStyle.alignment = TextAnchor.MiddleCenter;

      RectOffset padding = this.paddingStyle.padding;
      float thumbnailMaxHeight = 80;
      float labelHeight = 20f;
      float selectedExtensionHeight = 27f;

      RectOffset thumbnailPadding = new RectOffset(6, 6, 6, 3);
      EditorGUILayout.BeginVertical();

      DrawBackIcon();

      foreach (SpawnObjectInfo oneObject in objectLibrary) {

        if (oneObject.spawnType != groupType) continue;

        bool isSelected = this.IsSelected(oneObject.GetHashCode());
        Color boxColor = (isSelected ? spriteBoxHighlightColor : spriteBoxNormalColor);

        GUILayout.Space(6f);
        EditorGUILayout.BeginHorizontal(this.paddingStyle);

        float thumbnailHeight = thumbnailMaxHeight;
        if (oneObject.icon != null)
          thumbnailHeight = (oneObject.icon.rect.height > thumbnailMaxHeight) ? thumbnailMaxHeight : oneObject.icon.rect.height;

        float thumbnailHeightWithPadding = thumbnailHeight + thumbnailPadding.top + thumbnailPadding.bottom;

        Rect controlRect = GUILayoutUtility.GetRect(0.0f, (thumbnailHeightWithPadding + labelHeight + (isSelected ? (selectedExtensionHeight + padding.top) : 0f)), GUILayout.ExpandWidth(true));

        Rect clickRect = new Rect(controlRect.x, controlRect.y, controlRect.width, (controlRect.height - (isSelected ? (selectedExtensionHeight + padding.top) : 0f)));

        GUI.color = boxColor;
        GUI.Box(new Rect(controlRect.x - padding.left, controlRect.y - padding.top, controlRect.width + (padding.left + padding.right), controlRect.height + (padding.top + padding.bottom)), "", this.boxStyle);
        GUI.color = Color.white;

        if (oneObject.icon != null)
          this.DrawThumbnail(oneObject.icon, thumbnailHeight, controlRect, thumbnailPadding);

        GUI.color = Color.black;
        GUI.Label(new Rect(controlRect.x, (controlRect.y + thumbnailHeightWithPadding + 1f), controlRect.width, labelHeight), (String.IsNullOrEmpty(oneObject.title) ? oneObject.prefab.name : oneObject.title), labelStyle);
        GUI.color = Color.white;

        if (Event.current.type == EventType.MouseUp && clickRect.Contains(Event.current.mousePosition)) {
          EditorGUIUtility.PingObject(oneObject.prefab.gameObject);

          _activeRunBlock.SpawnObjectEditor(oneObject);

          if (!isSelected) this.SetSelected(oneObject.GetHashCode());
        }

        EditorGUILayout.EndHorizontal();

      }
      EditorGUILayout.EndVertical();
      EditorGUILayout.EndHorizontal();
    }


    private bool IsSelected(int id) {
      return (this.m_SelectedSpriteInstanceID == id);
    }

    private void SetSelected(int id) {
      this.m_SelectedSpriteInstanceID = id;
      base.Repaint();
    }

    private void DrawThumbnail(Sprite info, float height, Rect controlRect, RectOffset padding) {
      // Calculate the sprite rect inside the texture
      Rect spriteRect = new Rect(info.textureRect.x / info.texture.width,
                                 info.textureRect.y / info.texture.height,
                                 info.textureRect.width / info.texture.width,
                                 info.textureRect.height / info.texture.height);

      Vector2 spriteSize = new Vector2(info.rect.width, info.rect.height);

      Vector2 thumbMaxSize = new Vector2((controlRect.width - (padding.left + padding.right)), height);

      if (spriteSize.x > thumbMaxSize.x) {
        spriteSize *= thumbMaxSize.x / spriteSize.x;
      }
      if (spriteSize.y > thumbMaxSize.y) {
        spriteSize *= thumbMaxSize.y / spriteSize.y;
      }

      Rect thumbRect = new Rect(0f, 0f, spriteSize.x, spriteSize.y);

      thumbRect.x = controlRect.x + ((controlRect.width - spriteSize.x) / 2f);
      thumbRect.y = controlRect.y + padding.top + ((height - spriteSize.y) / 2f);

      GUI.DrawTextureWithTexCoords(thumbRect, info.texture, spriteRect, true);
    }

    private void DrawBackIcon() {

      GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
      labelStyle.fontStyle = FontStyle.Normal;
      labelStyle.alignment = TextAnchor.MiddleCenter;

      RectOffset padding = this.paddingStyle.padding;
      float thumbnailMaxHeight = 80;
      float labelHeight = 20f;
      float selectedExtensionHeight = 27f;

      RectOffset thumbnailPadding = new RectOffset(6, 6, 6, 3);
      EditorGUILayout.BeginVertical();

      Color boxColor = spriteBoxNormalColor;

      GUILayout.Space(6f);
      EditorGUILayout.BeginHorizontal(this.paddingStyle);

      float thumbnailHeight = thumbnailMaxHeight;
      if (this._activeRunBlock.objectLibrary.backIcon != null)
        thumbnailHeight = (this._activeRunBlock.objectLibrary.backIcon.rect.height > thumbnailMaxHeight) ? thumbnailMaxHeight : this._activeRunBlock.objectLibrary.backIcon.rect.height;

      float thumbnailHeightWithPadding = thumbnailHeight + thumbnailPadding.top + thumbnailPadding.bottom;

      Rect controlRect = GUILayoutUtility.GetRect(0.0f, (thumbnailHeightWithPadding + labelHeight), GUILayout.ExpandWidth(true));

      Rect clickRect = new Rect(controlRect.x, controlRect.y, controlRect.width, controlRect.height);

      GUI.color = boxColor;
      GUI.Box(new Rect(controlRect.x - padding.left, controlRect.y - padding.top, controlRect.width + (padding.left + padding.right), controlRect.height + (padding.top + padding.bottom)), "", this.boxStyle);
      GUI.color = Color.white;

      this.DrawThumbnail(this._activeRunBlock.objectLibrary.backIcon, thumbnailHeight, controlRect, thumbnailPadding);

      GUI.color = Color.black;
      GUI.Label(new Rect(controlRect.x, (controlRect.y + thumbnailHeightWithPadding + 1f), controlRect.width, labelHeight), "Назад", labelStyle);
      GUI.color = Color.white;

      if (Event.current.type == EventType.MouseUp && clickRect.Contains(Event.current.mousePosition)) {
        groupType = SpawnType.none;
        Repaint();
      }

      EditorGUILayout.EndHorizontal();
      EditorGUILayout.EndVertical();
    }

  }
}