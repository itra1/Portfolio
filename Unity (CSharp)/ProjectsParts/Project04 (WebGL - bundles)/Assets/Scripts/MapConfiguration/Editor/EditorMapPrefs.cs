using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace MapConfiguration {

	public class EditorMapPrefs : Editor {

		public static bool mapEditor;

		public static void InitEditroPrefs() {

			mapEditor = false;

			EditorPrefs.SetBool("mapEditor", false);

		}

		public bool mapEditroPrefs() {
			return EditorPrefs.GetBool("mapEditor", false);
		}

	}
}