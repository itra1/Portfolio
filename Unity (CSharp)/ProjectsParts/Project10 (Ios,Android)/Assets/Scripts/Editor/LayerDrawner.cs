using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerDrawner : PropertyDrawer {

  int id = 0;
  string[] layerName;
  int[] layerId;

  void Init(SerializedProperty property) {
    layerId = new int[SortingLayer.layers.Length];
    layerName = new string[SortingLayer.layers.Length];
    for (int i = 0; i < SortingLayer.layers.Length; i++) {
      layerName[i] = SortingLayer.layers[i].name;
      layerId[i] = SortingLayer.layers[i].id;
    }

    for (int i = 0; i < layerId.Length; i++)
      if (layerId[i] == property.intValue) id = i;
  }

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    
    if(property.propertyType != SerializedPropertyType.Integer) {
      EditorGUI.LabelField(position, label.text, "Only integer support.");
      return;
    }

    if (layerName == null)
      Init(property);
    
    id = EditorGUI.Popup(position,"Layer", id, layerName);
    property.intValue = layerId[id];

  }
}
