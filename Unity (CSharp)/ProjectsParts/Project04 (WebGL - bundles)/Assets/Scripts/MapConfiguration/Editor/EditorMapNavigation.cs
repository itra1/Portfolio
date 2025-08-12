using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MapConfiguration {

	public class EditorMapNavigation : Editor {

		public static void Start() {
			Debug.Log("Start");
		}


		[MenuItem("Tools/Map Manager")]
		public static void OpenMapConfiguration() {
			EditorWindow.GetWindow(typeof(EditorMapWindow));
		}

	}
}