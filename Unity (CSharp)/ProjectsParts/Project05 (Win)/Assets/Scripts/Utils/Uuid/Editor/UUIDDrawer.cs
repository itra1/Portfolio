using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(UUIDAttribute))]
public class UUIDDrawer: PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    if (property.propertyType != SerializedPropertyType.String) {
      EditorGUI.LabelField(position, label.text, "Only string support.");
      return;
    }

	 UUIDAttribute sttrib = attribute as UUIDAttribute;

	 if (sttrib.drawnNewButton)
	 {

		position.width -= (40 + 5);
		property.stringValue = EditorGUI.TextField(position, label.text, property.stringValue);

		// new uuid
		position.x += position.width + 5;
		position.width = 40;

		if (GUI.Button(position, "New"))
		  GetUUID(property);
	 }else
		property.stringValue = EditorGUI.TextField(position, label.text, property.stringValue);

	 if (string.IsNullOrEmpty(property.stringValue))
		GetUUID(property);


  }

  private void GetUUID(SerializedProperty property)
  {
	 property.stringValue = System.Guid.NewGuid().ToString();
  }
}