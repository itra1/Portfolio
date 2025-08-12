using UnityEditor;
using UnityEngine;

namespace Uuid.Editor {

	[CustomPropertyDrawer(typeof(UUIDAttribute))]
	public class UUIDDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			if (property.propertyType != SerializedPropertyType.String) {
				EditorGUI.LabelField(position, label.text, "Only string support.");
				return;
			}

			_ = ((UUIDAttribute)attribute).EmptyOnlyEdit;

			position.width -= (40 + 5);
			property.stringValue = EditorGUI.TextField(position, label.text, property.stringValue);

			// new uuid
			position.x += position.width + 5;
			position.width = 40;
			if (GUI.Button(position, "New")) {
				property.stringValue = System.Guid.NewGuid().ToString();
			}
		}
	}
}