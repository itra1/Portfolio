using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NavigationHelpers : MonoBehaviour
{
  [MenuItem("Tools/Delete PlayerPrefs")]
  public static void ClearPlayerPrefs() {
    PlayerPrefs.DeleteAll();
  }
}
