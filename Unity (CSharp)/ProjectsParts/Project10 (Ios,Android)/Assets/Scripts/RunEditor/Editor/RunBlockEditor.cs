using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditRun {

	[CustomEditor(typeof(RunBlock))]
	public class RunBlockEditor : Editor {

		public EditRunLibrary _objectLibrary;
		private RunBlock _emInstance;
		private Vector2 scrollViewOffset = Vector2.zero;
		private int m_SelectedSpriteInstanceID = 0;

		private GUIStyle paddingStyle;
		private GUIStyle boxStyle;

		private static Color spriteBoxNormalColor = new Color(0.897f, 0.897f, 0.897f, 1f);
		private static Color spriteBoxHighlightColor = new Color(0.798f, 0.926f, 0.978f, 1f);

		private SpawnType groupType;

		protected void OnEnable() {
			this._emInstance = this.target as RunBlock;

			this.groupType = SpawnType.none;
			this.boxStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box);
			this.paddingStyle = new GUIStyle();
			this.paddingStyle.padding = new RectOffset(3, 3, 3, 3);
			Selection.selectionChanged += ChangeSelection;
		}

		private void OnDisable() {
			this._emInstance = null;
			Selection.selectionChanged -= ChangeSelection;
		}

		public override void OnInspectorGUI() {

			this.serializedObject.Update();

			this._emInstance.title = EditorGUILayout.TextField("Название", this._emInstance.title);
			this._emInstance.objectLibrary = EditorGUILayout.ObjectField("Выбор библиатеки", this._emInstance.objectLibrary, typeof(EditRunLibrary), false) as EditRunLibrary;
			EditorGUILayout.LabelField("Дистанция блока = " + this._emInstance.blockDistantion.ToString() + " м");
      EditorGUILayout.LabelField("Количество блоков = " + this._emInstance.saveObject.Count);
      EditorGUILayout.Space();

			this.DrawGrideObject();
			EditorGUILayout.Space();
			this.DrawSaveButton();
			EditorGUILayout.Space();

			if (GUILayout.Button("Сохранить")) {
				EditorUtility.SetDirty(this._emInstance);
			}

		}

		private void DrawSaveButton() {

			if (GUILayout.Button("Сохранить объекты")) _emInstance.SaveData();
			if (GUILayout.Button("Удалить все объекты")) _emInstance.DeleteObject();
			if (GUILayout.Button("Очистить все объекты")) _emInstance.DeleteObject(true);
			if (GUILayout.Button("Загрузить объекты")) _emInstance.LoadObject();

		}

		private void DrawGrideObject() {

			if (this._emInstance == null) return;

			Rect controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(controlRect, "Object (" + this._emInstance.objectLibrary.objectList.Count + ")", EditorStyles.boldLabel);

			EditorGUILayout.BeginVertical(this.boxStyle);

			this.scrollViewOffset = EditorGUILayout.BeginScrollView(this.scrollViewOffset, GUILayout.Height(500));

			if (this._emInstance.objectLibrary != null) {

				if (groupType == SpawnType.none)
					this.DrawGroup();
				else
					this.DrawSpritesObject();
			}
			
			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndVertical();

		}

		private bool IsSelected(int id) {
			return (this.m_SelectedSpriteInstanceID == id);
		}

		private void SetSelected(int id) {
			this.m_SelectedSpriteInstanceID = id;
			base.Repaint();
		}

		private void DrawSpritesObject() {
			List<SpawnObjectInfo> objectLibrary = this._emInstance.objectLibrary.objectList;

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

					_emInstance.SpawnObjectEditor(oneObject);
					
					if (!isSelected) this.SetSelected(oneObject.GetHashCode());
				}

				EditorGUILayout.EndHorizontal();

			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
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
			if (this._emInstance.objectLibrary.backIcon != null)
				thumbnailHeight = (this._emInstance.objectLibrary.backIcon.rect.height > thumbnailMaxHeight) ? thumbnailMaxHeight : this._emInstance.objectLibrary.backIcon.rect.height;

			float thumbnailHeightWithPadding = thumbnailHeight + thumbnailPadding.top + thumbnailPadding.bottom;
			
			Rect controlRect = GUILayoutUtility.GetRect(0.0f, (thumbnailHeightWithPadding + labelHeight ), GUILayout.ExpandWidth(true));
			
			Rect clickRect = new Rect(controlRect.x, controlRect.y, controlRect.width, controlRect.height);

			GUI.color = boxColor;
			GUI.Box(new Rect(controlRect.x - padding.left, controlRect.y - padding.top, controlRect.width + (padding.left + padding.right), controlRect.height + (padding.top + padding.bottom)), "", this.boxStyle);
			GUI.color = Color.white;
			
			this.DrawThumbnail(this._emInstance.objectLibrary.backIcon, thumbnailHeight, controlRect, thumbnailPadding);

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

		private void DrawGroup() {
			List<GroupSpawnParametr> objectLibrary = this._emInstance.objectLibrary.groupList;
			
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

		void ChangeSelection() {


			try {
				SelectElement select = Selection.activeGameObject.GetComponent<SelectElement>();
				if (select != null && Selection.activeGameObject != select.parentObject) {

					IEnumerator e = SelectObject(select.parentObject);
					while (e.MoveNext()) ;
				}
				//useSelect.Add(select.parentObject);
			} catch { }


			//List<GameObject> useSelect = new List<GameObject>();

			//foreach (var elem in Selection.gameObjects) {
			//	try {
			//		SelectElement select = elem.GetComponent<SelectElement>();
			//		if (select != null && elem != select.parentObject)
			//			Selection.activeGameObject = select.parentObject;
			//						//useSelect.Add(select.parentObject);
			//	} catch { }
			//}

			//if (useSelect.Count > 0)
			//	Selection.objects = useSelect.ToArray();

			//Selection.activeGameObject = 


			//Debug.Log(Selection.objects.Length);
		}

		IEnumerator SelectObject(UnityEngine.Object selectObject) {
			yield return null;
			Selection.objects = new UnityEngine.Object[] {selectObject};
		}

	}
	

}