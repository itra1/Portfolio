using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ResourcePathAttribute))]
public class ResourcePathDrawner : PropertyDrawer {

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

          string name = DragAndDrop.objectReferences[0].name;
          string path = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]);

          if (!path.Contains("Assets/Resources/"))
          {
            Debug.Log("No resource path");
            return;
          }

          path = path.Remove(0, ("Assets/Resources/").Length);
          path = path.Substring(0, path.IndexOf(name) + name.Length);

          property.stringValue = path;
        }
        break;
      }
    }

  }

}
