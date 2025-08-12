using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Navigation  {

  [MenuItem("Utils/Clear player prefs")]
  public static void ClearPlayerPrefs() {
    PlayerPrefs.DeleteAll();
  }

}
