using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapBehaviour))]
public class MapBehaviourEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Отрисовать сетку")) {
			((MapBehaviour)target).DrawGride();
		}

		if (GUILayout.Button("Сохранить")) {
			((MapBehaviour)target).SaveObject();
		}

		if (GUILayout.Button("Рассчитать регион minimap")) {
			((MapBehaviour)target).CalcMinimapRegion();
		}

		if (GUILayout.Button("Исправить сетку по новым стандартам")) {
			((MapBehaviour)target).RecalcNet();
		}


	}
}
