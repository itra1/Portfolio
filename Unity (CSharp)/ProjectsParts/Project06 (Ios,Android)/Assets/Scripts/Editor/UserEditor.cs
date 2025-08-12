using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Game.User
{
  
[CustomEditor(typeof(UserManager))]
public class UserEditor : Editor {

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    GUILayout.BeginHorizontal();
    if(GUILayout.Button("SaveData")) {
      ((UserManager)target).SaveData();
    }
    if(GUILayout.Button("LoadData")) {
      ((UserManager)target).LoadData();
    }
    GUILayout.EndHorizontal();
    
    if(GUILayout.Button("Test")) {
      Debug.Log(((UserManager)target).UserProgress.StatPriceLevel(25));
    }

		if(GUILayout.Button("Save Test")) {
			((UserManager)target).SaveData();
		}
	}
}


}