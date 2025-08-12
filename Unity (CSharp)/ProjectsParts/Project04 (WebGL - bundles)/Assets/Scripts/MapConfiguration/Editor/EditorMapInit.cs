using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MapConfiguration {

	[InitializeOnLoad]
	public class EditorMapInit : Editor {

		static EditorMapInit() {
			EditorMapPrefs.InitEditroPrefs();
		}

	}
}