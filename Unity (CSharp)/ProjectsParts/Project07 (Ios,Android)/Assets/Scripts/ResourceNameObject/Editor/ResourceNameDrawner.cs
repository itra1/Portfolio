using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ResourceNameAttribute))]
public class ResourceNameDrawner: PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    if (property.propertyType != SerializedPropertyType.String) {
      EditorGUI.LabelField(position, label.text, "Only string support.");
      return;
    }

    property.stringValue = EditorGUI.TextField(position, label.text, property.stringValue);
    if (DragAndDrop.objectReferences.Length > 0) {
      position.height = 0;
      DrawnBlock(property, property.displayName);
    }
  }

  private void DrawnBlock(SerializedProperty property, string labelName) {

    Event evt = Event.current;
    Rect drop_area = GUILayoutUtility.GetRect(0f, 0f, GUILayout.ExpandWidth(true), GUILayout.Height(30));
    GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
    boxStyle.alignment = TextAnchor.MiddleCenter;
    GUI.color = Color.green;
    GUI.Box(drop_area, labelName + " name object (Drop Here)", boxStyle);
    GUI.color = Color.white;

    switch (evt.type) {
      case EventType.DragUpdated:
      case EventType.DragPerform: {
        if (!drop_area.Contains(evt.mousePosition))
          return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        if (evt.type == EventType.DragPerform) {
          DragAndDrop.AcceptDrag();
          property.stringValue = DragAndDrop.objectReferences[0].name;
        }
        break;
      }
    }

  }

}
