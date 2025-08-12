using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(XMLParcer))]
public class XMLParcerEditor : Editor {

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    if(GUILayout.Button("Read Xml")) {
      ((XMLParcer)target).ReadXmlButtonEditor();
    }

  }

}
